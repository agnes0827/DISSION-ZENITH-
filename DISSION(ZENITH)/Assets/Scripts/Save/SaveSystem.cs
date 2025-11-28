using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string fileName = "saveData.json";

    public static void SaveGame()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("GameStateManager.Instance가 없습니다. 세이브 실패");
            return;
        }

        SaveData data = GameStateManager.Instance.CreateSaveData();
        string json = JsonUtility.ToJson(data, true);

        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, json);

        Debug.Log("게임 세이브 완료: " + path);
    }

    // (참고용) 나중에 로드 만들 때 쓸 함수 틀
    public static SaveData LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning("세이브 파일이 없습니다: " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }
}
