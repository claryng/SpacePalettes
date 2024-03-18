using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private GameObject _sliderDisplay;
    [SerializeField] private WatchAdForBonus _watchAdForBonus;
    [SerializeField] private Slider _sliderShield;
    [SerializeField] private Player _player;
    [SerializeField] private GameObject _bubble;
    [SerializeField] private ParticleSystem _shatter;

    private const float Max = 1f;
    private const float Min = 0f;

    private bool _isRunning;
    private Coroutine _co;
    private void OnEnable()
    {
        _player.OnShielded += ReduceSliderValue;
        _player.OnBrokeShield += BreakShield;
        _watchAdForBonus.OnShielded += ReduceSliderValue;
        _isRunning = false;
    }

    private void OnDisable()
    {
        _player.OnShielded -= ReduceSliderValue;
        _player.OnBrokeShield -= BreakShield;
        _watchAdForBonus.OnShielded -= ReduceSliderValue;
    }

    private void ReduceSliderValue(Action OnAfterShielded)
    {
        if (_isRunning)
        {
            StopCoroutine(_co);
        }
        _co = StartCoroutine(Shielded(OnAfterShielded));
    }
    private IEnumerator Shielded(Action OnAfterShielded)
    {
        _sliderDisplay.SetActive(true);
        _sliderShield.value = Max;
        _isRunning = true;
        _bubble.SetActive(true);
        for (var t = 0f; t <= _duration; t += Time.deltaTime)
        {
            _sliderShield.value = Mathf.Lerp(Max, Min, t/_duration);
            yield return null;
        }
        _isRunning = false;
        _bubble.SetActive(false);
        OnAfterShielded?.Invoke();
        _sliderDisplay.SetActive(false);
    }

    private void BreakShield()
    {
        if(_co != null)
        {
            StopCoroutine(_co);
        }
        StartCoroutine(Shatter());
        _isRunning = false;
        _bubble.SetActive(false);
        _sliderDisplay.SetActive(false);
    }

    private IEnumerator Shatter()
    {
        ParticleSystem instance = Instantiate(_shatter, _player.transform.position, Quaternion.identity);
        yield return Helpers.GetWaitForSeconds(.5f);
        Destroy(instance);
    }

}
