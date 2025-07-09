using UnityEngine;
using Firebase.Extensions;
using Firebase.Analytics;
using System.Threading.Tasks;
using Firebase.Crashlytics;
using System;
using Firebase.RemoteConfig;

public partial class FirebaseSetup : MonoBehaviour
{
  public static Action onNeedUpdate;
  public static Action onUpdateRemote;

  public static FirebaseSetup Instance { get; private set; }
  public int FirebaseStatusCode { get; private set; }
  private Firebase.FirebaseApp app;

  [Header("Internal Dependences")]
  [SerializeField] FirebaseRemoteData _firebaseRemoteData;
  // [SerializeField] UpdateConfigData updateConfigData;
  public FirebaseRemoteData FirebaseRemoteData
  {
    get { return _firebaseRemoteData; }
  }

  [HideInInspector] public bool IsStartCheckInternet;

  private const float TIME_COOLDOWN_CHECKINTERNET = 5;

  private float _currentTimeCoolDownCheckInternet;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;

      _currentTimeCoolDownCheckInternet = TIME_COOLDOWN_CHECKINTERNET;

      Init();
    }
    else Destroy(gameObject);
    DontDestroyOnLoad(gameObject);
  }

  private void Update()
  {
    if (!IsStartCheckInternet) return;

    _currentTimeCoolDownCheckInternet -= Time.deltaTime;
    if (_currentTimeCoolDownCheckInternet > 0) return;
    _currentTimeCoolDownCheckInternet = TIME_COOLDOWN_CHECKINTERNET;

    IntervalCheckInternet();
  }

  void Init()
  {
    if (Debug.isDebugBuild) { FirebaseStatusCode = 2; return; }

    Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
    {
      var dependencyStatus = task.Result;
      if (dependencyStatus == Firebase.DependencyStatus.Available)
      {
        // Create and hold a reference to your FirebaseApp,
        // where app is a Firebase.FirebaseApp property of your application class.
        app = Firebase.FirebaseApp.DefaultInstance;

        // When this property is set to true, Crashlytics will report all
        // uncaught exceptions as fatal events. This is the recommended behavior.
        if (Debug.isDebugBuild)
        {
          Crashlytics.ReportUncaughtExceptionsAsFatal = false;
        }
        else
        {
          Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        }

        FirebaseStatusCode = 1;

        // Set a flag here to indicate whether Firebase is ready to use by your app.
        InitializeFirebaseMessage();

        int firstOpen = PlayerPrefs.GetInt(KeyString.FIREBASE_FIRST_OPEN, 0);
        if (firstOpen == 0)
        {
          // first time open app
          PlayerPrefs.SetInt(KeyString.FIREBASE_FIRST_OPEN, 1);
          if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
          {
            FirebaseAnalytics.LogEvent(KeyString.FIREBASE_FIRST_OPEN);
          }
        }
        // 
        FetchDataAsync();
      }
      else
      {
        FirebaseStatusCode = 2;
        Debug.LogError(System.String.Format(
              "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
        // Firebase Unity SDK is not safe to use here.
      }
    });
  }

  // Setup message event handlers.
  private string topic = "TestTopic";
  void InitializeFirebaseMessage()
  {
    Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
    Firebase.Messaging.FirebaseMessaging.SubscribeAsync(topic).ContinueWithOnMainThread(task =>
    {
      LogTaskCompletion(task, "SubscribeAsync");
    });
    Debug.Log("Firebase Messaging Initialized");

    // On iOS, this will display the prompt to request permission to receive
    // notifications if the prompt has not already been displayed before. (If
    // the user already responded to the prompt, thier decision is cached by
    // the OS and can be changed in the OS settings).
    // On Android, this will return successfully immediately, as there is no
    // equivalent system logic to run.
    Firebase.Messaging.FirebaseMessaging.RequestPermissionAsync().ContinueWithOnMainThread(
      task =>
      {
        LogTaskCompletion(task, "RequestPermissionAsync");
      }
    );
  }

  public virtual void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
  {
    Debug.Log("Received a new message");
    var notification = e.Message.Notification;
    if (notification != null)
    {
      Debug.Log("title: " + notification.Title);
      Debug.Log("body: " + notification.Body);
      var android = notification.Android;
      if (android != null)
      {
        Debug.Log("android channel_id: " + android.ChannelId);
      }
    }
    if (e.Message.From.Length > 0)
      Debug.Log("from: " + e.Message.From);
    if (e.Message.Link != null)
    {
      Debug.Log("link: " + e.Message.Link.ToString());
    }
    if (e.Message.Data.Count > 0)
    {
      Debug.Log("data:");
      foreach (System.Collections.Generic.KeyValuePair<string, string> iter in
               e.Message.Data)
      {
        Debug.Log("  " + iter.Key + ": " + iter.Value);
      }
    }
  }

  public virtual void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
  {
    Debug.Log("Received Registration Token: " + token.Token + ";token end here;");
  }

  // [START fetch_async]
  // Start a fetch request.
  // FetchAsync only fetches new data if the current data is older than the provided
  // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
  // By default the timespan is 12 hours, and for production apps, this is a good
  // number. For this example though, it's set to a timespan of zero, so that
  // changes in the console will always show up immediately.
  public Task FetchDataAsync()
  {
    Debug.Log("Fetching data...");
    System.Threading.Tasks.Task fetchTask =
    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
        System.TimeSpan.Zero);
    return fetchTask.ContinueWithOnMainThread(FetchComplete);
  }
  //[END fetch_async]

  void FetchComplete(Task fetchTask)
  {
    if (fetchTask.IsCanceled)
    {
      Debug.Log("Fetch canceled.");
    }
    else if (fetchTask.IsFaulted)
    {
      Debug.Log("Fetch encountered an error.");
    }
    else if (fetchTask.IsCompleted)
    {
      Debug.Log("Fetch completed successfully!");
    }

    var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
    switch (info.LastFetchStatus)
    {
      case Firebase.RemoteConfig.LastFetchStatus.Success:
        Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
        .ContinueWithOnMainThread(task =>
        {
          Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                               info.FetchTime));
          ConvertDataFromRemote();
        });

        break;
      case Firebase.RemoteConfig.LastFetchStatus.Failure:
        switch (info.LastFetchFailureReason)
        {
          case Firebase.RemoteConfig.FetchFailureReason.Error:
            Debug.Log("Fetch failed for unknown reason");
            break;
          case Firebase.RemoteConfig.FetchFailureReason.Throttled:
            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
            break;
          default:
            break;
        }
        break;
      case Firebase.RemoteConfig.LastFetchStatus.Pending:
        Debug.Log("Latest Fetch call still pending.");
        break;
    }
  }


  public void LogMeButton()
  {
    FirebaseAnalytics.LogEvent("LogMe_button_pressed");
  }

  public void PressNumberButton(int number)
  {
    FirebaseAnalytics.LogEvent("Press_Number_button_pressed", new Parameter[] {
            new("ButtonNumber", number),
            new("ButtonNumber", number),
        });
  }

  #region Handle Function
  private void ConvertDataFromRemote()
  {
    var firebaseRemoteData = _firebaseRemoteData;
    // var parameterGroupsJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_PARAMETERGROUPS).StringValue;
    var updateconfigJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_UPDATECONFIG).StringValue;
    var configGameJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_CONFIGGAME).StringValue;
    var admobJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_ADMOB).StringValue;
    var luckyWheelJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_LUCKYWHEEL).StringValue;
    var levelDesignJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_LEVELDESIGNDATA).StringValue;
    var networkRequiredJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_NETWORKREQUIRED).StringValue;
    var freeCoinJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_FREECOIN).StringValue;
    var cheatSettingJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_CHEATSETTING).StringValue;
    var refillHeartJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_REFILLHEART).StringValue;
    var eggHuntOfferJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_EGGHUNT_OFFER).StringValue;
    var luckyRewardJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_LUCKYREWARD).StringValue;
    var dailyTaskJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_DAILYTASK).StringValue;
    var dailyWeeklyJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_DAILYWEEKLY).StringValue;
    var eggHuntEventJson = FirebaseRemoteConfig.DefaultInstance.GetValue(KeyString.FIREBASE_REMOTECONFIG_EGGHUNT_EVENT).StringValue;

    if (!updateconfigJson.Equals(""))
    {
      var updateConfig = JsonUtility.FromJson<UpdateConfigData>(updateconfigJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.update_config = updateConfig;
    }

    if (!configGameJson.Equals(""))
    {
      var configGame = JsonUtility.FromJson<ConfigGameData>(configGameJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.config_game = configGame;
    }

    if (!admobJson.Equals(""))
    {
      var admob = JsonUtility.FromJson<AdmobData>(admobJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.Admob = admob;
    }

    if (!luckyWheelJson.Equals(""))
    {
      var luckyWheel = JsonUtility.FromJson<LuckyWheelDataRemote>(luckyWheelJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel = luckyWheel;
    }

    if (!levelDesignJson.Equals(""))
    {
      var datas = JsonUtility.FromJson<LevelDesignDatas>(levelDesignJson);

      LoadOverwriteLevelDataFrom(datas);
    }

    if (!networkRequiredJson.Equals(""))
    {
      var networkRequired = JsonUtility.FromJson<NetworkRequiredData>(luckyWheelJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.network_required = networkRequired;
    }

    if (!freeCoinJson.Equals(""))
    {
      var freeCoin = JsonUtility.FromJson<FreeCoinData>(freeCoinJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.free_coin = freeCoin;
    }

    if (!cheatSettingJson.Equals(""))
    {
      var cheatSetting = JsonUtility.FromJson<CheatData>(cheatSettingJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.cheat_setting = cheatSetting;
    }

    if (!refillHeartJson.Equals(""))
    {
      var refillHeart = JsonUtility.FromJson<RefillHeartData>(refillHeartJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.refill_heart = refillHeart;
    }

    if (!eggHuntOfferJson.Equals(""))
    {
      var eggHuntOffer = JsonUtility.FromJson<EggHuntLuckyOfferData>(eggHuntOfferJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer = eggHuntOffer;
    }

    if (!luckyRewardJson.Equals(""))
    {
      var luckyReward = JsonUtility.FromJson<LuckyRewardRemoteData>(luckyRewardJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.lucky_reward = luckyReward;
    }

    if (!dailyTaskJson.Equals(""))
    {
      var dailyTask = JsonUtility.FromJson<DailyTaskData>(dailyTaskJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.daily_task = dailyTask;
    }

    if (!dailyWeeklyJson.Equals(""))
    {
      var dailyWeekly = JsonUtility.FromJson<DailyWeeklyData>(dailyWeeklyJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.daily_weekly = dailyWeekly; // dòng ghi đè 
    }

    if (!eggHuntEventJson.Equals(""))
    {
      var eggHuntEvent = JsonUtility.FromJson<EggHuntLuckyEventData>(eggHuntEventJson);

      firebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event = eggHuntEvent; // dòng ghi đè 
    }

    _firebaseRemoteData = firebaseRemoteData;
    onUpdateRemote?.Invoke();
  }

  public void CheckVersion()
  {
    var version = "";
    var reward = 0;

#if UNITY_ANDROID
    version = _firebaseRemoteData.parameterGroups.Lobby.parameters.update_config.android.version;
    reward = _firebaseRemoteData.parameterGroups.Lobby.parameters.update_config.android.reward;
#elif UNITY_IOS
    version = _firebaseRemoteData.parameterGroups.Lobby.parameters.update_config.ios.version;
    reward = _firebaseRemoteData.parameterGroups.Lobby.parameters.update_config.ios.reward;
#endif

    var keyVersion = "version" + version;
    if (!IsNeedUpdateWith(version))
    {
      if (PlayerPrefs.GetInt(keyVersion, 0) == 0)
      {
        PlayerPrefs.SetInt(keyVersion, 1);
        ClaimReward(reward);
      }
    }
    else
    {
      LobbyPanel.Instance.OpenUpdateModal();
      onNeedUpdate?.Invoke();
    }
  }

  private void ClaimReward(int coinReward)
  {
    var coin = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_COIN, 100);
    coin += coinReward;

    PlayerPrefs.SetInt(KeyString.KEY_CURRENT_COIN, coin);
  }

  // Log the result of the specified task, returning true if the task
  // completed successfully, false otherwise.
  protected bool LogTaskCompletion(Task task, string operation)
  {
    bool complete = false;
    if (task.IsCanceled)
    {
      Debug.Log(operation + " canceled.");
    }
    else if (task.IsFaulted)
    {
      Debug.Log(operation + " encounted an error.");
      foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
      {
        string errorCode = "";
        Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
        if (firebaseEx != null)
        {
          errorCode = String.Format("Error.{0}: ",
            ((Firebase.Messaging.Error)firebaseEx.ErrorCode).ToString());
        }
        Debug.Log(errorCode + exception.ToString());
      }
    }
    else if (task.IsCompleted)
    {
      Debug.Log(operation + " completed");
      complete = true;
    }
    return complete;
  }

  #endregion

  #region Check Function
  private bool IsNeedUpdateWith(string remoteVersion)
  {
    var currentVersion = Application.version;

    if (currentVersion.Equals(remoteVersion)) return false;

    var curVers = currentVersion.Split('.');
    var remoteVers = remoteVersion.Split('.');

    if (remoteVers.Length == 0) return false;
    if (curVers.Length <= remoteVers.Length)
    {
      for (int i = 0; i < curVers.Length; i++)
      {
        var curVer = int.Parse(curVers[i]);
        // Debug.Log("---" + remoteVers[i]);
        var remoteVer = int.Parse(remoteVers[i]);


        if (curVer < remoteVer) return true;
        if (curVer > remoteVer) return false;

        continue;
      }

      return true;
    }

    if (curVers.Length > remoteVers.Length)
    {
      for (int i = 0; i < remoteVers.Length; i++)
      {
        var curVer = int.Parse(curVers[i]);
        var remoteVer = int.Parse(remoteVers[i]);

        if (curVer < remoteVer) return true;
        if (curVer > remoteVer) return false;

        continue;
      }

      return false;
    }

    return false;
  }

  #endregion


  private void IntervalCheckInternet()
  {
#if UNITY_IOS
    if (FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.network_required.ios.network_required)
    {
      if (!GameManager.Instance.HasInternet())
      {
        FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(true);
      }
      else
      {
        FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(false);
      }
    }
#elif UNITY_ANDROID
    if (FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.network_required.android.network_required)
    {
      if (!GameManager.Instance.HasInternet())
      {
        FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(true);
      }
      else
      {
        FirebaseSetup.Instance.CheckInternetModal.gameObject.SetActive(false);
      }
    }
#endif 
  }
}

[Serializable]
public struct FirebaseRemoteData
{
  public ParameterGroups parameterGroups;
}

[Serializable]
public struct ParameterGroups
{
  public LobbyData Lobby;
}
