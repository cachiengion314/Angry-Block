using System;
using System.Collections;
using DG.Tweening;
using Firebase.Analytics;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class LobbyPanel : MonoBehaviour
{
  public static Action onOpenLuckyWheel;
  public static Action onOpenDailySweetPass;

  public static LobbyPanel Instance { get; private set; }
  [Header("---ImageEvent---")]

  // [SerializeField] Image bgShop;
  [SerializeField] SpriteRenderer bgHome;
  [SerializeField] Image LevelActive;
  [SerializeField] Image SnowHome;
  [SerializeField] Image SnowShop;
  [SerializeField] Image SNowLeaderbroad;
  // [SerializeField] Image TextHome;

  [Header("Internal dependencies")]

  [SerializeField] DailyRewardModal dailyRewardModal;
  [SerializeField] RectTransform dailyBonusModel;
  [SerializeField] RectTransform settingModal;
  [SerializeField] RectTransform plinkoModal;
  [SerializeField] RectTransform luckyWheelModal;
  [SerializeField] RectTransform luckyWheelEventModal;
  [SerializeField] RectTransform shopModal;
  [SerializeField] RectTransform dailyGoalModal;
  [SerializeField] RectTransform goalCompletedNotify;
  [SerializeField] RectTransform loadingIAP;
  [SerializeField] RectTransform showCanvas;
  [SerializeField] RectTransform dailySweetPassModal;
  [SerializeField] RectTransform taskNoelModal;
  [SerializeField] RectTransform dailyBuyPass;
  [SerializeField] RectTransform MetaProgerssModal;
  [SerializeField] RectTransform darkPanel;

  // v1.4
  [SerializeField] RectTransform buyHeartModal;
  [SerializeField] Animator shopAnim;
  [SerializeField] Button quitBtn;
  [SerializeField] GameObject snowPrefab;

  // v1.5
  [SerializeField] SkeletonGraphic luckyWheel;
  [SerializeField] GameObject luckyWheelNoel;
  [SerializeField] RectTransform doubleValuePackModal;
  [SerializeField] RectTransform updateWeeklyModal;
  [Header("---DailyWeekly---")]
  [SerializeField] RectTransform _dailyWeekly;
  [Header("---DailyTask---")]
  [SerializeField] RectTransform _dailyTask;
  [SerializeField] RectTransform _eventLuckyEggHunt;
  [Header("ProgerssSteakBtnSystem")]
  public ProgerssSteakBtnSystem ProgerssSteakBtnSystem;

  public bool IsShowingNotify { get; private set; }
  public bool HasShowAnyPopup { get; set; }

  private int _amountOpenPlinko;
  public int AmountOpenPlinko
  {
    get
    {
      return _amountOpenPlinko;
    }
    set
    {
      _amountOpenPlinko = value;
      PlayerPrefs.SetInt("Amount_Open_Plinko", _amountOpenPlinko);
    }
  }

  private void OnEnable()
  {
    ShopModal.onWatchFreeCoinAds += InitNoticeFreeCoin;
    FirebaseSetup.onUpdateRemote += UpdateClockTimeImg;
  }

  private void Start()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);

    InitUIBeginState();
    CheckRemoveAds7d();
  }

  private void OnDisable()
  {
    ShopModal.onWatchFreeCoinAds -= InitNoticeFreeCoin;
    FirebaseSetup.onUpdateRemote -= UpdateClockTimeImg;
  }

  private void OnDestroy()
  {
    DOTween.KillAll();
  }

  void InitUIBeginState()
  {
    if (
      SceneManager.GetActiveScene().name == "Gameplay"
      || SceneManager.GetActiveScene().name == "LevelEditor"
      || SceneManager.GetActiveScene().name == "GameplayTool"
    )
    {
      settingModal.gameObject.SetActive(false);
      shopModal.GetComponent<ShopModal>().InitPosDefault();
      return;
    }

    plinkoModal.gameObject.SetActive(false);
    settingModal.gameObject.SetActive(false);
    luckyWheelEventModal.gameObject.SetActive(false);
    luckyWheelModal.gameObject.SetActive(false);
    shopModal.gameObject.SetActive(false);
    dailyRewardModal.gameObject.SetActive(false);
    // dailyGoalModal.gameObject.SetActive(false);
    buyHeartModal.gameObject.SetActive(false);
    loadingIAP.gameObject.SetActive(false);
    dailySweetPassModal.gameObject.SetActive(false);
    taskNoelModal.gameObject.SetActive(false);
    dailyBuyPass.gameObject.SetActive(false);
    HideRemoveAdsModal();
    HideMissionModal();
    InitLuckyEggHuntOfferModal();
    InitOfferRewardModal();
    InitHeartFullRect();

    UpdateLuckyWheel();
    InitRemoveAdsIcon();
    InitPosDefaultLiveOps();
    shopModal.GetComponent<ShopModal>().InitPosDefault();

    // StartCoroutine(InitUIFollowEvent());

    // var isNeedShowDailyReward = dailyRewardModal.CheckDayilyReward();
    // if (isNeedShowDailyReward)
    // {
    //   TurnOnNoticeDailyReward();
    //   HasShowAnyPopup = true;
    //   if (GameManager.Instance.IsEvented() && PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALNOEL, 0) == 1)
    //   {
    //     OpenDailyRewardModal();
    //   }
    //   else if (!GameManager.Instance.IsEvented())
    //   {
    //     OpenDailyRewardModal();
    //   }
    // }
    // else
    // {
    //   TurnOffNoticeDailyReward();
    // }

    // InitNoticeLuckyWheel();
    // InitNoticeFreeCoin();
    // InitNoticeLuckySpin();
    // InitNoticeLuckyEggHunt();
    // if (ProgerssSteakBtnSystem.Instance != null)
    // {
    //   ProgerssSteakBtnSystem.Instance.UpdateUi(0);
    // }

    // HeartSystem.Instance.InvokeOnce();

    // if (SceneManager.GetActiveScene().name == "Lobby" && !HasShowAnyPopup && GameManager.Instance.IsMetaProgressHappenning())
    // {
    //   GameManager.Instance.PlayAnimClaimRewardMeta();
    // }

    // if (SceneManager.GetActiveScene().name == "Lobby" && !HasShowAnyPopup && IsLuckyEggHuntProcessing())
    // {
    //   GameManager.Instance.PlayAnimClaimEggHuntProgress();
    // }
  }

  public bool IsLuckyEggHuntProcessing()
  {
    var isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.is_unlock_event;
    var dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_off_event;
    var dayStartEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_start_event;

#if UNITY_ANDROID
    isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.is_unlock_event;
    dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_off_event;
    dayStartEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_start_event;
#endif

    if (!isUnlockEvent) return false;

    // Check timeEnd
    var coms = dayOffEvent.Split('/');
    if (coms.Length != 3) { return false; }
    if (!int.TryParse(coms[0], out int dayOff)) { return false; }
    if (!int.TryParse(coms[1], out int monthOff)) { return false; }
    if (!int.TryParse(coms[2], out int yearOff)) { return false; }

    var calendarOff = new DateTime(yearOff, monthOff, dayOff);
    TimeSpan remainingTime = calendarOff - DateTime.Now;

    if (remainingTime <= TimeSpan.Zero)
    {
      GameManager.Instance.ClearDataEventLuckyEggHunt();
      return false;
    }

    // Check timeStart
    var coms1 = dayStartEvent.Split('/');
    if (coms1.Length != 3) { return false; }
    if (!int.TryParse(coms1[0], out int dayStart)) { return false; }
    if (!int.TryParse(coms1[1], out int monthStart)) { return false; }
    if (!int.TryParse(coms1[2], out int yearStart)) { return false; }

    var calendarStart = new DateTime(yearStart, monthStart, dayStart);
    TimeSpan remainingTime1 = DateTime.Now - calendarStart;

    if (remainingTime1 <= TimeSpan.Zero)
    {
      GameManager.Instance.ClearDataEventLuckyEggHunt();
      return false;
    }

    return true;
  }

  public void UpdateLuckyWheel()
  {
    if (GameManager.Instance.IsEvented())
    {
      luckyWheel?.gameObject.SetActive(false);
      luckyWheelNoel?.SetActive(true);
    }
    else
    {
      luckyWheel?.gameObject.SetActive(true);
      luckyWheelNoel?.SetActive(false);
    }
  }

  IEnumerator InitUIFollowEvent()
  {
    yield return new WaitUntil(() => RendererSystem.Instance != null);
    SpawnSnow();
  }

  public void SpawnSnow()
  {
    if (GameManager.Instance.IsEvented())
    {
      Instantiate(snowPrefab);
    }
  }

  RectTransform GetLuckyWheelModal()
  {
    if (GameManager.Instance.IsEvented())
      return luckyWheelEventModal;
    else
      return luckyWheelModal;
  }

  void OpenModal(Transform panel)
  {
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenModal");
  }

  void CloseModal(Transform panel)
  {
    panel.GetComponentInChildren<Animator>().Play("CloseModal");
    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      panel.gameObject.SetActive(false);
    });
  }

  void OpenNotify(RectTransform panel)
  {
    IsShowingNotify = true;
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenNotify");

    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      IsShowingNotify = false;
      Destroy(panel.gameObject);
    });
  }

  public void ShowNotifyWith(string content)
  {
    var _goalCompletedNotify = Instantiate(goalCompletedNotify, showCanvas);
    _goalCompletedNotify.GetComponent<GoalCompletedNotify>().ShowNotify(content);
    OpenNotify(_goalCompletedNotify);
  }

  public void HomeBtn()
  {
    GameManager.Instance.SetGameState(GameState.GamepPause);
    BalloonSystem.Instance.HideBalloon();
  }

  public void GoLobby()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (GameManager.Instance.CurrentLevel == 0)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALS1) == 0)
      {
        PlayerPrefs.SetInt("PlayLevel1AnimAt_0", 0);
        PlayerPrefs.SetInt("PlayLevel1AnimAt_1", 0);
        PlayerPrefs.SetInt("PlayLevel1AnimAt_2", 0);
      }
    }

    // if (GameManager.Instance.IsRemoveAds)
    // {
    //   HeartSystem.Instance.TryDecreaseHeart(1);
    //   LoadSceneWith("Lobby");
    //   return;
    // }

    LevelPlayAds.Instance.ShowInterstitialAd(
      (info) =>
      {

      },
      "GoLobby",
      (info) =>
      {
        LeanTween.delayedCall(gameObject, 0.2f, () =>
        {
          HeartSystem.Instance.TryDecreaseHeart(1);
          LoadSceneWith("Lobby");
          quitBtn.interactable = true;
        });
      },
      () =>
      {
        LeanTween.delayedCall(gameObject, 0.2f, () =>
        {
          HeartSystem.Instance.TryDecreaseHeart(1);
          LoadSceneWith("Lobby");
          quitBtn.interactable = true;
        });
      }
    );
  }

  public void LoadSceneWithDelay(int i)
  {
    SoundManager.Instance.PlayPressBtnSfx();

    LeanTween.delayedCall(gameObject, .2f, () =>
    {
      if (!HeartSystem.Instance.TryDecreaseHeart(1))
      {
        ToggleBuyHeartPanel();
        return;
      }

      HeartSystem.Instance.TryIncreaseHeart(1);
      if (GameManager.Instance.IsEvented())
      {
        PlayerPrefs.SetInt("AmountPlayGame", PlayerPrefs.GetInt("AmountPlayGame", 0) + 1);
      }

      PlayerPrefs.SetInt("user_joined_noel_event", 1);
      GameManager.Instance.Login();
      LoadSceneAt(i);
    });
  }

  public void LoadSceneWithDelay(string sceneName)
  {
    SoundManager.Instance.PlayPressBtnSfx();
    SceneManager.LoadScene(sceneName);

    // LeanTween.delayedCall(gameObject, .2f, () =>
    // {
    //   if (!HeartSystem.Instance.TryDecreaseHeart(1))
    //   {
    //     ToggleBuyHeartPanel();
    //     return;
    //   }

    //   HeartSystem.Instance.TryIncreaseHeart(1);
    //   SceneManager.LoadScene(sceneName);
    // });
  }

  public void LoadSceneAt(int i)
  {
    SceneManager.LoadScene(i);
  }

  public void LoadSceneWith(string nameScene)
  {
    SceneManager.LoadScene(nameScene);
  }

  public void ToggleDarkPanel()
  {
    var isShow = !darkPanel.gameObject.activeSelf;
    darkPanel.gameObject.SetActive(isShow);
  }

  public void ToggleDailyBounsModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!dailyBonusModel.gameObject.activeSelf)
    {
      OpenModal(dailyBonusModel);
    }
    else
    {
      CloseModal(dailyBonusModel);
    }
  }

  public void ToggleDailyGoalModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!dailyGoalModal.gameObject.activeSelf)
    {
      OpenModal(dailyGoalModal);
    }
    else
    {
      CloseModal(dailyGoalModal);
    }
  }

  public void TogglePlinkoPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!plinkoModal.gameObject.activeSelf)
    {
      OpenModal(plinkoModal);

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(
          "plinko_ball",
            new Parameter[]{
              new("open", AmountOpenPlinko)
            }
          );
      }
    }
    else
    {
      CloseModal(plinkoModal);
    }
  }

  public void ToggleMetaProgerssPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    var isShow = !MetaProgerssModal.gameObject.activeSelf;
    MetaProgerssModal.gameObject.SetActive(isShow);
    ShowPanel.Instance.CoinBlock.gameObject.SetActive(!isShow);
  }

  public void ToggleSettingPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!settingModal.gameObject.activeSelf)
    {
      OpenModal(settingModal);
    }
    else
    {
      CloseModal(settingModal);
    }
  }

  public void ToggleDailyBuyPass()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!dailyBuyPass.gameObject.activeSelf)
    {
      OpenModal(dailyBuyPass);
    }
    else
    {
      CloseModal(dailyBuyPass);
    }
  }

  public void OpenUpdateModal()
  {
    OpenModal(updateWeeklyModal);
  }

  public void CloseUpdateModal()
  {
    CloseModal(updateWeeklyModal);
  }

  public void OpenDailyRewardModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    OpenModal(dailyRewardModal.transform);
  }

  public void ToggleDailyRewardPanel(float _timeDelayMeta)
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!dailyRewardModal.gameObject.activeSelf)
    {
      OpenModal(dailyRewardModal.transform);
    }
    else
    {
      CloseModal(dailyRewardModal.transform);

      if (GameManager.Instance.IsMetaProgressHappenning())
      {
        DOVirtual.DelayedCall(
          _timeDelayMeta,
          () =>
          {
            if (HasShowAnyPopup)
            {
              HasShowAnyPopup = false;
              GameManager.Instance.PlayAnimClaimRewardMeta();
            }
          }
        );
      }

      if (IsLuckyEggHuntProcessing())
      {
        GameManager.Instance.PlayAnimClaimEggHuntProgress();
      }
    }
  }

  public void ToggleLuckyWheelPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!GetLuckyWheelModal().gameObject.activeSelf)
    {
      OpenModal(GetLuckyWheelModal());
      onOpenLuckyWheel?.Invoke();
    }
    else
    {
      CloseModal(GetLuckyWheelModal());
    }
  }

  public bool TryCloseLuckyWheelEventModal()
  {
    if (luckyWheelEventModal.gameObject.activeSelf)
    {
      CloseModal(luckyWheelEventModal);
      return true;
    }

    return false;
  }

  public void ToggleTaskNoelModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!taskNoelModal.gameObject.activeSelf)
    {
      OpenModal(taskNoelModal);
    }
    else
    {
      CloseModal(taskNoelModal);
    }
  }

  //thang
  public void ToggleDailySweetPassPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!dailySweetPassModal.gameObject.activeSelf)
    {
      OpenModal(dailySweetPassModal);
      onOpenDailySweetPass?.Invoke();
    }
    else
    {
      CloseModal(dailySweetPassModal);
    }
  }

  public void CloseShopPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    // if (shopModal.gameObject.activeSelf)
    // {
    //   shopAnim.enabled = true;
    //   CloseModal(shopModal);
    // }
    shopModal.GetComponent<ShopModal>().HideAnimTrans();
  }

  public void OpenShopPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    // LeanTween.delayedCall(gameObject, 0.4f, () =>
    // {
    //   shopAnim.enabled = false;
    //   shopModal.GetComponent<ShopModal>().TurnAnim();
    // });
    // OpenMadal(shopModal);
    shopModal.GetComponent<ShopModal>().ShowAnimTrans();
  }

  // v1.4
  public void ToggleBuyHeartPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!buyHeartModal.gameObject.activeSelf)
    {
      OpenModal(buyHeartModal);
    }
    else
    {
      CloseModal(buyHeartModal);
    }
  }
  public void ShowDailyWeeklyPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (!_dailyWeekly.gameObject.activeSelf)
    {
      OpenModal(_dailyWeekly.transform);
    }
    else
    {
      CloseModal(_dailyWeekly.transform);
    }

  }
  public void ShowDailyTaskPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (!_dailyTask.gameObject.activeSelf)
    {
      OpenModal(_dailyTask.transform);
    }
    else
    {
      CloseModal(_dailyTask.transform);
    }

  }

  public void OpenLoadingIAP()
  {
    loadingIAP.gameObject.SetActive(true);
  }

  public void CloseLoadingIAP()
  {
    loadingIAP.gameObject.SetActive(false);
  }

  // v1.5
  public void ToggleDoubleValuePackModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!doubleValuePackModal.gameObject.activeSelf)
    {
      OpenModal(doubleValuePackModal);
    }
    else
    {
      CloseModal(doubleValuePackModal);
    }
  }

  public void OpenToolEditor()
  {
    SceneManager.LoadScene("TestNew");
  }


  public void ToggleEventLuckEggHuntPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (!_eventLuckyEggHunt.gameObject.activeSelf)
    {
      OpenModal(_eventLuckyEggHunt.transform);
    }
    else
    {
      CloseModal(_eventLuckyEggHunt.transform);
    }

  }
}
