using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{

    private SaveData data;
    private static string dataFilePath = Path.Combine(Application.persistentDataPath, "GameData.json");

    public SaveSystem() {
        data = new SaveData();
        SetProgress("Area_1", "CamTrigger1");
    }

    public void SetProgress(string area, string level) {
        data.area = area;
        data.level = level;
    }

    public SaveData GetData() {
        return data;
    }

    public void Save() {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(dataFilePath, json);
    }

    public void Load() {
        if (File.Exists(dataFilePath)) {
            string json = File.ReadAllText(dataFilePath);
            data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log(data.area + " " + data.level);
        }
    }

    public void ClearData() {
        if (File.Exists(dataFilePath)) {
            File.Delete(dataFilePath);
        }
    }

    public class SaveData {
        public string area;
        public string level;
    }
}
