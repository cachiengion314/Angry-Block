using System;
using UnityEngine;

/// <summary>
/// Time Lucky Offer
/// </summary> <summary>
/// 
/// </summary>
public partial class GameManager : MonoBehaviour
{
  public Action<float> onTimeLuckyOfferChanged;

  private float _timeCoolDownLuckyOffer;
  public float TimeCoolDownLuckyOffer
  {
    get { return _timeCoolDownLuckyOffer; }
    set
    {
      _timeCoolDownLuckyOffer = value;
      PlayerPrefs.SetFloat(KeyString.KEY_TIME_COOLDOWN_LUCKYOFFER, _timeCoolDownLuckyOffer);
    }
  }

  private int _currentClaimLuckyOffer;
  public int CurrentClaimLuckyOffer
  {
    get { return _currentClaimLuckyOffer; }
    set
    {
      _currentClaimLuckyOffer = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_CLAIM_LUCKYOFFER, _currentClaimLuckyOffer);
    }
  }

  private const float TIME_COOLDOWN_LUCKYOFFER_DEFAULT = 600; // seconds
  private const float DELTA_TIME_COOLDOWN_LUCKYOFFER = 300; // seconds

  private bool _isStartCoolDown = false;

  private void InitParameterLuckyOffer()
  {
    TimeCoolDownLuckyOffer = PlayerPrefs.GetFloat(KeyString.KEY_TIME_COOLDOWN_LUCKYOFFER, TIME_COOLDOWN_LUCKYOFFER_DEFAULT);
    CurrentClaimLuckyOffer = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_CLAIM_LUCKYOFFER, 0);

    _isStartCoolDown = true;
  }

  private void CalculateTimeLuckyOffer()
  {
    if (!_isStartCoolDown) return;
    if (!IsUnlockLuckyReward()) return;
    if (TimeCoolDownLuckyOffer <= 0) return;

    TimeCoolDownLuckyOffer -= Time.deltaTime;
    if (TimeCoolDownLuckyOffer <= 0)
    {
      TimeCoolDownLuckyOffer = 0;
    }

    onTimeLuckyOfferChanged?.Invoke(_timeCoolDownLuckyOffer);
  }

  public void AddClaimLuckyOffer()
  {
    CurrentClaimLuckyOffer++;
  }

  public void ClearTimeLuckyOffer()
  {
    TimeCoolDownLuckyOffer = 0;
    onTimeLuckyOfferChanged?.Invoke(0);
  }

  public void ResetTimeLuckyOffer()
  {
    TimeCoolDownLuckyOffer = CalculateTimeReset();
  }

  public float CalculateTimeReset()
  {
    return TIME_COOLDOWN_LUCKYOFFER_DEFAULT + _currentClaimLuckyOffer * DELTA_TIME_COOLDOWN_LUCKYOFFER;
  }

  private void ResetTimeLuckyOfferNewDay()
  {
    CurrentClaimLuckyOffer = 0;
    ResetTimeLuckyOffer();
  }

  public string FormatTimeLuckyOffer()
  {
    TimeSpan time = TimeSpan.FromSeconds(_timeCoolDownLuckyOffer);
    var timeOffer = ((int)time.TotalHours).ToString("00") + "h" + time.Minutes.ToString("00") + "m" + time.Seconds.ToString("00") + "s";

    return timeOffer;
  }
}