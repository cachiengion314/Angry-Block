using System;
using HoangNam;
using UnityEngine;

public class InitDailyBonus : MonoBehaviour
{
  static public bool HasUserOpened = false;

  public void Init()
  {
    HasUserOpened = true;

    DateTime currentTime = DateTime.Now;

    DailyBonusData data;
    if (
      FirebaseSetup.Instance.FirebaseStatusCode == 1 &&
      GameManager.Instance.GotUserIdResultCode == 1
    )
      data = FirebaseSetup.Instance.DailyBonusData;
    else
      data = SaveSystem.LoadWith<DailyBonusData>(KeyString.NAME_DAILYBONUS_DATA);

    if (data.IsNull)
    {
      // if _data is still null we have to create default _data
      data = new DailyBonusData();
      Save(currentTime, data);
      return;
    }

    DateTime startTime = DateTime.ParseExact(data.startTime, "yyyy-MM-dd HH:mm", null);
    DateTime endTime = DateTime.ParseExact(data.endTime, "yyyy-MM-dd HH:mm", null);

    bool isIvent = InitEvent.CheckEvent(currentTime, startTime, endTime);
    if (isIvent) return;

    Save(currentTime, data);
  }

  static public DailyBonusData CreateDefaultDailyBonusData()
  {
    var data = new DailyBonusData();

    DateTime currentTime = DateTime.Now;
    InitEvent.Init(
      currentTime,
      GameManager.Instance.DailyBonusData,
      out DateTime startTime,
      out DateTime endTime
    );

    data.startTime = startTime.ToString("yyyy-MM-dd HH:mm");
    data.endTime = endTime.ToString("yyyy-MM-dd HH:mm");
    data.data = GetDailyBonusRewards();

    return data;
  }

  void Save(DateTime currentTime, DailyBonusData data)
  {
    InitEvent.Init(
      currentTime,
      GameManager.Instance.DailyBonusData,
      out DateTime startTime,
      out DateTime endTime
    );
    data.startTime = startTime.ToString("yyyy-MM-dd HH:mm");
    data.endTime = endTime.ToString("yyyy-MM-dd HH:mm");
    data.data = GetDailyBonusRewards();
    SaveSystem.SaveWith(data, KeyString.NAME_DAILYBONUS_DATA);

    GameManager.Instance.IsShowNoticeDailyBouns = true;

    FirebaseSetup.Instance.SaveData(data);
  }

  static bool[] GetDailyBonusRewards()
  {
    bool[] rewards = new bool[GameManager.Instance.RewardDatas.Length];
    for (int i = 0; i < GameManager.Instance.RewardDatas.Length; i++)
    {
      rewards[i] = false;
    }
    return rewards;
  }

  public void UpdateBounsData(int id)
  {
    DailyBonusData data;
    if (
      FirebaseSetup.Instance.FirebaseStatusCode == 1 &&
      GameManager.Instance.GotUserIdResultCode == 1
    )
      data = FirebaseSetup.Instance.DailyBonusData;
    else
      data = SaveSystem.LoadWith<DailyBonusData>(KeyString.NAME_DAILYBONUS_DATA);

    data.data[id] = true;

    SaveSystem.SaveWith(data, KeyString.NAME_DAILYBONUS_DATA);
    FirebaseSetup.Instance.SaveData(data);
  }

  public DailyBonusData GetDailyBonusData()
  {
    DailyBonusData data;
    if (
      FirebaseSetup.Instance.FirebaseStatusCode == 1 &&
      GameManager.Instance.GotUserIdResultCode == 1
    )
      data = FirebaseSetup.Instance.DailyBonusData;
    else
      data = SaveSystem.LoadWith<DailyBonusData>(KeyString.NAME_DAILYBONUS_DATA);

    return data;
  }

  public RewardData GetRewardAt(int id)
  {
    return GameManager.Instance.RewardDatas[id];
  }
}
