using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolinePlatformLogic : BasePlatformLogic
{
    [SerializeField]
    private Animator _animator;

    protected override void OnTouched(bool boosted = false)
    {
        _animator.SetBool("Touched", true);
        base.OnTouched(true);
    }
}
