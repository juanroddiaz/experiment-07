using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeObjectLogic : MonoBehaviour
{
    [SerializeField]
    private LootRewardFeedback _feedbackObj;

    private Transform _hudTimeTarget;
    private HudGameplayController _hudGameplayController;
    private Action _onAfterCollected;
    private int _secondsAdded = 0;

    public void Initialize(HudGameplayController hudGameplay, int seconds, Action afterCollected)
    {
        _hudGameplayController = hudGameplay;
        _hudTimeTarget = hudGameplay.GetTimeHudTargetTransform();
        _onAfterCollected = afterCollected;
        _secondsAdded = seconds;
    }

    public void OnCollected()
    {
        GenerateRewardVfx();
        _onAfterCollected.Invoke();
        Destroy(gameObject);
    }

    private void GenerateRewardVfx()
    {
        var rewardGo = Instantiate(_feedbackObj);
        LootRewardFeedback feedback = rewardGo.GetComponent<LootRewardFeedback>();
        var targetPosition = _hudTimeTarget.position;
        targetPosition.z = -Camera.main.transform.position.z;
        var data = new LootRewardFeedbackData
        {
            StartTransform = transform,
            TargetPosition = Camera.main.ScreenToWorldPoint(targetPosition),
            OnFeedbackStarts = null,
            OnFeedbackReachedEnd = _hudGameplayController.UpdateTimeCounter,
            Amount = _secondsAdded
        };
        feedback.Init(data);
    }
}
