using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameLevelData
{
    public string Name = "";
    public int MaxHeight = 0;
    public int MaxPlatforms = 0;
}

public class GameDataLoader : MonoBehaviour
{
    public List<GameLevelData> GameData { get; private set; }
    public bool MusicOn { get; private set; }
    public int LastSelectedLevel { get; private set; }

    private static string _lastSelectedLevelKey = "LastSelectedLevel";
    private static string _musicOnKey = "MusicOn";

    public void Initialize(List<string> levelNames)
    {
        GameData = new List<GameLevelData>();
        foreach (var name in levelNames)
        {
            if (PlayerPrefs.HasKey(name + "_height"))
            {
                GameData.Add(new GameLevelData {
                    Name = name,
                    MaxHeight = PlayerPrefs.GetInt(name + "_height"),
                    MaxPlatforms = PlayerPrefs.GetInt(name + "_platforms"),
                });
            }
        }

        LastSelectedLevel = 0;
        if (PlayerPrefs.HasKey(_lastSelectedLevelKey))
        {
            LastSelectedLevel = PlayerPrefs.GetInt(_lastSelectedLevelKey);
        }
        MusicOn = true;
        if (PlayerPrefs.HasKey(_musicOnKey))
        {
            MusicOn = PlayerPrefs.GetInt(_musicOnKey, 1) == 1 ? true : false;
        }
    }

    public GameLevelData GetLevelMaxData(string levelName)
    {
        var entry = GameData.Find(k => string.Equals(k.Name, levelName));
        if (entry == null)
        {
            Debug.Log("No data for level " + levelName);
            return new GameLevelData
            {
                Name = levelName,
                MaxHeight = 0,
                MaxPlatforms = 0,
            };
        }
        return entry;
    }

    public bool TrySaveLevelMaxRecord(GameLevelData data)
    {
        var key = GameData.Find(k => string.Equals(k.Name, data.Name));
        if (key == null)
        {
            GameData.Add(new GameLevelData
            {
                Name = data.Name,
                MaxHeight = data.MaxHeight,
                MaxPlatforms = data.MaxPlatforms,
            });

            // save
            PlayerPrefs.SetInt(data.Name + "_height", data.MaxHeight);
            PlayerPrefs.SetInt(data.Name + "_platforms", data.MaxPlatforms);
            return true;
        }

        bool dataSaved = false;
        if (key.MaxHeight < data.MaxHeight)
        {
            key.MaxHeight = data.MaxHeight;
            PlayerPrefs.SetInt(data.Name + "_height", data.MaxHeight);
            dataSaved = true;
        }
        if (key.MaxPlatforms < data.MaxPlatforms)
        {
            key.MaxPlatforms = data.MaxPlatforms;
            PlayerPrefs.SetInt(data.Name + "_platforms", data.MaxPlatforms);
            dataSaved = true;
        }

        return dataSaved;
    }

    public void DeleteData()
    {
        foreach (var data in GameData)
        {
            data.MaxHeight = 0;
            data.MaxPlatforms = 0;
            PlayerPrefs.SetInt(data.Name + "_height", data.MaxHeight);
            PlayerPrefs.SetInt(data.Name + "_platforms", data.MaxPlatforms);
        }
        SaveLastSelectedLevel(0);
        SaveMusicOnOption(true);
    }

    public void SaveLastSelectedLevel(int idx)
    {
        LastSelectedLevel = idx;
        PlayerPrefs.SetInt(_lastSelectedLevelKey, idx);
    }

    public void SaveMusicOnOption(bool on)
    {
        MusicOn = on;
        PlayerPrefs.SetInt(_musicOnKey, on ? 1 : 0);
    }
}
