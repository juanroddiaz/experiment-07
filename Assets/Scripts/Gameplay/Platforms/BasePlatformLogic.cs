using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatformLogic : MonoBehaviour
{
    private PlatformLayerLogic _layer;
    private bool _firstTouched = false;

    public void Initialize(PlatformLayerLogic layer)
    {
        _layer = layer;
        _firstTouched = true;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_firstTouched)
        {
            return;
        }

        if (collision.attachedRigidbody.velocity.y < 0.0f)
        {
            Debug.Log("Collision! " + collision.gameObject.name);
            _layer.UpdateReachedLayerIndex(_firstTouched);
            _firstTouched = false;
        }        
    }
}
