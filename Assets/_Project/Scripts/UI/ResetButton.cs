using UnityEngine;

public class ResetButton : MonoBehaviour
{
    /// <summary>
    /// 全てをリセットしてオープニング画面に戻る
    /// </summary>
    public void ResetGame()
    {
        if (GameManager.Instance != null)
        {
            // GameManager のリセット処理を呼び出す
            GameManager.Instance.ResetToOpening();
            Debug.Log("ゲームをリセットしました。");
        }
    }

    /// <summary>
    /// カスタマイズを破棄してメニュー画面に戻る（作成中などの場合）
    /// </summary>
    public void BackToMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BackToMenu();
        }
    }
}