using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudMenuController : MonoBehaviour
{
    [SerializeField]
    private CarouselSelectorLogic _carouselLogic;
    [SerializeField]
    private Button _playButton;
    [SerializeField]
    private GameObject _soundOnObj;
    [SerializeField]
    private GameObject _soundOffObj;

    private bool _soundOn = true;

    void Start()
    {
        _carouselLogic.Initialize();
        _soundOn = GameController.Instance.DataLoader.MusicOn;
        UpdateSoundImages();
    }

    public void OnPlayButtonClick()
    {
        GameController.Instance.LoadGameplayScenario(_carouselLogic.CurrentIndex);
    }

    public void OnResetDataClick()
    {
        GameController.Instance.DeleteData();
    }

    public void OnToggleSound()
    {
        _soundOn = !_soundOn;
        UpdateSoundImages();
        GameController.Instance.ToggleMusicEnabled();
    }

    private void UpdateSoundImages()
    {
        _soundOnObj.SetActive(_soundOn);
        _soundOffObj.SetActive(!_soundOn);
    }
}
