using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LobbyPanel : MonoBehaviour
{
  [Header("Notice LiveOps")]
  [SerializeField] RectTransform iconNoticeDailyReward;
  [SerializeField] RectTransform iconNoticeLuckyWheel;
  [SerializeField] RectTransform iconNoticeFreeCoin;
  [SerializeField] RectTransform iconNoticeFreeCoinTab;
  [SerializeField] RectTransform iconNoticeFreeCoinInShop;
  [SerializeField] RectTransform iconNoticeLuckySpin;
  [SerializeField] RectTransform iconNoticeLuckyEggHunt;

  private void InitNoticeLuckyWheel()
  {
    if (GameManager.Instance.CurrentTicket > 0)
    {
      TurnOnNoticeLuckyWheel();
    }
    else
    {
      TurnOffNoticeLuckyWheel();
    }
  }

  private void InitNoticeFreeCoin()
  {
    if (GameManager.Instance.FreeCoinTodayCount < 1)
    {
      TurnOnNoticeFreeCoin();
    }
    else
    {
      TurnOffNoticeFreeCoin();
    }
  }

  private void InitNoticeLuckySpin()
  {
    if (GameManager.Instance.PlinkoFreeTodayCount > 0 && GameManager.Instance.CurrentLevel >= 14)
    {
      TurnOnNoticeLuckySpin();
    }
    else
    {
      TurnOffNoticeLuckySpin();
    }
  }

  private void InitNoticeLuckyEggHunt()
  {
    if (GameManager.Instance.MoveEggTicketsAmount > 0 && GameManager.Instance.CurrentNeedSolvingEggOrderIndex < 15)
    {
      TurnOnNoticeLuckyEggHunt();
    }
    else
    {
      TurnOffNoticeLuckyEggHunt();
    }
  }

  public void TurnOnNoticeDailyReward()
  {
    iconNoticeDailyReward.gameObject.SetActive(true);
  }

  public void TurnOffNoticeDailyReward()
  {
    iconNoticeDailyReward.gameObject.SetActive(false);
  }

  public void TurnOnNoticeLuckyWheel()
  {
    iconNoticeLuckyWheel.gameObject.SetActive(true);
  }

  public void TurnOffNoticeLuckyWheel()
  {
    iconNoticeLuckyWheel.gameObject.SetActive(false);
  }

  public void TurnOnNoticeLuckySpin()
  {
    iconNoticeLuckySpin.gameObject.SetActive(true);
  }

  public void TurnOffNoticeLuckySpin()
  {
    iconNoticeLuckySpin.gameObject.SetActive(false);
  }

  public void TurnOnNoticeFreeCoin()
  {
    iconNoticeFreeCoinInShop.gameObject.SetActive(true);

    if (SceneManager.GetActiveScene().name != "Lobby") return;
    iconNoticeFreeCoin.gameObject.SetActive(true);
    iconNoticeFreeCoinTab.gameObject.SetActive(true);
  }

  public void TurnOffNoticeFreeCoin()
  {
    iconNoticeFreeCoinInShop.gameObject.SetActive(false);

    if (SceneManager.GetActiveScene().name != "Lobby") return;
    iconNoticeFreeCoin.gameObject.SetActive(false);
    iconNoticeFreeCoinTab.gameObject.SetActive(false);
  }

  public void TurnOnNoticeLuckyEggHunt()
  {
    iconNoticeLuckyEggHunt.gameObject.SetActive(true);
  }

  public void TurnOffNoticeLuckyEggHunt()
  {
    iconNoticeLuckyEggHunt.gameObject.SetActive(false);
  }

  bool _isShowingUnlockDailyTask = false;
  public void NoticeUnlockDailyWeek()
  {
    if (_isShowingUnlockDailyTask) return;
    _isShowingUnlockDailyTask = true;

    if (GameManager.Instance.CurrentLevel < 5)
    {
      ShowNotifyWith("UNLOCK AT LEVEL 6");
    }

    DOVirtual.DelayedCall(2f, () =>
    {
      _isShowingUnlockDailyTask = false;
    });
  }
}