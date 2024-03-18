using UnityEngine;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames;
using System;

[DefaultExecutionOrder(-2)]
public class GPGSManager : MonoBehaviour
{
    public bool LoggedIn { get; private set; }
    [SerializeField] private ScoreKeeper _scoreKeeper;
    [SerializeField] private GameOver _gameOver;
    [SerializeField] private PauseGame _pauseGame;

    private bool _isSaving;
    private int _cloudSavedScore;
    private PlayGamesPlatform _platform;
    private void OnEnable()
    {
        _gameOver.OnReportScore += ReportScoreOnLeaderboard;
        _pauseGame.OnReportScore += ReportScoreOnLeaderboard;
    }

    private void OnDisable()
    {
        _gameOver.OnReportScore -= ReportScoreOnLeaderboard;
        _pauseGame.OnReportScore -= ReportScoreOnLeaderboard;
    }

    private void Start()
    {
        _platform = PlayGamesPlatform.Activate();
        _platform.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // Continue with Play Games Services
            LoggedIn = true;
            OpenSave(false);
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            //PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
            LoggedIn = false;
        }
    }

    private void OpenSave(bool saving)
    {
        if(LoggedIn)
        {
            _isSaving = saving;
            ISavedGameClient savedGameClient = _platform.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution("SpacePalettes_SavedGame_Resolution", 
                DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, SaveGameOpen);
        }
    }

    private void SaveGameOpen(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedGameClient = _platform.SavedGame;
            if(_isSaving) // save Game
            {
                // Convert data to a byte array
                byte[] myData = System.Text.Encoding.ASCII.GetBytes(GetSaveString());

                // Update our metadata
                SavedGameMetadataUpdate updateForMetadata = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription("I have updated my game at: " + DateTime.Now.ToString()).Build();

                // Commit your save
                savedGameClient.CommitUpdate(meta, updateForMetadata, myData, SaveCallback);
            }
            else // load game
            {
                savedGameClient.ReadBinaryData(meta, LoadCallback);
            }
        }
    }

    private void LoadCallback(SavedGameRequestStatus status, byte[] data)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            string loadedData = System.Text.Encoding.ASCII.GetString(data);

            LoadSaveString(loadedData);
        }
    }

    private void LoadSaveString(string cloudData)
    {
        string[] cloudStringArr = cloudData.Split();
        _cloudSavedScore = int.TryParse(cloudStringArr[0], out int result) ? result : 0;

        PlayerPrefs.SetInt(Helpers.BestScoreKey, _cloudSavedScore); 
        PlayerPrefs.Save();
    }

    private string GetSaveString()
    {
        string dataToSave = _scoreKeeper.Score.ToString();

        return dataToSave;
    }

    private void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Successfully saved to the cloud");
        }
        else
        {
            Debug.Log("Failed to save to the cloud");
        }
    }

    private void ReportScoreOnLeaderboard(int score)
    {
        Social.ReportScore(score, SpacePalettes.GPGSIds.leaderboard_ranking, (bool success) =>
        {
            if (success)
            {
                if (score > _cloudSavedScore)
                {
                    OpenSave(true); 
                }
                Debug.Log("Successfully saved to leaderboard");
            }
            else
            {
                Debug.Log("Cannot save score to board");
            }
        });
    }
}

