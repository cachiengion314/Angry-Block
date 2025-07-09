using DG.Tweening;
using Firebase.Analytics;
using Spine.Unity;
using TMPro;
using UnityEngine;

/// <summary>
/// Offer Reward Modal
/// </summary> <summary>
/// 
/// </summary>
public partial class LobbyPanel : MonoBehaviour
{
  [Header("Offer Reward Modal")]
  [SerializeField] RectTransform offerRewardModal;
  [SerializeField] SkeletonGraphic skePlayBtn;
  public SkeletonGraphic SkePlayBtn { get { return skePlayBtn; } }

  [SerializeField] RectTransform offerRewardLock;
  [SerializeField] SkeletonGraphic skeLuckyReward;

  private int _amountOpenLuckyReward;
  public int AmountOpenLuckyReward
  {
    get
    {
      return _amountOpenLuckyReward;
    }
    set
    {
      _amountOpenLuckyReward = value;
      PlayerPrefs.SetInt("Amount_Open_LuckyReward", _amountOpenLuckyReward);
    }
  }

  private void InitOfferRewardModal()
  {
    offerRewardModal.gameObject.SetActive(false);
    AmountOpenLuckyReward = PlayerPrefs.GetInt("Amount_Open_LuckyReward", 0);

    if (!GameManager.Instance.IsUnlockLuckyReward())
    {
      offerRewardLock.GetComponentInChildren<TMP_Text>().text = "Lv." + GameManager.Instance.LevelUnlockLuckyReward;
      offerRewardLock.gameObject.SetActive(true);
    }
    else
    {
      offerRewardLock.gameObject.SetActive(false);
      skeLuckyReward.AnimationState.SetAnimation(0, "animation", true);
    }
  }

  public void ToggleOfferRewardModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!GameManager.Instance.IsUnlockLuckyReward())
    {
      ShowNotifyWith("UNLOCK AT LEVEL " + GameManager.Instance.LevelUnlockLuckyReward);
      return;
    }

    if (!offerRewardModal.gameObject.activeSelf)
    {
      AmountOpenLuckyReward++;

      OpenModal(offerRewardModal);
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(
          "lucky_offer_reward",
            new Parameter[]{
              new("open", AmountOpenLuckyReward)
            }
          );
      }
    }
    else
    {
      CloseModal(offerRewardModal);
    }
  }

  public void VisualizeOfferReward(
    ref Sequence seq,
    ref float currentTimeDuration,
    LuckyOfferRewardData luckyOfferRewardData
  )
  {
    var rewardType = luckyOfferRewardData.RewardType;
    var amount = luckyOfferRewardData.Amount;
    var timeShowReward = 0.6f;
    var timeMoveReward = 0.5f;

    if (rewardType == PriceType.Coin) timeMoveReward = 1.5f;

    seq.InsertCallback(
      currentTimeDuration,
      () =>
      {

      }
    );

    currentTimeDuration += timeShowReward;

    seq.InsertCallback(
      currentTimeDuration,
      () =>
      {
        // darkPanel.gameObject.SetActive(false);
      }
    );

    currentTimeDuration += timeMoveReward;
  }

  private string GetDescriptionFrom(LuckyOfferRewardData luckyOfferRewardData)
  {

    return "";
  }
}