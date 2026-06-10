using System.Collections;// 2024-06-01 17:00:00
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class CharacterFinalizeScreen : MonoBehaviour
{
    [Header("Calculator")]

    public BattleCalculator battleCalculator;

    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text levelText;
    public Text hpText;
    public Text attackText;
    public Text defenseText;

    public Text speedText;

    [Header("Settings")]
    public float waitTime = 3f;
    private CharacterData currentData;

    void OnEnable()
    {
        StartCoroutine(FinalizeRoutine());
    }

    private IEnumerator FinalizeRoutine()
    {
        if(GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager.Instance is null.");
            yield break;
        }

        currentData = GameManager.Instance.GetSelectedCharacter();

        if(currentData == null)
        {
            Debug.LogWarning("currentData is null.");
            yield break;
        }

        Debug.Log(currentData.name);

        if(battleCalculator == null)
        {
            yield break;
        }

        // Lvを先に決める
        currentData.level = Random.Range(1, 11);


        // ステータス計算
        battleCalculator.FinalizeCharacter(currentData);

        // 画面表示
        UpdateUI();

        //保存
        SaveSystem.SaveNewCharacter(currentData);
        //GameManager.Instance.SetCurrentCharacter(currentData);
        GameManager.Instance.SetSelectedCharacter(currentData);
        
        // 一定時間待つ
        //yield return new WaitForSeconds(waitTime);

        // 次の画面へ遷移
        //GameManager.Instance.GoToMatchUp();
    }

    private void UpdateUI()
    {
        if(nameText != null)
        {
            nameText.text = currentData.name;
        }

        if(levelText != null)
        {
            levelText.text = "Lv. " + currentData.level;
        }

        if(hpText != null)
        {
            hpText.text = "HP: " + currentData.hpMax;
        }

        if(attackText != null)
        {
            attackText.text = "攻撃: " + currentData.attack;
        }

        if(defenseText != null)
        {
            defenseText.text = "防御: " + currentData.defense;
        }

        if(speedText != null)
        {
            speedText.text = "素早さ: " + currentData.speed;
        }
    }
}