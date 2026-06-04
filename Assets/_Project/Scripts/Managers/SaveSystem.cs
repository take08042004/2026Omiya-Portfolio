using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class SaveSystem
{
    // パス設定
    private static string SaveFolderPath => Path.Combine(Application.dataPath, "_Project", "DataBase", "SaveFile");

    private static string ConfigPath => Path.Combine(SaveFolderPath, "SaveConfig.json");

    // 設定データ（インデックス管理）
    [System.Serializable]
    private class SaveConfig
    {
        public int lastPlayerIndex = 0;
    }

    // キャラ一覧ロード（画面3用）
    // キャラ一覧ロード（画面3用）
// 最新の10人だけ読み込む
public static List<CharacterData> LoadCharacters()
{
    List<CharacterData> list = new List<CharacterData>();

    if (!Directory.Exists(SaveFolderPath))
    {
        Directory.CreateDirectory(SaveFolderPath);
        return list;
    }

    SaveConfig config = LoadConfig();

    for (int i = config.lastPlayerIndex; i >= 1 && list.Count < 10; i--)
    {
        string filePath = GetPlayerPath(i);

        if (!File.Exists(filePath))
        {
            continue;
        }

        string json = File.ReadAllText(filePath);
        CharacterData data = JsonUtility.FromJson<CharacterData>(json);

        if (data != null)
        {
            list.Add(data);
        }
    }

    return list;
}
    /*public static List<CharacterData> LoadCharacters()
    {
        List<CharacterData> list = new List<CharacterData>();

        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
            return list;
        }

        // player_*.json を全部取得
        string[] files = Directory.GetFiles(SaveFolderPath, "player_*.json");

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            CharacterData data = JsonUtility.FromJson<CharacterData>(json);

            if (data != null)
            {
                list.Add(data);
            }
        }

        // 新しい順に並べる（playerIndex降順）
        list = list
            .OrderByDescending(d => d.playerIndex)
            .ToList();

        // 最大10件
        if (list.Count > 10)
        {
            list = list.Take(10).ToList();
        }

        return list;
    }*/

    // 新規キャラ保存（画面8用）
    public static void SaveNewCharacter(CharacterData data)
    {
        SaveConfig config = LoadConfig();

        // インデックス更新
        config.lastPlayerIndex++;
        data.playerIndex = config.lastPlayerIndex;

        // 保存日時
        data.saveDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        // JSON保存
        string json = JsonUtility.ToJson(data, true);
        string filePath = GetPlayerPath(data.playerIndex);

        File.WriteAllText(filePath, json);

        // 設定保存
        SaveConfigData(config);
    }

    // 設定読み込み
    private static SaveConfig LoadConfig()
    {
        if (!File.Exists(ConfigPath))
        {
            return new SaveConfig();
        }

        string json = File.ReadAllText(ConfigPath);
        return JsonUtility.FromJson<SaveConfig>(json);
    }

    // 設定保存
    //SaveConfigが他のクラス名と被っていたため、クラス名をSaveConfigDataに変更
    private static void SaveConfigData(SaveConfig config)
    {
        string json = JsonUtility.ToJson(config, true);
        File.WriteAllText(ConfigPath, json);
    }

    // パス生成
    private static string GetPlayerPath(int index)
    {
        return Path.Combine(SaveFolderPath, $"player_{index}.json");
    }

    public static List<CharacterData> LoadAllCharacters()
    {
        List<CharacterData> list = new List<CharacterData>();
        
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
            return list;
        }
        
        string[] files = Directory.GetFiles(SaveFolderPath, "player_*.json");
        
        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            CharacterData data = JsonUtility.FromJson<CharacterData>(json);
            
            if (data != null)
            {
                list.Add(data);
            }
        }
        
        return list;
    }

    public static void UpdateCharacter(CharacterData data)
    {
        if (data == null)
        {
            Debug.LogWarning("SaveSystem: 更新する CharacterData が null です");
            return;
        }
        
        if (data.playerIndex <= 0)
        {
            Debug.LogWarning("SaveSystem: playerIndex が未設定です");
            return;
        }
        
        data.saveDateTime = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        
        string json = JsonUtility.ToJson(data, true);
        string filePath = GetPlayerPath(data.playerIndex);
        
        File.WriteAllText(filePath, json);
    }

    public static CharacterData GetRandomEnemy(int excludePlayerIndex = -1)
    {
        List<CharacterData> candidates = LoadAllCharacters()
        .Where(c => c != null && c.playerIndex != excludePlayerIndex)
        .ToList();
        
        if (candidates.Count == 0)
        {
            return null;
        }
        
        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }
}