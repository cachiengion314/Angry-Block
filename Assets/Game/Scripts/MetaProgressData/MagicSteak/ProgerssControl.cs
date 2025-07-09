using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgerssControl : MonoBehaviour
{
  [SerializeField] Slider progerssBar;
  [SerializeField] TextMeshProUGUI progerssTxt;
  [SerializeField] Image item;
  [SerializeField] TMP_Text amountItemTxt;

  public Transform GetItem()
  {
    return item.transform;
  }

  public void SetImage(RewardData data)
  {
    item.sprite = data.Img;

    var type = data.Type;
    string amountTxt = "";

    if (data.Type == PriceType.InfinityHeart)
    {
      var time = data.Value[0];

      if (time < 60) amountTxt = time + "m";
      else amountTxt = (time / 60f) + "h";
    }
    else if (data.Type == PriceType.Coin)
    {
      amountTxt = data.Value[0].ToString();
    }
    else
    {
      amountTxt = "x" + data.Value[0];
    }

    amountItemTxt.text = amountTxt;
  }

  public void SetValueProgerssBar(float CurrentValue, float TotalValue)
  {
    float value = CurrentValue / TotalValue;
    progerssBar.value = value;
  }

  public void SetTextProgerssBar(int CurrentValue, int TotalValue)
  {
    progerssTxt.text = $"{CurrentValue}/{TotalValue}";
  }
}
