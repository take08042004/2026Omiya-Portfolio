using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // Buttonコンポーネントを使用するため追加

public class HistoryListScreen : MonoBehaviour
{
    public GameObject buttonPrefab; //キャラ選択ボタンのプレハブ
    public Transform contentParent; //スクロールビューのContentオブジェクト
    
    [Header("追加: プレビュー用のマージスクリプト")]
    public EquipmentMerger previewMerger; 

    private const int MAX_DISPLAY = 10; //最大表示数

    void OnEnable()
    {
        LoadAndDisplay();
    }

    void LoadAndDisplay()
    {
        //既存のボタンを削除
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        //SaveSystemからのデータ取得
        List<CharacterData> dataList = SaveSystem.LoadCharacters();

        if(dataList == null || dataList.Count == 0)
        {
            return;
        }

        //新しい順に並べる（playerIndex降順）
        dataList.Sort((a, b) => b.playerIndex.CompareTo(a.playerIndex));

        //ボタンのPrefab生成
        int count = Math.Min(dataList.Count, MAX_DISPLAY);

        for (int i = 0; i < count; i++)
        {
            CharacterData data = dataList[i];
            GameObject obj = Instantiate(buttonPrefab, contentParent);
            HistoryItemButton button = obj.GetComponent<HistoryItemButton>();

            button.Setup(data);

            // --- 追加: ボタンが押されたときのイベントを登録 ---
            Button uiButton = obj.GetComponent<Button>();
            if (uiButton != null)
            {
                // ラムダ式を使って、ループ内の特定のdataを関数に渡す
                uiButton.onClick.AddListener(() => OnCharacterSelected(data));
            }
        }
        
        // 最初（最新）のデータがあれば、初期表示としてプレビューに反映しておく
        if (dataList.Count > 0 && previewMerger != null)
        {
            previewMerger.ApplyCharacterData(dataList[0]);
        }
    }

    /// <summary>
    /// 追加: 履歴内のキャラクターが選択されたときの処理
    /// </summary>
    private void OnCharacterSelected(CharacterData data)
    {
        // 1. 画面上の3Dモデルを選択されたデータに着替えさせる
        if (previewMerger != null)
        {
            previewMerger.ApplyCharacterData(data);
        }

        // 2. 必要に応じて、現在選択中のデータをGameManager等にキャッシュする処理をここに書く
        // GameManager.Instance.SelectCharacter(data);
    }

    public void OnBack()
    {
        GameManager.Instance.BackToMenuFromHistory();
    }
}