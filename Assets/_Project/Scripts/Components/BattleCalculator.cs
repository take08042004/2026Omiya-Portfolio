using UnityEngine;
using System.Collections.Generic;

public class BattleCalculator : MonoBehaviour
{
    [System.Serializable]
    public class BattleLog
    {
        public string message;
        public bool playerTurn;
        public bool critical;
        public float damage;
    }
    
    [System.Serializable]
    public class BattleResult
    {
        public bool playerWin;
        public float playerRemainingHp;
        public float enemyRemainingHp;
        public List<BattleLog> logs = new List<BattleLog>();
    }

    [System.Serializable]
    public class LevelTableEntry
    {
        public int minPoint;
        public int maxPoint;
        public int level;
    }

    [Header("Name Database")]
    public NameDatabase nameDatabase;

    [Header("Parts")]
    public List<PartData> allParts = new List<PartData>();

    [Header("Base Status")]
    public float baseHp = 100f;
    public float baseAttack = 20f;
    public float baseDefense = 10f;
    public float baseSpeed = 10f;

    public float baseCritRate = 5f; // クリティカル率の基本値
    public int baseAttackCount = 1;

    [Header("Level Table")]
    public List<LevelTableEntry> levelTable = new List<LevelTableEntry>();

    /// <summary>
    /// 画面8などからキャラクターを確定させるメイン処理
    /// </summary>
    public void FinalizeCharacter(CharacterData data)
    {
        if(data == null) return;

        // 名前の抽選
        if(nameDatabase != null)
        {
            data.name = nameDatabase.GetRandomName();
        }

        // 装備の隠しポイントを加算
        int totalLevelPoint = CalculateTotalLevelPoint(data);
        data.totalLevelPoints = totalLevelPoint;

        // レベルのランダム決定と、レベル成長率・装備ボーナスを合算した最終ステータス算出を一本化して実行
        DetermineLevelAndStatus(data);
    }

    private int CalculateTotalLevelPoint(CharacterData data)
    {
        int total = 0;

        PartData head = FindPart(data.headID);
        PartData body = FindPart(data.bodyID);
        PartData arm = FindPart(data.armID);
        PartData leg = FindPart(data.legID);

        if(head != null) total += head.levelPoint;
        if(body != null) total += body.levelPoint;
        if(arm != null) total += arm.levelPoint;
        if(leg != null) total += leg.levelPoint;

        return total;
    }

    /// <summary>
    /// キャラクターのtotalLevelPoints（最大12）に基づいてレベル（最大55）を決定し、最終ステータスを算出します。
    /// </summary>
    public void DetermineLevelAndStatus(CharacterData data)
    {
        if (data == null) return;

        int pts = data.totalLevelPoints;

        // 1. 4の倍数ベースに最適化した決定テーブル（最大12点 ➔ 最大55Lv）
        if (pts >= 1 && pts <= 4)
        {
            data.level = Random.Range(1, 21);  // point 1~4  ➔ 1～20Lv
        }
        else if (pts >= 5 && pts <= 8)
        {
            data.level = Random.Range(21, 41); // point 5~8  ➔ 21～40Lv
        }
        else if (pts >= 9 && pts <= 12)
        {
            data.level = Random.Range(41, 56); // point 9~12 ➔ 41～55Lv (Random.Rangeの最大値は未満のため56指定)
        }
        else
        {
            data.level = Random.Range(1, 11); // 例外安全用
        }

        int lv = data.level;

        // 各ステータスのレベルボーナス累積用変数
        float hpBonus = 0f;
        float attackBonus = 0f;
        float defenseBonus = 0f;
        float speedBonus = 0f;
        float critBonus = 0f;

        // 2. 累積成長率の計算 (Lv55まで破綻なく滑らかに積み上げ)
        for (int currentLv = 2; currentLv <= lv; currentLv++)
        {
            if (currentLv <= 20)
            {
                hpBonus += 3f;
                attackBonus += 2f;
                defenseBonus += 2.1f;
                speedBonus += 1f;
            }
            else if (currentLv <= 40)
            {
                hpBonus += 1.5f;
                attackBonus += 1f;
                defenseBonus += 1.05f;
                speedBonus += 0.75f;
            }
            else // 41～55lv
            {
                hpBonus += 1f;
                attackBonus += 0.5f;
                defenseBonus += 0.55f;
                speedBonus += 0.55f;
            }
        }

        // 3. クリティカル率の段階計算 (Lv51~55に達した時点でさらに+10%され、累計+50%ボーナスになる)
        int additionalCritLayers = (lv - 1) / 10;
        critBonus = additionalCritLayers * 10f;

        // 4. 各装備パーツごとの直接的なステータス加算ボーナス（hpBonus等）を算出・合算
        PartData head = FindPart(data.headID);
        PartData body = FindPart(data.bodyID);
        PartData arm = FindPart(data.armID);
        PartData leg = FindPart(data.legID);

        float partHp = 0f, partAttack = 0f, partDefense = 0f, partSpeed = 0f, partCrit = 0f;
        int partAttackCount = 0;

        AddPartBonus(head, ref partHp, ref partAttack, ref partDefense, ref partSpeed, ref partCrit, ref partAttackCount);
        AddPartBonus(body, ref partHp, ref partAttack, ref partDefense, ref partSpeed, ref partCrit, ref partAttackCount);
        AddPartBonus(arm, ref partHp, ref partAttack, ref partDefense, ref partSpeed, ref partCrit, ref partAttackCount);
        AddPartBonus(leg, ref partHp, ref partAttack, ref partDefense, ref partSpeed, ref partCrit, ref partAttackCount);

        // 5. ベースステータス ＋ レベル成長値 ＋ 装備パーツボーナス をすべて統合して確定
        data.hpMax = baseHp + hpBonus + partHp;
        data.currentHp = data.hpMax;
        
        data.attack = baseAttack + attackBonus + partAttack;
        data.defense = baseDefense + defenseBonus + partDefense;
        data.speed = baseSpeed + speedBonus + partSpeed;
        data.critRate = baseCritRate + critBonus + partCrit;
        data.attackCount = baseAttackCount + partAttackCount;
    }
    
    private void AddPartBonus(
        PartData part,
        ref float hp,
        ref float attack,
        ref float defense,
        ref float speed,
        ref float crit,
        ref int attackCount
    )
    {
        if (part == null) return;

        hp += part.hpBonus;
        attack += part.attackBonus;
        defense += part.defenseBonus;
        speed += part.speedBonus;
        crit += part.critBonus;
        attackCount += Mathf.RoundToInt(part.attackNumber);
    }

    private PartData FindPart(string partID)
    {
        if (string.IsNullOrEmpty(partID)) return null;

        foreach (PartData part in allParts)
        {
            if (part != null && part.partID == partID)
            {
                return part;
            }
        }
        return null;
    }

    public BattleResult SimulateBattle(CharacterData player, CharacterData enemy, BattleCommand command)
    {
        BattleResult result = new BattleResult();
        
        CharacterData p = CloneForBattle(player);
        CharacterData e = CloneForBattle(enemy);
        
        ApplyCommandBonus(p, command);
        
        int maxTurn = 20;
        
        for (int turn = 1; turn <= maxTurn; turn++)
        {
            bool playerFirst = p.speed >= e.speed;
            
            if (playerFirst)
            {
                Attack(p, e, result, true);
                if (e.currentHp <= 0) break;
                
                Attack(e, p, result, false);
                if (p.currentHp <= 0) break;
            }
            else
            {
                Attack(e, p, result, false);
                if (p.currentHp <= 0) break;
                
                Attack(p, e, result, true);
                if (e.currentHp <= 0) break;
            }
        }
        
        result.playerWin = p.currentHp > 0 && e.currentHp <= 0;
        result.playerRemainingHp = Mathf.Max(0, p.currentHp);
        result.enemyRemainingHp = Mathf.Max(0, e.currentHp);
        return result;
    }

    private CharacterData CloneForBattle(CharacterData original)
    {
        CharacterData copy = new CharacterData();
        
        copy.name = original.name;
        copy.level = original.level;
        copy.hpMax = original.hpMax;
        copy.currentHp = original.hpMax;
        copy.attack = original.attack;
        copy.defense = original.defense;
        copy.speed = original.speed;
        copy.critRate = original.critRate;
        copy.attackCount = original.attackCount;
        
        return copy;
    }

    private void ApplyCommandBonus(CharacterData data, BattleCommand command)
    {
        switch (command)
        {
            case BattleCommand.Attack:
                data.attack *= 1.3f;
                break;
            
            case BattleCommand.Guard:
                data.defense *= 1.3f;
                break;
                
            case BattleCommand.Random:
                if (Random.value < 0.5f)
                    data.attack *= 1.25f;
                else
                    data.defense *= 1.25f;
                break;
        }
    }

    private void Attack(CharacterData attacker, CharacterData defender, BattleResult result, bool playerTurn)
    {
        bool critical = Random.Range(0f, 100f) < attacker.critRate;
        
        float damage = attacker.attack - defender.defense * 0.5f;
        damage = Mathf.Max(1, damage);
        
        if (critical)
        {
            damage *= 1.5f;
        }
        
        defender.currentHp -= damage;
        
        BattleLog log = new BattleLog();
        log.playerTurn = playerTurn;
        log.critical = critical;
        log.damage = damage;
        
        log.message = critical
        ? $"{attacker.name}のクリティカル！ {defender.name}に{damage:F0}ダメージ！"
        : $"{attacker.name}の攻撃！ {defender.name}に{damage:F0}ダメージ！";
        
        result.logs.Add(log);
    }
}