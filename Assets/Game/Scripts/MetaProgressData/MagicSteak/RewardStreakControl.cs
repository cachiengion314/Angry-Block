using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardStreakControl : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI CurrentStreakTxt;
  [SerializeField] TextMeshProUGUI CurrentStreak1Txt;
  [SerializeField] GameObject lockLevel;
  [SerializeField] GameObject selectLevel;
  [SerializeField] Image item;
  [SerializeField] TextMeshProUGUI amountTxt;
  [SerializeField] GameObject lockReward;

  public void Lock()
  {
    lockReward.SetActive(true);
    lockLevel.SetActive(true);
    selectLevel.SetActive(false);
  }

  public void Unlock()
  {
    lockReward.SetActive(false);
    lockLevel.SetActive(false);
    selectLevel.SetActive(true);
  }

  public void SetTxtCurrentStreak(int currentStreak)
  {
    CurrentStreakTxt.text = currentStreak.ToString();
    CurrentStreak1Txt.text = currentStreak.ToString();
  }

  public void SetImage(RewardData data)
  {
    item.sprite = data.Img;
    if (data.Value.Length == 0)
    {
      amountTxt.text = "";
    }
    else
    {
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

      this.amountTxt.text = amountTxt;
    }
  }
}
