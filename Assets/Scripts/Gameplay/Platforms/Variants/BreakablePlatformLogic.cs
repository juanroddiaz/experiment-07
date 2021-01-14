using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatformLogic : BasePlatformLogic
{
    [SerializeField]
    private Collider2D _collider;
    [SerializeField]
    private Animator _animator;

    protected override void OnTouched()
    {
        _collider.enabled = false;
        _animator.SetBool("Touched", true);
    }
}
