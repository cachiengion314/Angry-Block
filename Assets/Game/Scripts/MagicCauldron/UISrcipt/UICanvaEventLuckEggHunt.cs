using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.SceneManagement;
using System;
using Firebase.Analytics;
using HoangNam;

public class UICanvaEventLuckEggHunt : MonoBehaviour
{
  public static UICanvaEventLuckEggHunt Instance { get; private set; }

  [SerializeField] GameObject panelTutorial;
  // [SerializeField] GameObject parentProgress; // có 15 thằng con
  [SerializeField] Slider sliderProgess;
  [Header("---Move---")]
  // [SerializeField] int currentProgress; // <=15;
  // [SerializeField] int currentTurnMove;
  [SerializeField] TMP_Text textTurnMove;
  int _progressFull = 15; // tổng số mốc có thể đạt
  [Header("---Notif---")]
  [SerializeField] string textNoti;
  float _coutdowntShowNotif = 0f;
  float _moveNotif = 100f;
  bool _isShowNotif = false;

  [Header("---Reward---")]
  [SerializeField] GameObject parentReward;
  public SkeletonGraphic skeletonGraphic;
  public GameObject notiWinAll;
  [Header("---Tutorial---")]
  [SerializeField] GameObject parentTutorial;
  [Header("---TutorialGameplay---")]
  [SerializeField] GameObject panelTutorialGamePlay;
  [SerializeField] GameObject[] statesTuturials;
  [SerializeField] Button btnNextStateTutorialGamePlay;
  int indexTutorialGamePlay = 0;
  [SerializeField] Canvas state6Progress;
  [SerializeField] GameObject BgTurn1;
  [SerializeField] GameObject BgTurn2;

  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else Destroy(gameObject);


    panelTutorial.SetActive(false);
    panelTutorialGamePlay.SetActive(false);
    LevelSystem.onInitedLevel += SetupTextTurnMove;
    SetupStatusEggs(false);
    // SetImageReward(0);


    // var eggHolder = LevelSystem.Instance.ItemSystem.GetFirstEggHolder(); // lấy khay đầu tiên 
    // var egg = eggHolder.GetChildAt(0); // lấy trứng trên khay theo index

    // var waitingEgg = LevelSystem.Instance.ItemSystem.GetEggAt(0);// lấy trứng trên hàng chờ theo index

    notiWinAll.SetActive(false);
  }
  void OnDestroy()
  {
    LevelSystem.onInitedLevel -= SetupTextTurnMove;
  }

  public void SetupStatusEggs(bool _isplay)
  {
    if (_isplay == true)
    {
      // Debug.Log("SetupStatusEggs true");
      skeletonGraphic.AnimationState.SetAnimation(0, "basket_collect", true); // Chạy anim_1 khi true
    }
    else
    {
      // Debug.Log("SetupStatusEggs False");
      skeletonGraphic.AnimationState.SetAnimation(0, "basket_idle", true); // Chạy anim_1 khi true
    }
  }

  void SetupTextTurnMove()
  {
    int _currentTurnMove = LevelSystem.Instance.MoveEggTicketsAmount;
    textTurnMove.text = _currentTurnMove.ToString();
    SetupProgress(LevelSystem.Instance.CurrentNeedSolvingEggOrderIndex);
    Debug.Log("CurrentGiftIndex--" + LevelSystem.Instance.CurrentGiftIndex);
    SetImageReward(LevelSystem.Instance.CurrentGiftIndex);

    if (LevelSystem.Instance.IsNeedEggTutorial == true)
    {
      LevelSystem.Instance.IndexTutorialGamePlay = 0; // lại lượt  chọn trứng
      SetupTuTorialGamePlay();
      ShowTutorialGamePlay();
    }
    if (LevelSystem.Instance.IsNeedEggTutorial == false)
    {

    }
  }

  public void SetImageReward(int _index)
  {
    Debug.Log("_index--" + _index);
    if (parentReward == null || parentReward.transform == null)
    {
      Debug.LogWarning("parentReward has been destroyed or is null!");
      return;
    }

    for (int i = 0; i < parentReward.transform.childCount; i++)
    {
      parentReward.transform.GetChild(i).gameObject.SetActive(false);
    }
    if (_index >= parentReward.transform.childCount)
    {
      notiWinAll.SetActive(true);
      return;
    }
    parentReward.transform.GetChild(_index).gameObject.SetActive(true);
  }


  // Update is called once per frame
  void Update()
  {
    CoutdownNotif();
  }

  void SetupProgress(int _currentProgress)
  {
    // Debug.Log("---" + _currentProgress);
    sliderProgess.value = _currentProgress * 1f / (float)_progressFull;
    // sliderProgess.value = 15 * 1f / (float)_progressFull;

  }

  public void UpdateProgress(int _currentProgress)
  {
    // // print("parentProgress.transform.childCount " + parentProgress.transform.childCount);
    // _currentProgress++;
    if (_progressFull >= _currentProgress)
    {
      sliderProgess.DOKill();
      // sliderProgess.value = currentProgress * 1f / (float)_progressFull;
      sliderProgess.DOValue(_currentProgress * 1f / (float)_progressFull, 0.8f).SetEase(Ease.InOutSine); // Thời gian mượt mà là 0.5s
                                                                                                         // print("liderProgess.value " + sliderProgess.value);
    }
  }

  public void UpdateTurnMove(int _currentTurnMove)
  {
    textTurnMove.text = _currentTurnMove.ToString();
  }

  public void BTNShowTutorial()
  {
    panelTutorial.SetActive(true);
    ShowTutorial();
  }

  public void BTNHideTutorial()
  {
    panelTutorial.SetActive(false);
  }

  public void ShowNotifNoMove()
  {
    if (_isShowNotif == false)
    {
      Vector2 Target = new Vector2(0, -400f); // vị trí bắt đầu
      NotifyManager.Instance.SetupNotifyDesignatedPoS(this.gameObject, textNoti, StypeAnim.DesignatedLocation, Target, _moveNotif, 1.2f);
      NotifyManager.Instance.CreatNotif();
      _isShowNotif = true;
    }
  }

  void CoutdownNotif()
  {
    _coutdowntShowNotif += Time.deltaTime;
    if (_coutdowntShowNotif >= 5f)
    {
      _isShowNotif = false;
      _coutdowntShowNotif = 0;
    }
  }

  public void BtnBackLobby()
  {
    DOTween.KillAll();
    SceneManager.LoadScene("Lobby");

    LevelSystem.Instance.TimeEndLogin = DateTime.Now;
    var duration = LevelSystem.Instance.TimeEndLogin - LevelSystem.Instance.TimeStartLogin;

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(
        "lucky_egg_event_quit",
        new Parameter[]{
          new ("ticket", GameManager.Instance.MoveEggTicketsAmount),
          new ("stage", LevelSystem.Instance.CurrentGiftIndex + 1),
          new ("duration", duration.TotalSeconds)
        }
      );
    }

    Utility.Print("Ticket: " + GameManager.Instance.MoveEggTicketsAmount + "_Stage: " + (LevelSystem.Instance.CurrentGiftIndex + 1) + "_Duration: " + duration.TotalSeconds);
  }

  void ShowTutorial()
  {
    float delayStep = 0.15f; // khoảng thời gian delay giữa các item
    for (int i = 0; i < parentTutorial.transform.childCount; i++)
    {
      Transform child = parentTutorial.transform.GetChild(i);
      child.DOKill();
      child.localScale = Vector2.zero; // bắt đầu từ scale 0

      // Tạo hiệu ứng phóng to lên từ (0,0) đến (1,1) với delay tăng dần
      child.DOScale(Vector2.one, 0.3f)
          .SetEase(Ease.OutBack) // hiệu ứng bật nảy dễ nhìn hơn
          .SetDelay(i * delayStep);
    }
  }

  void SetupTuTorialGamePlay()
  {
    for (int i = 0; i < statesTuturials.Length; i++)
    {
      statesTuturials[i].SetActive(false);
    }
    statesTuturials[0].SetActive(true);
    btnNextStateTutorialGamePlay.interactable = false;
    BgTurn1.SetActive(false);
    BgTurn2.SetActive(false);

  }

  void ShowTutorialGamePlay()
  {
    panelTutorialGamePlay.SetActive(true);

    StateTuTorial1();
  }

  public void BtnNextStateTutorialGamePlay()
  {
    // Debug.Log("NextTate---");

    switch (indexTutorialGamePlay)
    {
      case 0:
        StateTuTorial5();
        break;
      case 1:
        StateTuTorial6();
        break;
      case 2:
        EndTuTorialGamePlay();
        break;
    }
    indexTutorialGamePlay++;
  }

  void StateTuTorial1()
  {
    BgTurn1.SetActive(true);
    var waitingEgg0 = LevelSystem.Instance.ItemSystem.GetEggAt(0);// lấy trứng trên hàng chờ theo index
    var waitingEgg1 = LevelSystem.Instance.ItemSystem.GetEggAt(1);// lấy trứng trên hàng chờ theo index
    var waitingEgg2 = LevelSystem.Instance.ItemSystem.GetEggAt(2);// lấy trứng trên hàng chờ theo index

    // waitingEgg0.SetEggSortingLayerNameAt("Notif", 10);
    waitingEgg1.SetOffCollider();
    waitingEgg2.SetOffCollider();
  }

  public void PickEggsTutorial(int _index)
  {

    if (_index == 0)
    {
      StateTuTorial2();

      var waitingEgg0 = LevelSystem.Instance.ItemSystem.GetEggAt(0);// lấy trứng trên hàng chờ theo index
      waitingEgg0.SetEggSortingLayerNameAt("Notif", 10);
      waitingEgg0.SetOnCollider();
      // PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_MOVE_TUTORIAL_GAMEPLAY_EGG, 1);


    }
    if (_index == 1)
    {
      var waitingEgg0 = LevelSystem.Instance.ItemSystem.GetEggAt(0);// lấy trứng trên hàng chờ theo index
      waitingEgg0.SetEggSortingLayerNameAt("Notif", 10);
      waitingEgg0.SetOnCollider();

      StateTuTorial3();
      // PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_MOVE_TUTORIAL_GAMEPLAY_EGG, 2);

    }
    if (_index == 2)
    {
      GameManager.Instance.SetGameState(GameState.GamepPause);

      // 
    }
    LevelSystem.Instance.IndexTutorialGamePlay++;
  }

  void StateTuTorial2()
  {
    // var waitingEgg = LevelSystem.Instance.ItemSystem.GetEggAt(0);// lấy trứng trên hàng chờ theo index

    // waitingEgg.SetEggSortingLayerNameAt("Item", 201);

    statesTuturials[0].SetActive(false);
    statesTuturials[1].SetActive(true);
  }

  void StateTuTorial3()
  {
    statesTuturials[1].SetActive(false);
    statesTuturials[2].SetActive(true);

    // var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ cả khay 2
    // var eggSecond = eggSecondHolder.GetChildAt(0); // lấy trứng trên khay theo index

    // eggSecond.SetEggSortingLayerNameAt("Notif", 10);
    // eggSecondHolder.SetBlockSortingLayerNameAt(0, "Notif", 10);

    // state2Move.sortingLayerName = "Ui";
    // state2Move.sortingLayerID = 1;
  }

  public void StateTuTorial4()
  {

    BgTurn1.SetActive(false);
    BgTurn2.SetActive(true);
    statesTuturials[2].SetActive(false);
    statesTuturials[3].SetActive(true);

    var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ cả khay 2
    var eggSecond = eggSecondHolder.GetChildAt(0); // lấy trứng trên khay theo index

    // eggSecondHolder.SetBlockSortingLayerNameAt(0, "Notif", 10);
    eggSecondHolder.SetSortinglayerTutorial(0);
    eggSecond.SetSortingTutorial();
    // eggSecond.SetEggSortingLayerNameAt("Notif", 10);


    // var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ cả khay 2
    // var eggSecond = eggSecondHolder.GetChildAt(0); // lấy trứng trên khay theo index

    // eggSecond.SetEggSortingLayerNameAt("Item", 201);
    // eggSecondHolder.SetBlockSortingLayerNameAt(0, "Item", 201);

    // eggSecondHolder.SetDecalSortingLayerNameAt("Notif", 10);
    // eggSecondHolder.SetBlockSortingLayerNameAt(1, "Notif", 201);
    // eggSecondHolder.SetBlockSortingLayerNameAt(2, "Notif", 201);
  }

  void StateTuTorial5()
  {
    statesTuturials[3].SetActive(false);
    statesTuturials[4].SetActive(true);

    var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ cả khay 2
    var eggSecond = eggSecondHolder.GetChildAt(0); // lấy trứng trên khay theo index

    eggSecond.ResetSortingTutorial();


    // eggSecondHolder.SetBlockSortingLayerNameAt(0, "Item", 201);
    eggSecondHolder.ReSetSortinglayerTutorial(0);

    eggSecondHolder.SetDecalSortingLayerTutorial();
    eggSecondHolder.SetSortinglayerTutorial(1);
    eggSecondHolder.SetSortinglayerTutorial(2);

    // var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ cả khay 2

    // eggSecondHolder.SetDecalSortingLayerNameAt("Item", 201);
    // eggSecondHolder.SetBlockSortingLayerNameAt(1, "Item", 201);
    // eggSecondHolder.SetBlockSortingLayerNameAt(2, "Item", 201);
  }

  void StateTuTorial6()
  {
    var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ cả khay 2

    eggSecondHolder.ReSetDecalSortingLayerTutorial();
    // eggSecondHolder.SetSortinglayerTutorial(1);
    // eggSecondHolder.SetSortinglayerTutorial(2);
    eggSecondHolder.ReSetSortinglayerTutorial(1);
    eggSecondHolder.ReSetSortinglayerTutorial(2);

    statesTuturials[4].SetActive(false);
    statesTuturials[5].SetActive(true);
    state6Progress.GetComponent<Canvas>().sortingLayerName = "Notif";
    state6Progress.GetComponent<Canvas>().sortingOrder = 10;
    sliderProgess.value = 0.5f;
  }
  void EndTuTorialGamePlay()
  {
    panelTutorialGamePlay.SetActive(false);
    // PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_TUTORIAL_GAMEPLAY_EGG, 1);

    state6Progress.GetComponent<Canvas>().sortingLayerName = "UI";
    state6Progress.GetComponent<Canvas>().sortingOrder = 1;
    sliderProgess.value = 0f;
    GameManager.Instance.SetGameState(GameState.Gameplay);
    // reset trung khay dau
    var eggFirstHolder = LevelSystem.Instance.ItemSystem.GetFirstEggHolder(); // lấy thứ 2
    var eggFirst = eggFirstHolder.GetChildAt(0); // lấy trứng trên khay theo index
    eggFirst.ResetSortingTutorial();

    // reset trung khay 2
    var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ 2
    var eggSecond = eggSecondHolder.GetChildAt(0); // lấy trứng trên khay theo index

    eggSecond.ResetSortingTutorial();
    // reset block khau 2
  }

  public void SetActiveButtonNextState()
  {
    btnNextStateTutorialGamePlay.interactable = true;
  }

  public void SetLayerEggHolder()
  {
    var eggHolder = LevelSystem.Instance.ItemSystem.GetFirstEggHolder(); // lấy khay đầu tiên 
    var egg = eggHolder.GetChildAt(0); // lấy trứng trên khay theo index
                                       // egg.SetEggSortingLayerNameAt("Item", 201);
    egg.ResetSortingOrder();

    var eggSecondHolder = LevelSystem.Instance.ItemSystem.GetSecondEggHolder(); // lấy thứ 2
    var eggSecond = eggSecondHolder.GetChildAt(0); // lấy trứng trên khay theo index
    eggSecond.SetSortingTutorial();
  }
}