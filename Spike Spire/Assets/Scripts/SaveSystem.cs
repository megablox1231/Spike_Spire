using System.IO;
using UnityEngine;

/// <summary>
/// Controls persistant info about game progression including
/// current scene, level, and if forward slash is active with JSON.
/// </summary>
public class SaveSystem {

    SaveData data;
    static string dataFilePath = Path.Combine(Application.persistentDataPath, "GameData.json");

    public SaveSystem() {
        data = new SaveData();
        SetProgress("Area_1", "CamTrigger1");
        SetForwardSlash(false);
    }

    public void SetProgress(string area, string level) {
        data.area = area;
        data.level = level;
    }

    public void SetForwardSlash(bool forwardSlash) {
        data.hasForwardSlash = forwardSlash;
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
        public bool hasForwardSlash;
    }
}
