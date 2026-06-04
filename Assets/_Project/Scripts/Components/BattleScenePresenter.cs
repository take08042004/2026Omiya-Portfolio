using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleScenePresenter : MonoBehaviour
{
    [Header("Commentary")]
    public CommentaryDatabase commentaryDatabase; // インスペクターでアセットを紐付け

    [Header("Equipment Mergers (アバター表示用)")]
    public EquipmentMerger playerEquipmentMerger;
    public EquipmentMerger enemyEquipmentMerger;

    [Header("UI")]
    public Text customCommentaryText;   // 上段の実況吹き出しUI用
    public Text battleLogText;          // システムログ用（必要であれば）

    [Header("3D Stage Control")]
    public GameObject battleStage3D;   // Canvas直下のBattleStage3Dを紐付け

    [Header("Timing")]
    public float actionInterval = 1.5f; // 各攻防の演出間隔
    public float finishDelay = 2.5f;

    private CharacterData playerData;
    private CharacterData enemyData;
    private BattleCalculator.BattleResult currentResult;

    private Animator playerAnimator;
    private Animator enemyAnimator;

    void OnEnable()
    {
        // データの引き継ぎ
        playerData = GameManager.Instance.GetSelectedCharacter();
        enemyData = GameManager.Instance.GetEnemyCharacter();
        currentResult = GameManager.Instance.GetBattleResult();

        if (currentResult == null) return;

        // アバターの取得と、にらみ合い（戦闘構え）の維持
        SetupAvatars();

        // ご指定いただいた固定の戦闘演出タイムラインを再生
        StartCoroutine(FixedBattleTimelineRoutine());
    }

    private void SetupAvatars()
    {
        if (playerEquipmentMerger != null && playerData != null)
        {
            playerEquipmentMerger.ApplyCharacterData(playerData);
            playerAnimator = playerEquipmentMerger.targetAnimator;
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsMatchUp", false);
                playerAnimator.SetBool("IsBattle", true); // にらみ合い待機状態
            }
        }

        if (enemyEquipmentMerger != null && enemyData != null)
        {
            enemyEquipmentMerger.ApplyCharacterData(enemyData);
            enemyAnimator = enemyEquipmentMerger.targetAnimator;
            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool("IsMatchUp", false);
                enemyAnimator.SetBool("IsBattle", true); // にらみ合い待機状態
            }
        }
    }

    private IEnumerator FixedBattleTimelineRoutine()
    {
        // 最終的な計算結果から、このバトルにおける「勝つ側」と「倒れる側」を自動マッピング
        Animator winnerAnim = currentResult.playerWin ? playerAnimator : enemyAnimator;
        Animator loserAnim = currentResult.playerWin ? enemyAnimator : playerAnimator;

        string winnerName = currentResult.playerWin ? playerData.name : enemyData.name;
        string loserName = currentResult.playerWin ? enemyData.name : playerData.name;

        string winnerWeapon = currentResult.playerWin ? playerData.armID : enemyData.armID;
        string loserWeapon = currentResult.playerWin ? enemyData.armID : playerData.armID;

        // --- ① 1手目：倒れる側から攻撃モーション発火 (される側は防御モーション発火) ---
        if (loserAnim != null) loserAnim.SetTrigger("Attack");
        if (winnerAnim != null) winnerAnim.SetTrigger("Defence");
        PlayWeaponSound(loserWeapon);

        // 実況が反応 (データベースからランダムに通常セリフを抽選、~{名前}+抽選されたセリフ)
        if (commentaryDatabase != null && customCommentaryText != null)
        {
            customCommentaryText.text = $"{loserName}{commentaryDatabase.GetAttackComment()}";
        }
        yield return new WaitForSeconds(actionInterval);


        // --- ② 2手目：勝つ側からの返しの攻撃モーション発火 (される側は防御モーション) ---
        if (winnerAnim != null) winnerAnim.SetTrigger("Attack");
        if (loserAnim != null) loserAnim.SetTrigger("Defence");
        PlayWeaponSound(winnerWeapon);

        // 実況が反応
        if (commentaryDatabase != null && customCommentaryText != null)
        {
            customCommentaryText.text = $"{winnerName}{commentaryDatabase.GetAttackComment()}";
        }
        yield return new WaitForSeconds(actionInterval);


        // --- ③ 3手目：それに対して返しの攻撃モーション発火 (倒れる側の攻撃) ---
        if (loserAnim != null) loserAnim.SetTrigger("Attack");
        if (winnerAnim != null) winnerAnim.SetTrigger("Defence");
        PlayWeaponSound(loserWeapon);

        // 実況が反応
        if (commentaryDatabase != null && customCommentaryText != null)
        {
            customCommentaryText.text = $"{loserName}{commentaryDatabase.GetAttackComment()}";
        }
        yield return new WaitForSeconds(actionInterval);


        // --- ④ 4手目：勝つ側からの返しの攻撃発火 (トドメの一撃 / クリティカル演出) ---
        if (winnerAnim != null) winnerAnim.SetTrigger("Attack");
        if (loserAnim != null) loserAnim.SetTrigger("Defence");
        PlayWeaponSound(winnerWeapon);

        // 実況が反応 (この時 Critical Comments の中から抽選)
        if (commentaryDatabase != null && customCommentaryText != null)
        {
            customCommentaryText.text = $"{winnerName}の{commentaryDatabase.GetCriticalComment()}";
        }
        yield return new WaitForSeconds(actionInterval);


        // --- ⑤ 決着：TriggerのDownの条件からDownモーションを発火、表情切り替え ---
        if (loserAnim != null)
        {
            loserAnim.SetTrigger("Down");
            loserAnim.SetInteger("FaceType", 3); // 3: 痛そうな顔(Sad_Face)
        }
        if (winnerAnim != null)
        {
            winnerAnim.SetTrigger("Victory");
            winnerAnim.SetInteger("FaceType", 2); // 2: 喜ぶ顔(Happy_Face)
        }

        // 勝敗に応じたBGMの切り替え再生
        bool isFirstMatch = (playerData.battleLogs == null || playerData.battleLogs.Count == 0);
        if (AudioManager.Instance != null)
        {
            if (currentResult.playerWin)
                AudioManager.Instance.PlayBGM(isFirstMatch ? AudioManager.Instance.bgmWinFirst : AudioManager.Instance.bgmResult);
            else
                AudioManager.Instance.PlayBGM(isFirstMatch ? AudioManager.Instance.bgmLoseFirst : AudioManager.Instance.bgmResult);
        }

        // ⑥ 実況に上段の吹き出し「あーーっと！！選手倒れました！！」
        if (customCommentaryText != null)
        {
            customCommentaryText.text = "あーーっと！！選手倒れました！！";
        }
        yield return new WaitForSeconds(1.8f);

        // ⑦ 実況に上段の吹き出し「どうやら決着がついたようです！！！」
        if (customCommentaryText != null)
        {
            customCommentaryText.text = "どうやら決着がついたようです！！！";
        }
        yield return new WaitForSeconds(finishDelay);


        // 次のリザルト画面(Resultパネル)へ移る直前に、3Dステージの表示を完全に消す
        if (battleStage3D != null)
        {
            battleStage3D.SetActive(false);
        }

        // ⑧ 次の画面に遷移
        GameManager.Instance.OnBattleFinished();
    }

    private void PlayWeaponSound(string armID)
    {
        if (AudioManager.Instance == null) return;
        int seIndex = Mathf.Abs(armID.GetHashCode()) % Mathf.Max(1, AudioManager.Instance.seAttackList.Count);
        if (AudioManager.Instance.seAttackList.Count > seIndex)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seAttackList[seIndex]);
        }
    }
}