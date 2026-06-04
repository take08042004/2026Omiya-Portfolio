using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommentaryManager : MonoBehaviour
{
    [Header("Database")]
    public CommentaryDatabase database;

    [Header("UI")]
    public Text commentaryText;

    [Header("Settings")]
    public float displayTime = 1.5f;

    private Coroutine displayCoroutine;

    public void ShowCommentary(BattleCalculator.BattleLog log)
    {
        if (database == null || commentaryText == null || log == null)
        {
            return;
        }

        string comment;

        if (log.critical)
        {
            comment = database.GetCriticalComment();
        }
        else
        {
            comment = database.GetAttackComment();
        }

        ShowText(comment);
    }

    public void ShowVictoryComment()
    {
        if (database == null) return;

        ShowText(database.GetVictoryComment());
    }

    public void ShowDefeatComment()
    {
        if (database == null) return;

        ShowText(database.GetDefeatComment());
    }

    private void ShowText(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(DisplayRoutine(text));
    }

    private IEnumerator DisplayRoutine(string text)
    {
        commentaryText.text = text;

        yield return new WaitForSeconds(displayTime);

        commentaryText.text = "";
        displayCoroutine = null;
    }
}