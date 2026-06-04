using UnityEngine;
using UnityEngine.UI;



public class CustomizationScreen : MonoBehaviour
{
    [Header("Controller")]
    public DrumRollController headRollController;
    public DrumRollController bodyRollController;
    public DrumRollController armRollController;
    public DrumRollController legRollController;

    [Header("Avatar")]
    public EquipmentMerger equipmentMerger;
    [Header("UI")]
    public Text selectedPartText;

    private CharacterData currentData;

    void OnEnable()
    {
        currentData = GameManager.Instance.GetSelectedCharacter();

        if(currentData == null)
        {
            currentData = new CharacterData();
        }

        RegisterEvents();

        if (GameManager.Instance.HasLastRandomPartType()){
            PartData.PartType lastType = GameManager.Instance.GetLastRandomPartType();
            
            if (equipmentMerger != null)
            {
                equipmentMerger.ApplyCharacterData(currentData);
            }
            
            GameManager.Instance.ClearLastRandomPartType();
        }
        else
        {
            if (equipmentMerger != null)
            {
                equipmentMerger.ApplyCharacterData(currentData);
            }
        }

        if (equipmentMerger != null)
        {
            equipmentMerger.ApplyCharacterData(currentData);
        }

    }

    void OnDisable()
    {
        UnregisterEvents();
    }

     private void RegisterEvents()
    {
        Register(headRollController);
        Register(bodyRollController);
        Register(armRollController);
        Register(legRollController);
    }

    private void UnregisterEvents()
    {
        Unregister(headRollController);
        Unregister(bodyRollController);
        Unregister(armRollController);
        Unregister(legRollController);
    }

    private void Register(DrumRollController controller)
    {
        if (controller == null) return;

        controller.onPartChanged -= OnPartSelected;
        controller.onPartChanged += OnPartSelected;
    }

    private void Unregister(DrumRollController controller)
    {
        if (controller == null) return;

        controller.onPartChanged -= OnPartSelected;
    }

    public void OnPartSelected(PartData part)
    {
        if(part == null || currentData == null) return;

        switch (part.type)
        {
            case PartData.PartType.Head:
                currentData.headID = part.partID;
                break;
            case PartData.PartType.Body:
                currentData.bodyID = part.partID;
                break;
            case PartData.PartType.Arm:
                currentData.armID = part.partID;
                break;
            case PartData.PartType.Leg:
                currentData.legID = part.partID;
                break;
        }

         if (selectedPartText != null)
        {
            selectedPartText.text = $"選択中：{part.partName}";
        }

        if (equipmentMerger != null)
        {
            equipmentMerger.ApplyCharacterData(currentData);
        }

        GameManager.Instance.SetSelectedCharacter(currentData);
    }

    //確認画面への遷移
    public void OnNext()
    {
        GameManager.Instance.SetSelectedCharacter(currentData);
        GameManager.Instance.GoToConfirm();
    }

    //ランダム画面への遷移
    public void OnRandom()
    {
        GameManager.Instance.SetSelectedCharacter(currentData);
        GameManager.Instance.GoToRandomRoll();
    }
}