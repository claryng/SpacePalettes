using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public event Action<bool> OnCountDown;
    public event Action<AudioClip> OnPlaySFX;
    public event Action<CanvasGroup, Action, Action> OnFading;
    public event Action<int> OnReportScore;
    public event Action<bool> OnResetPause;

    [SerializeField] private GameObject _gameOverMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _bestScore;
    [SerializeField] private ScoreKeeper _scoreKeeper;
    [SerializeField] private GoogleAds _rewardedAds;
    [SerializeField] private Button _watchAdButton;
    [SerializeField] private AudioClip _click;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Player _player;
    [SerializeField] private GPGSManager _gpgsManager;
    [SerializeField] private GameObject _menuAdInfo;
    [SerializeField] private GameObject _menu;

    private int _scoreToDisplay;
    private int _bestScoreToDisplay;
    private static int _timeWatchedAd;
    private Action OnRestart;
    private Action OnWatchAd;

    private void Awake()
    {
        _player.OnGameOver += SetUpGameOver;
        _rewardedAds.OnBonus += WatchAdProcess;
        OnRestart += RestartProcess;
        OnWatchAd += WatchAdProcess;
    }


    private void OnDisable()
    {
        _player.OnGameOver -= SetUpGameOver;
        _rewardedAds.OnBonus -= WatchAdProcess;
        OnRestart -= RestartProcess;
        OnWatchAd -= WatchAdProcess;
    }

    private void SetUpGameOver()
    {
        _gameOverMenu.SetActive(true);

        if (_pauseMenu.activeSelf)
        {
            OnResetPause?.Invoke(false); 
        }

        // Only allow watch ad 3 times
        if (_timeWatchedAd >= 3)
        {
            _watchAdButton.gameObject.SetActive(false);
            _timeWatchedAd = 0;
        }
        Time.timeScale = 0f;

        _scoreToDisplay = _scoreKeeper.Score;

        SaveBestScore();

        if(_gpgsManager.LoggedIn)
        {
            OnReportScore?.Invoke(_bestScoreToDisplay);
        }

        _scoreText.text = "Score: " + _scoreToDisplay.ToString();
        _bestScore.text = "Best: " + _bestScoreToDisplay.ToString();
    }

    public void GameOver_Restart()
    {
        OnRestart?.Invoke();
    }

    public void WatchAd()
    {
        OnWatchAd?.Invoke();
    }

    private void RestartProcess()
    {
        OnFading?.Invoke(_canvasGroup, () =>
        {
            Time.timeScale = 1.0f;
            _canvasGroup.gameObject.SetActive(true);
            OnPlaySFX?.Invoke(_click);
        }, () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    private void WatchAdProcess()
    {
        _gameOverMenu.SetActive(false);
        OnCountDown?.Invoke(_gameOverMenu.activeSelf);
        Time.timeScale = 1.0f;
        _timeWatchedAd++;
    }

    private void SaveBestScore()
    {
        if (_scoreToDisplay > PlayerPrefs.GetInt(Helpers.BestScoreKey, 0))
        {
            PlayerPrefs.SetInt(Helpers.BestScoreKey, _scoreToDisplay);
            PlayerPrefs.Save();
        }
        _bestScoreToDisplay = PlayerPrefs.GetInt(Helpers.BestScoreKey, 0);
    }

    public void CloseCannotWatchAd()
    {
        OnPlaySFX?.Invoke(_click);
        _menuAdInfo.SetActive(false);
        _menu.SetActive(true);
    }

}

