using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour
{
    public event Action OnFreezeMovement;

    [SerializeField] private StartGame _startMenu;
    [SerializeField] private PauseGame _pauseMenu;
    [SerializeField] private GameOver _gameOverMenu;
    [SerializeField] private CanvasGroup _fadingScreen;
    [SerializeField] private float _duration = 1f;

    private const float Low = 0f;
    private const float High = 1f;

    private bool _isTransiting = false;
    private bool _isReducing = false;
    private void OnEnable()
    {

        _startMenu.OnFading += ExitScene;

        _pauseMenu.OnFading += EnterScene;

        _gameOverMenu.OnFading += EnterScene;

        _startMenu.OnReduceBGM += ReduceSound;

    }
    private void Start()
    {
        ExitScene(_fadingScreen, () => { }, () =>
        {
            _fadingScreen.gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {

        _startMenu.OnFading -= ExitScene;

        _pauseMenu.OnFading -= EnterScene;

        _gameOverMenu.OnFading -= EnterScene;

        _startMenu.OnReduceBGM -= ReduceSound;
    }

    private void ExitScene(CanvasGroup canvasToFade, Action onBeforeFade, Action onAfterFaded)
    {
        if (_isTransiting) { return; }

        StartCoroutine(FadeCanvasCoroutine(canvasToFade, High, Low, onBeforeFade, onAfterFaded));
    }

    private void EnterScene(CanvasGroup canvasToFade, Action onBeforeFade, Action onAfterFaded)
    {
        if (_isTransiting) { return; }

        StartCoroutine(FadeCanvasCoroutine(canvasToFade, Low, High, onBeforeFade, onAfterFaded));
    }

    private IEnumerator FadeCanvasCoroutine(CanvasGroup canvasToFade, float startPoint, float endPoint,
        Action onBeforeFade, Action onAfterFaded)
    {
        _isTransiting = true;

        onBeforeFade?.Invoke();
        OnFreezeMovement?.Invoke();

        for (var t = 0f; t < _duration; t += Time.deltaTime)
        {
            canvasToFade.alpha = Mathf.Lerp(startPoint, endPoint, t/_duration);

            yield return null;
        }

        canvasToFade.alpha = endPoint;

        _isTransiting = false;

        onAfterFaded?.Invoke();
    }

    private void ReduceSound(float currentVol, float minVol, Action<float> OnSetVolume, Action OnAfterFaded)
    {
        if(_isReducing) { return; }

        StartCoroutine(FadeSoundCoroutine(currentVol, minVol, OnSetVolume, OnAfterFaded));
    }

    private IEnumerator FadeSoundCoroutine(float currentVol, float minVol, Action<float> OnSetVolume, Action OnAfterFaded)
    {
        float cur = currentVol;
        for(var t = 0f; t < _duration; t += Time.deltaTime)
        {
            cur = Mathf.Lerp(currentVol,minVol,t/_duration);
            OnSetVolume?.Invoke(cur);
            yield return null;
        }
        OnAfterFaded();
    }
}
