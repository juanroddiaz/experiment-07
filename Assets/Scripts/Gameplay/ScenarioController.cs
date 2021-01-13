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


    private GameLevelData _levelData;
    private Vector3 _baseCameraPivotPos = Vector3.zero;

    public float CurrentMaxHeight { get; private set; }
    public bool LevelStarted { get; private set; }
    public int LevelHeight { get; private set; }

    private void Awake()
    {
        _character.Initialize(this);
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

        //var levelObj = GameObject.Instantiate(levelEntry.GamePrefab, _tilemapGrid.transform);
        //levelObj.transform.localPosition = _levelLocalPosition;
        _baseCameraPivotPos = _cameraPivotPosition.position;
        InitializeLevel();
        CurrentMaxHeight = 0.0f;
        _hud.UpdateHeightCounter(0);
        TogglePause(false);
        LevelStarted = false;
    }

    private void InitializeLevel()
    {
        if (_config == null)
        {
            return;
        }

        int layerIndex = 0;
        float layerPosition = _startLayerPosition;
        foreach (var layerChunk in _config.LayersData)
        {
            for (int i = 0; i < layerChunk.LayersAmount; i++)
            {
                var layerObj = Instantiate(_layerPrefab, _layersParent);
                layerObj.transform.localPosition = new Vector3(0.0f, layerPosition, 0.0f);
                PlatformLayerLogic logic = layerObj.GetComponent<PlatformLayerLogic>();
                logic.Initialize(this, layerChunk, layerIndex, i);
                layerPosition += 2.0f;
            }

            layerIndex++;
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

    //private void CreateTimeBonusInCell(Vector3 place, int n, int p)
    //{
    //    var emptyCellObj = Instantiate(_timeBonusPrefab, _emptyCellsParent);
    //    emptyCellObj.name = "Timecell_" + n.ToString() + "_" + p.ToString();
    //    emptyCellObj.transform.position = place;
    //    emptyCellObj.GetComponent<TimeObjectLogic>().Initialize(_hud, _timeBonusInSeconds, () => 
    //    {
    //        CreateCoinInCell(place, n, p);
    //        CurrentLevelTime += _timeBonusInSeconds;
    //    });
    //}

    //private void CreateSpikeInCell(Vector3 place, SpikeData spike)
    //{
    //    var emptyCellObj = Instantiate(_spikePrefab, _emptyCellsParent);
    //    emptyCellObj.name = "Spike_" + spike.CellCoords.x.ToString() + "_" + spike.CellCoords.y.ToString();
    //    emptyCellObj.transform.position = place;
    //    emptyCellObj.GetComponent<SpikeObjectLogic>().Initialize(spike.Position);
    //}

    //private void CreateCoinInCell(Vector3 place, int n, int p)
    //{
    //    var emptyCellObj = Instantiate(_coinPrefab, _emptyCellsParent);
    //    emptyCellObj.name = "cell_" + n.ToString() + "_" + p.ToString();
    //    emptyCellObj.transform.position = place;
    //    emptyCellObj.GetComponent<CoinObjectLogic>().Initialize(_hud);
    //}

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


        //CurrentMaxHeight -= Time.deltaTime;
        //if (CurrentMaxHeight <= 0.0f)
        //{
        //    // end level
        //    LevelStarted = false;
        //    CurrentMaxHeight = 0.0f;
        //}

        // hud update time and check level end
        //_hud.UpdateLevelCountdown(CurrentMaxHeight, !LevelStarted);
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
