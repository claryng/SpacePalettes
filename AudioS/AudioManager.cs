using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public event Action<float> OnSetSliderSFXValue;
    public event Action<float> OnSetSliderBGMValue;
    [SerializeField] private float _defaultVolSFX = .25f;
    [SerializeField] private float _defaultVolBGM = .5f;
    [SerializeField] private AudioSetting _audioSetting;
    [SerializeField] private StartGame _startMenu;
    [SerializeField] private PauseGame _pauseMenu;
    [SerializeField] private CountDown _countDown;
    [SerializeField] private GameOver _gameOverMenu;
    [SerializeField] private SpawnedObjectsController _spawnedObjectsController;
    [SerializeField] private Player _player;

    private float _volumeSFX;
    private readonly string _keySFXVolume = "Space Palettes - SFXVolume";
    private readonly string _keyBGMVolume = "Space Palettes - BGMVolume";
    private Vector3 _position;
    private AudioSource _bgm;

    private static AudioManager instance;

    private void Awake()
    {
        ManageSingleton();
    }

    private void OnEnable()
    {
        _player.OnPlaySFX += PlaySFX;
        _pauseMenu.OnPlaySFX += PlaySFX;
        _startMenu.OnPlaySFX += PlaySFX;
        _gameOverMenu.OnPlaySFX += PlaySFX;
        _spawnedObjectsController.OnPlaySFX += PlaySFX;
        _countDown.OnPlaySFX += PlaySFX;

        _audioSetting.OnSetSFXVolume += SetSFXVolume;
        _audioSetting.OnSetBGMVolume += SetBGMVolume;
        _startMenu.OnSetBGM += SetBGMVolume;
        _audioSetting.OnClicked += PlaySFX;
    }
    private void Start()
    {
        _bgm = GetComponent<AudioSource>();
        _volumeSFX = PlayerPrefs.GetFloat(_keySFXVolume, _defaultVolSFX);
        OnSetSliderSFXValue?.Invoke(_volumeSFX);

        OnSetSliderBGMValue?.Invoke(PlayerPrefs.GetFloat(_keyBGMVolume, _defaultVolBGM));

        _position = Camera.main.transform.position;
    }
    private void OnDisable()
    {
        _player.OnPlaySFX -= PlaySFX;
        _pauseMenu.OnPlaySFX -= PlaySFX;
        _startMenu.OnPlaySFX -= PlaySFX;
        _gameOverMenu.OnPlaySFX -= PlaySFX;
        _spawnedObjectsController.OnPlaySFX -= PlaySFX;
        _countDown.OnPlaySFX -= PlaySFX;

        _audioSetting.OnSetSFXVolume -= SetSFXVolume;
        _audioSetting.OnSetBGMVolume -= SetBGMVolume;
        _startMenu.OnSetBGM -= SetBGMVolume;
        _audioSetting.OnClicked -= PlaySFX;
    }

    private void ManageSingleton()
    {
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, _position, _volumeSFX);
    }

    private void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(_keySFXVolume, value);
        PlayerPrefs.Save();
        _volumeSFX = value;
    }

    private void SetBGMVolume(float value)
    {
        PlayerPrefs.SetFloat(_keyBGMVolume, value);
        PlayerPrefs.Save();
        _bgm.volume = _bgm? value : 0f;
    }

    public float GetBGMVol()
    {
        return _bgm ? _bgm.volume : 0;
    }

}
