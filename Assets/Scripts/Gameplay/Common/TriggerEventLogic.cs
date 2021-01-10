using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventData
{
    public Action<Transform> TriggerEnterAction;
    public Action<Transform> TriggerExitAction;
}

public class TriggerEventLogic : MonoBehaviour
{
    private TriggerEventData _data;
    private bool _initialized = false;
    private Collider2D _collider;

    public void Initialize(TriggerEventData data)
    {
        _data = data;
        _collider = GetComponent<Collider2D>();
        _initialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("On collision enter: other " + collision.gameObject.name + ", collider: " + gameObject.name);
        if (_initialized)
        {
            _data.TriggerEnterAction?.Invoke(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("On collision exit: other " + collision.gameObject.name + ", collider: " + gameObject.name);
        if (_initialized)
        {
            _data.TriggerExitAction?.Invoke(collision.transform);
        }
    }
}
