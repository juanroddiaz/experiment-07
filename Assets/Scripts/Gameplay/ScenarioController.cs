using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ScenarioController : MonoBehaviour
{
    [SerializeField]
    private CharacterManager _character;
    [SerializeField]
    private HudGameplayController _hud;
    [SerializeField]
    private GameObject _layerPrefab;
    [SerializeField]
    private CoinObjectLogic _coinPrefab;
    [SerializeField]
    private TimeObjectLogic _timeBonusPrefab;
    [SerializeField]
    private Vector2 _characterInitialPosition;
    [SerializeField]
    private Transform _cameraPivotPosition;
    [Header("Platforms")]
    [SerializeField]
    private PlatformCatalogue _catalogue;
    [SerializeField]
    private ScenarioConfig _config;
    [SerializeField]
    private Transform _layersParent;
    [SerializeField]
    private float _startLayerPosition = -2.0f;
    [SerializeField]
    private int _layerInstancesAmount = 20;


    private GameLevelData _levelData;
    private Vector3 _baseCameraPivotPos;
    private List<PlatformLayerLogic> _layerInstances = new List<PlatformLayerLogic>();
    private float _currentLayerPosition = 0;
    private int _currentReachedLayerIndex = 0;
    private int _newLayerInstantiationIndex = 0;
    private int _lastInstantiatedLayer = 0;
    private int _lastInstantiatedChunk = 0;

    public float CurrentMaxHeight { get; private set; }
    public bool LevelStarted { get; private set; }
    public int LevelHeight { get; private set; }

    private void Awake()
    {
        // instantiated objects in scene, let's keep it without too many objects
        _newLayerInstantiationIndex = _layerInstancesAmount / 2;
        InitializeScene();
    }

    public void InitializeScene()
    {
        _character.Initialize(this);
        _character.transform.position = _characterInitialPosition;
        _hud.Initialize(this, _character.OnSwipe, _character.OnLeftDown, _character.OnRightDown, _character.OnButtonUp);
        // level init
        LevelData levelEntry = GameController.Instance.GetSelectedLevelData();
        _levelData = new GameLevelData
        {
            Name = levelEntry.Name,
            MaxHeight = 0
        };
        _levelData.MaxHeight = GameController.Instance.DataLoader.GetLevelMaxHeight(levelEntry.Name);
        Debug.Log("Level " + _levelData.Name + " max height: " + _levelData.MaxHeight.ToString());

        _cameraPivotPosition.position = new Vector3(0.0f, 0.0f, -5.0f);
        _baseCameraPivotPos = _cameraPivotPosition.position;
        InitializeLevel();
        CurrentMaxHeight = 0.0f;
        TogglePause(false);
        LevelStarted = false;
    }

    private void InitializeLevel()
    {
        if (_layerInstances.Count > 0)
        {
            foreach (var layer in _layerInstances)
            {
                Destroy(layer.gameObject);
            }
        }
        _layerInstances.Clear();

        _currentReachedLayerIndex = 0;
        _lastInstantiatedLayer = 0;
        _lastInstantiatedChunk = 0;
        _currentLayerPosition = _startLayerPosition;
        foreach (var layerChunk in _config.LayersData)
        {
            for (int i = 0; i < layerChunk.LayersAmount; i++)
            {
                CreateLayer(layerChunk);
                if (_lastInstantiatedLayer >= _layerInstancesAmount)
                {
                    return;
                }
            }

            _lastInstantiatedChunk++;
        }
    }

    private void CreateLayer(LayersChunkData layerChunk)
    {
        var layerObj = Instantiate(_layerPrefab, _layersParent);
        layerObj.transform.localPosition = new Vector3(0.0f, _currentLayerPosition, 0.0f);
        PlatformLayerLogic logic = layerObj.GetComponent<PlatformLayerLogic>();
        logic.Initialize(this, layerChunk, _lastInstantiatedLayer);
        _layerInstances.Add(logic);
        _currentLayerPosition += 2.0f;
        _lastInstantiatedLayer++;
    }

    public void UpdateReachedLayerIndex(int index, bool platformFirstTouched)
    {
        if (platformFirstTouched)
        {
            _hud.UpdatePlatformCounter();
        }

        if (_currentReachedLayerIndex < index)
        {
            _currentReachedLayerIndex = index;
            if (_currentReachedLayerIndex > _newLayerInstantiationIndex)
            {
                UpdateNewLayers();
            }
        }
    }

    private void UpdateNewLayers()
    {
        int chunkIndex = _lastInstantiatedChunk;
        if (_config.LayersData.Count <= _lastInstantiatedChunk)
        {
            chunkIndex = _lastInstantiatedChunk - 1;
        }

        int currentLayersPerChunk = 0;
        for(int i=0; i<=chunkIndex; i++)
        {
            currentLayersPerChunk += _config.LayersData[chunkIndex].LayersAmount;
        }

        Destroy(_layerInstances[0].gameObject);
        _layerInstances.RemoveAt(0);
        var chunk = _config.LayersData[chunkIndex];
        CreateLayer(chunk);

        // update chunk index
        if (currentLayersPerChunk <= _lastInstantiatedLayer)
        {
            if (_config.LayersData.Count > _lastInstantiatedChunk)
            {
                _lastInstantiatedChunk++;
            }
        }
    }

    public int GetPlatformAmountByDifficulty(PlatformDifficulty difficulty)
    {
        return _catalogue.LayerData.Find(x => x.Difficulty == difficulty).Amount;
    }

    public List<PlatformConfig> GetPlatformsByDifficulty(PlatformDifficulty difficulty)
    {
        return _catalogue.Platforms.FindAll(x => x.Difficulty == difficulty);
    }

    public void StartLevel()
    {
        LevelStarted = true;
        GameController.Instance.ToggleCurrentLevelMusic(true);
        _character.StartLevel();
    }

    public void FinishLevel()
    {
        GameController.Instance.ToggleCurrentLevelMusic(false);
        TogglePause(true);
        if (GameController.Instance.DataLoader.TrySaveLevelMaxCoins(new GameLevelData
        {
            Name = _levelData.Name,
            MaxHeight = LevelHeight
        }))
        {
            _levelData.MaxHeight = LevelHeight;
        }
    }

    public int GetMaxLevelCoins()
    {
        return _levelData.MaxHeight;
    }

    public void TogglePause(bool toggle)
    {
        // cheap but effective
        Time.timeScale = toggle ? 0.0f : 1.0f;
    }

    public void OnCoinCollected(int amount)
    {
        LevelHeight += amount;
    }

    private void Update()
    {
        if (!LevelStarted)
        {
            return;
        }

        if (_character.transform.position.y >= CurrentMaxHeight)
        {
            CurrentMaxHeight = _character.transform.position.y;
            _hud.UpdateHeightCounter(Mathf.FloorToInt(CurrentMaxHeight));
        }

        if (_cameraPivotPosition.position.y <= CurrentMaxHeight)
        {
            _baseCameraPivotPos.y = CurrentMaxHeight;
            _cameraPivotPosition.position = _baseCameraPivotPos;
        }
    }

    public void OnDeath()
    {
        LevelStarted = false;
        _hud.OnDeath();
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
