using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using Firebase.Auth;
using UnityEngine;
using System;
using System.Reflection;
using HoangNam;

/// <summary>
/// User firestore data model
/// </summary>
public class UserData
{
  // User’s PlayerPrefs 
  public int RetryCount;
  public int AppOpennedTodayCount;
  public int FreeCoinTodayCount;
  public int LuckyWheelAdsTodayCount;
  public string LatestOpennedDate;
  public int PlinkoAdsTodayCount;
  public int PlinkoFreeTodayCount;
  public int CurrentSwap;
  public int CurrentRefresh;
  public int CurrentHammer;
  public int CurrentRocket;
  public int CurrentTicket;
  public int CurrentLevel;
  public int AmountCompletedQuest;
  public int MoveEggTicketsAmount;
  public int MoveEggTicketsAmountNeedClaim;
  public int CurrentCoin;
  public bool IsSoundOn;
  public bool IsMusicOn;
  public bool IsHapticOn;
  public bool IsReceivedGift1;
  public bool IsReceivedGift2;
  public bool IsReceivedGift3;
  public bool IsUnlockFollowing;
  public bool IsRemoveAds;
  public bool IsRemoveAds7d;
  // lucky offer
  public float TimeCoolDownLuckyOffer;
  public int CurrentClaimLuckyOffer;
  // meta progress
  public int CurrentMetaProgressTotal;
  public int MetaProgressNeedClaim;
  public int CurrentMetaStreak;
  public int NextMetaStreak;
  public int CurrentMilestone;
  // daily task
  public string TaskDataJsonBase;
  public string VersionDailyTask;
  public string VersionDataDailyTask;
  public string TodayDailyTask;
  public int RewardDailTaskClaimed_0;
  public int RewardDailTaskClaimed_1;
  public int RewardDailTaskClaimed_2;
  // heart
  public float UseInfinityHeartTime;
  public DateTime LastedTimeHeart;
  public DateTime LastedTimeInfinityHeart;
  public int CurrentHeart;
  public int FirstTimeHeart;
  // daily reward
  public string DayRewardData;
  // daily bonus
  public string DailyBonusData;
  // daily week
  public string WeeklyTaskDataJsonBase;
  public string VersionDailyWeekly;
  public string VersionDataDailyWeekly;
  public int RewardWeeklyClaimed_0;
  public int RewardWeeklyClaimed_1;

  // Tutorial Booster
  public int IsCompletedTutorialHammer;
  public int IsCompletedTutorialRocket;
  public int IsCompletedTutorialSwap;
  public int IsCompletedTutorialRefresh;

  public int IsFirstUseHammer;
  public int IsFirstUseRocket;
  public int IsFirstUseSwap;
  public int IsFirstUseRefresh;

  /// <summary>
  /// indexer
  /// </summary>
  /// <param name="fieldName"></param>
  /// <returns></returns>
  public object this[string fieldName]
  {
    get
    {
      FieldInfo field = GetType().GetField(fieldName);
      return field?.GetValue(this);
    }
    set
    {
      FieldInfo field = GetType().GetField(fieldName);
      field?.SetValue(this, value);
    }
  }
}

/// <summary>
/// UsersFirestoreManager
/// </summary> <summary>
/// </summary>
public partial class FirebaseSetup : MonoBehaviour
{
  [Header("UsersFirestoreManager")]
  AuthResult _authResult;
  readonly string _DEFAULT_PASSWORD = "123456";
  bool _hasAuthenticateDone;
  public bool HasAuthenticateDone { get { return _hasAuthenticateDone; } }
  string _taskDataJsonBase = null;
  public string TaskDataJsonBase { get { return _taskDataJsonBase; } }
  string _dayRewardData = null;
  public string dayRewardData { get { return _dayRewardData; } }
  public DayRewardData DayRewardData = null;
  string _dailyBonusData = null;
  public string dailyBonusData { get { return _dailyBonusData; } }
  public DailyBonusData DailyBonusData;
  string _weeklyTaskDataJsonBase;
  public string weeklyTaskDataJsonBase { get { return _weeklyTaskDataJsonBase; } }

  public void AuthenticateUser()
  {
    if (GameManager.Instance.GotUserIdResultCode != 1 || FirebaseStatusCode != 1)
    {
      print("AuthenticateUser.1.GotUserIdResultCode " + GameManager.Instance.GotUserIdResultCode);
      _hasAuthenticateDone = true;
      return;
    }
    print("AuthenticateUser.2");
    AuthenticateUser(
      GameManager.Instance.UserId,
      result =>
      {
        // if (PlayerPrefs.Get(KeyString.KEY_FIRST_TIME_OPENING, 1) == 1)
        // {
        //   PlayerPrefs.Set(KeyString.KEY_FIRST_TIME_OPENING, 0);
        _authResult = result;
        TryGetUserFirestoreData(
          GameManager.Instance.UserId, result,
          userData =>
          {
            OnGetUserDataSuccessfully(userData);
            _hasAuthenticateDone = true;
          },
          () =>
          {
            OnUserDataNotExisted(result);
            _hasAuthenticateDone = true;
          },
          () =>
          {
            _hasAuthenticateDone = true;
          }
        );
        //   return;
        // }
        print("AuthenticateUser.3");
        // _hasAuthenticateDone = true;
      },
      () =>
      {
        print("AuthenticateUser.4");
        _hasAuthenticateDone = true;
      }
    );
  }

  public void UpdateOneUserField(
   string fieldName,
   object value,
   Action _onSuccess = null
 )
  {
    var userId = GameManager.Instance.UserId;
    var db = FirebaseFirestore.DefaultInstance;
    DocumentReference docRef = db.Collection(KeyString.KEY_USERSDB).Document(userId);

    Dictionary<string, object> updates = new()
    {
      { "UpdatedAt", Timestamp.GetCurrentTimestamp()},
      { fieldName, value},
    };

    docRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
    {
      if (task.IsFaulted)
      {
        Debug.LogWarning("Update failed: Document does not exist or other error occurred.");
      }

      if (task.IsCompletedSuccessfully)
      {
        _onSuccess?.Invoke();
      }
    });
  }

  public void CreateUserFields(AuthResult result)
  {
    if (FirebaseStatusCode != 1) return;

    var userData = SnapshotUserData(true);
    TryCreateUserFirestore(
      GameManager.Instance.UserId, result,
      GameManager.Instance.UserName,
      Application.platform,
      userData
    );
  }

  public void BatchUpdateUserFields()
  {
    if (FirebaseStatusCode != 1) return;

    var userData = SnapshotUserData();
    TryUpdateUserFirestore(
      GameManager.Instance.UserId, _authResult,
      userData
    );
  }

  /// <summary>
  /// temporary saving for upload firestore latter
  /// </summary>
  /// <param name="dayRewardData"></param>
  public void SaveData(DayRewardData dayRewardData)
  {
    if (FirebaseStatusCode != 1) return;

    DayRewardData = dayRewardData;
  }

  /// <summary>
  /// temporary saving for upload firestore latter
  /// </summary>
  public void SaveData(DailyBonusData dailyBonusData)
  {
    if (FirebaseStatusCode != 1) return;

    DailyBonusData = dailyBonusData;
  }

  void AuthenticateUser(
   string userId,
   Action<AuthResult> _onSuccess = null,
   Action _onFail = null
  )
  {
    var email = GetUserEmailFrom(userId);
    SignIn(
      email,
      _DEFAULT_PASSWORD,
      result =>
      {
        // successfully sign in
        _onSuccess?.Invoke(result);
      },
      e =>
      {
        // sign in fail so we sign up user
        SignUp(
          email,
          _DEFAULT_PASSWORD,
          r => _onSuccess?.Invoke(r),
          e => _onFail?.Invoke()
        );
      }
    );
  }

  void SignIn(
    string email,
    string password,
    Action<AuthResult> _onSuccessfully = null,
    Action<Exception> _onError = null
  )
  {
    var auth = FirebaseAuth.DefaultInstance;
    auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
    {
      if (task.IsCanceled)
      {
        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
        _onError?.Invoke(task.Exception);
        return;
      }
      if (task.IsFaulted)
      {
        _onError?.Invoke(task.Exception);
        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        return;
      }

      AuthResult result = task.Result;
      Debug.LogFormat("User signed in successfully: {0} ({1})",
          result.User.DisplayName, result.User.UserId);

      _onSuccessfully?.Invoke(result);
    });
  }

  void SignUp(
    string email,
    string password,
    Action<AuthResult> _onSuccessfully = null,
    Action<Exception> _onError = null
  )
  {
    var auth = FirebaseAuth.DefaultInstance;
    auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
    {
      if (task.IsCanceled)
      {
        Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
        _onError?.Invoke(task.Exception);
        return;
      }
      if (task.IsFaulted)
      {
        Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
        _onError?.Invoke(task.Exception);
        return;
      }

      // Firebase user has been created.
      AuthResult result = task.Result;
      Debug.LogFormat("Firebase user created successfully: {0} ({1})",
          result.User.DisplayName, result.User.UserId);

      _onSuccessfully?.Invoke(result);
    });
  }

  string GetUserEmailFrom(string userId)
  {
    return userId + "@mail.com";
  }

  void TryCreateUserFirestore(
    string userId, AuthResult result,
    string username,
    RuntimePlatform platform,
    UserData userData
  )
  {
    var db = FirebaseFirestore.DefaultInstance;
    // Create user data
    Dictionary<string, object> data = new()
    {
      { "UserId", userId },
      { "Username", username },
      { "Platform", platform.ToString() },
      { "CreatedAt", Timestamp.GetCurrentTimestamp() },
    };

    var _data = CreateUserDicFrom(userData);
    foreach (var kvp in _data)
    {
      data[kvp.Key] = kvp.Value;
    }
    // Store user data in Firestore
    DocumentReference docRef
      = db.Collection(KeyString.KEY_USERSDB).Document(result.User.UserId);
    print("TryCreateUserFirestore.authResult.User.UserId " + result.User.UserId);
    docRef.SetAsync(data).ContinueWithOnMainThread(task =>
    {
      if (task.IsCompleted)
      {
        Debug.Log("User data added to Firestore.");
      }
      else
      {
        Debug.LogError("Failed to add user data to Firestore: " + task.Exception);
      }
    });
  }

  void TryUpdateUserFirestore(
   string userId, AuthResult authResult,
   UserData userData
  )
  {
    if (authResult == null) return;
    var db = FirebaseFirestore.DefaultInstance;
    DocumentReference docRef
      = db.Collection(KeyString.KEY_USERSDB).Document(authResult.User.UserId);
    print("TryUpdateUserFirestore.authResult.User.UserId " + authResult.User.UserId);
    // Prepare the data to update
    Dictionary<string, object> updates = new()
    {
      { "UpdatedAt", Timestamp.GetCurrentTimestamp() },
    };

    var _data = CreateUserDicFrom(userData);
    foreach (var kvp in _data)
    {
      updates[kvp.Key] = kvp.Value;
    }

    // Attempt to update the document
    docRef.UpdateAsync(updates).ContinueWithOnMainThread(task =>
    {
      if (task.IsCompletedSuccessfully)
      {
        Debug.Log("User data updated successfully.");
      }
      else
      {
        // If the document does not exist
        Debug.Log("User data updated fail.");
      }
    });
  }

  Dictionary<string, object> CreateUserDicFrom(UserData userData)
  {
    Dictionary<string, object> data = new();
    Type type = userData.GetType();
    FieldInfo[] fields = type.GetFields();

    foreach (FieldInfo field in fields)
    {
      string name = field.Name;
      object value = field.GetValue(userData);
      data[name] = value;
    }

    return data;
  }

  void TryGetUserFirestoreData(
   string userId, AuthResult result,
   Action<UserData> _onSuccessfully = null,
   Action _onNotExisted = null,
   Action _onError = null
  )
  {
    DocumentReference docRef = FirebaseFirestore.DefaultInstance
      .Collection(KeyString.KEY_USERSDB)
      .Document(result.User.UserId);
    print("TryGetUserFirestoreData.000");
    docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    {
      if (task.IsFaulted)
      {
        Debug.LogError("Error retrieving document: " + task.Exception);
        _onError?.Invoke();
        return;
      }

      print("TryGetUserFirestoreData.001");
      DocumentSnapshot snapshot = task.Result;
      if (snapshot.Exists)
      {
        print("TryGetUserFirestoreData.002");
        var user = new UserData();
        Dictionary<string, object> data = snapshot.ToDictionary();

        foreach (KeyValuePair<string, object> kvp in data)
        {
          string key = kvp.Key;
          object value = kvp.Value;

          if (
            key == "UserId" ||
            key == "Username" ||
            key == "UpdatedAt" ||
            key == "CreatedAt" ||
            key == "Platform"
          ) continue;

          FieldInfo field = user.GetType().GetField(key);
          if (field == null || value == null) continue;

          if (field.FieldType == typeof(DateTime))
          {
            Timestamp firestoreTimestamp = (Timestamp)value;
            DateTime dateTime = firestoreTimestamp.ToDateTime().ToLocalTime();
            value = dateTime;
          }
          else
            value = Convert.ChangeType(value.ToString(), field.FieldType);

          user[key] = value;
        }

        _taskDataJsonBase = user.TaskDataJsonBase;
        _weeklyTaskDataJsonBase = user.WeeklyTaskDataJsonBase;
        _dayRewardData = user.DayRewardData;
        _dailyBonusData = user.DailyBonusData;
        print("TryGetUserFirestoreData.onSuccessfully ");
        _onSuccessfully?.Invoke(user);
      }
      else
      {
        print("TryGetUserFirestoreData._onNotExisted");
        _onNotExisted?.Invoke();
      }
    });
  }

  UserData SnapshotUserData(bool _isTheFirstTimeTakeSnapshot = false)
  {
    var taskDataJsonBase = JsonUtility.ToJson(DailyTaskManager.Instance._taskDataJsonBase);
    string weeklyTaskDataJsonBase = null;
    if (DailyWeeklyManager.Instance.weeklyTaskDataJsonBase != null)
      weeklyTaskDataJsonBase = JsonUtility.ToJson(DailyWeeklyManager.Instance.weeklyTaskDataJsonBase);

    if (!DailyRewardModal.HasUserOpened)
    {
      DayRewardData = SaveSystem.LoadWith<DayRewardData>(KeyString.NAME_DAY_REWARD_DATA);
      DayRewardData ??= DailyRewardModal.CreateDefaultDayRewardData();
    }
    string dayRewardData = JsonUtility.ToJson(DayRewardData);

    if (!InitDailyBonus.HasUserOpened)
    {
      DailyBonusData = SaveSystem.LoadWith<DailyBonusData>(KeyString.NAME_DAILYBONUS_DATA);
      if (DailyBonusData.IsNull)
        DailyBonusData = InitDailyBonus.CreateDefaultDailyBonusData();
    }
    var dailyBonusData = JsonUtility.ToJson(DailyBonusData);

    var userData = new UserData()
    {
      // User’s PlayerPrefs 
      RetryCount = GameManager.Instance.RetryCount,
      AppOpennedTodayCount = GameManager.Instance.AppOpenadTodayCount,
      FreeCoinTodayCount = GameManager.Instance.FreeCoinTodayCount,
      LuckyWheelAdsTodayCount = GameManager.Instance.LuckyWheelAdsTodayCount,
      LatestOpennedDate = PlayerPrefs.GetString(KeyString.KEY_LATEST_OPENED_DATE, ""),
      PlinkoAdsTodayCount = GameManager.Instance.PlinkoAdsTodayCount,
      PlinkoFreeTodayCount = GameManager.Instance.PlinkoFreeTodayCount,
      CurrentSwap = GameManager.Instance.CurrentSwap,
      CurrentRefresh = GameManager.Instance.CurrentRefresh,
      CurrentHammer = GameManager.Instance.CurrentHammer,
      CurrentRocket = GameManager.Instance.CurrentRocket,
      CurrentTicket = GameManager.Instance.CurrentTicket,
      CurrentLevel = GameManager.Instance.CurrentLevel,
      AmountCompletedQuest = GameManager.Instance.AmountCompletedQuest,
      MoveEggTicketsAmount = GameManager.Instance.MoveEggTicketsAmount,
      MoveEggTicketsAmountNeedClaim
        = GameManager.Instance.MoveEggTicketsAmountNeedClaim,
      CurrentCoin = GameManager.Instance.CurrentCoin,
      IsSoundOn = GameManager.Instance.IsSoundOn,
      IsMusicOn = GameManager.Instance.IsMusicOn,
      IsHapticOn = GameManager.Instance.IsHapticOn,
      IsReceivedGift1 = GameManager.Instance.IsReceivedGift1,
      IsReceivedGift2 = GameManager.Instance.IsReceivedGift2,
      IsReceivedGift3 = GameManager.Instance.IsReceivedGift3,
      IsUnlockFollowing = GameManager.Instance.IsUnlockFollowing,
      IsRemoveAds = GameManager.Instance.IsRemoveAds,
      IsRemoveAds7d = GameManager.Instance.IsRemoveAds7d,
      // lucky offer
      TimeCoolDownLuckyOffer = GameManager.Instance.TimeCoolDownLuckyOffer,
      CurrentClaimLuckyOffer = GameManager.Instance.CurrentClaimLuckyOffer,
      // meta progress
      CurrentMetaProgressTotal = GameManager.Instance.CurrentMetaProgressTotal,
      MetaProgressNeedClaim = GameManager.Instance.MetaProgressNeedClaim,
      CurrentMetaStreak = GameManager.Instance.CurrentMetaStreak,
      NextMetaStreak = GameManager.Instance.NextMetaStreak,
      CurrentMilestone = GameManager.Instance.CurrentMilestone,
      // daily task
      TaskDataJsonBase = taskDataJsonBase,
      VersionDailyTask = PlayerPrefs.GetString(KeyString.KEY_VERSION_DAILY_TASK, ""),
      VersionDataDailyTask = PlayerPrefs.GetString(KeyString.KEY_VERSION_DATA_DAILY_TASK, ""),
      TodayDailyTask = PlayerPrefs.GetString(KeyString.KEY_TODAY_DAILY_TASK, ""),
      RewardDailTaskClaimed_0 = PlayerPrefs.GetInt($"RewardDailTaskClaimed_{0}", 0),
      RewardDailTaskClaimed_1 = PlayerPrefs.GetInt($"RewardDailTaskClaimed_{1}", 0),
      RewardDailTaskClaimed_2 = PlayerPrefs.GetInt($"RewardDailTaskClaimed_{2}", 0),
      // heart
      UseInfinityHeartTime = (float)HeartSystem.Instance.UseInfinityHeartTime,
      LastedTimeHeart = HeartSystem.Instance.LastedTimeHeart,
      LastedTimeInfinityHeart = HeartSystem.Instance.LastedTimeInfinityHeart,
      CurrentHeart = HeartSystem.Instance.CurrentHeart,
      FirstTimeHeart = PlayerPrefs.GetInt(KeyString.KEY_FIRST_TIME_HEART, 0),
      // daily reward
      DayRewardData = dayRewardData,
      // daily bonus
      DailyBonusData = dailyBonusData,
      // daily week
      WeeklyTaskDataJsonBase = weeklyTaskDataJsonBase,
      VersionDailyWeekly = PlayerPrefs.GetString(KeyString.KEY_VERSION_DAILY_WEEKLY, ""),
      VersionDataDailyWeekly = PlayerPrefs.GetString(KeyString.KEY_VERSION_DATA_DAILY_WEEKLY, ""),
      RewardWeeklyClaimed_0 = PlayerPrefs.GetInt($"RewardWeeklyClaimed_{0}", 0),
      RewardWeeklyClaimed_1 = PlayerPrefs.GetInt($"RewardWeeklyClaimed_{1}", 0),
      // tutorial
      IsCompletedTutorialHammer = PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER),
      IsCompletedTutorialRocket = PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET),
      IsCompletedTutorialSwap = PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP),
      IsCompletedTutorialRefresh = PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH),
      IsFirstUseHammer = PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_HAMMER),
      IsFirstUseRocket = PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_ROCKET),
      IsFirstUseSwap = PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_SWAP),
      IsFirstUseRefresh = PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_REFRESH),
    };

    return userData;
  }

  /// <summary>
  /// separate one apply into 2 applies: ApplyJsonData and ApplyPlayerPrefData
  /// to avoid JsonUtility.FromJson heavy workload blocking main thread
  /// </summary>
  public void ApplyJsonData()
  {
    if (_taskDataJsonBase != null)
      DailyTaskManager.Instance._taskDataJsonBase
        = JsonUtility.FromJson<DailyTaskManager.TaskDataJsonBase>(_taskDataJsonBase);
    if (_weeklyTaskDataJsonBase != null)
      DailyWeeklyManager.Instance.weeklyTaskDataJsonBase
        = JsonUtility.FromJson<DailyWeeklyManager.WeeklyTaskDataJsonBase>(_weeklyTaskDataJsonBase);
    if (_dayRewardData != null)
      DayRewardData = JsonUtility.FromJson<DayRewardData>(_dayRewardData);
    if (_dailyBonusData != null)
      DailyBonusData = JsonUtility.FromJson<DailyBonusData>(_dailyBonusData);
  }

  // apply userData tu tren firestore vao GameManager
  void ApplyPlayerPrefData(UserData userData)
  {
    // User’s PlayerPrefs 
    GameManager.Instance.RetryCount = userData.RetryCount;
    GameManager.Instance.AppOpenadTodayCount = userData.AppOpennedTodayCount;
    GameManager.Instance.FreeCoinTodayCount = userData.FreeCoinTodayCount;
    GameManager.Instance.LuckyWheelAdsTodayCount = userData.LuckyWheelAdsTodayCount;
    PlayerPrefs.SetString(KeyString.KEY_LATEST_OPENED_DATE, userData.LatestOpennedDate);
    GameManager.Instance.PlinkoAdsTodayCount = userData.PlinkoAdsTodayCount;
    GameManager.Instance.PlinkoFreeTodayCount = userData.PlinkoFreeTodayCount;
    GameManager.Instance.CurrentSwap = userData.CurrentSwap;
    GameManager.Instance.CurrentRefresh = userData.CurrentRefresh;
    GameManager.Instance.CurrentHammer = userData.CurrentHammer;
    GameManager.Instance.CurrentRocket = userData.CurrentRocket;
    GameManager.Instance.CurrentTicket = userData.CurrentTicket;
    GameManager.Instance.CurrentLevel = userData.CurrentLevel;
    GameManager.Instance.AmountCompletedQuest = userData.AmountCompletedQuest;
    GameManager.Instance.MoveEggTicketsAmount = userData.MoveEggTicketsAmount;
    GameManager.Instance.MoveEggTicketsAmountNeedClaim
      = userData.MoveEggTicketsAmountNeedClaim;
    GameManager.Instance.CurrentCoin = userData.CurrentCoin;
    GameManager.Instance.IsSoundOn = userData.IsSoundOn;
    GameManager.Instance.IsMusicOn = userData.IsMusicOn;
    GameManager.Instance.IsHapticOn = userData.IsHapticOn;
    GameManager.Instance.IsReceivedGift1 = userData.IsReceivedGift1;
    GameManager.Instance.IsReceivedGift2 = userData.IsReceivedGift2;
    GameManager.Instance.IsReceivedGift3 = userData.IsReceivedGift3;
    GameManager.Instance.IsRemoveAds = userData.IsRemoveAds;
    GameManager.Instance.IsRemoveAds7d = userData.IsRemoveAds7d;
    // lucky offer
    GameManager.Instance.TimeCoolDownLuckyOffer = userData.TimeCoolDownLuckyOffer;
    GameManager.Instance.CurrentClaimLuckyOffer = userData.CurrentClaimLuckyOffer;
    // meta progress
    GameManager.Instance.CurrentMetaProgressTotal = userData.CurrentMetaProgressTotal;
    GameManager.Instance.MetaProgressNeedClaim = userData.MetaProgressNeedClaim;
    GameManager.Instance.CurrentMetaStreak = userData.CurrentMetaStreak;
    GameManager.Instance.NextMetaStreak = userData.NextMetaStreak;
    GameManager.Instance.CurrentMilestone = userData.CurrentMilestone;
    // daily task
    PlayerPrefs.SetString(KeyString.KEY_VERSION_DAILY_TASK, userData.VersionDailyTask);
    PlayerPrefs.SetString(KeyString.KEY_VERSION_DATA_DAILY_TASK, userData.VersionDataDailyTask);
    PlayerPrefs.SetString(KeyString.KEY_TODAY_DAILY_TASK, userData.TodayDailyTask);
    PlayerPrefs.SetInt($"RewardDailTaskClaimed_{0}", userData.RewardDailTaskClaimed_0);
    PlayerPrefs.SetInt($"RewardDailTaskClaimed_{1}", userData.RewardDailTaskClaimed_1);
    PlayerPrefs.SetInt($"RewardDailTaskClaimed_{2}", userData.RewardDailTaskClaimed_2);
    // heart
    HeartSystem.Instance.UseInfinityHeartTime = userData.UseInfinityHeartTime > 0 ? userData.UseInfinityHeartTime : 0.0f;
    HeartSystem.Instance.LastedTimeHeart = userData.LastedTimeHeart;
    HeartSystem.Instance.LastedTimeInfinityHeart = userData.LastedTimeInfinityHeart;
    HeartSystem.Instance.CurrentHeart = userData.CurrentHeart;
    PlayerPrefs.SetInt(KeyString.KEY_FIRST_TIME_HEART, userData.FirstTimeHeart);
    // daily week
    PlayerPrefs.SetString(KeyString.KEY_VERSION_DAILY_WEEKLY, userData.VersionDailyWeekly);
    PlayerPrefs.SetString(KeyString.KEY_VERSION_DATA_DAILY_WEEKLY, userData.VersionDataDailyWeekly);
    PlayerPrefs.SetInt($"RewardWeeklyClaimed_{0}", userData.RewardWeeklyClaimed_0);
    PlayerPrefs.SetInt($"RewardWeeklyClaimed_{1}", userData.RewardWeeklyClaimed_1);
    // Tutorial
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER, userData.IsCompletedTutorialHammer);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, userData.IsCompletedTutorialRocket);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, userData.IsCompletedTutorialSwap);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH, userData.IsCompletedTutorialRefresh);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_HAMMER, userData.IsFirstUseHammer);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_ROCKET, userData.IsFirstUseRocket);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_SWAP, userData.IsFirstUseSwap);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_REFRESH, userData.IsFirstUseRefresh);
  }

  void OnGetUserDataSuccessfully(UserData userData)
  {
    // case: have userData before
    // we will using userData get from firestore and apply it to the game
    ApplyPlayerPrefData(userData);
  }

  void OnUserDataNotExisted(AuthResult result)
  {
    // case: don't have any userData before
    // so we will create new user
    CreateUserFields(result);
  }

  void OnApplicationPause(bool pauseStatus)
  {
    if (pauseStatus == true)
    {
      print("OnApplicationPause.BatchUpdateUserFields");
      BatchUpdateUserFields();
    }
  }
}