using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlatformLogic : MonoBehaviour
{
    private PlatformLayerLogic _layer;

    public void Initialize(PlatformLayerLogic layer)
    {
        _layer = layer;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.velocity.y < 0.0f)
        {
            Debug.Log("Collision! " + collision.gameObject.name);
        }        
    }
}
