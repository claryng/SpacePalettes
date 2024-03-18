using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    public event Action<AudioClip> OnClicked;
    public event Action<float> OnSetSFXVolume;
    public event Action<float> OnSetBGMVolume;
    [SerializeField] private GameObject _audioSettingMenu;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _subStartMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private AudioClip _click;
    private event Action<bool> OnToggleSetting;

    private int _prevMenuIsStart;

    private void OnEnable()
    {
        _audioManager.OnSetSliderSFXValue += SetSliderSFXValue;
        _audioManager.OnSetSliderBGMValue += SetSliderBGMValue;
        OnToggleSetting += ToglleSetting;
        _prevMenuIsStart = 0;
    }

    private void OnDisable()
    {
        _audioManager.OnSetSliderSFXValue -= SetSliderSFXValue;
        _audioManager.OnSetSliderBGMValue -= SetSliderBGMValue;
        OnToggleSetting -= ToglleSetting;
    }

    public void OpenAudioSetting()
    {
        OnToggleSetting?.Invoke(true);
        OnClicked?.Invoke(_click);
    }

    public void CloseAudioSetting()
    {
        OnToggleSetting?.Invoke(false);
        OnClicked?.Invoke(_click);
    }

    public void ChangeSFXVolume()
    {
        OnSetSFXVolume?.Invoke(_sfxSlider.value);
    }

    public void ChangeBGMVolume()
    {
        OnSetBGMVolume?.Invoke(_bgmSlider.value);
    }

    private void ToglleSetting(bool state)
    {
        _audioSettingMenu.SetActive(state);

        if(state)
        {
            if (_startMenu.activeSelf)
            {
                _startMenu.SetActive(false);
                _prevMenuIsStart = 0;
            }
            else if (_subStartMenu.activeSelf)
            {
                _subStartMenu.SetActive(false);
                _prevMenuIsStart = 1;
            }else if (_pauseMenu.activeSelf)
            {
                _pauseMenu.SetActive(false);
                _prevMenuIsStart = 2;
            }
        }
        else
        {
            if(_prevMenuIsStart == 0)
            {
                _startMenu.SetActive(true);
            }
            else if (_prevMenuIsStart == 1)
            {
                _subStartMenu.SetActive(true);
            }else if (_prevMenuIsStart == 2) 
            { 
                _pauseMenu.SetActive(true);
            }
        }
    }

    private void SetSliderSFXValue(float valueToSet)
    {
        _sfxSlider.value = valueToSet;
    }

    private void SetSliderBGMValue(float valueToSet)
    {
        _bgmSlider.value = valueToSet;
    }
}
