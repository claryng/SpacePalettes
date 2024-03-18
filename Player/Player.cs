using System.Collections.Generic;
using UnityEngine;
using ColorsAvailable;
using System;

public class Player : MonoBehaviour
{
    public Colors PlayerColor { get; private set; }
    public event Action OnDestroySpawned;
    public event Action OnIncrementScore;
    public event Action<Colors, string> OnExplode;
    public event Action OnGameOver;
    public event Action<AudioClip> OnPlaySFX;
    public event Action<GameObject> OnDestroyHitObject;
    public event Action<Action> OnShielded;
    public event Action OnBrokeShield;

    [Header("Swipe")]
    [SerializeField] private int _pixelDistToDetect = 15;
    [SerializeField] private GameObject _startGame;
    [SerializeField] private GameObject _subStartMenu;
    [SerializeField] private GameObject _pauseGame;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private GameObject _audioSetting;
    [SerializeField] private GameObject _countDownHolder;
    [SerializeField] private GameObject _sliderShieldDisplay;
    [SerializeField] private WatchAdForBonus _watchAdForBonus;
    [SerializeField] private AudioClip _sfxHitCorrectColor;
    [SerializeField] private AudioClip _sfxSwipe;
    [SerializeField] private AudioClip _sfxSpecial;
    [SerializeField] private AudioClip _sfxError;
    [SerializeField] private AudioClip _sfxBroken;

    private readonly int _whiteHash = Animator.StringToHash("Idle_White");
    private readonly int _purpleHash = Animator.StringToHash("Idle_Purple");
    private readonly int _greenHash = Animator.StringToHash("Idle_Green");
    private Animator _animator;

    private Transform _t;

    // Swipe
    private Vector2 _startPos;
    private bool _fingerDown;
    private float _diff;

    // Handle color switch
    private Dictionary<Colors, int> _animationByColor;
    private List<Colors> _possibleAnimationToSwitch;

    private void Awake()
    {
        _animationByColor = new Dictionary<Colors, int>();
        _possibleAnimationToSwitch = new List<Colors>();
        _t = transform;
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    private void Start()
    {
        PlayerColor = Colors.White;
        _diff = ScreenUtils.DifferentDistToMoveObj;
        _animator.Play(_whiteHash);

        _possibleAnimationToSwitch.Add(Colors.White);
        _possibleAnimationToSwitch.Add(Colors.Purple);
        _possibleAnimationToSwitch.Add(Colors.Green);

        _animationByColor.Add(Colors.Purple, _purpleHash);
        _animationByColor.Add(Colors.Green, _greenHash);
        _animationByColor.Add(Colors.White, _whiteHash);

    }

    // Update is called once per frame
    private void Update()
    {
        if (!_startGame.activeSelf && !_pauseGame.activeSelf && !_gameOver.activeSelf && !_countDownHolder.activeSelf 
            && !_audioSetting.activeSelf && !_subStartMenu.activeSelf)
        {
            #region Swipe
            if (!_fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                _startPos = Input.touches[0].position;
                _fingerDown = true;
            }

            if (_fingerDown && Input.touchCount > 0)
            {
                if (Input.touches[0].position.x < _startPos.x - _pixelDistToDetect)
                {
                    _fingerDown = false;

                    if (_t.position.x > -_diff)
                    {
                        _t.position += Vector3.left * _diff;
                        OnPlaySFX?.Invoke(_sfxSwipe);
                    }
                }
                else if (Input.touches[0].position.x > _startPos.x + _pixelDistToDetect)
                {
                    _fingerDown = false;

                    if (_t.position.x < _diff)
                    {
                        _t.position += Vector3.right * _diff;
                        OnPlaySFX?.Invoke(_sfxSwipe);
                    }
                }
            }

            if (_fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
            {
                _fingerDown = false;
            }
            #endregion  
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collided = collision.gameObject;
        collided.SetActive(false);

        if (collided.CompareTag("Special"))
        {
            SwitchColor();
            OnPlaySFX?.Invoke(_sfxSpecial);
            OnExplode?.Invoke(Colors.Empty, "Special");
            Destroy(collided);
        }else if (collided.CompareTag("Shield"))
        {
            OnPlaySFX?.Invoke(_sfxSpecial);
            OnShielded?.Invoke(() => { });
            OnExplode?.Invoke(Colors.Empty, "Shield");
            Destroy(collided);
        }
        else if (collided.layer == 7)
        {
            Colors colorOfCollidedObj = Colors.Empty;
            switch (collided.tag)
            {
                case "Purple":
                    colorOfCollidedObj = Colors.Purple;
                    break;
                case "Green":
                    colorOfCollidedObj = Colors.Green;
                    break;
                case "White":
                    colorOfCollidedObj = Colors.White;
                    break;
                default:
                    break;
            }

            if(colorOfCollidedObj.Equals(PlayerColor) || _sliderShieldDisplay.activeSelf)
            {
                if(_sliderShieldDisplay.activeSelf && !_watchAdForBonus.BonusForWatchingAds && !colorOfCollidedObj.Equals(PlayerColor))
                {
                    OnPlaySFX?.Invoke(_sfxBroken);
                    OnBrokeShield?.Invoke();
                }
                else
                {
                    OnPlaySFX?.Invoke(_sfxHitCorrectColor); 
                }
                OnIncrementScore?.Invoke();
                OnExplode?.Invoke(colorOfCollidedObj, "");
                OnDestroyHitObject?.Invoke(collided);
            }
            else
            {
                OnPlaySFX?.Invoke(_sfxError);
                OnGameOver?.Invoke();
            }
        }

    }

    private void SwitchColor()
    {
        if (_possibleAnimationToSwitch.Contains(PlayerColor))
        {
            _possibleAnimationToSwitch.Remove(PlayerColor);
        }

        int colorToSwitch = UnityEngine.Random.Range(0, _possibleAnimationToSwitch.Count);
        Colors newColor = _possibleAnimationToSwitch[colorToSwitch];
        _animator.Play(_animationByColor[newColor]);

        if (!_possibleAnimationToSwitch.Contains(PlayerColor))
        {
            _possibleAnimationToSwitch.Add(PlayerColor);
        }
        PlayerColor = newColor;
        OnDestroySpawned?.Invoke();
    }

}
