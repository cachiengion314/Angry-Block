using UnityEngine;
using UnityEngine.UI;

public class SpecialDayBlock : MonoBehaviour, IDayBlock
{
  [SerializeField] Image day7plusBg;
  [SerializeField] Color gray1Color;
  [SerializeField] Color gray2Color;
  [SerializeField] Image tickImg;
  [SerializeField] Image rewardImg;
  [SerializeField] RectTransform lightEffectRect;

  public void InjectRewardImg(Sprite sprite)
  {
    rewardImg.sprite = sprite;
  }

  public void ShowAlreadyActivatedDay()
  {
    day7plusBg.color = gray1Color;
    tickImg.gameObject.SetActive(true);
    lightEffectRect.gameObject.SetActive(false);
  }

  public void ShowToday()
  {
    day7plusBg.color = Color.white;
    tickImg.gameObject.SetActive(false);
    lightEffectRect.gameObject.SetActive(true);
  }

  public void ShowFollowingDay()
  {
    day7plusBg.color = Color.white;
    tickImg.gameObject.SetActive(false);
    lightEffectRect.gameObject.SetActive(false);
  }

  public void ShowTomorrow()
  {
    day7plusBg.color = Color.white;
    tickImg.gameObject.SetActive(false);
    lightEffectRect.gameObject.SetActive(false);
  }

  public void ShowTodayActivated()
  {
    day7plusBg.color = gray1Color;
    tickImg.gameObject.SetActive(true);
    lightEffectRect.gameObject.SetActive(false);
  }
}
