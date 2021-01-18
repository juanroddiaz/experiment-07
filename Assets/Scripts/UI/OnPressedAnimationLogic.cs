using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnPressedAnimationLogic : MonoBehaviour
{
    [SerializeField]
    private Image _buttonImage;

    private Vector3 _originalScale = Vector3.one;
    private Color _originalColor = Color.white;

    private void Start()
    {
        _originalColor = _buttonImage.color;
        _originalScale = _buttonImage.transform.localScale;
    }

    public void OnPressedDown()
    {
        _buttonImage.transform.localScale = _originalScale * 0.9f;
        _buttonImage.color = _originalColor * 0.75f;
    }

    public void OnPressedUp()
    {
        _buttonImage.transform.localScale = _originalScale;
        _buttonImage.color = _originalColor;
    }

    public void OnBounce()
    {
        StartCoroutine(OnBounceRoutine());
    }

    private IEnumerator OnBounceRoutine()
    {


        yield return null;
    }
}
