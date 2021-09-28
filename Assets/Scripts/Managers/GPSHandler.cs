using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPSHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("INIT GPSHandler");
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }

    // Update is called once per frame
    void Update()
    {

    }
    #region Sign In / Sign Out
    public static void SignIn()
    {
        Debug.Log("Attempting to sign in");

        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptOnce, (result) => //Googles best practice sign in process. Attempts a silent sign in. if fails continues to interactive sign in
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        Debug.Log("Success");
                        break;
                    case SignInStatus.UiSignInRequired:
                        Debug.Log("UiSignInRequired");
                        break;
                    case SignInStatus.DeveloperError:
                        Debug.Log("DeveloperError");
                        break;
                    case SignInStatus.NetworkError:
                        Debug.Log("NetworkError");
                        break;
                    case SignInStatus.InternalError:
                        Debug.Log("InternalError");
                        break;
                    case SignInStatus.Canceled:
                        Debug.Log("Canceled");
                        break;
                    case SignInStatus.AlreadyInProgress:
                        Debug.Log("AlreadyInProgress");
                        break;
                    case SignInStatus.Failed:
                        Debug.Log("Failed");
                        break;
                    case SignInStatus.NotAuthenticated:
                        Debug.Log("NotAuthenticated");
                        break;
                    default:
                        break;
                }
                GameManager.instance.uiM.InitGameMenu(); //Initialise game once authentication process has been completed
            });
        }
        else
        {
            Debug.Log("User already athenticated");
        }
    }

    public static void InteractiveSignIn(System.Action successAction)
    {
        Debug.Log("Attempting interactive sign in");

        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, (result) => //Googles best practice sign in process. Attempts a silent sign in. if fails continues to interactive sign in
            {
                switch (result)
                {
                    case SignInStatus.Success:
                        Debug.Log("Success");
                        if (successAction is object)
                            successAction.Invoke();
                        break;
                    case SignInStatus.UiSignInRequired:
                        Debug.Log("UiSignInRequired");
                        break;
                    case SignInStatus.DeveloperError:
                        Debug.Log("DeveloperError");
                        break;
                    case SignInStatus.NetworkError:
                        Debug.Log("NetworkError");
                        break;
                    case SignInStatus.InternalError:
                        Debug.Log("InternalError");
                        break;
                    case SignInStatus.Canceled:
                        Debug.Log("Canceled");
                        break;
                    case SignInStatus.AlreadyInProgress:
                        Debug.Log("AlreadyInProgress");
                        break;
                    case SignInStatus.Failed:
                        Debug.Log("Failed");
                        break;
                    case SignInStatus.NotAuthenticated:
                        Debug.Log("NotAuthenticated");
                        break;
                    default:
                        break;
                }
                GameManager.instance.uiM.LogInProccesed();
            });
        }
        else
        {
            Debug.Log("User already athenticated");
        }
    }

    public static bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.IsAuthenticated();
    }

    public static void LogOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }
    #endregion
    #region Leaderboard
    public static void UpdateHighScoreLeaderboard(int newScore)
    {
        Social.ReportScore(newScore, "CgkIvJX1_o0REAIQAQ", (bool success) =>
        {
            if (success)
            {
                Debug.Log("New highscore posted");
            }
            else
            {
                Debug.Log("Highscore upload failed");
            }
        });
    }

    #endregion

    #region Achievements

    public static void UpdateScoreAchievement(int newScore)
    {
        if (newScore > 20)
        {
            ReportProgress("CgkIvJX1_o0REAIQAw", 100f);
        }
        if (newScore > 75)
        { 
            ReportProgress("CgkIvJX1_o0REAIQBA", 100f);
        }
        if (newScore > 150)
        { 
            ReportProgress("CgkIvJX1_o0REAIQBQ", 100f);
        }
        if (newScore > 250)
        { 
            ReportProgress("CgkIvJX1_o0REAIQBg", 100f);
        }
    }

    public static void GamePlayed()
    {
        IncrementProgress("CgkIvJX1_o0REAIQBw", 1); //10games
        IncrementProgress("CgkIvJX1_o0REAIQCA", 1); //25games
        IncrementProgress("CgkIvJX1_o0REAIQCQ", 1); //50games
        IncrementProgress("CgkIvJX1_o0REAIQCg", 1); //100games
        IncrementProgress("CgkIvJX1_o0REAIQCw", 1); //1000games
    }

    public static void AllPalletesUnlocked()
    {
        ReportProgress("CgkIvJX1_o0REAIQDA", 100f);
    }
    private static void ReportProgress(string id, double progress)
    {
        Social.ReportProgress(id, progress, (bool success) =>
        {
            if (!success)
            {
                Debug.LogError("Score achivement could not be reported");
            }
        });
    }

    private static void IncrementProgress(string id, int steps)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(id, steps, (bool success) =>
        {
            if (!success)
            {
                Debug.LogError("Score achivement could not be incremented");
            }
        });
    }

    #endregion
}
