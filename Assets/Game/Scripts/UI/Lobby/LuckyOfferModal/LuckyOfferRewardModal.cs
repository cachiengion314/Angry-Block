using System;
using DG.Tweening;
using Firebase.Analytics;
using HoangNam;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckyOfferRewardModal : MonoBehaviour
{
  [Header("Internal Dependencies")]
  [SerializeField] Button buyOfferBtn;
  [SerializeField] Sprite buyOfferCoolDown;
  [SerializeField] Sprite buyOfferReady;
  [SerializeField] TMP_Text timeTxt;
  [SerializeField] RectTransform contentReward;
  [SerializeField] TMP_Text coinRewardTxt;

  [Header("Reward Renderers")]
  [SerializeField] Sprite[] rewardSprites;

  private int _amountClaim;
  public int AmountClaim
  {
    get { return _amountClaim; }
    set
    {
      _amountClaim = value;
      PlayerPrefs.SetInt("AmountClaimLuckyReward", _amountClaim);
    }
  }

  private int _amountSkip;
  public int AmountSkip
  {
    get { return _amountSkip; }
    set
    {
      _amountSkip = value;
      PlayerPrefs.SetInt("AmountSkipLuckyReward", _amountSkip);
    }
  }

  private const float TIME_MIN_CHANGE_REWARD = 1800;

  private bool _isCoolDownOffer = true;

  private void Start()
  {
    AmountClaim = PlayerPrefs.GetInt("AmountClaimLuckyReward", 0);
    AmountSkip = PlayerPrefs.GetInt("AmountSkipLuckyReward", 0);

    UpdateBtnOffer();
    UpdateRewardUI();

    GameManager.Instance.onTimeLuckyOfferChanged += UpdateTimeCoolDown;
    FirebaseSetup.onUpdateRemote += UpdateRewardUI;
  }

  private void OnDestroy()
  {
    GameManager.Instance.onTimeLuckyOfferChanged -= UpdateTimeCoolDown;
    FirebaseSetup.onUpdateRemote -= UpdateRewardUI;
  }

  private void Update()
  {
    if (!_isCoolDownOffer) return;
    if (!GameManager.Instance.IsUnlockLuckyReward()) return;

    UpdateTimeCoolDown(GameManager.Instance.TimeCoolDownLuckyOffer);
  }

  public void ClickBuyBtn()
  {
    if (_isCoolDownOffer)
    {
      LevelPlayAds.Instance.ShowRewardedAd(
        () =>
        {
          _isCoolDownOffer = false;
          AmountSkip++;
          GameManager.Instance.ClearTimeLuckyOffer();
          timeTxt.text = GameManager.Instance.FormatTimeLuckyOffer();
          UpdateBtnOffer();

          if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
          {
            FirebaseAnalytics.LogEvent(
              "lucky_offer_reward",
                new Parameter[]{
                  new("skipped", AmountSkip)
                }
              );
          }
        },
        "Click_Buy_Lucky_Offer",
        () =>
        {
          LobbyPanel.Instance.ShowNotifyWith("ADS NOT READY");
        }
      );

      return;
    }

    LevelPlayAds.Instance.ShowRewardedAd(
      () =>
      {
        _isCoolDownOffer = true;
        AmountClaim++;
        ClaimReward();
        GameManager.Instance.AddClaimLuckyOffer();
        GameManager.Instance.ResetTimeLuckyOffer();
        UpdateBtnOffer();
        UpdateRewardUI();
        ClickClose();

        if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
        {
          FirebaseAnalytics.LogEvent(
            "lucky_offer_reward",
            new Parameter[]{
                new("claim", AmountClaim)
            }
          );
        }
      },
      "Click_Buy_Lucky_Offer_Claim",
      () =>
      {
        LobbyPanel.Instance.ShowNotifyWith("ADS NOT READY");
      }
    );
  }

  private void UpdateTimeCoolDown(float seconds)
  {
    if (seconds <= 0)
    {
      _isCoolDownOffer = false;
      UpdateBtnOffer();
      return;
    }

    timeTxt.text = GameManager.Instance.FormatTimeLuckyOffer();
    _isCoolDownOffer = true;
    UpdateBtnOffer();
  }

  private void UpdateBtnOffer()
  {
    if (_isCoolDownOffer)
    {
      buyOfferBtn.image.sprite = buyOfferCoolDown;
      return;
    }

    buyOfferBtn.image.sprite = buyOfferReady;
  }

  private void UpdateRewardUI()
  {
    var timeReset = GameManager.Instance.CalculateTimeReset();
    var luckyOfferDatas = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_reward.ios.luckyOfferDatas;

#if UNITY_ANDROID
    luckyOfferDatas = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_reward.android.luckyOfferDatas;
#endif

    var luckyOfferData = timeReset >= TIME_MIN_CHANGE_REWARD ? luckyOfferDatas[1] : luckyOfferDatas[0];

    coinRewardTxt.text = "x" + luckyOfferData.RewardDatas[0].Amount.ToString();

    for (int i = 0; i < contentReward.childCount; i++)
    {
      var luckyReward = contentReward.GetChild(i).GetComponent<LuckyOfferRewardControl>();
      var rewardSprite = GetRewardRendererWith(luckyOfferData.RewardDatas[i + 1].RewardType);

      luckyReward.SetUIWith(rewardSprite, luckyOfferData.RewardDatas[i + 1]);
    }
  }

  private void ClaimReward()
  {
    var timeReset = GameManager.Instance.CalculateTimeReset();
    var luckyOfferDatas = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_reward.ios.luckyOfferDatas;

#if UNITY_ANDROID
    luckyOfferDatas = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.lucky_reward.android.luckyOfferDatas;
#endif

    var luckyOfferData = timeReset >= TIME_MIN_CHANGE_REWARD ? luckyOfferDatas[1] : luckyOfferDatas[0];

    ClaimLuckyOfferRewardFrom(luckyOfferData);
  }

  private void ClaimLuckyOfferRewardFrom(LuckyOfferData luckyOfferNeedClaim)
  {
    string reward = "";

    foreach (var luckyReward in luckyOfferNeedClaim.RewardDatas)
    {
      if (luckyReward.RewardType == PriceType.Coin)
        GameManager.Instance.CurrentCoin += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.Hammer)
        GameManager.Instance.CurrentHammer += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.Refresh)
        GameManager.Instance.CurrentRefresh += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.Rocket)
        GameManager.Instance.CurrentRocket += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.Swap)
        GameManager.Instance.CurrentSwap += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.Ticket)
        GameManager.Instance.CurrentTicket += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.TicketNoel)
        GameManager.Instance.CurrentTicketNoel += luckyReward.Amount;
      else if (luckyReward.RewardType == PriceType.InfinityHeart)
        HeartSystem.Instance.AddInfinityHeartTime(luckyReward.Amount);

      GameManager.Instance.AddLuckyOfferData(luckyReward);

      reward += "_Type: " + luckyReward.RewardType + "_Amount: " + luckyReward.Amount + "--------";
    }

    Utility.Print("Reward: " + reward);
  }

  private Sprite GetRewardRendererWith(PriceType type)
  {
    return rewardSprites[(int)type];
  }

  public void ClickClose()
  {
    LobbyPanel.Instance.ToggleOfferRewardModal();

    DOVirtual.DelayedCall(
      0.4f,
      () =>
      {
        GameManager.Instance.VisualizeLuckyOffer();
      }
    );
  }
}

[Serializable]
public struct LuckyOfferData
{
  public LuckyOfferRewardData[] RewardDatas;
}

[Serializable]
public struct LuckyOfferRewardData
{
  public int Amount;
  public PriceType RewardType;
}
