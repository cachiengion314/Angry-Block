using System;
using HoangNam;
using TMPro;
using UnityEngine;

public class BoosterTripleOfferModal : MonoBehaviour
{
  [Header("Setting Rewards")]
  [SerializeField] BoosterTripleOfferData[] boosterTripleOfferDatas;

  [Header("TMP_Text Pack1")]
  [SerializeField] TMP_Text[] itemPack1Txts;

  [Header("TMP_Text Pack2")]
  [SerializeField] TMP_Text[] itemPack2Txts;

  [Header("TMP_Text Pack3")]
  [SerializeField] TMP_Text[] itemPack3Txts;

  private void Start()
  {
    UpdateUIModal();
  }

  private void UpdateUIModal()
  {
    UpdateTxtIn(itemPack1Txts, boosterTripleOfferDatas[0].RewardDatas);
    UpdateTxtIn(itemPack2Txts, boosterTripleOfferDatas[1].RewardDatas);
    UpdateTxtIn(itemPack3Txts, boosterTripleOfferDatas[2].RewardDatas);
  }

  private void UpdateTxtIn(TMP_Text[] txts, BoosterTripleOfferRewardData[] rewardDatas)
  {
    for (int i = 0; i < txts.Length; i++)
    {
      txts[i].text = "x" + rewardDatas[i].Amount;
    }
  }

  public void BuyPack1()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_BOOSTERTRIPLEOFFER1, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {

        return;
      }


      BuyPackOfferAt(0);
    });
  }

  public void BuyPack2()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_BOOSTERTRIPLEOFFER2, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {

        return;
      }


      BuyPackOfferAt(1);
    });
  }

  public void BuyPack3()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_BOOSTERTRIPLEOFFER3, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        return;
      }

      BuyPackOfferAt(2);
    });
  }

  private void BuyPackOfferAt(int index)
  {
    ClaimLuckyOfferRewardFrom(boosterTripleOfferDatas[index]);
  }

  private void ClaimLuckyOfferRewardFrom(BoosterTripleOfferData boosterTripOfferData)
  {
    string reward = "";

    foreach (var boosterTripOffer in boosterTripOfferData.RewardDatas)
    {
      if (boosterTripOffer.RewardType == PriceType.Coin)
        GameManager.Instance.CurrentCoin += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.Hammer)
        GameManager.Instance.CurrentHammer += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.Refresh)
        GameManager.Instance.CurrentRefresh += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.Rocket)
        GameManager.Instance.CurrentRocket += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.Swap)
        GameManager.Instance.CurrentSwap += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.Ticket)
        GameManager.Instance.CurrentTicket += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.TicketNoel)
        GameManager.Instance.CurrentTicketNoel += boosterTripOffer.Amount;
      else if (boosterTripOffer.RewardType == PriceType.InfinityHeart)
        HeartSystem.Instance.AddInfinityHeartTime(boosterTripOffer.Amount);

      reward += "_Type: " + boosterTripOffer.RewardType + "_Amount: " + boosterTripOffer.Amount + "--------";
    }

    Utility.Print("Reward: " + reward);
  }

  public void ClickClose()
  {
    BalloonSystem.Instance.ShowBalloon();
  }
}

[Serializable]
public struct BoosterTripleOfferData
{
  public BoosterTripleOfferRewardData[] RewardDatas;
}

[Serializable]
public struct BoosterTripleOfferRewardData
{
  public int Amount;
  public PriceType RewardType;
}
