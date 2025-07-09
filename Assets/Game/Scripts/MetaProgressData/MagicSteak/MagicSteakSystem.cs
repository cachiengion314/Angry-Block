using TMPro;
using UnityEngine;

public class MagicSteakSystem : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI time;
  [SerializeField] SteakControl[] steakControls;
  [SerializeField] RewardStreakControl[] rewardStreakControls;
  [SerializeField] ProgerssControl progerssControl;

  void Start()
  {
    InitStreak();
    SetValueProgerssBar();
    SetItemImg();
    InitRewardStreak();

    CountdownSystem.Instance.OnTimeUpdate += UpdateTime;
    CountdownSystem.Instance.OnEndLoopTimeSpin += UpdateUi;
  }

  void OnDestroy()
  {
    CountdownSystem.Instance.OnTimeUpdate -= UpdateTime;
    CountdownSystem.Instance.OnEndLoopTimeSpin -= UpdateUi;
  }

  void UpdateUi(int x)
  {
    InitStreak();
    SetValueProgerssBar();
    SetItemImg();
    InitRewardStreak();
  }

  public void UpdateTime(string strTime)
  {
    time.text = strTime;
  }

  public void InitRewardStreak()
  {
    var NextMetaStreak = GameManager.Instance.CurrentMilestone;
    for (int i = 0; i < rewardStreakControls.Length; i++)
    {
      rewardStreakControls[i].SetTxtCurrentStreak(i + 1);
      var RewardData = GameManager.Instance.GetRewardStreakAt(i);
      rewardStreakControls[i].SetImage(RewardData);
      if (i < NextMetaStreak) rewardStreakControls[i].Unlock();
      else rewardStreakControls[i].Lock();
    }
  }

  public void InitStreak()
  {
    var CurrentMetaStreak = GameManager.Instance.CurrentMetaStreak;
    for (int i = 0; i < steakControls.Length; i++)
    {
      if (i == CurrentMetaStreak) steakControls[i].Unlock();
      else steakControls[i].Lock();
    }
  }

  public void SetValueProgerssBar()
  {
    var CurrentValue = GameManager.Instance.CurrentMetaProgressTotal;
    var TotalValue = GameManager.Instance.GetCurrentProgressTarget();

    progerssControl.SetValueProgerssBar(CurrentValue, TotalValue);
    progerssControl.SetTextProgerssBar(CurrentValue, TotalValue);
  }

  public void SetItemImg()
  {
    var CurrentMilestone = GameManager.Instance.CurrentMilestone;
    var RewardData = GameManager.Instance.GetRewardStreakAt(CurrentMilestone);
    progerssControl.SetImage(RewardData);
  }
}
