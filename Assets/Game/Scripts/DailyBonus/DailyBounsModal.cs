using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DailyBounsModal : MonoBehaviour
{
  [SerializeField] DailyBounsControl[] dailyBounsControls;
  [SerializeField] InitDailyBonus initDailyBonus;
  [SerializeField] TextMeshProUGUI timeTxt;
  void Start()
  {
    Init();
  }
  public void Init()
  {
    initDailyBonus.Init();
    DailyBonusData data = initDailyBonus.GetDailyBonusData();
    if (data.IsNull) return;
    StartCoroutine(UpdateTime(data.endTime));
    var rewards = data.data;
    int index = GetNextRewardId(rewards);
    for (int i = 0; i < rewards.Length; i++)
    {
      var rewardData = initDailyBonus.GetRewardAt(i);
      dailyBounsControls[i].Init(rewardData);
      if (rewards[i])
        dailyBounsControls[i].Complete();
      else
      {
        if (i == 0) dailyBounsControls[i].Accepted();
        else dailyBounsControls[i].NotAccepted();
      }
      if (i == index) dailyBounsControls[i].Accepted();
    }
  }

  IEnumerator UpdateTime(string endTimeStr)
  {
    DateTime endTime = DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null);
    while (true)
    {
      DateTime currentTime = DateTime.Now;
      TimeSpan time = endTime - currentTime;
      timeTxt.text = ConvertElapsedtimeSecondsToTimeStr(time.TotalSeconds);
      if (time.TotalSeconds <= 0)
      {
        Init();
        yield break;
      }
      yield return new WaitForSeconds(1f);
    }
  }

  string ConvertElapsedtimeSecondsToTimeStr(double elapsedTime)
  {
    int totalSeconds = Mathf.FloorToInt((float)elapsedTime);
    int days = totalSeconds / 86400;
    int hours = (totalSeconds % 86400) / 3600;
    int minutes = (totalSeconds % 3600) / 60;
    int seconds = totalSeconds % 60;

    if (days > 0)
    {
      return $"{days}d{hours}h";
    }
    else if (hours > 0)
    {
      return $"{hours}h{minutes}m";
    }
    else if (minutes > 0)
    {
      return $"{minutes}m{seconds}s";
    }
    else
    {
      return $"{seconds}s";
    }
  }

  int GetNextRewardId(bool[] data)
  {
    for (int i = 0; i < data.Length; i++)
    {
      if (i == 0) continue;
      if (!data[i]) return i;
    }
    return -1;
  }

  public void GetReard(int index)
  {
    if (index == 0) GameManager.Instance.IsShowNoticeDailyBouns = false;
    RewardData reward = initDailyBonus.GetRewardAt(index);
    EarnRandomCoins(reward);
    initDailyBonus.UpdateBounsData(index);
    Init();
  }

  public void GetRewardAds(int index)
  {
#if !UNITY_EDITOR
        LevelPlayAds.Instance.ShowRewardedAd(() =>
        {
            GetReard(index);
        }, "DailyBouns",
        () =>
        {
            LobbyPanel.Instance.ShowNotifyWith("ADS NOT READY");
        });
#else
    GetReard(index);
#endif
  }

  public void EarnRandomCoins(RewardData reward)
  {
    string bottomDescription;
    if (reward.Type == PriceType.Coin) bottomDescription = reward.Value[0].ToString();
    else if (reward.Type == PriceType.InfinityHeart) bottomDescription = $"{reward.Value[0]}m";
    else bottomDescription = $"x{reward.Value[0]}";
    ShowPanel.Instance
      .InjectPriceData(reward, ShowPanel.Instance.GlobalCoinImg.transform)
      .ShowImgWith(reward.Img, "", bottomDescription, Color.yellow, false);
    EarnPriceBy(reward);
  }

  void EarnPriceBy(RewardData priceData, int priceValueFactor = 1)
  {
    if (priceData.Type == PriceType.Coin)
      GameManager.Instance.CurrentCoin += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.Hammer)
      GameManager.Instance.CurrentHammer += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.Refresh)
      GameManager.Instance.CurrentRefresh += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.Rocket)
      GameManager.Instance.CurrentRocket += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.Swap)
      GameManager.Instance.CurrentSwap += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.Ticket)
      GameManager.Instance.CurrentTicket += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.TicketNoel)
      GameManager.Instance.CurrentTicketNoel += priceData.Value[0] * priceValueFactor;
    else if (priceData.Type == PriceType.InfinityHeart)
      HeartSystem.Instance.AddInfinityHeartTime(priceData.Value[0] * priceValueFactor);
  }
}
