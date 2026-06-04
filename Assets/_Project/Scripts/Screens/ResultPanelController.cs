using UnityEngine;

public class ResultPanelController : MonoBehaviour
{
    [Header("Result Panels")]
    public GameObject winBackground;
    public GameObject loseBackground;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        ShowResult();       
    }

    // Update is called once per frame
    private void ShowResult()
    {
        var result = GameManager.Instance.GetBattleResult();

        //仮で追加
        if(result == null)
        {
            return;
        }

        if(winBackground != null)
        {
            winBackground.SetActive(false);
        }

        if(loseBackground != null)
        {
            loseBackground.SetActive(false);
        }

        if (result.playerWin)
        {
            if(winBackground != null)
            {
                winBackground.SetActive(true);
            }
        }
        else
        {
            if(loseBackground != null)
            {
                loseBackground.SetActive(true);
            }
        }
        
    }

}
