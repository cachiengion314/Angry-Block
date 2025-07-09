using System;
using DG.Tweening;
using Firebase.Analytics;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelSystem : MonoBehaviour
{
  public static LevelSystem Instance { get; private set; }

  [Header("Childs dependencies")]
  ItemSystem itemSystem;
  public ItemSystem ItemSystem { get { return itemSystem; } }
  public void InjectItemSystem(ItemSystem itemSystem)
  {
    this.itemSystem = itemSystem;
  }

  [Header("Datas")]
  [Tooltip("Using this value to mesure how many time has passed since start game moment")]
  public float GameplayStartTime;

  [Range(.2f, 10)]
  public float IntervalLength;
  public bool ShouldIntervalTimer = false;
  public float IntervalTimer { get; private set; }

  [Header("Events")]
  public Action onStartCurrentLevel;
  public static Action onInitedLevel;

  public DateTime TimeStartLogin;
  public DateTime TimeEndLogin;

  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else Destroy(gameObject);

    IntervalTimer = IntervalLength;

    TouchDetect.Instance.onTouchBegan += TouchDetect_onTouchBegan;
    TouchDetect.Instance.onTouchMoved += TouchDetect_onTouchMoved;
    TouchDetect.Instance.onTouchEnd += TouchDetect_onTouchEnd;

    SpawnSystem.Instance.onCompleted += OnSpawnedCompleted;
    int _tutorialGamePlay = PlayerPrefs.GetInt(KeyString.KEY_DEFAULT_TUTORIAL_GAMEPLAY_EGG, 0);
    if (_tutorialGamePlay == 0)
    {
      IsNeedEggTutorial = true;
    }
    else
    {
      IsNeedEggTutorial = false;
    }
  }

  void Update()
  {
    if (!ShouldIntervalTimer) return;
    IntervalTimerInvoker();
    OnTouchUpdate();
  }

  void OnDestroy()
  {
    TouchDetect.Instance.onTouchBegan -= TouchDetect_onTouchBegan;
    TouchDetect.Instance.onTouchMoved -= TouchDetect_onTouchMoved;
    TouchDetect.Instance.onTouchEnd -= TouchDetect_onTouchEnd;

    SpawnSystem.Instance.onCompleted -= OnSpawnedCompleted;
  }

  private void OnSpawnedCompleted()
  {
    ShouldIntervalTimer = true;
    GameManager.Instance.SetGameState(GameState.Gameplay);
    DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(1250, 100);

    SetupCurrentLevel();
  }

  void IntervalTimerInvoker()
  {
    IntervalTimer -= Time.deltaTime;
    if (IntervalTimer > 0) return;
    IntervalTimer = IntervalLength;
    OnIntervalInvoke();
  }

  void OnIntervalInvoke()
  {

  }

  public void ClearCurrentLevel()
  {

  }

  public void StartNextLevel()
  {
    ClearCurrentLevel();
    SetupCurrentLevel();
  }

  private void TouchDetect_onTouchBegan(float2 touchPos, Collider2D[] cols)
  {
    OnTouchBegan(touchPos, cols);
  }

  private void TouchDetect_onTouchMoved(float2 touchPos, float2 touchingDir)
  {
    OnTouchMoved(touchPos, touchingDir);
  }

  private void TouchDetect_onTouchEnd(float2 touchPos, float2 touchingDir)
  {
    OnTouchEnd(touchPos, touchingDir);
  }

  // Start of the game
  public void SetupCurrentLevel()
  {
    var index = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_NEED_SOLVING_EGG_ORDER_INDEX, 0);
    GameManager.Instance.CurrentNeedSolvingEggOrderIndex = index;
    _currentNeedSolvingOrderIndex = index;

    var eggGiftIndex = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_EGG_GIFT_INDEX, 0);
    GameManager.Instance.CurrentEggGiftIndex = eggGiftIndex;
    _currentGiftIndex = eggGiftIndex;

    var amount = PlayerPrefs.GetInt(
      KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT,
      KeyString.KEY_DEFAULT_MOVE_EGG_TICKETS_AMOUNT
    );

    GameManager.Instance.MoveEggTicketsAmount = amount;
    _moveEggTicketsAmount = amount;

    if (IsNeedEggTutorial == true)
    { // reset lượt nếu tu torial
      // GameManager.Instance.SetCurrentNeedSolvingEggOrderIndexTutorialGameplay();
      // PlayerPrefs.SetInt(KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT, KeyString.KEY_DEFAULT_MOVE_EGG_TICKETS_AMOUNT);// lượt đi
      // _moveEggTicketsAmount = KeyString.KEY_DEFAULT_MOVE_EGG_TICKETS_AMOUNT;
      int retrunMove = PlayerPrefs.GetInt(KeyString.KEY_DEFAULT_MOVE_TUTORIAL_GAMEPLAY_EGG, 0);
      if (retrunMove != 0)
      {
        GameManager.Instance.SetCurrentNeedSolvingEggOrderIndex(retrunMove);
        Debug.Log(GameManager.Instance.MoveEggTicketsAmount);
        _moveEggTicketsAmount = GameManager.Instance.MoveEggTicketsAmount;
        PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_MOVE_TUTORIAL_GAMEPLAY_EGG, 0);

      }
    }
    if (_currentNeedSolvingOrderIndex >= _initOrders.Length)
    {
      GameManager.Instance.SetGameState(GameState.GamepPause);
      onInitedLevel?.Invoke();
      return;
    }
    if (!GameManager.Instance.IsRandomData)
    {
      ShuffleOrders();
      SaveMagicCauldronData();
      GameManager.Instance.IsRandomData = true;
    }
    InitData();

    var _currentEggOnScreen = GetCurrentEggOnScreen();
    var _currentEggHolder = GetCurrentEggHolderOrder();

    itemSystem.SpawnEggHolderAt(0, _currentEggHolder);
    SpawnAndMoveEggsToScreen(_currentEggOnScreen, _spawnedEggMoveToScreenDeltaLength);

    onInitedLevel?.Invoke();

    TimeStartLogin = DateTime.Now;

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(
        "lucky_egg_event_login",
        new Parameter[]{
          new ("ticket", GameManager.Instance.MoveEggTicketsAmount),
          new ("stage", _currentGiftIndex + 1)
        }
      );
    }

    Utility.Print("Ticket: " + GameManager.Instance.MoveEggTicketsAmount + "_Stage: " + (_currentGiftIndex + 1));
  }

  private void OnApplicationQuit()
  {
    TimeEndLogin = DateTime.Now;
    var duration = TimeEndLogin - TimeStartLogin;

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(
        "lucky_egg_event_quit",
        new Parameter[]{
          new ("ticket", GameManager.Instance.MoveEggTicketsAmount),
          new ("stage", _currentGiftIndex + 1),
          new ("duration", duration.TotalSeconds.ToString("000"))
        }
      );
    }

    Utility.Print("Ticket: " + GameManager.Instance.MoveEggTicketsAmount + "_Stage: " + (_currentGiftIndex + 1) + "_Duration: " + duration.TotalSeconds.ToString("000"));
  }
}