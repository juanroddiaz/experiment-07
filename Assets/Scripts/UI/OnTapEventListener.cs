using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnTapEventListener : MonoBehaviour, IPointerDownHandler
{
    private Action _onTapDown;

    public void Initialize(Action onTapDown)
    {
        _onTapDown = onTapDown;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        _onTapDown?.Invoke();
    }
}
