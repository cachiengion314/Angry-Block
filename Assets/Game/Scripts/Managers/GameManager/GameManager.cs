using System;
using Firebase.Analytics;
using HoangNam;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
  Menu,
  Gameplay,
  GamepPause,
  Gameover,
  Tutorial,
}

public partial class GameManager : MonoBehaviour
{
  public static Action onChangeProgressBalloon = delegate { };
  public static Action onChangeAmountTicket;
  public static Action onChangeLuckyWheelAds;
  public static Action onChangeRemoveAds;
  public static Action onChangeRemoveAdsSubscription;
  public static Action onChangeAmountSpin;
  public static Action onBuyUnlockFollowing;
  public static Action onReceivedReward;
  public static Action onNoticeDailyBounsChange;

  public static GameManager Instance { get; private set; }

  [Header("Events")]
  public Action<GameState> onGameStateChanged;

  [Header("User Settings")]
  [SerializeField] int maxLevelIndex;
  public int MaxLevelIndex
  {
    get { return maxLevelIndex; }
  }
  public bool IsDisableTutorial;

  GameState _gameState;

  public bool IsShowNoticeDailyBouns
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_SHOW_NOTICE_DAILYBOUNS, 1) == 1;
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_IS_SHOW_NOTICE_DAILYBOUNS, value ? 1 : 0);
      onNoticeDailyBounsChange?.Invoke();
    }
  }

  private bool _isSoundOn;
  public bool IsSoundOn
  {
    get
    {
      return _isSoundOn;
    }
    set
    {
      _isSoundOn = value;
      var _intOn = _isSoundOn ? 1 : 0;
      PlayerPrefs.SetInt(KeyString.KEY_IS_SOUND_ON, _intOn);
    }
  }

  private bool _isMusicOn;
  public bool IsMusicOn
  {
    get
    {
      return _isMusicOn;
    }
    set
    {
      _isMusicOn = value;
      if (value == false)
      {
        SoundManager.Instance.StopMainThemeSfx();
      }
      else
      {
        SoundManager.Instance.PlayMainThemeSfx();
      }

      var _intOn = _isMusicOn ? 1 : 0;
      PlayerPrefs.SetInt(KeyString.KEY_IS_MUSIC_ON, _intOn);
    }
  }

  private bool _isHapticOn;
  public bool IsHapticOn
  {
    get
    {
      return _isHapticOn;
    }
    set
    {
      _isHapticOn = value;
      var _intOn = _isHapticOn ? 1 : 0;
      PlayerPrefs.SetInt(KeyString.KEY_IS_HAPTIC_ON, _intOn);
    }
  }

  [Header("User Progress")]
  int _currentRefresh;
  public int CurrentRefresh
  {
    get { return _currentRefresh; }
    set
    {
      _currentRefresh = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_REFRESH, value);
    }
  }

  int _currentHammer;
  public int CurrentHammer
  {
    get { return _currentHammer; }
    set
    {
      _currentHammer = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_HAMMER, value);
    }
  }

  int _currentRocket;
  public int CurrentRocket
  {
    get { return _currentRocket; }
    set
    {
      _currentRocket = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_ROCKET, value);
    }
  }

  int _currentSwap;
  public int CurrentSwap
  {
    get { return _currentSwap; }
    set
    {
      _currentSwap = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_SWAP, value);
    }
  }

  int _currentTicket;
  public int CurrentTicket
  {
    get { return _currentTicket; }
    set
    {
      _currentTicket = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_TICKET, value);
      onChangeAmountTicket?.Invoke();
    }
  }

  int _currentTicketNoel;
  public int CurrentTicketNoel
  {
    get { return _currentTicketNoel; }
    set
    {
      _currentTicketNoel = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_TICKET_NOEl, value);
      onChangeAmountTicket?.Invoke();
    }
  }

  int _currentSpinNoel;
  public int CurrentSpinNoel
  {
    get { return _currentSpinNoel; }
    set
    {
      _currentSpinNoel = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_SPIN_NOEl, value);
      onChangeAmountSpin?.Invoke();
    }
  }

  bool _isReceivedGift1On;
  public bool IsReceivedGift1
  {
    get { return _isReceivedGift1On; }
    set
    {
      _isReceivedGift1On = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_GIFT1, value ? 1 : 0);
    }
  }

  bool _isReceivedGift2On;
  public bool IsReceivedGift2
  {
    get { return _isReceivedGift2On; }
    set
    {
      _isReceivedGift2On = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_GIFT2, value ? 1 : 0);
    }
  }

  bool _isReceivedGift3On;
  public bool IsReceivedGift3
  {
    get { return _isReceivedGift3On; }
    set
    {
      _isReceivedGift3On = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_GIFT3, value ? 1 : 0);
    }
  }

  int _currentScore;
  public int CurrentScore
  {
    get { return _currentScore; }
    set
    {
      _currentScore = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_SCORE, value);
    }
  }

  int _currentCoin;
  public int CurrentCoin
  {
    get { return _currentCoin; }
    set
    {
      _currentCoin = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_COIN, _currentCoin);
      if (ShowPanel.Instance)
        ShowPanel.Instance.GlobalCoinText.text = value.ToString("#,##0");
    }
  }

  int _currentLevel;
  public int CurrentLevel
  {
    get
    {
      return _currentLevel = _currentLevel > maxLevelIndex ? maxLevelIndex : _currentLevel;
    }
    set
    {
      // TODO
      _currentLevel = value > maxLevelIndex ? maxLevelIndex : value;
      // _currentLevel = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_LEVEL, _currentLevel);
    }
  }

  int _reviveCount;
  public int ReviveCount
  {
    get
    {
      return _reviveCount;
    }
    set
    {
      _reviveCount = value;
      PlayerPrefs.SetInt(KeyString.KEY_REVIVE_COUNT, value);
    }
  }

  int _appOpenadTodayCount;
  public int AppOpenadTodayCount
  {
    get
    {
      return _appOpenadTodayCount;
    }
    set
    {
      _appOpenadTodayCount = value;
      PlayerPrefs.SetInt(KeyString.KEY_APP_OPENAD_TODAY_COUNT, value);
    }
  }

  int _retryCount;
  public int RetryCount
  {
    get
    {
      return _retryCount;
    }
    set
    {
      _retryCount = value;
      PlayerPrefs.SetInt(KeyString.KEY_RETRY_COUNT, _retryCount);
    }
  }

  // v1.3
  int _progressBalloon;
  public int ProgressBalloon
  {
    get
    {
      _progressBalloon = PlayerPrefs.GetInt(KeyString.KEY_PROGRESS_BALLOON, 0);
      return _progressBalloon;
    }
    set
    {
      _progressBalloon = value < 0 ? 0 : value;
      PlayerPrefs.SetInt(KeyString.KEY_PROGRESS_BALLOON, _progressBalloon);

      onChangeProgressBalloon?.Invoke();
    }
  }

  // v1.4
  int _freeCoinTodayCount;
  public int FreeCoinTodayCount
  {
    get
    {
      return _freeCoinTodayCount;
    }
    set
    {
      _freeCoinTodayCount = value;
      PlayerPrefs.SetInt(KeyString.KEY_FREECOIN_TODAY_COUNT, value);
    }
  }

  int _luckyWheelAdsTodayCount;
  public int LuckyWheelAdsTodayCount
  {
    get
    {
      return _luckyWheelAdsTodayCount;
    }
    set
    {
      _luckyWheelAdsTodayCount = value;
      PlayerPrefs.SetInt(KeyString.KEY_LUCKYWHEELADS_TODAY_COUNT, value);
    }
  }

  public int PlinkoAdsTodayCount
  {
    get
    {
      return PlayerPrefs.GetInt(KeyString.KEY_PLINKO_ADS_TODAY_COUNT);
    }
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_PLINKO_ADS_TODAY_COUNT, value);
    }
  }

  public int PlinkoFreeTodayCount
  {
    get
    {
      return PlayerPrefs.GetInt(KeyString.KEY_PLINKO_FREE_TODAY_COUNT);
    }
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_PLINKO_FREE_TODAY_COUNT, value);
    }
  }

  bool _isRemoveAds;
  public bool IsRemoveAds
  {
    get
    {
      return _isRemoveAds;
    }
    set
    {
      _isRemoveAds = value;
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISREMOVEADS, _isRemoveAds ? 1 : 0);

      if (_isRemoveAds)
      {
        onChangeRemoveAds?.Invoke();
      }
    }
  }

  public bool IsRemoveAds7d
  {
    get
    {
      return PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISREMOVEADS_SUBSCRIPTION, 0) == 1;
    }
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISREMOVEADS_SUBSCRIPTION, value ? 1 : 0);
      onChangeRemoveAdsSubscription?.Invoke();
    }
  }

  int _amountCompletedQuest;
  public int AmountCompletedQuest
  {

    get
    {
      return _amountCompletedQuest;
    }
    set
    {
      _amountCompletedQuest = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_AMOUNTCOMPLETEDQUEST, _amountCompletedQuest);
    }
  }

  [HideInInspector] public DateTime TimePhaseStart;
  [HideInInspector] public DateTime TimePhaseEnd;
  [HideInInspector] public DateTime TimeLevelStart;
  [HideInInspector] public DateTime TimeLevelEnd;

  public bool GetLoginDayAt(int day)
  {
    return PlayerPrefs.GetInt($"day{day}", 0) == 1;
  }

  public void LoginDayAt(int day)
  {
    PlayerPrefs.SetInt($"day{day}", 1);
  }

  public bool GetRewardFreeDayAt(int day)
  {
    return PlayerPrefs.GetInt($"dayfree{day}", 0) == 1;
  }

  public void RewardFreeDayAt(int day)
  {
    PlayerPrefs.SetInt($"dayfree{day}", 1);
    onReceivedReward?.Invoke();
  }

  public bool GetRewardPassDayAt(int day)
  {
    return PlayerPrefs.GetInt($"daypass{day}", 0) == 1;
  }

  public void RewardPassDayAt(int day)
  {
    PlayerPrefs.SetInt($"daypass{day}", 1);
    onReceivedReward?.Invoke();
  }

  bool _isUnlockFollowing;
  public bool IsUnlockFollowing
  {
    get { return _isUnlockFollowing; }
    set
    {
      _isUnlockFollowing = value;
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISUNLOCK_FOLLOWING, value ? 1 : 0);
      onBuyUnlockFollowing?.Invoke();
    }
  }

  // TODO Sua ngay truoc khi up
  public bool IsEvented()
  {
    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALNOEL, 0) == 1)
    {
      PlayerPrefs.SetInt("user_joined_noel_event", 1);
    }

    if (PlayerPrefs.GetInt("user_joined_noel_event", 0) == 0)
    {
      return false;
    }

    DateTime today = DateTime.Today;
    // Kiểm tra tháng và ngày
    if (today.Month == 12 && today.Day >= 1 && today.Day <= 30)
    {
      return true;
    }
    return false;
  }

  public void Login()
  {
    if (IsEvented())
    {
      int day = DateTime.Today.Day;
      LoginDayAt(day);
    }
  }

  /// <summary>
  /// Event section
  /// </summary>
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;

      SceneManager.activeSceneChanged += ChangedActiveScene;
      SceneManager.sceneLoaded += OnSceneLoaded;

      InvokeOnce();
      Login();
    }
    else
    {
      Destroy(gameObject);
    }

    DontDestroyOnLoad(gameObject);
  }

  private void Update()
  {
    CalculateTimeLuckyOffer();
  }

  private void InvokeOnce()
  {

  }

  private void OnDestroy()
  {
    SceneManager.activeSceneChanged -= ChangedActiveScene;
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void ChangedActiveScene(Scene current, Scene next)
  {

  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (scene.name == "Gameplay")
    {
      int firstLoadingCompleted = PlayerPrefs.GetInt(KeyString.FIREBASE_FIRST_LOADING_COMPLETE, 0);
      if (firstLoadingCompleted == 0)
      {
        if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
        {
          PlayerPrefs.SetInt(KeyString.FIREBASE_FIRST_LOADING_COMPLETE, 1);
          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_FIRST_LOADING_COMPLETE);
        }
      }
    }

    _reviveCount = 0;
  }

  /// <summary>
  /// Game State Section
  /// </summary>
  /// <returns></returns>
  public GameState GetGameState()
  {
    return _gameState;
  }

  public void SetGameState(GameState state)
  {
    _gameState = state;
    onGameStateChanged?.Invoke(state);
  }

  public void LoadSceneFrom(GameState state)
  {
    _gameState = state;
    onGameStateChanged?.Invoke(state);
  }

  public void LoadSceneFrom(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }

  ///
  //
  //
  public void InitDailyUserProgressData()
  {
    if (PlayerPrefs.GetInt(KeyString.KEY_FIRST_TIME_OPENING, 1) == 1)
    {
      Utility.Print("First Time Opening");
      //Set first time opening to false
      PlayerPrefs.SetInt(KeyString.KEY_FIRST_TIME_OPENING, 0);
      //Do your stuff here
    }
    else
    {
      Utility.Print("NOT First Time Opening");
      //Do your stuff here
    }

    _retryCount = PlayerPrefs.GetInt(KeyString.KEY_RETRY_COUNT, 0);
    _appOpenadTodayCount = PlayerPrefs.GetInt(KeyString.KEY_APP_OPENAD_TODAY_COUNT, 0);
    _freeCoinTodayCount = PlayerPrefs.GetInt(KeyString.KEY_FREECOIN_TODAY_COUNT, 0);
    _luckyWheelAdsTodayCount = PlayerPrefs.GetInt(KeyString.KEY_LUCKYWHEELADS_TODAY_COUNT, 0);
    var latestOpenedDate = PlayerPrefs.GetString(KeyString.KEY_LATEST_OPENED_DATE, "");
    var currentDate = HoangNam.Utility.GetCurrentDate();
    if (currentDate != latestOpenedDate)
    {
      AppOpenadTodayCount = 0;
      FreeCoinTodayCount = 0;
      LuckyWheelAdsTodayCount = 0;
      PlinkoAdsTodayCount += 2;
      PlinkoFreeTodayCount = 1;
      CurrentTicket = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_TICKET, 0) + 1;
      PlayerPrefs.SetInt("KEY_FIRST_OPENAPPTODAY", 0);

      ResetTimeLuckyOfferNewDay();
    }
    PlayerPrefs.SetString(KeyString.KEY_LATEST_OPENED_DATE, currentDate);

    InitParameterLuckyOffer();

    _currentSwap = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_SWAP, 3);
    _currentRefresh = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_REFRESH, 3);
    _currentHammer = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_HAMMER, 3);
    _currentRocket = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_ROCKET, 3);
    _currentTicket = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_TICKET, 0);
    _currentLevel = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_LEVEL, 0);
    _currentScore = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_SCORE, 0);
    _currentSpinNoel = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_SPIN_NOEl, 0);
    _currentTicketNoel = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_TICKET_NOEl, 0);
    _amountCompletedQuest = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_AMOUNTCOMPLETEDQUEST, 0);

    MoveEggTicketsAmount = PlayerPrefs.GetInt(
      KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT,
      KeyString.KEY_DEFAULT_MOVE_EGG_TICKETS_AMOUNT
    );
    MoveEggTicketsAmountNeedClaim = PlayerPrefs.GetInt(KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT_NEEDCLAIM, 0);

    InitMetaProgressProperty();

    CurrentCoin = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_COIN, 100);

    var _intSound = PlayerPrefs.GetInt(KeyString.KEY_IS_SOUND_ON, 1);
    var _intMusic = PlayerPrefs.GetInt(KeyString.KEY_IS_MUSIC_ON, 1);
    var _intHaptic = PlayerPrefs.GetInt(KeyString.KEY_IS_HAPTIC_ON, 1);
    var _intRemoveAds = PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISREMOVEADS, 0);
    var _isReceivedGift1 = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_GIFT1, 0);
    var _isReceivedGift2 = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_GIFT2, 0);
    var _isReceivedGift3 = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_GIFT3, 0);
    var _isUnlockFollowing1 = PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISUNLOCK_FOLLOWING, 0);

    _isSoundOn = _intSound == 1;
    _isMusicOn = _intMusic == 1;
    _isHapticOn = _intHaptic == 1;
    _isReceivedGift1On = _isReceivedGift1 == 1;
    _isReceivedGift2On = _isReceivedGift2 == 1;
    _isReceivedGift3On = _isReceivedGift3 == 1;
    _isUnlockFollowing = _isUnlockFollowing1 == 1;
    IsRemoveAds = _intRemoveAds == 1;
    _luckyOfferRewardDatas = new();

    ConsiderTutorial();
  }

  public void ConsiderTutorial()
  {
    if (CurrentLevel > 0)
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALS1, 1);
    }

    if (CurrentLevel > 4)
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER, 1);
    }

    if (CurrentLevel > 7)
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, 1);
    }

    if (CurrentLevel > 9)
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, 1);
    }

    if (CurrentLevel > 12)
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH, 1);
    }

    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALS1, 0) == 0)
    {
      PlayerPrefs.SetInt("PlayLevel1AnimAt_0", 0);
      PlayerPrefs.SetInt("PlayLevel1AnimAt_1", 0);
      PlayerPrefs.SetInt("PlayLevel1AnimAt_2", 0);
    }
  }

  public bool HasInternet()
  {
    return Application.internetReachability != NetworkReachability.NotReachable;
  }
}
