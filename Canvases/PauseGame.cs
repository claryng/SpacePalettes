using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseGame : MonoBehaviour
{
    public event Action<AudioClip> OnPlaySFX;
    public event Action<CanvasGroup, Action, Action> OnFading;
    public event Action OnStopCountDown;
    public event Action<int> OnReportScore;

    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private GameOver _gameOver;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _subStartMenu;
    [SerializeField] private AudioClip _click;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GPGSManager _gpgsManager;
    [SerializeField] private ScoreKeeper _scoreKeeper;

    private Action OnResume;
    private Func<bool> OnPausePressed;
    private Action OnRestart;
    private static bool _isPaused;

    private void OnEnable()
    {
        OnResume += ResumeProcess;
        OnRestart += RestartProcess;
        _gameOver.OnResetPause += SetPause;
    }

    private void OnDisable()
    {
        OnResume -= ResumeProcess;
        OnRestart -= RestartProcess;
        _gameOver.OnResetPause -= SetPause;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_gameOverUI.activeSelf && !_startMenu.activeSelf && !_subStartMenu.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || OnPausePressed?.Invoke() == true)
            {
                if (_isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                    OnPausePressed = () => { return false; };
                }
            }
        }
    }

    private void SetPause(bool state)
    {
        _isPaused = state;
        _pauseMenuUI.SetActive(state);
    }

    public void Resume()
    {
        OnResume?.Invoke();
    }
    public void PauseGame_Restart()
    {
        OnRestart?.Invoke();
    }

    public void OnClick()
    {
        OnPlaySFX?.Invoke(_click);
        _isPaused = false;
        OnPausePressed = () => { return true; };
    }

    private void ResumeProcess()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        OnPlaySFX?.Invoke(_click);
        _isPaused = false;
    }

    private void Pause()
    {
        if(_gpgsManager.LoggedIn)
        {
            OnReportScore?.Invoke(_scoreKeeper.Score);
        }
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
    }

    private void RestartProcess()
    {
        OnFading?.Invoke(_canvasGroup, () =>
        {
            _isPaused = false;
            Time.timeScale = 1.0f;
            OnStopCountDown?.Invoke();
            _canvasGroup.gameObject.SetActive(true);
            OnPlaySFX?.Invoke(_click);
        }, () =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }
}
