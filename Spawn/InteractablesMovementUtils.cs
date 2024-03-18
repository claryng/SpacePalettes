using System;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InteractablesMovementUtils : MonoBehaviour
{
    public static Action OnIncrementSpeed { get; private set; }
    public static float _moveSpeed { get; private set; }
    [SerializeField] private float _minSpeed = 4f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _increaseSpeedAmount = 0.01f;
    [SerializeField] private WatchAdForBonus _watchAdForBonus;

    private static InteractablesMovementUtils instance;

    private void Awake()
    {
        ManageSingleton();
    }
    private void OnEnable()
    {
        _moveSpeed = _minSpeed;
        OnIncrementSpeed += IncrementSpeed;
        _watchAdForBonus.OnSetMoveSpeed += SetMoveSpeed;
    }

    private void OnDisable()
    {
        OnIncrementSpeed -= IncrementSpeed;
        _watchAdForBonus.OnSetMoveSpeed -= SetMoveSpeed;
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
    private void IncrementSpeed()
    {
        if (_watchAdForBonus.BonusForWatchingAds)
        {
            return;
        }
        _moveSpeed += Time.deltaTime * _increaseSpeedAmount;
        _moveSpeed = Mathf.Clamp(_moveSpeed, _minSpeed, _maxSpeed);
    }

    private void SetMoveSpeed(float amt)
    {
        _moveSpeed = amt;
    }
}
