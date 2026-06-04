using UnityEngine;

public class BattleCommandScreen : MonoBehaviour
{
    [Header("Calculator")]
    public BattleCalculator battleCalculator;

    [Header("3D Stage Control")]
    public GameObject battleStage3D; // ★追加：Canvas直下のBattleStage3Dを紐付けます

    private CharacterData playerData;
    private CharacterData enemyData;

    void OnEnable()
    {
        playerData = GameManager.Instance.GetSelectedCharacter();
        enemyData = GameManager.Instance.GetEnemyCharacter();

        // ★追加：作戦選択画面に入った瞬間に3Dステージを表示させてにらみ合わせる
        if (battleStage3D != null)
        {
            battleStage3D.SetActive(true);
        }

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
        BattleCalculator.BattleResult result = battleCalculator.SimulateBattle(playerData, enemyData, command);
        GameManager.Instance.SetBattleResult(result);
        GameManager.Instance.GoToBattleScene();
    }
}