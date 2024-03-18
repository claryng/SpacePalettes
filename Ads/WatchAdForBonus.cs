using System;
using UnityEngine;
public class WatchAdForBonus : MonoBehaviour
{
    public event Action<bool> OnCountDown;
    public event Action<Action> OnShielded;
    public event Action OnDisplayScore;
    public event Action<int> OnSetAmountToIncrementScore;
    public event Action<float> OnSetMoveSpeed;
    public bool WatchedAdForBooster { get; private set; }
    public bool BonusForWatchingAds { get; private set; }
    public Action OnSetWatchedAdForBooster { get; private set; }

    [SerializeField] private int _amountToIncrementScore = 30;
    [SerializeField] private int _fixedAmountToIncrementScore = 10;
    [SerializeField] private float _moveSpeedForBonus = 10f;
    [SerializeField] private float _moveSpeedOriginal = 4f;
    [SerializeField] private GameObject _subStartMenu;
    [SerializeField] private GameObject _scoreKeeper;
    [SerializeField] private GameObject _spawner;
    //[SerializeField] private UnityRewardedAds _ads;
    [SerializeField] private GameObject _pauseButton;

    [SerializeField] private GoogleAds _ads;

    private void OnEnable()
    {
        _ads.OnAfterWatchAdAtStartGame += Bonus;
    }

    private void OnDisable()
    {
        _ads.OnAfterWatchAdAtStartGame -= Bonus;
    }

    public void OnWatchAd()
    {
        WatchedAdForBooster = true;
        OnSetWatchedAdForBooster += () => { WatchedAdForBooster = false; };
        _ads.ShowRewardedAd();
    }
    private void Bonus()
    {
        _subStartMenu.SetActive(false);

        OnDisplayScore?.Invoke();

        // Triple score
        OnSetAmountToIncrementScore?.Invoke(_amountToIncrementScore);

        // Increment Speed
        BonusForWatchingAds = true;
        OnSetMoveSpeed?.Invoke(_moveSpeedForBonus);

        _spawner.SetActive(true);
        _pauseButton.SetActive(true);
        OnShielded?.Invoke(() =>
        {
            OnCountDown?.Invoke(false);
            BonusForWatchingAds = false;
            // Reset increment amount of score and speed
            OnSetMoveSpeed?.Invoke(_moveSpeedOriginal);
            OnSetAmountToIncrementScore?.Invoke(_fixedAmountToIncrementScore);
        });
    }

}
