using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudGameplayController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField]
    private OnTapEventListener _onTapListener;
    [Header("Panels")]
    [SerializeField]
    private GameObject _controlsPanel;
    [SerializeField]
    private GameObject _pausePanel;
    [SerializeField]
    private GameObject _topPanel;
    [SerializeField]
    private GameObject _startLevelPanel;
    [SerializeField]
    private GameObject _endLevelPanel;
    [SerializeField]
    private GameObject _gameplayPanel;
    [Header("Counters")]
    [SerializeField]
    private TextMeshProUGUI _coinsCounter;
    [SerializeField]
    private TextMeshProUGUI _startLevelCountdown;
    [SerializeField]
    private TextMeshProUGUI _endLevelCoinsCounter;
    [SerializeField]
    private TextMeshProUGUI _endLevelMaxCoinsCounter;
    [Header("Feedback")]
    [SerializeField]
    private TextMeshProUGUI _endTitleText;

    private ScenarioController _sceneController;
    private int _coinCounterVisualAmount = 0;

    private Action _onMovingLeftAction;
    private Action _onMovingRightAction;
    private Action _onButtonUpAction;

    public void Initialize(ScenarioController controller, Action<bool> onSwipe, Action onLeft, Action onRight, Action onButtonUp)
    {
        _sceneController = controller;
        _onTapListener.Initialize(onSwipe);
        _onMovingLeftAction = onLeft;
        _onMovingRightAction = onRight;
        _onButtonUpAction = onButtonUp;
        OnStartLevel();
    }

    private void OnStartLevel()
    {
        _topPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _controlsPanel.SetActive(false);
        _endLevelPanel.SetActive(false);
        _startLevelPanel.SetActive(true);
        StartCoroutine(StartLevelCountdown());
    }

    private IEnumerator StartLevelCountdown()
    {
        _startLevelCountdown.text = "3";
        yield return new WaitForSeconds(1.0f);
        _startLevelCountdown.text = "2";
        yield return new WaitForSeconds(1.0f);
        _startLevelCountdown.text = "1";
        yield return new WaitForSeconds(1.0f);
        _startLevelCountdown.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        _controlsPanel.SetActive(true);
        _startLevelPanel.SetActive(false);
        _topPanel.SetActive(true);
        _sceneController.StartLevel();
        yield return null;
    }

    public void OnPause()
    {
        _sceneController.TogglePause(true);
        _pausePanel.SetActive(true);
        _controlsPanel.SetActive(false);
    }

    public void OnUnpause()
    {
        _sceneController.TogglePause(false);
        _pausePanel.SetActive(false);
        _controlsPanel.SetActive(true);
    }

    public void OnReplay()
    {
        GameController.Instance.SceneLoader.ReloadScene();
    }

    public void OnBackToMainMenu()
    {
        _sceneController.TogglePause(false);
        GameController.Instance.LoadMainMenu();
    }

    public void OnQuitGame()
    {
        _sceneController.OnQuit();
    }

    public void UpdateHeightCounter(int add)
    {
        _coinCounterVisualAmount = add;
        _coinsCounter.text = _coinCounterVisualAmount.ToString();
        //if (add > 1)
        //{
        //    _feedbackAnimation.Stop("ExtraCoins");
        //    _extraCoinsCounter.text = "+" + add.ToString();
        //    _extraCoinsCounter.gameObject.SetActive(true);
        //    _feedbackAnimation.Play("ExtraCoins");
        //}
    }

    public void OnLeftButtonDown()
    {
        _onMovingLeftAction?.Invoke();
    }

    public void OnButtonUp()
    {
        _onButtonUpAction?.Invoke();
    }

    public void OnRightButton()
    {
        _onMovingRightAction?.Invoke();
    }

    private void OnFinishLevel()
    {
        _topPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _controlsPanel.SetActive(false);
        _endLevelPanel.SetActive(true);
        _sceneController.FinishLevel();
        _endLevelCoinsCounter.text = "x" + _sceneController.LevelHeight.ToString();
        _endLevelMaxCoinsCounter.text = "x" + _sceneController.GetMaxLevelCoins().ToString();
    }

    public void AddToGameplayUI(Transform t)
    {
        t.SetParent(_gameplayPanel.transform);
        t.localScale = Vector3.one;
    }

    public void OnDeath()
    {
        _endTitleText.text = "OOP! CAREFUL!";
        _endTitleText.color = Color.red;
        OnFinishLevel();
    }
}