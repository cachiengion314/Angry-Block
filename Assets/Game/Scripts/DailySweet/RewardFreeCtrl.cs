public class RewardFreeCtrl : BaseReward
{
  public override void Receive()
  {
    GameManager.Instance.RewardFreeDayAt(day);
  }
}
