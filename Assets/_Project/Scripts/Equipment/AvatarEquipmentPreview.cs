using UnityEngine;

public class AvatarEquipmentPreview : MonoBehaviour
{
    [Header("装備を付ける親Transform")]
    public Transform headParent;
    public Transform bodyParent;
    public Transform armParent;
    public Transform legParent;

    private GameObject currentHead;
    private GameObject currentBody;
    private GameObject currentArm;
    private GameObject currentLeg;

    public void EquipPart(PartData part)
    {
        if (part == null)
        {
            return;
        }

        if (part.modelPrefab == null)
        {
            Debug.LogWarning(part.partName + " の modelPrefab が設定されていません");
            return;
        }

        switch (part.type)
        {
            case PartData.PartType.Head:
                ReplacePart(ref currentHead, part.modelPrefab, headParent);
                break;

            case PartData.PartType.Body:
                ReplacePart(ref currentBody, part.modelPrefab, bodyParent);
                break;

            case PartData.PartType.Arm:
                ReplacePart(ref currentArm, part.modelPrefab, armParent);
                break;

            case PartData.PartType.Leg:
                ReplacePart(ref currentLeg, part.modelPrefab, legParent);
                break;
        }
    }

    private void ReplacePart(ref GameObject currentPart, GameObject newPrefab, Transform parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("装備を付ける親Transformが設定されていません");
            return;
        }

        if (currentPart != null)
        {
            Destroy(currentPart);
        }

        currentPart = Instantiate(newPrefab, parent);

        currentPart.transform.localPosition = Vector3.zero;
        currentPart.transform.localRotation = Quaternion.identity;
        currentPart.transform.localScale = Vector3.one;
    }

    public void ClearAllParts()
    {
        ClearPart(ref currentHead);
        ClearPart(ref currentBody);
        ClearPart(ref currentArm);
        ClearPart(ref currentLeg);
    }

    private void ClearPart(ref GameObject currentPart)
    {
        if (currentPart != null)
        {
            Destroy(currentPart);
            currentPart = null;
        }
    }
}
