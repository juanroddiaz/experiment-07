using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnTapEventListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Action<bool> _onSwipe;

    [SerializeField]
    private float SwipeMinDuration = 0.2f;
    [SerializeField]
    private float SwipeMaxDuration = 0.5f;
    [SerializeField]
    private float SwipeMinHorDistance = 1.0f;

    private bool _isSwipingTry = false;
    private float _currentDuration = 0.0f;
    private float _currentStartingPoint = 0.0f;

    public void Initialize(Action<bool> onSwipe)
    {
        _onSwipe = onSwipe;
    }

    void Update()
    {
        if (_isSwipingTry)
        {
            _currentDuration += Time.deltaTime;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");
        _currentDuration = 0.0f;
        _currentStartingPoint = eventData.position.x;
        _isSwipingTry = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
        if (_currentDuration > SwipeMinDuration && _currentDuration <= SwipeMaxDuration)
        {
            float horizontalDiff = eventData.position.x - _currentStartingPoint;
            if (Mathf.Abs(horizontalDiff) >= SwipeMinHorDistance)
            {
                Debug.Log("Swipe!");
                bool goingRight = Mathf.Sign(horizontalDiff) > 0.0f;
                _onSwipe?.Invoke(goingRight);
            }            
        }
        _isSwipingTry = false;
    }
}
