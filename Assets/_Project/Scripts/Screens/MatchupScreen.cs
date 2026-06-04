using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MatchupScreen : MonoBehaviour
{
    [Header("Player UI")]
    public TMP_Text playerNameText;
    public TMP_Text playerLevelText;
    public EquipmentMerger playerEquipmentMerger;

    [Header("Enemy UI")]
    public TMP_Text enemyNameText;
    public TMP_Text enemyLevelText;
    public EquipmentMerger enemyEquipmentMerger;

    [Header("Settings")]
    public float nextSceneDelay = 3f;

    private CharacterData playerData;
    private CharacterData enemyData;

    void OnEnable()
    {
        Debug.Log("MatchupScreen OnEnable");
        Debug.Log("GameManager.Instance= " + GameManager.Instance);
        
        StartCoroutine(SetupRoutine());
    }

    private IEnumerator SetupRoutine()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManagerが見つかりません。");
            yield break;
        }

        // 自分のデータ取得
        playerData = GameManager.Instance.GetSelectedCharacter();
        
        if(playerData == null)
        {
            Debug.LogWarning("プレイヤーデータが見つかりません。");

            yield break;
        }

        // 敵のデータ取得
        enemyData = GetEnemyCharacter();

        if(enemyData == null)
        {
            Debug.LogWarning("敵データが見つかりません。");
            yield break;
        }

        
        // --- 追加: 決定した敵データをGameManagerに登録 ---
        // これにより次のバトル画面へデータを引き継ぎます
        GameManager.Instance.SetEnemyCharacter(enemyData);

        // UIの更新
        UpdatePlayerUI();
        UpdateEnemyUI();

        // アバター更新
        UpdateAvatars();

        // 数秒待機する
        //yield return new WaitForSeconds(nextSceneDelay);

        // 画面10へ遷移
        //GameManager.Instance.StartBattle();

    }

    // 過去キャラから対戦相手を取得
    private CharacterData GetEnemyCharacter()
    {
        List<CharacterData> allCharacters = SaveSystem.LoadCharacters();

        if (allCharacters == null || allCharacters.Count == 0)
        {
            return CreateDummyEnemy();
        }

        foreach (CharacterData data in allCharacters)
        {
            if (data == null) continue;

            // 自分自身を除外
            if (data.playerIndex == playerData.playerIndex)
            {
                continue;
            }
            
            //一番新しい他人を探す
            return data;

        }

        return CreateDummyEnemy();

        /*List<CharacterData> candidates = new List<CharacterData>();

        foreach (CharacterData data in allCharacters)
        {
            if (data == null) continue;

            // 自分自身を除外
            if (data.playerIndex == playerData.playerIndex)
                continue;

            candidates.Add(data);
        }

        if (candidates.Count == 0)
        {
            return CreateDummyEnemy();
        }

        int randomIndex = Random.Range(0, candidates.Count);

        return candidates[randomIndex];*/
    }

    // ダミー敵
    private CharacterData CreateDummyEnemy()
    {
        CharacterData dummy = new CharacterData();

        dummy.name = "謎の冒険者";
        dummy.level = Random.Range(1, 5);

        dummy.hpMax = 100;
        dummy.currentHp = 100;

        dummy.attack = 15;
        dummy.defense = 10;
        dummy.speed = 8;

        return dummy;
    }

    // プレイヤーUI
    private void UpdatePlayerUI()
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerData.name;
        }

        if (playerLevelText != null)
        {
            playerLevelText.text = "Lv." + playerData.level;
        }
    }

    // 敵UI
    private void UpdateEnemyUI()
    {
        if (enemyNameText != null)
        {
            enemyNameText.text = enemyData.name;
        }

        if (enemyLevelText != null)
        {
            enemyLevelText.text = "Lv." + enemyData.level;
        }
    }

    // アバター表示
    private void UpdateAvatars()
    {
        if (playerEquipmentMerger != null)
        {
            // 1. モデルの結合とアニメーションの動体上書き
            playerEquipmentMerger.ApplyCharacterData(playerData);
            
            // 2. 追加: プレイヤーにファイティングポーズをとらせる
            if (playerEquipmentMerger.targetAnimator != null)
            {
                playerEquipmentMerger.targetAnimator.SetBool("IsMatchUp", true);
                playerEquipmentMerger.targetAnimator.SetBool("IsBattle", true);
            }
        }

        if (enemyEquipmentMerger != null)
        {
            // 1. モデルの結合とアニメーションの動体上書き
            enemyEquipmentMerger.ApplyCharacterData(enemyData);
            
            // 2. 追加: 敵キャラクターにファイティングポーズをとらせる
            if (enemyEquipmentMerger.targetAnimator != null)
            {
                enemyEquipmentMerger.targetAnimator.SetBool("IsMatchUp", true);
                enemyEquipmentMerger.targetAnimator.SetBool("IsBattle", true);
            }
        }
    }
}
