using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameLevelData
{
    public string Name = "";
    public int MaxCoins = 0;
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
            if (PlayerPrefs.HasKey(name))
            {
                GameData.Add(new GameLevelData {
                    Name = name,
                    MaxCoins = PlayerPrefs.GetInt(name)
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

    public int GetLevelMaxCoins(string levelName)
    {
        var key = GameData.Find(k => string.Equals(k.Name, levelName));
        if (key == null)
        {
            Debug.Log("No data for level " + levelName);
            return 0;
        }
        return key.MaxCoins;
    }

    public bool TrySaveLevelMaxCoins(GameLevelData data)
    {
        var key = GameData.Find(k => string.Equals(k.Name, data.Name));
        if (key == null)
        {
            GameData.Add(new GameLevelData
            {
                Name = data.Name,
                MaxCoins = data.MaxCoins
            });

            // save
            PlayerPrefs.SetInt(data.Name, data.MaxCoins);
            return true;
        }

        if (key.MaxCoins < data.MaxCoins)
        {
            key.MaxCoins = data.MaxCoins;
            // save
            PlayerPrefs.SetInt(data.Name, data.MaxCoins);
            return true;
        }

        return false;
    }

    public void DeleteData()
    {
        foreach (var data in GameData)
        {
            data.MaxCoins = 0;
            PlayerPrefs.SetInt(data.Name, 0);
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
