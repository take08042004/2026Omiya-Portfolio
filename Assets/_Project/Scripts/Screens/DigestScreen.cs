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

        // 2〜10戦
        for (int i = 2; i <= 10; i++)
        {
            CharacterData enemy =
                SaveSystem.GetRandomEnemy(playerData.playerIndex);

            if (enemy == null)
            {
                Debug.LogError($"敵キャラクターが見つかりません。戦闘 {i}。");
                continue;
            }

            BattleCalculator.BattleResult result =
                battleCalculator.SimulateBattle(
                    playerData,
                    enemy,
                    BattleCommand.Random
                );

            bool win = result.playerWin;

            digestResults.Add(win);

            playerData.battleLogs.Add(win);

            if (win)
            {
                playerData.winCount++;
            }

            RefreshUI();

            yield return new WaitForSeconds(interval);
        }

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
