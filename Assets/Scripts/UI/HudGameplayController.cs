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
    private TextMeshProUGUI _heightCounter;
    [SerializeField]
    private TextMeshProUGUI _platformCounter;
    [SerializeField]
    private TextMeshProUGUI _startLevelCountdown;
    [Header("EndGame Info")]
    [SerializeField]
    private TextMeshProUGUI _endLevelHeightCounter;
    [SerializeField]
    private TextMeshProUGUI _endLevelPlatformCounter;
    [SerializeField]
    private TextMeshProUGUI _endLevelMaxHeightCounter;
    [SerializeField]
    private TextMeshProUGUI _endLevelMaxPlatformCounter;
    [SerializeField]
    private Animator _endLevelAnimator;
    [SerializeField]
    private CanvasGroup _endLevelGroup;
    [SerializeField]
    private Animator _startLevelAnimator;
    [Header("Feedback")]
    [SerializeField]
    private TextMeshProUGUI _endTitleText;

    private ScenarioController _sceneController;
    private int _platformCounterAmount = 0;

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
        _endLevelGroup.alpha = 0.0f;
        _startLevelPanel.SetActive(true);
        UpdateHeightCounter(0);
        _platformCounterAmount = 0;
        _platformCounter.text = ": 0";
        StartCoroutine(StartLevelCountdown());
    }

    private IEnumerator StartLevelCountdown()
    {
        _startLevelCountdown.text = "3";
        _startLevelAnimator.SetTrigger("OnLevelStart");
        yield return new WaitForSeconds(1.0f);
        _startLevelCountdown.text = "2";
        _startLevelAnimator.SetTrigger("OnLevelStart");
        yield return new WaitForSeconds(1.0f);
        _startLevelCountdown.text = "1";
        _startLevelAnimator.SetTrigger("OnLevelStart");
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

    public void OnBackToMainMenu()
    {
        _sceneController.TogglePause(false);
        GameController.Instance.LoadMainMenu();
    }

    public void OnQuitGame()
    {
        _sceneController.OnQuit();
    }

    public void UpdateHeightCounter(int amount)
    {
        _heightCounter.text = amount.ToString();
    }

    public void UpdatePlatformCounter()
    {
        _platformCounterAmount++;
        _platformCounter.text = ": " + _platformCounterAmount.ToString();
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

        _endLevelHeightCounter.text = _sceneController.CurrentHeight.ToString();
        _endLevelPlatformCounter.text = " : " + _sceneController.CurrentPlatforms.ToString();
        _endLevelMaxHeightCounter.text = _sceneController.GetMaxHeight().ToString();
        _endLevelMaxPlatformCounter.text = " : " + _sceneController.GetMaxPlatforms().ToString();
        _endLevelAnimator.SetTrigger("OnLevelEnd");
    }

    public void OnReplay()
    {
        // no scene reset
        _sceneController.InitializeScene();
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