using UnityEngine;
#if UNITY_ANDROID
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
#endif

public class GooglePlayGameManager : MonoBehaviour
{
  public string Token;
  public string Error;


#if UNITY_ANDROID
  void Start()
  {
    if (Application.platform == RuntimePlatform.Android) // prevent invoke in unity editor
    {
      // PlayGamesPlatform.Activate();
      // LoginGooglePlayGames();
    }
  }

  public void LoginGooglePlayGames()
  {
    // PlayGamesPlatform.Instance.Authenticate((success) =>
    // {
    //   if (success == SignInStatus.Success)
    //   {
    //     Debug.Log("Login with Google Play games successful.");

    //     var id = PlayGamesPlatform.Instance.GetUserId();
    //     var name = PlayGamesPlatform.Instance.GetUserDisplayName();

    //     GameManager.Instance.OnGotUserIdSuccessfully(id, name);
    //   }
    //   else
    //   {
    //     Error = "Failed to retrieve Google play games authorization code";
    //     Debug.Log("Login Unsuccessful");
    //     GameManager.Instance.OnGotUserIdFail();
    //   }
    // });
  }
#endif
}