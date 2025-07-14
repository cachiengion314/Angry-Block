using System;
using Firebase.Analytics;
using HoangNam;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardModal : MonoBehaviour
{
  public static DailyRewardModal Instance { get; private set; }

  public static bool HasUserOpened = false;

  [Header("Injected Dependencies")]
  [SerializeField] DayBlock dayBlockPrefab;
  [SerializeField] RectTransform dayBlockParent;
  [SerializeField] SpecialDayBlock specialDayBlock;
  [SerializeField] Button claimBtn;
  [SerializeField] Button claimX2Btn;
  [SerializeField] Sprite chestSprite;
  [SerializeField] Sprite[] dailyRewardSprites;

  [Range(-1, 6)]
  public int FakeToday = -1;

  [Header("Settings")]
  DayRewardData DayRewardData;
  IDayBlock[] dayBlocks;
  DayBlock[] dayBlocks1;

  private void Awake()
  {
    // if (Instance == null)
    // {
    //   Instance = this;
    // }
    // else
    // {
    //   Destroy(gameObject);
    // }

    // LoadData();
    // CheckSelectedDaysChain();
    // InitUIBeginState();
    // for (int i = 0; i < dayBlocks.Length; ++i)
    // {
    //   dayBlocks[i].InjectRewardImg(dailyRewardSprites[i]);
    // }
  }

  public bool CheckDayilyReward()
  {
    var today = GetTodayOfWeek();
    var latestDay = FindLatestDayWhenUserClaim();
    if (today == latestDay) return false;
    return true;
  }

  private void OnEnable()
  {
    CheckSelectedDaysChain();
    UpdateUIClaimBtn();
    UpdateUIDayBlocks();
  }

  void InitUIBeginState()
  {
    var dayDatas = DayRewardData.DayDatas;
    dayBlocks = new IDayBlock[dayDatas.Length];
    dayBlocks1 = new DayBlock[dayDatas.Length];

    for (int i = 0; i < dayDatas.Length; i++)
    {
      var dayblockClone = Instantiate(dayBlockPrefab, dayBlockPrefab.transform.position, Quaternion.identity);
      dayblockClone.transform.SetParent(dayBlockParent);
      dayblockClone.transform.localScale = new Vector3(1, 1, 1);

      dayblockClone.InjectDailyRewardModal(this);
      dayblockClone.InjectDayData(dayDatas[i]);
      dayblockClone.InjectDayRewardData(DayRewardData);
      dayBlocks[i] = dayblockClone;
      dayBlocks1[i] = dayblockClone;
    }

    dayBlocks[dayBlocks.Length - 1] = specialDayBlock;
    UpdateUIDayBlocks();
  }

  void UpdateUIDayBlocks()
  {
    if (dayBlocks == null) return;

    var dayDatas = DayRewardData.DayDatas;
    var today = GetTodayOfWeek();

    var index = dayBlocks.Length - 1;

    while (index >= 0)
    {
      if (dayDatas[index].DayActivated < 0)
      {
        dayBlocks[index].ShowFollowingDay();
      }
      else break;
      index--;
    }
    if (index + 1 <= dayBlocks.Length - 1)
      dayBlocks[index + 1].ShowToday();


    for (int i = 0; i < dayBlocks.Length; ++i)
    {
      if (dayDatas[i].DayActivated >= 0)
      {
        dayBlocks[i].ShowAlreadyActivatedDay();
      }
    }

    for (int i = dayBlocks.Length - 1; i >= 0; i--)
    {
      if (dayDatas[i].DayActivated == today)
      {
        dayBlocks[i].ShowTodayActivated();
        if (i + 1 <= dayBlocks.Length - 1)
        {
          dayBlocks[i + 1].ShowTomorrow();
        }
        break;
      }
    }

    if (DayRewardData.IsReachMaxPrice)
    {
      for (int i = 0; i < dayBlocks.Length; ++i)
      {
        if (dayDatas[i].DayActivated >= 0)
        {
          dayBlocks[i].ShowAlreadyActivatedDay();
        }
      }

      dayBlocks[^1].ShowTodayActivated();
    }

    for (int i = 0; i < dayBlocks.Length; ++i)
    {
      if (i == 1 || i == 2 || i == 4 || i == 5)
      {
        dayBlocks1[i].ShowMulText();
      }
    }
  }

  public void UpdateUIClaimBtn()
  {
    if (CanPressClaim())
    {
      claimBtn.interactable = true;
      claimX2Btn.interactable = true;
    }
    else
    {
      claimBtn.interactable = false;
      claimX2Btn.interactable = false;
    }
  }

  static public DayRewardData CreateDefaultDayRewardData()
  {
    var dayDatas = new DayData[7];
    for (int i = 0; i < dayDatas.Length; ++i)
    {
      switch (i)
      {
        case 0:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 50 },
            DayActivated = -1,
          };
          break;
        case 1:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 1 },
            DayActivated = -1,
          };
          break;
        case 2:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 1 },
            DayActivated = -1,
          };
          break;
        case 3:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 100 },
            DayActivated = -1,
          };
          break;
        case 4:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 1 },
            DayActivated = -1,
          };
          break;
        case 5:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 1 },
            DayActivated = -1,
          };
          break;
        case 6:
          dayDatas[i] = new DayData
          {
            DayIndex = i,
            CoinValue = new int[] { 200, 30, 1 },
            DayActivated = -1,
          };
          break;
      }
    }

    return new DayRewardData
    {
      DayDatas = dayDatas,
    };
  }

  public void LoadData()
  {
    HasUserOpened = true;

    if (
      FirebaseSetup.Instance.FirebaseStatusCode == 1 &&
      FirebaseSetup.Instance.dayRewardData != null &&
      GameManager.Instance.GotUserIdResultCode == 1
    )
    {
      DayRewardData = FirebaseSetup.Instance.DayRewardData;
      SaveData(DayRewardData);
      return;
    }

    var result = SaveSystem.LoadWith<DayRewardData>(KeyString.NAME_DAY_REWARD_DATA);
    if (result == null)
    {
      DayRewardData = CreateDefaultDayRewardData();
      SaveData(DayRewardData);
      return;
    }
    DayRewardData = result;
  }

  public void SaveData(DayRewardData dayRewardData)
  {
    HoangNam.SaveSystem.SaveWith(dayRewardData, KeyString.NAME_DAY_REWARD_DATA);

    FirebaseSetup.Instance.SaveData(dayRewardData);
  }

  bool CanPressClaim()
  {
    int latestDay = FindLatestDayWhenUserClaim();
    int today = GetTodayOfWeek();
    if (today == latestDay) return false;
    return true;
  }

  public void Claim(bool isDoubleEarn = false)
  {
    SoundManager.Instance.PlayPressBtnSfx();
    SoundManager.Instance.PlayClaimDailyRewardSfx();

    int latestDay = FindLatestDayWhenUserClaim();
    int today = GetTodayOfWeek();
    if (today == latestDay) return;

    LobbyPanel.Instance.TurnOffNoticeDailyReward();
    // First time earn price
    if (latestDay == -1)
    {
      AssignDayAt(today);
      EarnPriceAt(today, isDoubleEarn);
      UpdateUIClaimBtn();
      UpdateUIDayBlocks();
      return;
    }

    if (latestDay >= 0 && (latestDay + 1) % 7 == today)
    {
      AssignDayAt(today);
      EarnPriceAt(today, isDoubleEarn);
      UpdateUIClaimBtn();
      UpdateUIDayBlocks();
      return;
    }

    ResetClaimedChain();
    AssignDayAt(today);
    EarnPriceAt(today, isDoubleEarn);
    UpdateUIClaimBtn();
    UpdateUIDayBlocks();
  }

  public void ClaimX2()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    int latestDay = FindLatestDayWhenUserClaim();
    int today = GetTodayOfWeek();
    if (today == latestDay) return;

    claimX2Btn.interactable = false;
    LevelPlayAds.Instance.ShowRewardedAd(() =>
    {
      CloseAdEarnedReward();
    },
    "ClaimX2",
    () =>
    {
      StartCoroutine(EffectManager.Instance.IEDelayShow(claimX2Btn, 1.5f));

      LobbyPanel.Instance.ShowNotifyWith("ADS NOT READY");
    });
  }

  public void CheckSelectedDaysChain()
  {
    if (DayRewardData == null) return;

    int latestDay = FindLatestDayWhenUserClaim();
    int today = GetTodayOfWeek();
    if (today == latestDay) return;

    if (latestDay >= 0 && (latestDay + 1) % 7 == today)
    {
      return;
    }
    ResetClaimedChain();
  }

  public int FindLatestDayWhenUserClaim()
  {
    var latestDay = -1;
    var dayDatas = DayRewardData.DayDatas;
    for (int i = dayDatas.Length - 1; i >= 0; i--)
    {
      if (dayDatas[i].DayActivated >= 0)
      {
        latestDay = dayDatas[i].DayActivated;
        break;
      }
    }
    return latestDay;
  }

  public int FindUserClaimDaysCount()
  {
    var count = 0;
    var dayDatas = DayRewardData.DayDatas;
    for (int i = 0; i < dayDatas.Length; i++)
    {
      if (dayDatas[i].DayActivated >= 0)
      {
        count++;
      }
      else
      {
        break;
      }
    }
    return count;
  }

  public void AssignDayAt(int day)
  {
    var dayDatas = DayRewardData.DayDatas;

    var isReachMaxPrice = DayRewardData.IsReachMaxPrice;
    if (isReachMaxPrice)
    {
      dayDatas[^1].DayActivated = day;
      DayRewardData.DayDatas = dayDatas;
      SaveData(DayRewardData);
      return;
    }

    for (int i = 0; i < dayDatas.Length; ++i)
    {
      if (dayDatas[i].DayActivated >= 0) continue;

      dayDatas[i].DayActivated = day;
      if (i == dayDatas.Length - 1) isReachMaxPrice = true;
      break;
    }

    DayRewardData.DayDatas = dayDatas;
    DayRewardData.IsReachMaxPrice = isReachMaxPrice;

    SaveData(DayRewardData);
  }

  public void ResetClaimedChain()
  {
    var dayDatas = DayRewardData.DayDatas;
    for (int i = 0; i < dayDatas.Length; ++i)
    {
      dayDatas[i].DayActivated = -1;
    }

    DayRewardData.DayDatas = dayDatas;
    DayRewardData.IsReachMaxPrice = false;

    SaveData(DayRewardData);
  }

  public void EarnPriceAt(int day, bool isDoubleEarn = false)
  {
    var isReachMaxPrice = DayRewardData.IsReachMaxPrice;

    var price = FindPriceAt(day, isReachMaxPrice);
    if (isDoubleEarn)
    {
      for (int id = 0; id < price.Length; id++)
      {
        price[id] *= 2;
      }
    }

    var priceData = ScriptableObject.CreateInstance<RewardData>();

    priceData.Value = price;
    var i = FindUserClaimDaysCount() - 1;
    if (i == 0)
    {
      GameManager.Instance.CurrentCoin += priceData.Value[0];
      priceData.Img = dailyRewardSprites[0];
      ShowPanel.Instance.ShowImgWith(priceData.Img, "", "" + price[0], Color.yellow, false);
    }
    else if (i == 1)
    {
      GameManager.Instance.CurrentRefresh += priceData.Value[0];
      priceData.Img = dailyRewardSprites[1];
      ShowPanel.Instance.ShowImgWith(priceData.Img, "", "x" + price[0], Color.yellow, false);
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Refresh"),
            new ("placement", "DailyReward")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Refresh" + "_DailyReward");
    }
    else if (i == 2)
    {
      GameManager.Instance.CurrentHammer += priceData.Value[0];
      priceData.Img = dailyRewardSprites[2];
      ShowPanel.Instance.ShowImgWith(priceData.Img, "", "x" + price[0], Color.yellow, false);
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Hammer"),
            new ("placement", "DailyReward")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Hammer" + "_DailyReward");
    }
    else if (i == 3)
    {
      GameManager.Instance.CurrentCoin += priceData.Value[0];
      priceData.Img = dailyRewardSprites[3];
      ShowPanel.Instance.ShowImgWith(priceData.Img, "", "" + price[0], Color.yellow, false);
    }
    else if (i == 4)
    {
      GameManager.Instance.CurrentTicket += priceData.Value[0];
      priceData.Img = dailyRewardSprites[4];
      ShowPanel.Instance.ShowImgWith(priceData.Img, "", "x" + price[0], Color.yellow, false);
    }
    else if (i == 5)
    {
      GameManager.Instance.CurrentSwap += priceData.Value[0];
      priceData.Img = dailyRewardSprites[5];
      ShowPanel.Instance.ShowImgWith(priceData.Img, "", "x" + price[0], Color.yellow, false);
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Swap"),
            new ("placement", "DailyReward")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Swap" + "_DailyReward");
    }
    else if (i == 6)
    {
      GameManager.Instance.CurrentCoin += priceData.Value[0];
      HeartSystem.Instance.AddInfinityHeartTime(priceData.Value[1]);
      GameManager.Instance.CurrentRocket += priceData.Value[2];

      var imgs = new Sprite[] { dailyRewardSprites[6], dailyRewardSprites[7], dailyRewardSprites[8] };
      var topTxts = new string[] { "", "", "" };
      var bottomTxts = new string[] { "" + price[0], price[1] + "m", "x" + price[2] };
      var colorTxts = new Color[] { Color.yellow, Color.yellow, Color.yellow };

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Rocket"),
            new ("placement", "DailyReward")
           });
      }

      Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Rocket" + "_DailyReward");

      ShowPanel.Instance.ShowImgsWith(imgs, topTxts, bottomTxts, colorTxts, false);
      if (isDoubleEarn)
      {
        for (int id = 0; id < price.Length; id++)
        {
          price[id] /= 2;
        }
      }

      LobbyPanel.Instance.ToggleDailyRewardPanel(1.5f);
      return;
    }

    if (isDoubleEarn)
    {
      for (int id = 0; id < price.Length; id++)
      {
        price[id] /= 2;
      }
    }
    LobbyPanel.Instance.ToggleDailyRewardPanel(1.5f);
  }

  public int[] FindPriceAt(int day, bool isReachMaxPrice = false)
  {
    var dayDatas = DayRewardData.DayDatas;
    if (isReachMaxPrice) return dayDatas[^1].CoinValue;

    for (int i = 0; i < dayDatas.Length; ++i)
    {
      if (dayDatas[i].DayActivated == day)
      {
        return dayDatas[i].CoinValue;
      }
    }
    return new int[] { 0 };
  }

  public bool IsReachMaxPrice()
  {
    return DayRewardData.IsReachMaxPrice;
  }

  public int GetTodayOfWeek()
  {
    if (FakeToday > -1)
    {
      return FakeToday;
    }

    int today = (int)System.DateTime.Now.DayOfWeek;
    return today;
  }

  public void CloseAdEarnedReward()
  {
    Claim(true);
  }
}
