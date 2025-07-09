using System;
using System.Collections.Generic;
using Firebase.Analytics;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Meta Progress
/// </summary> <summary>
/// 
/// </summary>
public partial class GameManager : MonoBehaviour
{
  [Header("Setting MetaProgress Lobby")]
  [SerializeField] int[] progressTargets;
  [SerializeField] MetaProgressRewardData[] metaProgressRewardDatas;
  [SerializeField] RewardData[] rewardDatas;

  public RewardData GetRewardStreakAt(int index)
  {
    if (index >= rewardDatas.Length) return rewardDatas[^1];
    return rewardDatas[index];
  }

  public int GetCurrentProgressTarget()
  {
    if (CurrentMilestone >= metaProgressRewardDatas.Length) return progressTargets[^1];

    return progressTargets[CurrentMilestone];
  }

  public void ResetEvent()
  {
    CurrentMetaProgressTotal = 0;
    MetaProgressNeedClaim = 0;
    CurrentMetaStreak = 0;
    NextMetaStreak = 0;
    CurrentMilestone = 0;
  }

  private int _currentMetaProgressTotal;
  public int CurrentMetaProgressTotal
  {
    get { return _currentMetaProgressTotal; }
    set
    {
      _currentMetaProgressTotal = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_METAPROGRESS_TOTAL, _currentMetaProgressTotal);
    }
  }

  private int _metaProgressNeedClaim;
  public int MetaProgressNeedClaim
  {
    get { return _metaProgressNeedClaim; }
    set
    {
      _metaProgressNeedClaim = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_METAPROGRESS_NEEDCLAIM, _metaProgressNeedClaim);
    }
  }

  private int _currentMetaStreak;
  public int CurrentMetaStreak
  {
    get { return _currentMetaStreak; }
    set
    {
      _currentMetaStreak = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_METASTREAK, _currentMetaStreak);
    }
  }

  private int _nextMetaStreak;
  public int NextMetaStreak
  {
    get { return _nextMetaStreak; }
    set
    {
      _nextMetaStreak = value;
      PlayerPrefs.SetInt(KeyString.KEY_NEXT_METASTREAK, _nextMetaStreak);
    }
  }

  private int _currentMilestone;
  public int CurrentMilestone
  {
    get { return _currentMilestone; }
    set
    {
      _currentMilestone = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_MILESTONE, _currentMilestone);
    }
  }

  private const int META_POINT = 1; // point can claim when win 1 level (not scale yet)
  private const int LEVEL_UNLOCK_METAPROGERSS_EVENT = 11;

  private int[] _scaleStreaks = new int[5] { 1, 5, 10, 25, 100 };
  public int[] ScaleStreaks { get { return _scaleStreaks; } }

  public bool IsEvent => CurrentLevel >= LEVEL_UNLOCK_METAPROGERSS_EVENT;

  public bool IsMetaProgressHappenning()
  {
    if (CurrentLevel < LEVEL_UNLOCK_METAPROGERSS_EVENT) return false;
    if (_currentMilestone >= progressTargets.Length)
    {
      Utility.Print("Claimed all reward");
      return false;
    }

    return true;
  }

  private void InitMetaProgressProperty()
  {
    CurrentMetaProgressTotal = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_METAPROGRESS_TOTAL, 0);
    MetaProgressNeedClaim = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_METAPROGRESS_NEEDCLAIM, 0);
    CurrentMetaStreak = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_METASTREAK, 0);
    NextMetaStreak = PlayerPrefs.GetInt(KeyString.KEY_NEXT_METASTREAK, 0);
    CurrentMilestone = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_MILESTONE, 0);
  }

  public void IncreaseMetaStreak()
  {
    _nextMetaStreak = _currentMetaStreak + 1;
    NextMetaStreak = math.min(_nextMetaStreak, _scaleStreaks.Length - 1);
  }

  public void ResetMetaStreak()
  {
    CurrentMetaStreak = 0;
    NextMetaStreak = 0;
  }

  public void CalculateTotalProgressNeedClaim()
  {
    MetaProgressNeedClaim = META_POINT * _scaleStreaks[_currentMetaStreak];
  }

  public void PlayAnimClaimRewardMeta()
  {
    if (_metaProgressNeedClaim == 0)
    {
      Utility.Print("Don't have meta progress need claim");
      return;
    }

    LobbyPanel.Instance.PanelFullScreen.gameObject.SetActive(true);
    Utility.Print("Have meta progress run");
    List<MetaProgressRewardData> _metaRewardNeedClaims = new(); // claim current
    List<AnimMetaProgressData> _animMetaProgressDatas = new();
    List<RewardData> _rewardNeedClaims = new();
    List<RewardData> _rewardNextClaims = new(); // reward that can claim next

    var metaNeedClaim = _metaProgressNeedClaim;
    var currentMetaProgressTotal = _currentMetaProgressTotal;
    bool _hasEndEvent = false;
    bool _hasClaimAnyReward = false;
    _currentMetaProgressTotal += _metaProgressNeedClaim;

    while (_currentMetaProgressTotal >= progressTargets[CurrentMilestone])
    {
      Utility.Print("Have meta progress run and have reward need claim");
      _hasClaimAnyReward = true;
      _currentMetaProgressTotal -= progressTargets[CurrentMilestone];
      _metaRewardNeedClaims.Add(metaProgressRewardDatas[CurrentMilestone]);

      _rewardNeedClaims.Add(rewardDatas[CurrentMilestone]);
      if (CurrentMilestone + 1 < progressTargets.Length)
        _rewardNextClaims.Add(rewardDatas[CurrentMilestone + 1]);


      _animMetaProgressDatas.Add(
        new AnimMetaProgressData()
        {
          StartValue = currentMetaProgressTotal,
          EndValue = progressTargets[CurrentMilestone],
          Target = progressTargets[CurrentMilestone],
          AnimMetaProgressType = AnimMetaProgressType.ClaimReward
        }
      );

      currentMetaProgressTotal = 0;
      CurrentMilestone++;

      if (CurrentMilestone == progressTargets.Length)
      {
        Utility.Print("Limit reward and progress will disappear");
        _currentMetaProgressTotal = progressTargets[CurrentMilestone - 1];
        _hasEndEvent = true;

        _animMetaProgressDatas.Add(
          new AnimMetaProgressData()
          {
            StartValue = progressTargets[CurrentMilestone - 1],
            EndValue = progressTargets[CurrentMilestone - 1],
            Target = progressTargets[CurrentMilestone - 1],
            AnimMetaProgressType = AnimMetaProgressType.EndEvent
          }
        );
        break;
      }
    }

    if (_currentMetaProgressTotal > 0 && !_hasEndEvent)
    {
      Utility.Print("Have meta progress run but don't have reward");

      var startValue = 0;
      if (!_hasClaimAnyReward)
      {
        Utility.Print("Have meta progress run, don't have reward and not yet claim any reward");
        startValue = _currentMetaProgressTotal - _metaProgressNeedClaim;
        Utility.Print("startValue: " + startValue);
      }

      _animMetaProgressDatas.Add(
        new AnimMetaProgressData()
        {
          StartValue = startValue,
          EndValue = _currentMetaProgressTotal,
          Target = progressTargets[CurrentMilestone],
          AnimMetaProgressType = AnimMetaProgressType.None
        }
      );
    }

    foreach (var metaReward in _metaRewardNeedClaims)
    {
      Utility.Print("------" + metaReward.RewardDatas[0].Value[0] + "" + metaReward.RewardDatas[0].Type);
    }

    CurrentMetaProgressTotal = _currentMetaProgressTotal;
    MetaProgressNeedClaim = 0;
    CurrentMetaStreak = _nextMetaStreak;

    ClaimMetaProgressRewardsFrom(_metaRewardNeedClaims);
    VisualizeMetaProgressReward(_rewardNeedClaims, _rewardNextClaims, _animMetaProgressDatas, metaNeedClaim);

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(
        "meta_progress_event",
        new Parameter[]{
          new("streak", _scaleStreaks[CurrentMetaStreak]),
          new("milestone", CurrentMilestone),
          new("level", CurrentLevel + 1)
        }
      );
    }

    Utility.Print("meta_progress_event: "
      + "_streak: " + _scaleStreaks[CurrentMetaStreak]
      + "_milestone: " + CurrentMilestone
      + "level" + (CurrentLevel + 1));
  }

  private void ClaimMetaProgressRewardsFrom(List<MetaProgressRewardData> _metaRewardNeedClaims)
  {
    foreach (var metaNeedClaim in _metaRewardNeedClaims)
    {
      ClaimMetaProgressRewardFrom(metaNeedClaim);
    }
  }

  private void ClaimMetaProgressRewardFrom(MetaProgressRewardData metaRewardNeedClaim)
  {
    foreach (var metaReward in metaRewardNeedClaim.RewardDatas)
    {
      if (metaReward.Type == PriceType.Coin)
        CurrentCoin += metaReward.Value[0];
      else if (metaReward.Type == PriceType.Hammer)
        CurrentHammer += metaReward.Value[0];
      else if (metaReward.Type == PriceType.Refresh)
        CurrentRefresh += metaReward.Value[0];
      else if (metaReward.Type == PriceType.Rocket)
        CurrentRocket += metaReward.Value[0];
      else if (metaReward.Type == PriceType.Swap)
        CurrentSwap += metaReward.Value[0];
      else if (metaReward.Type == PriceType.Ticket)
        CurrentTicket += metaReward.Value[0];
      else if (metaReward.Type == PriceType.TicketNoel)
        CurrentTicketNoel += metaReward.Value[0];
      else if (metaReward.Type == PriceType.InfinityHeart)
        HeartSystem.Instance.AddInfinityHeartTime(metaReward.Value[0]);
    }
  }
}

[Serializable]
public struct MetaProgressRewardData
{
  public RewardData[] RewardDatas;
}

public enum AnimMetaProgressType
{
  None,
  ClaimReward,
  EndEvent
}

public struct AnimMetaProgressData
{
  public int StartValue;
  public int EndValue;
  public int Target;
  public AnimMetaProgressType AnimMetaProgressType;
}