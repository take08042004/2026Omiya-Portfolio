using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DigestScreen : MonoBehaviour
{
    [Header("Calculator")]
    public BattleCalculator battleCalculator;

    /*[Header("UI")]
    public Text upperText;
    public Text lowerText;*/

    [Header("Result Images")]
    public Image[] resultImages; // 2戦目から10戦目までのImageを格納
    public Sprite winSprite;
    public Sprite loseSprite;

    [Header("Settings")]
    public float interval = 0.5f;

    private CharacterData playerData;

    private List<bool> digestResults =
        new List<bool>();

    void OnEnable()
    {
        digestResults.Clear();

        RefreshUI();


        StartCoroutine(DigestRoutine());
    }

    private IEnumerator DigestRoutine()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManagerが見つかりません。");
            yield break;
        }

        if (battleCalculator == null)
        {
            Debug.LogError("BattleCalculatorがアサインされていません。");
            yield break;
        }

        playerData =
            GameManager.Instance.GetSelectedCharacter();
    
        if (playerData == null)
        {
            Debug.LogError("選択されたキャラクターが見つかりません。");
            yield break;
        }

        if(playerData.battleLogs == null)
        {
            playerData.battleLogs = new List<bool>();
        }

        int currentWins = 0;
        int currentMatches = 0;

        // 1戦目の結果を加算
        BattleCalculator.BattleResult firstResult = GameManager.Instance.GetBattleResult();
        
        if (firstResult != null)
        {
            currentMatches++;
            
            if (firstResult.playerWin)
            {
                currentWins++;
            }
        }

        // 2〜10戦
        for (int i = 2; i <= 10; i++)
        {
            bool win = Random.Range(0, 2) == 0;
            
            digestResults.Add(win);
            playerData.battleLogs.Add(win);

            currentMatches++;
            
            if (win)
            {
                currentWins++;
            }
            
            RefreshUI();
            
            yield return new WaitForSeconds(interval);
        }

        playerData.winCount = currentWins;
        playerData.totalWins = currentWins;
        playerData.totalMatches = currentMatches;

        // Save更新
        SaveSystem.UpdateCharacter(playerData);

        yield return new WaitForSeconds(2f);

        GameManager.Instance.GoToRanking();
    }

    private void RefreshUI()
    {
        for (int i = 0; i < resultImages.Length; i++)
        {
            if (resultImages[i] == null) continue;

            if (i < digestResults.Count)
            {
                resultImages[i].gameObject.SetActive(true);
                resultImages[i].sprite = digestResults[i] ? winSprite : loseSprite;
            }
            else
            {
                resultImages[i].gameObject.SetActive(false);
            }
        }

        /*for (int i = 0; i < digestResults.Count; i++)
        {
            string text =
                $"{i + 2}戦目 : " +
                (digestResults[i] ? "WIN" : "LOSE");

            if (i < 4)
            {
                upperText.text += text + "\n";
            }
            else
            {
                lowerText.text += text + "\n";
            }*/
        }
    }
