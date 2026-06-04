using UnityEngine;
using TMPro;

public class RankingScreen : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text totalWinText;
    public TMP_Text rankText;

    [Header("Avatar")]
    public EquipmentMerger equipmentMerger;

    [Header("Animator")]
    public Animator avatarAnimator;

    [Header("Ranking")]
    public RankingSystem rankingSystem;

    [Header("3D Stage Control")]
    public GameObject rankingStage3D; // ★追加：Canvas直下のRankingStage3DをON/OFFする枠

    private CharacterData playerData;

    void OnEnable()
    {
        playerData = GameManager.Instance.GetSelectedCharacter();

        if (playerData == null)
            return;

        // ★追加：ランキング画面が開いた瞬間に3Dステージを表示
        if (rankingStage3D != null)
        {
            rankingStage3D.SetActive(true);
        }

        // 表示
        totalWinText.text = $"10戦中 {playerData.winCount}勝";

        // ランキング
        int rank = rankingSystem.CalculateRank(playerData);
        rankText.text = $"{rank} 位";

        // アバター
        if (equipmentMerger != null)
        {
            equipmentMerger.ApplyCharacterData(playerData);
        }

        // ポーズ
        if (avatarAnimator != null)
        {
            if (playerData.winCount >= 8)
            {
                avatarAnimator.SetTrigger("TopPose");
            }
            else if (playerData.winCount <= 2)
            {
                avatarAnimator.SetTrigger("LosePose");
            }
            else
            {
                avatarAnimator.SetTrigger("NormalPose");
            }
        }

        // 保存
        playerData.isEnemy = true;
        SaveSystem.UpdateCharacter(playerData);
    }

    public void OnBackToTitle()
    {
        // ★追加：タイトルに戻る前に3Dステージの表示を消す
        if (rankingStage3D != null)
        {
            rankingStage3D.SetActive(false);
        }

        GameManager.Instance.ResetToOpening();
    }
}