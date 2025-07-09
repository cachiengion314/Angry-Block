using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LuckyOfferRewardControl : MonoBehaviour
{
  [Header("Components")]
  [SerializeField] Image bodyImg;
  [SerializeField] TMP_Text amountTxt;

  public void SetUIWith(Sprite bodyImg, LuckyOfferRewardData luckyOfferRewardData)
  {
    var amount = luckyOfferRewardData.Amount;
    var type = luckyOfferRewardData.RewardType;

    this.bodyImg.sprite = bodyImg;
    this.bodyImg.SetNativeSize();

    if (type == PriceType.InfinityHeart)
    {
      amountTxt.text = amount + "min";
    }
    else
    {
      amountTxt.text = "x" + amount;
    }
  }
}
