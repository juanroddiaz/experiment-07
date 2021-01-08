using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string Name;
    public GameObject GamePrefab;
    public Sprite MenuImage;
    public Vector2 TimeBonusCellPosition;
    public AudioClip Music;
    public List<SpikeData> SpikesPosition;
}

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameDataLoader _dataLoader;
    [SerializeField]
    private SceneLoader _sceneLoader;
    [SerializeField]
    private SoundController _soundController;
    [Header("Audio")]
    [SerializeField]
    private AudioListener _audioListener;
    [SerializeField]
    private AudioClip _menuMusic;

    [SerializeField]
    private List<LevelData> _levelData;

    public static GameController Instance { get; private set; }
    public SceneLoader SceneLoader { get { return _sceneLoader; } }
    public GameDataLoader DataLoader => _dataLoader;
    
    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        if (objs.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        _dataLoader.Initialize(GetAllLevelNames());
        _sceneLoader.Initialize();
    }

    private List<string> GetAllLevelNames()
    {
        var list = new List<string>();
        foreach (var entry in _levelData)
        {
            list.Add(entry.Name);
        }
        return list;
    }

    public List<Sprite> GetMenuLevelSprites()
    {
        var list = new List<Sprite>();
        foreach (var entry in _levelData)
        {
            list.Add(entry.MenuImage);
        }
        return list;
    }

    public string GetLevelNameByIdx(int idx)
    {
        if (idx >= 0 && idx < _levelData.Count)
        {
            return _levelData[idx].Name;
        }
        Debug.LogError("Wrong index for level name!");
        return "???";
    }

    public void LoadGameplayScenario(int carouselIndex)
    {
        _soundController.Stop();
        _dataLoader.SaveLastSelectedLevel(carouselIndex);
        _sceneLoader.LoadGameplayScene();
    }

    public void LoadMainMenu()
    {
        _soundController.Play(_menuMusic);
        _sceneLoader.LoadMainMenu();
    }

    public void ToggleCurrentLevelMusic(bool toggle)
    {
        if (toggle)
        {
            _soundController.Play(_levelData[_dataLoader.LastSelectedLevel].Music);
        }
        else
        {
            _soundController.Stop();
        }
    }

    public LevelData GetSelectedLevelData()
    {
        return _levelData[_dataLoader.LastSelectedLevel];
    }

    public void DeleteData()
    {
        _dataLoader.DeleteData();
        _sceneLoader.ReloadScene();
        _soundController.IsEnabled = _dataLoader.MusicOn;
    }

    public void ToggleMusicEnabled()
    {
        _soundController.IsEnabled = !_soundController.IsEnabled;
        _dataLoader.SaveMusicOnOption(_soundController.IsEnabled);
    }
}
