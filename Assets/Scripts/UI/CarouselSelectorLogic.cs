using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
public class CarouselSelectorLogic : MonoBehaviour
{
    [SerializeField]
    private RectTransform _baseImageRectT;
    [SerializeField]
    private RectTransform _viewWindows;
    [SerializeField]
    private Transform _imageContainer;
    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private Image _rightArrowImage;
    [SerializeField]
    private Image _leftArrowImage;
    [SerializeField]
    private TextMeshProUGUI _bestCoinsCounter;
    [Header("Custom Thresholds")]
    [SerializeField]
    private float _imageSeparationFactor = 1.0f;
    [SerializeField]
    private float _imageGap = 25;
    [SerializeField]
    private int _swipeThrustHold = 30;

    private List<RectTransform> _images;

    private bool _canSwipe;
    private float _imageWitdh;
    private float _lerpTimer;
    private float _lerpPosition;
    private float _mousePositionStartX;
    private float _dragAmount = 0.0f;
    private float _screenPositionX;
    private float _lastScreenPositionX;
    private bool _initialized = false;

    /// The index of the current image on display.
    public int CurrentIndex = 0;

    // Use this for initialization
    public void Initialize()
    {
        _canSwipe = false;
        _imageWitdh = _viewWindows.rect.width * _imageSeparationFactor * Screen.height / Screen.width;
        _lerpTimer = 0.0f;
        _lerpPosition = 0.0f;
        _mousePositionStartX = 0.0f;
        _dragAmount = 0.0f;
        _screenPositionX = 0.0f;
        _lastScreenPositionX = 0.0f;

        _images = new List<RectTransform>();
        List<Sprite> worldMapSprites = GameController.Instance.GetMenuLevelSprites();
        for (int i = 0; i < worldMapSprites.Count; i++)
        {
            var image = Instantiate(_baseImageRectT, _imageContainer);
            image.anchoredPosition = new Vector2(((_imageWitdh + _imageGap) * i), 0);
            image.GetComponent<Image>().sprite = worldMapSprites[i];
            image.gameObject.SetActive(true);
            _images.Add(image);
        }

        CurrentIndex = GameController.Instance.DataLoader.LastSelectedLevel;
        GoToIndex(CurrentIndex);
        _initialized = true;
    }

    private void SwipeDetection()
    {
        // swipe detection
        if (Input.GetMouseButtonDown(0))
        {
            _canSwipe = true;
            _mousePositionStartX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_canSwipe)
            {
                float mousePositionEndX = Input.mousePosition.x;
                _dragAmount = mousePositionEndX - _mousePositionStartX;
                _screenPositionX = _lastScreenPositionX + _dragAmount;
            }
        }

        // only OK with touch
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch t = Input.GetTouch(0);
        if (t.phase == TouchPhase.Began)
        {
            //save began touch 2d point
            _mousePositionStartX = t.position.x;
        }
        if (t.phase == TouchPhase.Ended)
        {
            float mousePositionEndX = t.position.x;
            _dragAmount = mousePositionEndX - _mousePositionStartX;
            _screenPositionX = _lastScreenPositionX + _dragAmount;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_initialized)
        {
            return;
        }

        _lerpTimer = _lerpTimer + Time.deltaTime;

        if (_lerpTimer < 0.333f)
        {
            _screenPositionX = Mathf.Lerp(_lastScreenPositionX, _lerpPosition * -1, _lerpTimer * 3);
            _lastScreenPositionX = _screenPositionX;
        }

        SwipeDetection();

        if (Mathf.Abs(_dragAmount) > _swipeThrustHold && _canSwipe)
        {
            _canSwipe = false;
            _lastScreenPositionX = _screenPositionX;
            if (CurrentIndex < _images.Count)
                OnSwipeComplete();
            else if (CurrentIndex == _images.Count && _dragAmount < 0)
                _lerpTimer = 0;
            else if (CurrentIndex == _images.Count && _dragAmount > 0)
                OnSwipeComplete();
        }

        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].anchoredPosition = new Vector2(_screenPositionX + ((_imageWitdh + _imageGap) * i), 0);
        }
    }

    void OnSwipeComplete()
    {
        _lastScreenPositionX = _screenPositionX;

        if (_dragAmount > 0)
        {
            if (_dragAmount >= _swipeThrustHold)
            {
                if (CurrentIndex == 0)
                {
                    _lerpTimer = 0; _lerpPosition = 0;
                }
                else
                {
                    CurrentIndex--;
                    _lerpTimer = 0;
                    if (CurrentIndex < 0)
                        CurrentIndex = 0;
                    _lerpPosition = (_imageWitdh + _imageGap) * CurrentIndex;
                }
            }
            else
            {
                _lerpTimer = 0;
            }
        }
        else if (_dragAmount < 0)
        {
            if (Mathf.Abs(_dragAmount) >= _swipeThrustHold)
            {
                if (CurrentIndex == _images.Count - 1)
                {
                    _lerpTimer = 0;
                    _lerpPosition = (_imageWitdh + _imageGap) * CurrentIndex;
                }
                else
                {
                    _lerpTimer = 0;
                    CurrentIndex++;
                    _lerpPosition = (_imageWitdh + _imageGap) * CurrentIndex;
                }
            }
            else
            {
                _lerpTimer = 0;
            }
        }
        _dragAmount = 0;
        UpdateWorldInfo();
    }

    public void GoByIncrement(int increment)
    {
        int newIndex = CurrentIndex + increment;
        if (newIndex >= _images.Count)
            newIndex = 0;
        else if (newIndex < 0)
            newIndex = _images.Count - 1;
        GoToIndex(newIndex);
    }

    public void GoToIndex(int value)
    {
        if(value < 0 || value >= _images.Count)
        {
            Debug.LogError("Invalid index for carousel! " + value.ToString());
            value = 0;
        }

        CurrentIndex = value;
        _lerpTimer = 0;
        _lerpPosition = (_imageWitdh + _imageGap) * CurrentIndex;
        _screenPositionX = _lerpPosition * -1;
        _lastScreenPositionX = _screenPositionX;
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].anchoredPosition = new Vector2(_screenPositionX + ((_imageWitdh + _imageGap) * i), 0);
        }
        UpdateWorldInfo();
    }

    private void UpdateWorldInfo()
    {
        string levelName = GameController.Instance.GetLevelNameByIdx(CurrentIndex);
        _title.text = levelName;
        _bestCoinsCounter.text = "x" + GameController.Instance.DataLoader.GetLevelMaxCoins(levelName).ToString();
        _rightArrowImage.enabled = CurrentIndex != (_images.Count - 1);
        _leftArrowImage.enabled = CurrentIndex != 0;
    }

    public void GoToIndexSmooth(int value)
    {
        CurrentIndex = value;
        _lerpTimer = 0;
        _lerpPosition = (_imageWitdh + _imageGap) * CurrentIndex;
    }
}
