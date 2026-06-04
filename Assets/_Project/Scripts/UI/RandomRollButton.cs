using UnityEngine;

public class RandomRollButton : MonoBehaviour
{
    [Header("Target Roll")]
    public DrumRollController targetRoll;

    [Header("Part Type")]
    public PartData.PartType targetPartType;

    [Header("Random Visualizer")]
    public RandomRollVisualizer visualizer;

    [Header("Panel")]
    public GameObject randomRollPanel;

     [Header("All Center Rolls")]
    public GameObject headRandomRoll;
    public GameObject bodyRandomRoll;
    public GameObject armRandomRoll;
    public GameObject legRandomRoll;


    public void OnClickRandom()
    {
        GameManager.Instance.GoToRandomRoll();

        if (headRandomRoll != null) headRandomRoll.SetActive(false);
        if (bodyRandomRoll != null) bodyRandomRoll.SetActive(false);
        if (armRandomRoll != null) armRandomRoll.SetActive(false);
        if (legRandomRoll != null) legRandomRoll.SetActive(false);

        if (targetRoll != null)
        {
            targetRoll.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("targetRoll is not assigned.");
            return;
        }

        if (visualizer != null)
        {
            visualizer.StartRoll(targetRoll, targetPartType);
        }
        else
        {
            Debug.LogWarning("visualizer is not assigned.");
        }
    }
}