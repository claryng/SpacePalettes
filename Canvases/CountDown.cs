using System;
using System.Collections;
using TMPro;
using UnityEngine;
public class CountDown : MonoBehaviour
{
    public event Action OnDestroyAfterWatchAd;
    public event Action<AudioClip> OnPlaySFX;
    [SerializeField] private GameObject _countDownHolder;
    [SerializeField] private GameObject _spawner;
    [SerializeField] private int _countDownTime = 3;
    [SerializeField] private TextMeshProUGUI _countDownText;
    [SerializeField] private GameOver _gameOver;
    [SerializeField] private PauseGame _pauseGame;
    [SerializeField] private WatchAdForBonus _watchAdForBonus;
    [SerializeField] private CapsuleCollider2D _player;
    [SerializeField] private AudioClip _countSFX;
    [SerializeField] private AudioClip _goSFX;
    [SerializeField] private AudioSource _bgm;

    private Animator _anim;
    private Coroutine _co;
    private readonly int _popUpAnimation = Animator.StringToHash("CountDownAnimation");
    private readonly int _empty = Animator.StringToHash("Empty");
    private void Awake()
    {
        _gameOver.OnCountDown += StartCountDown;
        _watchAdForBonus.OnCountDown += StartCountDown;
        _pauseGame.OnStopCountDown += StopCountDownAnim;
        _anim = _countDownHolder.GetComponent<Animator>();
    }

    private void OnDisable()
    {
        _gameOver.OnCountDown -= StartCountDown; 
        _watchAdForBonus.OnCountDown-= StartCountDown;
        _pauseGame.OnStopCountDown -= StopCountDownAnim;
    }

    private void StartCountDown(bool state)
    {
        _countDownHolder.SetActive(true);
        _countDownTime = 3;
        _countDownText.text = "";
        if (!state)
        {
            _co = StartCoroutine(CountDownToResume());  
        }
    }
    private IEnumerator CountDownToResume()
    {
        if (_player)
        {
            _player.enabled = false;
        }

        OnDestroyAfterWatchAd?.Invoke();

        if (_spawner)
        {
            _spawner.SetActive(false);
        }

        _countDownText.gameObject.SetActive(true);
        _anim.enabled = true;
        while (_countDownTime > 0)
        {
            OnPlaySFX?.Invoke(_countSFX);    
            _countDownText.text = _countDownTime.ToString();
            _anim.Play(_popUpAnimation);
            yield return Helpers.GetWaitForSeconds(1f);
            _countDownTime--;
        }

        _countDownText.text = "GO!";
        OnPlaySFX.Invoke(_goSFX);
        yield return Helpers.GetWaitForSeconds(.9f);
        _anim.enabled = false;

        _bgm.Play();

        if(_player)
        {
            _player.enabled = true;
        }

        if (_spawner)
        {
            _spawner.SetActive(true);
        }

        _countDownHolder.SetActive(false);
    }

    private void StopCountDownAnim()
    {
        if (_co != null)
        {
            _anim.enabled = false;
            StopCoroutine(_co);
        }
    }
}
