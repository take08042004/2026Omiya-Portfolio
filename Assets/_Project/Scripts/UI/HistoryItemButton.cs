using UnityEngine;
using UnityEngine.UI;

public class HistoryItemButton : MonoBehaviour
{
    public Text nameText;
    public Image iconImage;

    private CharacterData data;
    private bool isHovered = false;

    //SetUpだとHistoryListScreen.csの中のSetupと表記がずれるため、Set"u"pに変更
    public void Setup(CharacterData characterData)
    {
        data = characterData;

        if(nameText != null)
        {
            nameText.text = $"{data.name}  Lv.{data.level}";
        }
    }

    public void OnClick()
    {
        GameManager.Instance.OnHistoryItemSelected(data);
    }
}