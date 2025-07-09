using UnityEngine;

public class SpawnDetailsRewardSystem : MonoBehaviour
{
    [SerializeField] DetailsRewardCtrl detailsRewardPrefab;
    [SerializeField] Transform detailsRewardParent;
    RewardData[] rewardDatas;
    
    public void InjectRewardData(RewardData[] rewardDatas)
    {
        this.rewardDatas = rewardDatas;
        SetUp();
    }

    public void ShowDetaile()
    {
        gameObject.SetActive(true);
    }

    public void HideDetaile()
    {
        gameObject.SetActive(false);
    }

    void SetUp()
    {
        foreach (var rewardData in rewardDatas)
        {
            var detailsRewardCtrl = Instantiate(detailsRewardPrefab, detailsRewardParent);
            detailsRewardCtrl.SetUp(rewardData.Img, rewardData.Value[0]);
        }
    }
}
