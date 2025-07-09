public class RewardPassCtrl : BaseReward
{
    public override void Receive()
    {
        GameManager.Instance.RewardPassDayAt(day);
    }
}
