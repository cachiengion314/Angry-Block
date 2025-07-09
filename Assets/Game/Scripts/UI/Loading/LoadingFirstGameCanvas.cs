using System.Collections;
using Unity.Advertisement.IosSupport.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingFirstGameCanvas : MonoBehaviour
{
  [SerializeField] Slider loadingSlider;

  private void OnEnable()
  {
    ContextScreenManager.onCompletedAT += LoadLevel;
  }

  private void OnDisable()
  {
    ContextScreenManager.onCompletedAT -= LoadLevel;
  }

  public void LoadLevel()
  {
    StartCoroutine(LoadSceneFake());
  }

  float[] waitLengths = new float[4] { .5f, .7f, .9f, .9f };
  IEnumerator LoadSceneFake()
  {
    var LENGTH = waitLengths[UnityEngine.Random.Range(0, waitLengths.Length)];
    var timer = LENGTH;

#if UNITY_IOS
    if (FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.network_required.ios.network_required)
    {
      if (Application.internetReachability == NetworkReachability.NotReachable)
      {
        FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(true);
      }

      yield return new WaitUntil(() => GameManager.Instance.HasInternet());
      FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(false);
    }
#elif UNITY_ANDROID
    if (FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.network_required.android.network_required)
    {
      if (Application.internetReachability == NetworkReachability.NotReachable)
      {
        FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(true);
      }

      yield return new WaitUntil(() => GameManager.Instance.HasInternet());
      FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(false);
    }
#endif

    FirebaseSetup.Instance.IsStartCheckInternet = true;

#if !UNITY_EDITOR
    yield return new WaitUntil(() => FirebaseSetup.Instance.FirebaseStatusCode != 0);
#endif

    while (timer > 0)
    {
      float progressValue = Mathf.Clamp01((LENGTH - timer) / LENGTH);
      loadingSlider.value = progressValue;
      timer -= Time.deltaTime;
      yield return null;
    }

    GameManager.Instance.InitDailyUserProgressData();
    DailyTaskManager.Instance.CheckIsExistingData();
    DailyWeeklyManager.Instance.CheckIsExistingData();
#if !UNITY_EDITOR
    FirebaseSetup.Instance.AuthenticateUser();
    yield return new WaitUntil(() => FirebaseSetup.Instance.HasAuthenticateDone);
#endif
    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseSetup.Instance.ApplyJsonData();
      DailyTaskManager.Instance.CheckDataFirebase();
      DailyWeeklyManager.Instance.CheckDataFirebase();
    }

    var sceneName = KeyString.NAME_SCENE_LOBBY;

    AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
    while (!loadAsync.isDone)
    {
      yield return null;
    }
  }
}
