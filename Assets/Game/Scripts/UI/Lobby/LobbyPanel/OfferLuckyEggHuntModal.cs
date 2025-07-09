using System;
using UnityEngine;

/// <summary>
/// OfferLuckyEggHuntModal
/// </summary> <summary>
/// 
/// </summary>
public partial class LobbyPanel : MonoBehaviour
{
  [Header("Lucky Offer Egg Hunt")]
  [SerializeField] RectTransform luckyEggHuntOfferModal;
  [SerializeField] RectTransform clockTimeRect;
  [SerializeField] LuckyEggHuntPackData[] luckyEggHuntPackDatas;

  private void InitLuckyEggHuntOfferModal()
  {
    if (luckyEggHuntOfferModal == null)
    {
      Debug.LogError("Lucky Egg Hunt Offer Modal is not assigned.");
      return;
    }

    UpdateClockTimeImg();
    luckyEggHuntOfferModal.gameObject.SetActive(false);
  }

  public void ClickBuyPackEasterBundle1()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_EASTERBUNDLE1, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotifyWith("PAYMENT ERROR");
        return;
      }

      ShowNotifyWith("PAYMENT SUCCEED");
      ClaimRewardEasterBundleAt(0);
    });
  }

  public void ClickBuyPackEasterBundle2()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_EASTERBUNDLE2, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotifyWith("PAYMENT ERROR");
        return;
      }

      ShowNotifyWith("PAYMENT SUCCEED");
      ClaimRewardEasterBundleAt(1);
    });
  }

  public void ClickBuyPackEasterBundle3()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_EASTERBUNDLE3, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotifyWith("PAYMENT ERROR");
        return;
      }

      ShowNotifyWith("PAYMENT SUCCEED");
      ClaimRewardEasterBundleAt(2);
    });
  }

  public void ClickBuyAllPackEasterBundle()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_EASTERCOMBO, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotifyWith("PAYMENT ERROR");
        return;
      }

      ShowNotifyWith("PAYMENT SUCCEED");
      for (int i = 0; i < luckyEggHuntPackDatas.Length; i++)
      {
        ClaimRewardEasterBundleAt(i);
      }
    });
  }

  public void ClickOpenLuckyEggHuntOfferModal()
  {
    OpenModal(luckyEggHuntOfferModal);
  }

  public void ClickCloseLuckyEggHuntOfferModal()
  {
    CloseModal(luckyEggHuntOfferModal);
  }

  private void ClaimRewardEasterBundleAt(int index)
  {
    var luckyEggHuntPackData = luckyEggHuntPackDatas[index];
    var rewards = luckyEggHuntPackData.Rewards;

    for (int i = 0; i < rewards.Length; i++)
    {
      var reward = rewards[i];
      switch (reward.OfferType)
      {
        case LuckyEggHuntOfferType.LuckyEggHunt:
          GameManager.Instance.MoveEggTicketsAmount += reward.Amount;
          break;
        case LuckyEggHuntOfferType.Coin:
          GameManager.Instance.CurrentCoin += reward.Amount;
          break;
        case LuckyEggHuntOfferType.Booster1:
          GameManager.Instance.CurrentRefresh += reward.Amount;
          break;
        case LuckyEggHuntOfferType.Booster2:
          GameManager.Instance.CurrentHammer += reward.Amount;
          break;
        case LuckyEggHuntOfferType.Booster3:
          GameManager.Instance.CurrentRocket += reward.Amount;
          break;
        case LuckyEggHuntOfferType.Booster4:
          GameManager.Instance.CurrentSwap += reward.Amount;
          break;
        case LuckyEggHuntOfferType.InfinityHeart:
          HeartSystem.Instance.AddInfinityHeartTime(reward.Amount);
          break;
      }
    }
  }

  private void UpdateClockTimeImg()
  {
    var isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.ios.is_unlock_full_event;

#if UNITY_ANDROID
    isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.android.is_unlock_full_event;
#endif 

    if (isUnlockFullEvent)
    {
      clockTimeRect?.gameObject.SetActive(false);
      return;
    }

    clockTimeRect?.gameObject.SetActive(true);
  }
}

public enum LuckyEggHuntOfferType
{
  LuckyEggHunt = 1,
  Coin = 2,
  Booster1 = 3,
  Booster2 = 4,
  Booster3 = 5,
  Booster4 = 6,
  InfinityHeart = 7
}

[Serializable]
public struct LuckyEggHuntPackData
{
  public LuckyEggHuntRewardData[] Rewards;
}

[Serializable]
public struct LuckyEggHuntRewardData
{
  public LuckyEggHuntOfferType OfferType;
  public int Amount;
}