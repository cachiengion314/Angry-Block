using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftCtrl : MonoBehaviour
{
    [SerializeField] Button giftBtn;
    [SerializeField] GameObject giftReceived;
    [SerializeField] Image giftIdel;
    [SerializeField] GameObject tickImg;
    [SerializeField] TextMeshProUGUI spinAmount;
    
    public RewardData rewardData;
    public int spinMax;
    public void UpdateProgress(int amount)
    {
        spinAmount.text = $"{amount}/{spinMax}";
    }
    public void Received()
    {
        giftIdel.gameObject.SetActive(true);
        tickImg.SetActive(true);
        giftReceived.SetActive(false);
        giftBtn.interactable = false;
    }

    public void NotReceived()
    {
        giftIdel.gameObject.SetActive(true);
        tickImg.SetActive(false);
        giftReceived.SetActive(false);
        giftBtn.interactable = false;
    }

    public void ReachMilestone()
    {
        giftIdel.gameObject.SetActive(false);
        tickImg.SetActive(false);
        giftReceived.SetActive(true);
        giftBtn.interactable = true;
    }

    public void Receive()
    {
        Received();
        TryEarnPrice();
    }

    void TryEarnPrice()
    {
        EarnPriceBy(rewardData);
        if (rewardData.Type == PriceType.InfinityHeart)
        {
            ShowPanel.Instance.ShowImgWith(
              rewardData.Img, "",
              rewardData.Value[0] + "m"
              , Color.yellow, false
            );
            return;
        }

        if (rewardData.Type == PriceType.Coin)
        {
            ShowPanel.Instance.ShowImgWith(
              rewardData.Img, "",
              "" + rewardData.Value[0]
              , Color.yellow, false
            );
            return;
        }


        ShowPanel.Instance.ShowImgWith(
          rewardData.Img, "",
          "x" + rewardData.Value[0]
          , Color.yellow, false
        );
    }

    void EarnPriceBy(RewardData priceData, int priceValueFactor = 1)
    {
        if (priceData == null) return;
        Debug.Log(priceData.Type);
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
