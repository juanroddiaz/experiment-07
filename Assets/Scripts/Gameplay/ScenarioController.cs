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
    private Grid _tilemapGrid;
    [SerializeField]
    private Transform _emptyCellsParent;
    [SerializeField]
    private CoinObjectLogic _coinPrefab;
    [SerializeField]
    private TimeObjectLogic _timeBonusPrefab;
    [SerializeField]
    private float _levelTotalTime = 20.0f;
    [SerializeField]
    private Vector2 _characterInitialPosition;
    [SerializeField]
    private Vector3 _levelLocalPosition = new Vector3(-1.0f, 0.0f, 0.0f);
    [SerializeField]
    private SpikeObjectLogic _spikePrefab;
    [Header("Time Bonus")]
    [SerializeField]
    private int _timeBonusInSeconds = 10;

    private GameLevelData _levelData;
    private Vector2 _timeBonusPosition = Vector2.zero;
    private List<SpikeData> _spikesPosition = new List<SpikeData>();

    public float CurrentLevelTime { get; private set; }
    public bool LevelStarted { get; private set; }
    public int LevelCoins { get; private set; }

    private void Awake()
    {
        _character.Initialize(this);
        _hud.Initialize(this, _character.OnTapDown);
        // level init
        LevelData levelEntry = GameController.Instance.GetSelectedLevelData();
        _timeBonusPosition = levelEntry.TimeBonusCellPosition;
        _spikesPosition = levelEntry.SpikesPosition;
        _levelData = new GameLevelData
        {
            Name = levelEntry.Name,
            MaxCoins = 0
        };
        _levelData.MaxCoins = GameController.Instance.DataLoader.GetLevelMaxCoins(levelEntry.Name);
        Debug.Log("Level " + _levelData.Name + " max coins: " + _levelData.MaxCoins.ToString());

        var levelObj = GameObject.Instantiate(levelEntry.GamePrefab, _tilemapGrid.transform);
        levelObj.transform.localPosition = _levelLocalPosition;
        InitializeTilemap();
        CurrentLevelTime = _levelTotalTime;
        TogglePause(false);
        LevelStarted = false;
    }

    private void InitializeTilemap()
    {
        Tilemap tileMap = _tilemapGrid.GetComponentInChildren<Tilemap>();
        BoundsInt size = tileMap.cellBounds;

        for (int n = size.xMin; n < size.xMax; n++)
        {
            for (int p = size.yMin; p < size.yMax; p++)
            {
                if(_characterInitialPosition.x == n && _characterInitialPosition.y == p)
                {
                    continue;
                }

                Vector3Int localPlace = (new Vector3Int(n, p, (int)tileMap.transform.position.y));
                if (!tileMap.HasTile(localPlace))
                {
                    //No tile at "place"
                    Vector3 place = tileMap.CellToWorld(localPlace);
                    var spike = _spikesPosition.Find(pos => pos.CellCoords.x == n && pos.CellCoords.y == p);
                    if (spike != null)
                    {
                        CreateSpikeInCell(place, spike);
                    }

                    bool isTimeBonus = _timeBonusPosition.x == n && _timeBonusPosition.y == p;
                    if (isTimeBonus)
                    {
                        CreateTimeBonusInCell(place, n, p);
                        continue;
                    }

                    CreateCoinInCell(place, n, p);
                }
            }
        }
    }

    private void CreateTimeBonusInCell(Vector3 place, int n, int p)
    {
        var emptyCellObj = Instantiate(_timeBonusPrefab, _emptyCellsParent);
        emptyCellObj.name = "Timecell_" + n.ToString() + "_" + p.ToString();
        emptyCellObj.transform.position = place;
        emptyCellObj.GetComponent<TimeObjectLogic>().Initialize(_hud, _timeBonusInSeconds, () => 
        {
            CreateCoinInCell(place, n, p);
            CurrentLevelTime += _timeBonusInSeconds;
        });
    }

    private void CreateSpikeInCell(Vector3 place, SpikeData spike)
    {
        var emptyCellObj = Instantiate(_spikePrefab, _emptyCellsParent);
        emptyCellObj.name = "Spike_" + spike.CellCoords.x.ToString() + "_" + spike.CellCoords.y.ToString();
        emptyCellObj.transform.position = place;
        emptyCellObj.GetComponent<SpikeObjectLogic>().Initialize(spike.Position);
    }

    private void CreateCoinInCell(Vector3 place, int n, int p)
    {
        var emptyCellObj = Instantiate(_coinPrefab, _emptyCellsParent);
        emptyCellObj.name = "cell_" + n.ToString() + "_" + p.ToString();
        emptyCellObj.transform.position = place;
        emptyCellObj.GetComponent<CoinObjectLogic>().Initialize(_hud);
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
            MaxCoins = LevelCoins
        }))
        {
            _levelData.MaxCoins = LevelCoins;
        }
    }

    public int GetMaxLevelCoins()
    {
        return _levelData.MaxCoins;
    }

    public void TogglePause(bool toggle)
    {
        // cheap but effective
        Time.timeScale = toggle ? 0.0f : 1.0f;
    }

    public void OnCoinCollected(int amount)
    {
        LevelCoins += amount;
    }

    private void Update()
    {
        if (!LevelStarted)
        {
            return;
        }

        CurrentLevelTime -= Time.deltaTime;
        if (CurrentLevelTime <= 0.0f)
        {
            // end level
            LevelStarted = false;
            CurrentLevelTime = 0.0f;
        }

        // hud update time and check level end
        _hud.UpdateLevelCountdown(CurrentLevelTime, !LevelStarted);
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
