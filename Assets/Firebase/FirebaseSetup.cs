using UnityEngine;
using Firebase.Extensions;
using Firebase.Analytics;
using System.Threading.Tasks;
using Firebase.Crashlytics;
using System;

public partial class FirebaseSetup : MonoBehaviour
{
  public static Action onNeedUpdate;
  public static Action onUpdateRemote;

  public static FirebaseSetup Instance { get; private set; }
  public bool IsFirebaseReady { get; private set; }
  private Firebase.FirebaseApp app;

  [Header("Internal Dependences")]
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
  }

  void Init()
  {
    if (Debug.isDebugBuild) return;

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

        IsFirebaseReady = true;

        // Set a flag here to indicate whether Firebase is ready to use by your app.
        InitializeFirebaseMessage();

        int firstOpen = PlayerPrefs.GetInt(KeyString.FIREBASE_FIRST_OPEN, 0);
        if (firstOpen == 0)
        {
          // first time open app
          PlayerPrefs.SetInt(KeyString.FIREBASE_FIRST_OPEN, 1);
          if (FirebaseSetup.Instance.IsFirebaseReady)
          {
            FirebaseAnalytics.LogEvent(KeyString.FIREBASE_FIRST_OPEN);
          }
        }


      }
      else
      {
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
}