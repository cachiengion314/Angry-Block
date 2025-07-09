using System;
using DG.Tweening;
using Firebase.Analytics;
using HoangNam;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LuckyWheelModal : MonoBehaviour
{
  public static LuckyWheelModal Instance { get; private set; }

  [Header("Internal Dependencies")]
  [SerializeField] RewardData[] luckyWheelPriceDatas;
  [SerializeField] Image luckyWheelImg;
  [SerializeField] Button passBtn;
  [SerializeField] Button taskBtn;
  [SerializeField] Button stopBtn;
  [SerializeField] Button spinFreeBtn;
  [SerializeField] Button spinAdsBtn;
  [SerializeField] Button closeBtn;
  [SerializeField] TextMeshProUGUI ticketTxt;
  [SerializeField] TextMeshProUGUI adsLimitTxt;

  [Range(0, 7)]
  public int AnglesIndex; // Indicate where to stop the wheel at (AnglesIndex * 45) degree

  [Header("Events")]
  public Action<RewardData> onStoppedRotate;

  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void OnEnable()
  {
    UpdateUI();
    UpdateButtonSpin();

    GameManager.onChangeAmountTicket += UpdateUI;
    GameManager.onChangeLuckyWheelAds += UpdateUI;
  }

  private void OnDisable()
  {
    GameManager.onChangeAmountTicket -= UpdateUI;
    GameManager.onChangeLuckyWheelAds += UpdateUI;
  }

  void UpdateUI()
  {
    UpdateUILuckyWheel();
  }

  private void UpdateUILuckyWheel()
  {
    var dailyLimit = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.dailyLimit;
#if UNITY_ANDROID
    dailyLimit = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.dailyLimit;
#endif
    ticketTxt.text = ": " + GetTicketValue();

    var amountAdsCanSpin = dailyLimit - GameManager.Instance.LuckyWheelAdsTodayCount;
    if (amountAdsCanSpin <= 0) amountAdsCanSpin = 0;
    adsLimitTxt.text = "DAILY LIMIT: " + amountAdsCanSpin + "/" + dailyLimit;

    if (GetTicketValue() > 0)
    {
      LobbyPanel.Instance.TurnOnNoticeLuckyWheel();
      return;
    }
    LobbyPanel.Instance.TurnOffNoticeLuckyWheel();

    if (GameManager.Instance.IsEvented()) return;
  }

  public void ClickSpinByTicket()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (GetTicketValue() > 0)
    {
      StartRotateByTicket();
      return;
    }
  }

  public void ClickSpinByAds()
  {
    StartRotateByAd();
  }

  void StartRotateByTicket()
  {
    StartRotate("regular");
    SetTicketValue(-1);

    var data = new MissonDataDailyTask(enumListeningDataDailyTask.SpinLuckyWheel, Math.Abs(1));
    EventActionManager.TriggerEvent(enumListeningDataDailyTask.SpinLuckyWheel, data);
    UpdateUI();
  }

  void StartRotateByAd()
  {
    spinFreeBtn.interactable = false;
    spinAdsBtn.interactable = false;
    closeBtn.interactable = false;

    var dailyLimit = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.dailyLimit;
#if UNITY_ANDROID
    dailyLimit = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.dailyLimit;
#endif

    if (GameManager.Instance.LuckyWheelAdsTodayCount >= dailyLimit)
    {
      if (SceneManager.GetActiveScene().name == "Lobby")
      {
        LobbyPanel.Instance.ShowNotifyWith("REACHED LIMIT TODAY");
        StartCoroutine(EffectManager.Instance.IEDelayShow(spinAdsBtn, 1.5f));
        StartCoroutine(EffectManager.Instance.IEDelayShow(spinFreeBtn, 1.5f));
        StartCoroutine(EffectManager.Instance.IEDelayShow(closeBtn, 1.5f));
        return;
      }
    }

    LevelPlayAds.Instance.ShowRewardedAd(() =>
    {
      StartRotate(
        "withrv"
      , () =>
      {
        GameManager.Instance.LuckyWheelAdsTodayCount++;
        var data = new MissonDataDailyTask(enumListeningDataDailyTask.SpinLuckyWheel, Math.Abs(1));
        EventActionManager.TriggerEvent(enumListeningDataDailyTask.SpinLuckyWheel, data);
      });
    },
    "StopRotate",
    () =>
    {
      StartCoroutine(EffectManager.Instance.IEDelayShow(spinAdsBtn, 1.5f));
      StartCoroutine(EffectManager.Instance.IEDelayShow(spinFreeBtn, 1.5f));
      StartCoroutine(EffectManager.Instance.IEDelayShow(closeBtn, 1.5f));

      LobbyPanel.Instance.ShowNotifyWith("ADS NOT READY");
    });
  }

  public void StartRotate(string spinType, Action onCompleted = null)
  {
    // Check interactable stop button
    spinFreeBtn.interactable = false;
    spinAdsBtn.interactable = false;
    closeBtn.interactable = false;

    if (taskBtn != null)
    {
      taskBtn.interactable = false;
      passBtn.interactable = false;
    }

    UpdateUI();

    DOTween.Kill(luckyWheelImg.transform);
    var currentTime = 0f;

    if (taskBtn != null)
    {
      taskBtn.interactable = false;
      passBtn.interactable = false;
    }

    var currentIndex = AnglesIndex;
    AnglesIndex = GetCalculateAnglesIndex();
    var angleZ = (AnglesIndex - currentIndex) * 45;
    angleZ = angleZ > 0 ? angleZ - 360 : angleZ;

    Sequence seq = DOTween.Sequence();
    seq.Insert(
      currentTime,
      luckyWheelImg.transform
        .DORotate(
          new float3(0, 0, -360 * 11 + angleZ),
          0.4f * 15,
          RotateMode.FastBeyond360
        )
        .SetRelative(true)
        .SetEase(Ease.InOutQuad)
    );

    currentTime += 0.4f * 15;

    seq.InsertCallback(currentTime + 0.2f,
      () =>
      {
        TryEarnPrice();
        onCompleted?.Invoke();
      }
    );
  }

  public void OnCompletedStopRotate()
  {
    UpdateButtonSpin();
    closeBtn.interactable = true;

    if (taskBtn != null)
    {
      taskBtn.interactable = true;
      passBtn.interactable = true;
    }

    UpdateUI();
  }

  void TryEarnPrice()
  {
    onStoppedRotate?.Invoke(luckyWheelPriceDatas[AnglesIndex]);
    ShowPanel.Instance.InjectPriceData(luckyWheelPriceDatas[AnglesIndex],
        ShowPanel.Instance.GlobalCoinImg.transform,
        (priceData) =>
        {
          // Give price when user hit claim button
          EarnPriceBy(priceData);
          // Restart the rotation of the wheel
          LeanTween.delayedCall(gameObject, .55f, OnCompletedStopRotate);
        },
        (priceData) =>
        {
          // Give price when user hit claim button
          EarnPriceBy(priceData, 2);
          // Restart the rotation of the wheel
          LeanTween.delayedCall(gameObject, .55f, OnCompletedStopRotate);
        });

    if (luckyWheelPriceDatas[AnglesIndex].Type == PriceType.InfinityHeart)
    {
      ShowPanel.Instance.ShowImgWith(
        luckyWheelPriceDatas[AnglesIndex].Img, "",
        luckyWheelPriceDatas[AnglesIndex].Value[0] + "m"
        , Color.yellow
      );
      return;
    }

    if (luckyWheelPriceDatas[AnglesIndex].Type == PriceType.Coin)
    {
      ShowPanel.Instance.ShowImgWith(
        luckyWheelPriceDatas[AnglesIndex].Img, "",
        "" + luckyWheelPriceDatas[AnglesIndex].Value[0]
        , Color.yellow
      );
      return;
    }


    ShowPanel.Instance.ShowImgWith(
      luckyWheelPriceDatas[AnglesIndex].Img, "",
      "x" + luckyWheelPriceDatas[AnglesIndex].Value[0]
      , Color.yellow
    );
  }

  public void EarnPriceBy(RewardData priceData, int priceValueFactor = 1)
  {
    if (priceData.Type == PriceType.Coin)
      GameManager.Instance.CurrentCoin += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.Hammer)
    {
      GameManager.Instance.CurrentHammer += priceData.Value[0] * priceValueFactor;
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Hammer"),
            new ("placement", "LuckyWheel")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Hammer" + "_LuckyWheel");
    }
    else if (priceData.Type == PriceType.Refresh)
    {
      GameManager.Instance.CurrentRefresh += priceData.Value[0] * priceValueFactor;
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Refresh"),
            new ("placement", "LuckyWheel")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Refresh" + "_LuckyWheel");
    }
    else if (priceData.Type == PriceType.Rocket)
    {
      GameManager.Instance.CurrentRocket += priceData.Value[0] * priceValueFactor;
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Rocket"),
            new ("placement", "LuckyWheel")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Rocket" + "_LuckyWheel");
    }
    else if (priceData.Type == PriceType.Swap)
    {
      GameManager.Instance.CurrentSwap += priceData.Value[0] * priceValueFactor;
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Swap"),
            new ("placement", "LuckyWheel")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Swap" + "_LuckyWheel");
    }
    else if (priceData.Type == PriceType.Ticket)
      GameManager.Instance.CurrentTicket += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.TicketNoel)
      GameManager.Instance.CurrentTicketNoel += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.InfinityHeart)
      HeartSystem.Instance.AddInfinityHeartTime(priceData.Value[0] * priceValueFactor);

    LuckyWheelManager.Instance.SaveLuckyWheelProgress();
  }

  public int GetCalculateAnglesIndex()
  {
    if (GameManager.Instance.IsEvented())
      return CalculateAnglesIndexEvent();
    else
      return CalculateAnglesIndex();
  }

  private int CalculateAnglesIndex()
  {
    var tier1 = new int[] { 2, 5, 7 };
    var tier2 = new int[] { 1, 3, 6 };
    var tier3 = 4;
    var tier4 = 0;

    var rate = UnityEngine.Random.Range(0, 1f);
    var id = UnityEngine.Random.Range(0, 3);

    var rateTier1 = 0f;
    var rateTier2 = 0f;
    var rateTier3 = 0f;
    var rateTier4 = 0f;

#if UNITY_ANDROID
    rateTier1 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.tier1;
    rateTier2 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.tier2;
    rateTier3 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.tier3;
    rateTier4 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.tier4;
#elif UNITY_IOS
    rateTier1 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.tier1;
    rateTier2 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.tier2;
    rateTier3 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.tier3;
    rateTier4 = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.tier4;
#endif

    if (rate <= rateTier1)
    {
      return tier1[id];
    }
    else if (rate <= rateTier1 + rateTier2)
    {
      return tier2[id];
    }
    else if (rate <= rateTier1 + rateTier2 + rateTier3)
    {
      return tier3;
    }

    return tier4;
  }

  private int CalculateAnglesIndexEvent()
  {
    var tier1 = 0;
    var tier2 = new int[] { 1, 2, 3 };
    var tier3 = new int[] { 4, 5, 6 };
    var tier4 = 7;

    var rate = UnityEngine.Random.Range(0, 1f);
    var id = UnityEngine.Random.Range(0, 3);

    if (rate >= 0.8f)
    {
      return tier1;
    }
    else if (rate >= 0.35f)
    {
      return tier2[id];
    }
    else if (rate >= 0.05f)
    {
      return tier3[id];
    }

    return tier4;
  }

  int GetTicketValue()
  {
    if (GameManager.Instance.IsEvented())
      return GameManager.Instance.CurrentTicketNoel;
    else
      return GameManager.Instance.CurrentTicket;
  }

  void SetTicketValue(int value)
  {
    if (GameManager.Instance.IsEvented())
    {
      GameManager.Instance.CurrentTicketNoel += value;
    }
    else
    {
      GameManager.Instance.CurrentTicket += value;
    }
  }

  private void UpdateButtonSpin()
  {
    if (GetTicketValue() > 0) spinFreeBtn.interactable = true;
    else spinFreeBtn.interactable = false;

    var dailyLimit = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.ios.dailyLimit;
#if UNITY_ANDROID
    dailyLimit = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_wheel.android.dailyLimit;
#endif

    if (GameManager.Instance.LuckyWheelAdsTodayCount < dailyLimit) spinAdsBtn.interactable = true;
    else spinAdsBtn.interactable = false;
  }
}