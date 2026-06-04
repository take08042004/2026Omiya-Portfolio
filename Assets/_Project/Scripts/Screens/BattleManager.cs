using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public enum BattlePhase
    {
        WaitCommand,
        Execute,
        Victory,
        Defeat
    }

    [Header("Calculator")]
    public BattleCalculator battleCalculator;

    [Header("Commentary")]
    public CommentaryManager commentaryManager;

    [Header("Animator")]
    public Animator playerAnimator;
    public Animator enemyAnimator;

    [Header("UI")]
    public Text battleLogText;

    [Header("Timing")]
    public float actionInterval = 1.2f;
    public float finishDelay = 2f;

    private CharacterData playerData;
    private CharacterData enemyData;
    private BattleCalculator.BattleResult currentResult;
    private BattlePhase currentPhase;

    void OnEnable()
    {
        playerData = GameManager.Instance.GetSelectedCharacter();
        enemyData = GameManager.Instance.GetEnemyCharacter();

        currentPhase = BattlePhase.WaitCommand;

        if (battleLogText != null)
        {
            battleLogText.text = "作戦を選択してください";
        }

        // 戦闘BGMの再生
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgmBattle);
        }
    }

    public void OnAttackCommand() => SelectCommand(BattleCommand.Attack);
    public void OnGuardCommand() => SelectCommand(BattleCommand.Guard);
    public void OnRandomCommand() => SelectCommand(BattleCommand.Random);

    private void SelectCommand(BattleCommand command)
    {
        if (currentPhase != BattlePhase.WaitCommand) return;

        currentResult = battleCalculator.SimulateBattle(playerData, enemyData, command);
        GameManager.Instance.SetBattleResult(currentResult);

        StartCoroutine(BattleRoutine());
    }

    private IEnumerator BattleRoutine()
    {
        currentPhase = BattlePhase.Execute;

        foreach (BattleCalculator.BattleLog log in currentResult.logs)
        {
            yield return StartCoroutine(PlayLog(log));
        }

        // --- 決着演出フェーズ ---
        bool isFirstMatch = (playerData.battleLogs == null || playerData.battleLogs.Count == 0);

        if (currentResult.playerWin)
        {
            currentPhase = BattlePhase.Victory;
            if (battleLogText != null) battleLogText.text = "WIN!";

            // アニメーションと表情の制御
            if (playerAnimator != null) playerAnimator.SetTrigger("Victory");
            if (playerAnimator != null) playerAnimator.SetInteger("FaceType", 2); // 2: Happy_Face
            if (enemyAnimator != null) enemyAnimator.SetTrigger("Down");
            if (enemyAnimator != null) enemyAnimator.SetInteger("FaceType", 3); // 3: Sad_Face

            // 1人目かそれ以外かでBGMを出し分け
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(isFirstMatch ? AudioManager.Instance.bgmWinFirst : AudioManager.Instance.bgmResult);
            }
        }
        else
        {
            currentPhase = BattlePhase.Defeat;
            if (battleLogText != null) battleLogText.text = "LOSE...";

            // アニメーションと表情の制御
            if (playerAnimator != null) playerAnimator.SetTrigger("Down");
            if (playerAnimator != null) playerAnimator.SetInteger("FaceType", 3); // 3: Sad_Face
            if (enemyAnimator != null) enemyAnimator.SetTrigger("Victory");
            if (enemyAnimator != null) enemyAnimator.SetInteger("FaceType", 2); // 2: Happy_Face

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(isFirstMatch ? AudioManager.Instance.bgmLoseFirst : AudioManager.Instance.bgmResult);
            }
        }

        yield return new WaitForSeconds(finishDelay);
        GameManager.Instance.OnBattleFinished();
    }

    private IEnumerator PlayLog(BattleCalculator.BattleLog log)
    {
        // 1. 前進・攻撃モーション発動
        PlayAttackAnimation(log.playerTurn);
        
        // 2. 攻撃・防御SEの再生
        PlayActionSound(log);

        yield return new WaitForSeconds(0.3f);

        // 3. ログ・実況表示
        if (battleLogText != null) battleLogText.text = log.message;
        if (commentaryManager != null) commentaryManager.ShowCommentary(log);

        yield return new WaitForSeconds(actionInterval);

        // 4. 後退・待機へ戻る
        PlayReturnAnimation(log.playerTurn);
    }

    private void PlayActionSound(BattleCalculator.BattleLog log)
    {
        if (AudioManager.Instance == null) return;

        // もしログが敵の攻撃をプレイヤーが「防御（Guard）」した瞬間などの判定ロジックがあればガード音
        // 今回は簡易的に、クリティカルやダメージ状況、あるいはランダムに武器音を鳴らし分ける
        if (!log.playerTurn && currentResult.logs.Count > 0 && false /* 防御判定のフラグがあれば */)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seGuard);
        }
        else
        {
            // 武器の攻撃音7種をIDやインデックスに基づいて鳴らし分け
            // ここではプレイヤーと敵の武器IDの個性を出すため、文字列ハッシュ等から0〜6のインデックスを算出
            string activeWeaponID = log.playerTurn ? playerData.armID : enemyData.armID;
            int seIndex = Mathf.Abs(activeWeaponID.GetHashCode()) % Mathf.Max(1, AudioManager.Instance.seAttackList.Count);
            
            if (AudioManager.Instance.seAttackList.Count > seIndex)
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.seAttackList[seIndex]);
            }
        }
    }

    private void PlayAttackAnimation(bool playerTurn)
    {
        Animator target = playerTurn ? playerAnimator : enemyAnimator;
        if (target != null) target.SetTrigger("Attack");
    }

    private void PlayReturnAnimation(bool playerTurn)
    {
        // 先ほど組んだステートマシンの通り、AttackからIdleへは「Has Exit Time」で自動で戻るため、
        // もし特有の"Return"トリガーを使っていない場合はこの関数は空（自動待機）で問題ありません。
    }
}