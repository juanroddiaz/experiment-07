using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinObjectState
{
    Coin,
    Chest,
}

public class CoinObjectLogic : MonoBehaviour
{
    [SerializeField]
    private float _randomSpawnMinTime = 2.0f;
    [SerializeField]
    private float _randomSpawnMaxTime = 5.0f;
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private LootRewardFeedback _feedbackObj;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private ChestCounterLogic _chestCounterPrefab;
    [Header("Coins")]
    [SerializeField]
    private GameObject _coinSpriteObj;
    [Header("Chests")]
    [SerializeField]
    private float _chestChancePercentage = 2.0f;
    [SerializeField]
    private float _chestBonusPercentage = 10.0f;
    [SerializeField]
    private float _chestSpawnCountdown = 5.0f;
    [SerializeField]
    private GameObject _chestSpriteObj;

    private CoinObjectState _state;
    private GameObject _currentSpriteObj;
    private Transform _hudCoinTarget;
    private HudGameplayController _hudGameplayController;
    private ChestCounterLogic _currentChestCounter;

    public void Initialize(HudGameplayController hudGameplay)
    {
        SetState(CoinObjectState.Coin);
        _currentChestCounter = null;
        _hudGameplayController = hudGameplay;
        _hudCoinTarget = hudGameplay.GetCoinHudTargetTransform();
    }

    private void SetState(CoinObjectState state)
    {
        _state = state;
        var isCoin = state == CoinObjectState.Coin;
        _animator.SetBool("Coin", isCoin);
        _coinSpriteObj.SetActive(isCoin);
        _animator.SetBool("Chest", !isCoin);
        _chestSpriteObj.SetActive(!isCoin);
        _currentSpriteObj = isCoin ? _coinSpriteObj : _chestSpriteObj;
        if (isCoin && _currentChestCounter != null)
        {
            _currentChestCounter.OnFinish();
            _currentChestCounter = null;
        }
    }

    public int OnCollected(int currentCoins)
    {
        var ret = 0;
        switch(_state)
        {
            case CoinObjectState.Coin:
                ret = 1;
                break;
            case CoinObjectState.Chest:
                {
                    ret = Mathf.CeilToInt(currentCoins * (_chestBonusPercentage / 100.0f));
                    SetState(CoinObjectState.Coin);
                }
                break;
        }
        _collider.enabled = false;
        UpdateState();
        GenerateRewardVfx(ret);
        StartCoroutine(Respawn());
        return ret;
    }

    private void UpdateState()
    {
        _currentSpriteObj.SetActive(false);
        var chestChance = Random.Range(0.0f, 100.0f);
        var isChest = chestChance <= _chestChancePercentage;
        var newState = isChest ? CoinObjectState.Chest : CoinObjectState.Coin;
        SetState(newState);
        _currentSpriteObj.SetActive(false);
    }

    private void GenerateRewardVfx(int amount)
    {
        var rewardGo = Instantiate(_feedbackObj);
        LootRewardFeedback feedback = rewardGo.GetComponent<LootRewardFeedback>();
        var targetPosition = _hudCoinTarget.position;
        targetPosition.z = -Camera.main.transform.position.z;
        var data = new LootRewardFeedbackData
        {
            StartTransform = _currentSpriteObj.transform,
            TargetPosition = Camera.main.ScreenToWorldPoint(targetPosition),
            OnFeedbackStarts = null,
            OnFeedbackReachedEnd = _hudGameplayController.UpdateCoinCounter,
            Amount = amount
        };
        feedback.Init(data);
    }

    private IEnumerator Respawn()
    {
        _collider.enabled = false;
        _currentSpriteObj.SetActive(false);
        var countdown = Random.Range(_randomSpawnMinTime, _randomSpawnMaxTime);
        yield return new WaitForSeconds(countdown);
        _currentSpriteObj.SetActive(true);
        _collider.enabled = true;
        if (_state == CoinObjectState.Chest)
        {
            StartCoroutine(RunChestCountdown());
        }
        yield return null;
    }

    private IEnumerator RunChestCountdown()
    {
        _currentChestCounter = Instantiate<ChestCounterLogic>(_chestCounterPrefab);
        _hudGameplayController.AddToGameplayUI(_currentChestCounter.transform);
        _currentChestCounter.Initialize(_chestSpriteObj.transform);
        float countDown = _chestSpawnCountdown;
        while (countDown > 0.0f && _currentChestCounter != null)
        {
            _currentChestCounter.UpdateValue(countDown);
            countDown -= Time.deltaTime;
            yield return null;
        }

        if (_state == CoinObjectState.Chest)
        {
            SetState(CoinObjectState.Coin);
        }        
    }
}
