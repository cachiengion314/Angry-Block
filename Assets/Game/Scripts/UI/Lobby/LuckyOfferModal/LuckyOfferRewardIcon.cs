using TMPro;
using UnityEngine;

public class LuckyOfferRewardIcon : MonoBehaviour
{
  [Header("Internal Dependencies")]
  [SerializeField] RectTransform noticeLuckyReward;
  [SerializeField] TMP_Text timeTxt;

  void Start()
  {
    GameManager.Instance.onTimeLuckyOfferChanged += UpdateTimeCoolDown;

    if (!GameManager.Instance.IsUnlockLuckyReward())
    {
      timeTxt.text = GameManager.Instance.FormatTimeLuckyOffer();
      noticeLuckyReward.gameObject.SetActive(false);
      return;
    }
  }

  void OnDestroy()
  {
    GameManager.Instance.onTimeLuckyOfferChanged -= UpdateTimeCoolDown;
  }

  private void UpdateTimeCoolDown(float seconds)
  {
    timeTxt.text = GameManager.Instance.FormatTimeLuckyOffer();

    if (seconds <= 0)
    {
      noticeLuckyReward.gameObject.SetActive(true);
      return;
    }

    noticeLuckyReward.gameObject.SetActive(false);
  }
}
