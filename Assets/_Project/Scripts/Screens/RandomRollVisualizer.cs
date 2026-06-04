using UnityEngine;
using System.Collections;

public class RandomRollVisualizer : MonoBehaviour
{
    private PartData.PartType rollingPartType;


    [Header("Drum Roll")]
    public DrumRollController drumRollController;

    [Header("Settings")]
    public float rollTime = 2.5f; // ドラムロールが回る時間
    public float waitAfterStop = 2.0f; //　停止してから画面5に戻るまでの待ち時間
    public float randomMinPower = 800f;
    public float randomMaxPower = 1600f;

    private Coroutine rollCoroutine;

    void OnEnable()
    {
        
    } 

    void OnDisable()
    {
        if(rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
            rollCoroutine = null;
        }
    }

    private IEnumerator RandomRollRoutine()
    {
        if(drumRollController == null)
        {
            Debug.LogWarning("DrumRollController is not assigned.");
            yield break;
        }

        //　ランダムな速度でドラムロールを回す
        float randomPower = Random.Range(randomMinPower, randomMaxPower);
        drumRollController.StartRandomRoll(randomPower);

        // 一定時間回す
        yield return new WaitForSeconds(rollTime);

        // ドラムロールを止めて、最寄りアイテムにスナップさせる
        drumRollController.StopAndSnapFromExternal();

        PartData resultPart = drumRollController.GetCurrentSelectedPart();
        
        if (resultPart != null)
        {
            CharacterData data = GameManager.Instance.GetSelectedCharacter();
            
            if (data == null)
            {
                data = new CharacterData();
            }
            
            switch (rollingPartType)
            {
                case PartData.PartType.Head:
                    data.headID = resultPart.partID;
                    break;

                case PartData.PartType.Body:
                    data.bodyID = resultPart.partID;
                    break;

                case PartData.PartType.Arm:
                    data.armID = resultPart.partID;
                    break;

                case PartData.PartType.Leg:
                    data.legID = resultPart.partID;
                    break;
            }

             GameManager.Instance.SetSelectedCharacter(data);
             GameManager.Instance.SetLastRandomPartType(rollingPartType);
        }

        // 結果を見せるため、少し待つ
        yield return new WaitForSeconds(waitAfterStop);

        //画面5への遷移
        GameManager.Instance.OnRandomRollFinished();
    }

    public void StartRoll(DrumRollController controller, PartData.PartType partType)
    {
        drumRollController = controller;
        rollingPartType = partType;
        
        if (rollCoroutine != null)
        {
            StopCoroutine(rollCoroutine);
        }
        
        rollCoroutine = StartCoroutine(RandomRollRoutine());
    }
}
