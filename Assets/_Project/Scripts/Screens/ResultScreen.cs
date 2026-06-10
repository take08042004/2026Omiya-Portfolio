using UnityEngine;

public class ResultScreen : MonoBehaviour
{
    [Header("Result Backgrounds (班員のコードを統合)")]
    public GameObject winBackground;
    public GameObject loseBackground;


    [Header("UI Graphics (透過文字画像を別オブジェクトにする場合はここにアタッチ、背景に含まれている場合は空欄でOK)")]
    public GameObject winTextGraphic;
    public GameObject loseTextGraphic;

    [Header("Avatar")]
    public EquipmentMerger equipmentMerger;

    [Header("Animator")]
    public Animator avatarAnimator;

    [Header("3D Stage Control")]
    public GameObject resultStage3D; // Canvas直下のResultStage3DをON/OFFする枠

    private CharacterData playerData;
    private BattleCalculator.BattleResult result;

    void OnEnable()
    {
        playerData = GameManager.Instance.GetSelectedCharacter();
        result = GameManager.Instance.GetBattleResult();

        if (playerData == null || result == null)
            return;

        // 1. リザルト画面が開いた瞬間に3Dステージを表示
        if (resultStage3D != null)
        {
            resultStage3D.SetActive(true);
        }

        // 2. 背景と文字画像の初期化（一旦すべてオフにする）
        if (winBackground != null) winBackground.SetActive(false);
        if (loseBackground != null) loseBackground.SetActive(false);
        if (winTextGraphic != null) winTextGraphic.SetActive(false);
        if (loseTextGraphic != null) loseTextGraphic.SetActive(false);

        // 3. 勝敗に応じて背景と透過文字画像を表示
        if (result.playerWin)
        {
            if (winBackground != null) winBackground.SetActive(true);
            if (winTextGraphic != null) winTextGraphic.SetActive(true); // 勝ち文字画像をON
        }
        else
        {
            if (loseBackground != null) loseBackground.SetActive(true);
            if (loseTextGraphic != null) loseTextGraphic.SetActive(true); // 負け文字画像をON
        }

        // 4. アバター表示の適用
        if (equipmentMerger != null)
        {
            equipmentMerger.ApplyCharacterData(playerData);
        }

        // 5. 勝敗ポーズの実行（ご指定の命名規則 Down）
        if (avatarAnimator != null)
        {
            if (result.playerWin)
            {
                avatarAnimator.SetTrigger("Victory");
            }
            else
            {
                avatarAnimator.SetTrigger("Down");
            }
        }

        // 6. 勝利数加算
        if (result.playerWin)
        {
            playerData.winCount++;
            SaveSystem.UpdateCharacter(playerData);
        }
    }

    // 「つぎへ」ボタン
    public void OnNextButton()
    {
        // 次の画面へ行く前に3Dステージの表示を消す
        if (resultStage3D != null)
        {
            resultStage3D.SetActive(false);
        }

        GameManager.Instance.GoToDigest();
    }
}