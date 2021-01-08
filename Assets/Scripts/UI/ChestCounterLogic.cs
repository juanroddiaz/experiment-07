using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestCounterLogic : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    [SerializeField]
    private Vector3 _localPositionOffset;

    public void Initialize(Transform target)
    {
        transform.position = Camera.main.WorldToScreenPoint(target.position);
        transform.localPosition += _localPositionOffset;
    }

    public void UpdateValue(float value)
    {
        _text.text = value.ToString("0.0");
    }

    public void OnFinish()
    {
        Destroy(gameObject);
    }
}
