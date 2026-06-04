using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ConfirmScreen : MonoBehaviour
{
    [Header("Avatar")]

    public EquipmentMerger equipmentMerger;
    [Header("UI Text")]
    public Text nameText;
    public Text levelText;
    public Text messageText;

    [Header("Button")]
    public CanvasGroup confirmButtonGroup;
    public CanvasGroup backButtonGroup;

    private CharacterData currentData;

    void OnEnable()
    {
        currentData = GameManager.Instance.GetSelectedCharacter();

        if(currentData != null) 
        {
            return;
        }

        UpdateText();
        UpdateAvatar();
        SetButtonsVisible(true);
    }

    private void UpdateText()
    {
        if(nameText != null)
        {
            nameText.text = currentData.name;
        }

        if(levelText != null)
        {
            levelText.text = "Lv. " + currentData.level;
        }

        if(messageText != null)
        {
            messageText.text = "これでいい?";
        }
    }

    private void UpdateAvatar()
    {
        if(equipmentMerger != null)
        {
            equipmentMerger.ApplyCharacterData(currentData);
        }
    }

    private void SetButtonsVisible(bool visible)
    {
        SetCanvasGroup(confirmButtonGroup, visible);
        SetCanvasGroup(backButtonGroup, visible);
    }

    private void SetCanvasGroup(CanvasGroup group, bool visible)
    {
        if(group == null) return;

        group.alpha = visible ? 1f : 0f;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }

    public void OnConfirmButton()
    {
        GameManager.Instance.ConfirmCharacter();
    }

    public void OnBackButton()
    {
        GameManager.Instance.BackToCustomization();
    }


}