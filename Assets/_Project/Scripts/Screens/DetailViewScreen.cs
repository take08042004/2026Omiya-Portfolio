using UnityEngine;
using UnityEngine.UI;

public class DetailViewScreen : MonoBehaviour
{
    [Header("UI")]
    public Text nameText;
    public Text levelText;
    public Text rankText;
    
    [Header("追加: ステータス表示用UI")]
    public Text hpText;
    public Text attackText;
    public Text defenseText;

    [Header("Avatar")]
    public EquipmentMerger equipmentMerger;

    private CharacterData currentData;

    void OnEnable()
    {
        currentData = GameManager.Instance.GetSelectedCharacter();

        if (currentData != null)
        {
            nameText.text = currentData.name;
            levelText.text = "Lv." + currentData.level;
            rankText.text = "順位:" + currentData.rank;

            // ステータス等の表示を拡張する場合
            if (hpText != null) hpText.text = "HP: " + currentData.hpMax;
            if (attackText != null) attackText.text = "ATK: " + currentData.attack;
            if (defenseText != null) defenseText.text = "DEF: " + currentData.defense;
        }

        if(equipmentMerger != null)
        {
            equipmentMerger.ApplyCharacterData(currentData);
        }
    }

    public void OnBack()
    {
        GameManager.Instance.BackToHistory();
    }
}