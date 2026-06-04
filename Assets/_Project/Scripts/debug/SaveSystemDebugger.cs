using UnityEngine;
using System.Collections.Generic;

public class SaveSystemDebugger : MonoBehaviour
{
    [Header("テスト実行後にチェックを入れて再生するとデータを10件生成します")]
    public bool generateTestData = false;

    [Header("登録してあるパーツIDのサンプル（データベースにある実在のIDを入れてください）")]
    public string sampleHeadID = "head_01";
    public string sampleBodyID = "body_01";
    public string sampleArmID = "arm_01";
    public string sampleLegID = "leg_01";

    void Awake()
    {
        if (generateTestData)
        {
            CreateTenDummyCharacters();
        }
    }

    private void CreateTenDummyCharacters()
    {
        // 既存のデータをクリアしてテストしたい場合は、一度フォルダ内を空にしてください。
        Debug.Log("--- ダミーデータの自動生成を開始します ---");

        for (int i = 1; i <= 10; i++)
        {
            CharacterData dummy = new CharacterData();
            dummy.name = $"テスト戦士 {i} 号";
            dummy.level = Random.Range(1, 50);
            dummy.rank = Random.Range(1, 100);
            
            // ステータス
            dummy.hpMax = Random.Range(100, 500);
            dummy.attack = Random.Range(10, 50);
            dummy.defense = Random.Range(10, 50);

            // 装備ID（手動設定したPartDataのpartIDと一致させてください）
            dummy.headID = sampleHeadID;
            dummy.bodyID = sampleBodyID;
            dummy.armID = sampleArmID;
            dummy.legID = sampleLegID;

            // SaveSystemの正規ルートで保存（Configのインデックスも自動更新されます）
            SaveSystem.SaveNewCharacter(dummy);
        }

        Debug.Log("--- 10体分のダミーデータの生成が完了しました。Unityを停止し、generateTestDataのチェックを外してください ---");
    }
}