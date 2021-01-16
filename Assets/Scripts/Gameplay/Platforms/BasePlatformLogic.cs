using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatformLogic : MonoBehaviour
{
    private PlatformLayerLogic _layer;
    private bool _firstTouched = true;

    virtual public void Initialize(PlatformLayerLogic layer)
    {
        _layer = layer;
        _firstTouched = true;
    }

    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.velocity.y < 0.0f)
        {
            if (_firstTouched)
            {
                _layer.UpdateReachedLayerIndex(true);
                _firstTouched = false;
            }
            
            OnTouched();
        }        
    }

    virtual protected void OnTouched(bool boosted = false)
    {
        _layer.TriggerPlayerJump(boosted);
    }
}
