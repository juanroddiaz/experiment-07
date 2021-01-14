using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatformLogic : BasePlatformLogic
{
    [SerializeField]
    private float _moveDiff = 1.0f;
    [SerializeField]
    private float _speed = 0.1f;

    private bool _isHorizontal = false;
    private Vector3 _originalPosition = Vector3.zero;
    private float _lerpTime = 0.0f;
    private Vector3 _targetPosition = Vector3.zero;
    private float startingDirection = 0.0f;

    public override void Initialize(PlatformLayerLogic layer)
    {
        base.Initialize(layer);
        _isHorizontal = Random.Range(0.0f, 1.0f) < 0.5f;
        startingDirection = Random.Range(0.0f, 1.0f) < 0.5f ? -1.0f * _moveDiff : _moveDiff;
        _originalPosition = transform.position;
        _targetPosition = _originalPosition;
        _targetPosition.x += _isHorizontal ? startingDirection : 0.0f;
        _targetPosition.y += _isHorizontal ? 0.0f : startingDirection;
    }

    private void Update()
    {
        _lerpTime += Time.deltaTime * _speed;
        transform.position = Vector3.Lerp(transform.position, _targetPosition, _lerpTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1)
        {
            _lerpTime = 0.0f;
            startingDirection *= -1.0f;
            if (_isHorizontal)
            {
                _targetPosition.x = _originalPosition.x + startingDirection;
                return;
            }

            _targetPosition.y = _originalPosition.y + startingDirection;
        }
    }
}
