//-----------------------------------------------------------------------
// LootRewardFeedback.cs
//
// Copyright 2020 Social Point SL. All rights reserved.
//
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum LootRewardFeedbackState
{
    Spawning = 0,
    Moving,
    Dissapearing,
    None
}

public class LootRewardFeedbackData
{
    public Transform StartTransform;
    public Vector3 TargetPosition;
    public Action OnFeedbackStarts;
    public Action<int> OnFeedbackReachedEnd;
    public int Amount;
}

public class LootRewardFeedback : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 2.0f;
    [SerializeField]
    private float _moveReachingDistance = 0.5f;
    [SerializeField]
    private float _scaleSpeed = 1.0f;
    [SerializeField]
    private float _scaleReachingDistance = 0.1f;
    [SerializeField]
    private Vector3 _initialScale = Vector3.one;
    [SerializeField]
    private Vector3 _beforeMovingScale = Vector3.one;
    [SerializeField]
    private ParticleSystem _mainParticleSystem;
    [SerializeField]
    private GameObject _specialObject;

    private Action _onFeedbackStarts;
    private Action<int> _onFeedbackReachedEnd;

    private LootRewardFeedbackState _state = LootRewardFeedbackState.None;
    private Vector3 _targetPosition;
    private int _amount;

    public void Init(LootRewardFeedbackData data)
    {
        transform.position = data.StartTransform != null ? data.StartTransform.position : Vector3.zero;
        transform.localScale = _initialScale;
        _targetPosition = data.TargetPosition;
        _onFeedbackStarts = data.OnFeedbackStarts;
        _onFeedbackStarts?.Invoke();
        _onFeedbackReachedEnd = data.OnFeedbackReachedEnd;
        _amount = data.Amount;
        InitParticle(_amount > 1);
        _state = LootRewardFeedbackState.Spawning;
    }

    private void InitParticle(bool isSpecial)
    {
        var main = _mainParticleSystem.main;
        main.startColor = isSpecial ? Color.red : main.startColor.color;
        _mainParticleSystem.Play();
        _specialObject.SetActive(isSpecial);
    }

    void Update()
    {
        switch(_state)
        {
            case LootRewardFeedbackState.None:
                break;
            case LootRewardFeedbackState.Spawning:
                transform.localScale = Vector3.Lerp(transform.localScale, _beforeMovingScale, _scaleSpeed * Time.deltaTime);
                if(Vector3.Distance(transform.localScale, _beforeMovingScale) < _scaleReachingDistance)
                {
                    _state = LootRewardFeedbackState.Moving;
                }
                break;
            case LootRewardFeedbackState.Moving:
                transform.position = Vector3.Lerp(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
                if(Vector3.Distance(transform.position, _targetPosition) < _moveReachingDistance)
                {
                    _state = LootRewardFeedbackState.Dissapearing;
                    _onFeedbackReachedEnd?.Invoke(_amount);
                }
                break;
            case LootRewardFeedbackState.Dissapearing:
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, _scaleSpeed * Time.deltaTime);
                if(Vector3.Distance(transform.localScale, Vector3.zero) < _scaleReachingDistance)
                {
                    _state = LootRewardFeedbackState.None;
                    OnFeedbackEnded();
                }
                break;
        }
    }

    public void OnFeedbackEnded()
    {
        //Debug.Log("LootRewardFeedback ended! Target: " + _targetTransform.name);
        _state = LootRewardFeedbackState.None;
        Destroy(gameObject);
    }
}
