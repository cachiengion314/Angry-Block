using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayBlock : MonoBehaviour, IDayBlock
{
  [Header("Injected Dependencies")]
  [SerializeField] Image bgImg;
  [SerializeField] TMP_Text dayText;
  [SerializeField] Image lightImg;
  [SerializeField] TMP_Text cointText;
  [SerializeField] Image tickImg;
  [SerializeField] Image maskImg;
  [SerializeField] Image frameImg;
  [SerializeField] Sprite normalBoard;
  [SerializeField] Sprite activeBoard;
  [SerializeField] Image rewardImg;
  [SerializeField] Color yellow;
  [SerializeField] Color crimson;
  [SerializeField] RectTransform lightEffectRect;

  public DailyRewardModal dailyRewardModal;
  public DayRewardData dayRewardData;
  public DayData dayData;

  public void InjectDayRewardData(DayRewardData dayRewardData)
  {
    this.dayRewardData = dayRewardData;
  }

  public void InjectDayData(DayData dayData)
  {
    this.dayData = dayData;
  }

  public void InjectRewardImg(Sprite sprite)
  {
    rewardImg.sprite = sprite;
  }

  public void InjectDailyRewardModal(DailyRewardModal dailyRewardModal)
  {
    this.dailyRewardModal = dailyRewardModal;
  }

  internal void ShowDefault()
  {
    gameObject.SetActive(true);
    cointText.text = dayData.CoinValue[0].ToString();
    dayText.text = "DAY " + (dayData.DayIndex + 1).ToString();
    lightEffectRect.gameObject.SetActive(false);
  }

  public void ShowAlreadyActivatedDay()
  {
    ShowDefault();
    bgImg.sprite = normalBoard;
    tickImg.gameObject.SetActive(true);
    lightImg.gameObject.SetActive(false);
    maskImg.gameObject.SetActive(true);
    dayText.color = yellow;
  }

  public void ShowToday()
  {
    ShowDefault();
    bgImg.sprite = normalBoard;
    tickImg.gameObject.SetActive(false);
    lightImg.gameObject.SetActive(false);
    maskImg.gameObject.SetActive(false);
    dayText.color = crimson;
    dayText.text = "DAY " + (dayData.DayIndex + 1).ToString();
    lightEffectRect.gameObject.SetActive(true);
  }

  public void ShowTodayActivated()
  {
    ShowDefault();
    bgImg.sprite = normalBoard;
    tickImg.gameObject.SetActive(true);
    lightImg.gameObject.SetActive(false);
    maskImg.gameObject.SetActive(true);
    dayText.color = crimson;
    dayText.text = "DAY " + (dayData.DayIndex + 1).ToString();
  }

  public void ShowFollowingDay()
  {
    ShowDefault();
    bgImg.sprite = normalBoard;
    tickImg.gameObject.SetActive(false);
    lightImg.gameObject.SetActive(false);
    maskImg.gameObject.SetActive(false);
    dayText.color = yellow;
  }

  public void ShowTomorrow()
  {
    ShowDefault();
    bgImg.sprite = normalBoard;
    tickImg.gameObject.SetActive(false);
    lightImg.gameObject.SetActive(true);
    maskImg.gameObject.SetActive(false);
    dayText.color = crimson;
    dayText.text = "DAY " + (dayData.DayIndex + 1).ToString();
  }

  public void ShowMulText(){
    cointText.text = "x" + dayData.CoinValue[0].ToString();
  }
}
