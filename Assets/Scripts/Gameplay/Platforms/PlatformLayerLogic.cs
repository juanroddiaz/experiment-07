using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLayerLogic : MonoBehaviour
{
    [SerializeField]
    private float _halfHorizontalSpace = 2.0f;
    [SerializeField]
    private float _halfVerticalSpace = 0.5f;

    private ScenarioController _controller;
    private Vector2 _platformDistances = Vector2.zero;
    private List<Vector3> _platformPositions = new List<Vector3>();
    private PlatformDifficulty _minimumDifficulty;
    private PlatformDifficulty _maximumDifficulty;
    private int _index = 0;

    public void Initialize(ScenarioController controller, LayersChunkData data, int index)
    {
        _controller = controller;
        // get difficulty platforms amount
        _minimumDifficulty = data.LowerDifficulty;
        _maximumDifficulty = data.UpperDifficulty;

        int amount = _controller.GetPlatformAmountByDifficulty(_minimumDifficulty);
        List<PlatformConfig> platforms = _controller.GetPlatformsByDifficulty(_minimumDifficulty);
        // if different, get a random amount between the two
        if (_minimumDifficulty != _maximumDifficulty)
        {
            amount = Random.Range(_controller.GetPlatformAmountByDifficulty(_maximumDifficulty), amount);
            platforms.AddRange(_controller.GetPlatformsByDifficulty(_maximumDifficulty));
        }

        // instantiate platforms between the position constrains
        _platformDistances = new Vector2(_halfHorizontalSpace, _halfVerticalSpace) * 2.0f / (amount - 1);
        int totalPlatformsAmount = platforms.Count;
        for (int i = 0; i < amount; i++)
        {
            var randomPlatform = platforms[Random.Range(0, totalPlatformsAmount)];
            var platform = Instantiate(randomPlatform.Asset, transform);
            platform.transform.localPosition = GetPlatformPosition(amount);
            BasePlatformLogic logic = platform.GetComponent<BasePlatformLogic>();
            logic.Initialize(this);
        }

        _index = index;
        gameObject.name = "Layer_" + index;
    }

    private Vector3 GetPlatformPosition(int amount)
    {
        float x = -1.0f * _halfHorizontalSpace + _platformDistances.x * Random.Range(0, amount);
        float y = -1.0f * _halfVerticalSpace + _platformDistances.y * Random.Range(0, amount);
        if (_platformPositions.FindIndex(position => position.x == x && position.y == y) != -1)
        {
            return GetPlatformPosition(amount);
        }

        Vector3 ret = new Vector3(x, y, 0.0f);
        _platformPositions.Add(ret);
        // adding an used position slot around
        _platformPositions.Add(ret + new Vector3(0.0f, _platformDistances.y, 0.0f));
        _platformPositions.Add(ret + new Vector3(0.0f, -1.0f * _platformDistances.y, 0.0f));
        _platformPositions.Add(ret + new Vector3(_platformDistances.x, 0.0f, 0.0f));
        _platformPositions.Add(ret + new Vector3(-1.0f * _platformDistances.x, 0.0f, 0.0f));
        return ret;
    }

    public void UpdateReachedLayerIndex(bool firstTouched)
    {
        _controller.UpdateReachedLayerIndex(_index, firstTouched);
    }

    public void TriggerPlayerJump(bool boosted)
    {
        _controller.TriggerPlayerJump(boosted);
    }

    public void TriggerPlayerDeath()
    {
        _controller.TriggerPlayerDeath();
    }
}
