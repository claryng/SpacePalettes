using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    [SerializeField] private int _amountToIncrement;
    [SerializeField] private int _maxScore = 999999;
    [SerializeField] private float _popUpDuration = 2f;
    [SerializeField] private TextMeshProUGUI _currentScoreToDisplay;
    [SerializeField] private Player _player;
    [SerializeField] private StartGame _startMenu;
    [SerializeField] private WatchAdForBonus _watchAdForBonus;

    private float _fontSize;
    public int Score { get; private set; }

    static ScoreKeeper instance;

    private void Awake()
    {
        ManageSingleton();
        _fontSize = _currentScoreToDisplay.fontSize;
    }

    private void OnEnable()
    {
        _player.OnIncrementScore += IncrementScore;
        _startMenu.OnDisplayScore += DisplayScore;
        _watchAdForBonus.OnDisplayScore += DisplayScore;
        _watchAdForBonus.OnSetAmountToIncrementScore += SetAmountToIncrement;
    }

    private void OnDisable()
    {
        _player.OnIncrementScore -= IncrementScore;
        _startMenu.OnDisplayScore -= DisplayScore;
        _watchAdForBonus.OnDisplayScore -= DisplayScore;
        _watchAdForBonus.OnSetAmountToIncrementScore -= SetAmountToIncrement;
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

    private void IncrementScore()
    {
        StartCoroutine(AnimateScore());
    }

    private IEnumerator AnimateScore()
    {
        Score += _amountToIncrement;
        Score = Mathf.Clamp(Score, 0, _maxScore);
        _currentScoreToDisplay.text = Score.ToString();
        _currentScoreToDisplay.fontSize = _fontSize + 10f;

        // Pop up score text 
        yield return Helpers.GetWaitForSeconds(_popUpDuration);
        _currentScoreToDisplay.fontSize = _fontSize;
        _currentScoreToDisplay.text = Score.ToString();
    }

    private void DisplayScore()
    {
        _currentScoreToDisplay.text = Score.ToString();
    }

    private void SetAmountToIncrement(int amt)
    {
        _amountToIncrement = amt;
    }
}
