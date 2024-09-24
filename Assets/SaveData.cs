using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public int currentLevel;
    public SaveFile saveFile;

    public void SerializeData()
    {
        var json = JsonUtility.ToJson(saveFile);
        File.WriteAllText(Application.dataPath + "/savedata.json", json);
    }

    public void DeserializeData()
    {
        if (File.Exists(Application.dataPath + "/savedata.json"))
        {
            string json = File.ReadAllText(Application.dataPath + "/savedata.json");
            saveFile = JsonUtility.FromJson<SaveFile>(json);
        }
        else
        {
            saveFile = new();
            saveFile.bgmVolume = 0.3f;
            saveFile.voiceVolume = 0.37f;
            saveFile.sfxVolume = 0.4f;
        }
    }

    public static SaveData Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(Instance);
    }
}
