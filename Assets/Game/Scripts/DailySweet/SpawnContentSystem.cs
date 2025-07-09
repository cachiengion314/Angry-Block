using UnityEngine;

public class SpawnContentSystem : MonoBehaviour
{
    public static SpawnContentSystem Instance { get; private set; }
    [SerializeField] GameObject dayContentPrefab;
    public Transform dayContentParent;
    [SerializeField] DailyReward[] dailyRewardFrees;
    [SerializeField] DailyReward[] dailyRewardPasss;
    [SerializeField] Sprite chest;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        SetUp();
    }
    void SetUp()
    {
        for (int i = 1; i <= 30; i++)
        {
            var dayContent = Instantiate(dayContentPrefab, dayContentParent);
            if (dayContent.TryGetComponent(out ContentCtrl contentCtrl))
            {
                contentCtrl.SetDay(i);
            }
            if (dayContent.TryGetComponent(out RewardFreeCtrl rewardFreeCtrl))
            {
                
                rewardFreeCtrl.SetReward(dailyRewardFrees[i - 1].data);
                rewardFreeCtrl.day = i;
                if (dailyRewardFrees[i - 1].data.Length == 1)
                {
                    rewardFreeCtrl.SetAmountTxt(dailyRewardFrees[i - 1].data[0].Value[0].ToString());
                    rewardFreeCtrl.SetIcon(dailyRewardFrees[i - 1].data[0].Img);
                }
                else
                {
                    rewardFreeCtrl.SetAmountTxt(null);
                    rewardFreeCtrl.SetIcon(chest);
                }
            }
            if (dayContent.TryGetComponent(out RewardPassCtrl rewardPassCtrl))
            {
                rewardPassCtrl.SetReward(dailyRewardPasss[i - 1].data);
                rewardPassCtrl.day = i;
                if (dailyRewardPasss[i - 1].data.Length == 1)
                {
                    rewardPassCtrl.SetAmountTxt(dailyRewardPasss[i - 1].data[0].Value[0].ToString());
                    rewardPassCtrl.SetIcon(dailyRewardPasss[i - 1].data[0].Img);
                }
                else
                {
                    rewardPassCtrl.SetAmountTxt(null);
                    rewardPassCtrl.SetIcon(chest);
                }
            }
        }
    }
}

[System.Serializable]
public struct DailyReward
{
    public RewardData[] data;
}
