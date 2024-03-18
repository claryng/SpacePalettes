using System;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public event Action<AudioClip> OnPlaySFX;
    public event Action<CanvasGroup, Action, Action> OnFading;
    public event Action<float, float, Action<float>, Action> OnReduceBGM;
    public event Action<float> OnSetBGM;

    public event Action OnDisplayScore;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private CanvasGroup _canvasToFade;
    [SerializeField] private GameObject _spawner;
    [SerializeField] private AudioClip _click;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] private GameObject _subStartMenu;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject _cannotWatchAd;
    [SerializeField] private GameObject _subMenu;
    [SerializeField] private GoogleAds _googleAds;
    [SerializeField] private GameObject _tutorial;

    private Action OnStart;
    private Action OnPlay;

    private void OnEnable()
    {
        OnStart += StartProcess;
        OnPlay += PlayGameProcess;
        _googleAds.OnCannotShowAd += ShowCannotWatchAd;
    }

    private void OnDisable()
    {
        OnStart -= StartProcess;
        OnPlay -= PlayGameProcess;
        _googleAds.OnCannotShowAd -= ShowCannotWatchAd;
    }

    public void Tap()
    {
        OnStart?.Invoke();
    }

    public void PlayGame()
    {
        OnPlay?.Invoke();
        float vol = _audioManager.GetBGMVol();
        OnReduceBGM?.Invoke(vol, 0f, OnSetBGM, () =>
        {
            _audioSource.Play();
            OnSetBGM?.Invoke(vol);
        });
    }
    private void StartProcess()
    {
        OnFading?.Invoke(_canvasToFade, () =>
        {
            _anim.Play(Animator.StringToHash("Pressed"));
            OnPlaySFX?.Invoke(_click);
        } , 
        () =>
        {
            _subStartMenu.SetActive(true);
            ShowTutorialOrPlay();
        });
    }

    private void PlayGameProcess()
    {
        OnFading?.Invoke(_subStartMenu.GetComponent<CanvasGroup>(), () =>
        {
            OnPlaySFX?.Invoke(_click);
        }, () =>
        {
            _spawner.SetActive(true);
            _pauseButton.SetActive(true);
            _subStartMenu.SetActive(false);
            OnDisplayScore?.Invoke();
        });
    }

    private void ShowTutorialOrPlay()
    {
        if(PlayerPrefs.GetInt(Helpers.TutorialKey, 0) == 0)
        {
            PlayerPrefs.SetInt(Helpers.TutorialKey, 1);
            PlayerPrefs.Save();
            _subMenu.SetActive(false);
            _tutorial.SetActive(true);
        }
        else
        {
            _canvasToFade.gameObject.SetActive(false);
        }
    }

    public void CloseTutorial()
    {
        OnPlaySFX?.Invoke(_click);    
        _tutorial.SetActive(false);
        _subMenu.SetActive(true);
        _canvasToFade.gameObject.SetActive(false);
    }

    private void ShowCannotWatchAd()
    {
        _subMenu.SetActive(false);
        _cannotWatchAd.SetActive(true);
    }

    public void Close()
    {
        OnPlaySFX?.Invoke(_click);
        _cannotWatchAd.SetActive(false);
        _subMenu.SetActive(true);
    }

    public void ReturnToHome()
    {
        OnPlaySFX?.Invoke(_click);
        _subStartMenu.SetActive(false);
        _startMenu.SetActive(true);
        _canvasToFade.alpha = 1.0f;
    }

}
