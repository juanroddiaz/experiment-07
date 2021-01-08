using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionEventData
{
    public Action<Transform> CollisionEnterAction;
    public Action<Transform> CollisionExitAction;
}

public class CollisionEventLogic : MonoBehaviour
{
    private CollisionEventData _data;
    private bool _initialized = false;
    private Collider2D _collider;

    public void Initialize(CollisionEventData data)
    {
        _data = data;
        _collider = GetComponent<Collider2D>();
        _initialized = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("On collision enter: other " + collision.gameObject.name + ", collider: " + gameObject.name);
        if (_initialized)
        {
            _data.CollisionEnterAction?.Invoke(collision.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("On collision exit: other " + collision.gameObject.name + ", collider: " + gameObject.name);
        if (_initialized)
        {
            _data.CollisionExitAction?.Invoke(collision.transform);
        }
    }
}