using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseReward : MonoBehaviour
{
    [SerializeField] protected Button receiveBtn;
    [SerializeField] protected RectTransform unlock;
    [SerializeField] protected RectTransform locked;
    [SerializeField] protected RectTransform tickImg;
    [SerializeField] protected Image iconImg;
    [SerializeField] protected TextMeshProUGUI amountTxt;
    [SerializeField] protected SpawnDetailsRewardSystem spawnDetailsRewardSystem;
    [SerializeField] protected RewardData[] rewardDatas;
    protected RewardState rewardState;
    public int day;
    public void SetReward(RewardData[] rewardDatas)
    {
        this.rewardDatas = rewardDatas;
        spawnDetailsRewardSystem.InjectRewardData(rewardDatas);
    }

    public RewardData[] RewardDatas { get { return rewardDatas; } }

    public void SetIcon(Sprite icon)
    {
        iconImg.sprite = icon;
    }

    public void SetAmountTxt(string amount)
    {
        amountTxt.text = amount;
    }

    public virtual void UnLock()
    {
        rewardState = RewardState.Unlocked;
        unlock.gameObject.SetActive(true);
        tickImg.gameObject.SetActive(true);
        locked.gameObject.SetActive(false);
    }

    public virtual void Lock()
    {
        rewardState = RewardState.Locking;
        unlock.gameObject.SetActive(false);
        tickImg.gameObject.SetActive(false);
        locked.gameObject.SetActive(true);
    }

    public virtual void ReachMilestone()
    {
        rewardState = RewardState.ReachMilestone;
        unlock.gameObject.SetActive(false);
        locked.gameObject.SetActive(false);
        tickImg.gameObject.SetActive(false);
    }

    public void Miss()
    {
        rewardState = RewardState.Miss;
        unlock.gameObject.SetActive(true);
        locked.gameObject.SetActive(false);
        tickImg.gameObject.SetActive(false);
    }

    public virtual void Claim()
    {
        Debug.Log(rewardState);
        switch (rewardState)
        {
            case RewardState.ReachMilestone:
                EarnPriceBys(rewardDatas);
                Receive();
                UnLock();
                break;
            default:
                // spawnDetailsRewardSystem.ShowDetaile();
                // DailySweetSystem.Instance.OpenDetails(spawnDetailsRewardSystem);
                break;
        }
    }

    public abstract void Receive();

    protected virtual void EarnPriceBys(RewardData[] priceDatas, int priceValueFactor = 1)
    {
        var rewardDataImgs = new Sprite[priceDatas.Length];
        var amountReward = new string[priceDatas.Length];
        var color = new Color[priceDatas.Length];
        var topDescription = new string[priceDatas.Length];
        for (int i = 0; i < priceDatas.Length; i++)
        {
            if (priceDatas[i].Type == PriceType.InfinityHeart)
                amountReward[i] = priceDatas[i].Value[0] + "m";
            else if (priceDatas[i].Type == PriceType.Coin)
                amountReward[i] = priceDatas[i].Value[0].ToString();
            else
                amountReward[i] = "x" + priceDatas[i].Value[0];

            rewardDataImgs[i] = priceDatas[i].Img;
            color[i] = Color.yellow;
            EarnPriceBy(priceDatas[i], priceValueFactor);
        }

        ShowPanel.Instance.ShowImgsWith(
            rewardDataImgs, topDescription,
            amountReward, color, false);
    }

    protected virtual void EarnPriceBy(RewardData priceData, int priceValueFactor = 1)
    {
        if (priceData.Type == PriceType.Coin)
            GameManager.Instance.CurrentCoin += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.Hammer)
            GameManager.Instance.CurrentHammer += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.Refresh)
            GameManager.Instance.CurrentRefresh += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.Rocket)
            GameManager.Instance.CurrentRocket += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.Swap)
            GameManager.Instance.CurrentSwap += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.Ticket)
            GameManager.Instance.CurrentTicket += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.TicketNoel)
            GameManager.Instance.CurrentTicketNoel += priceData.Value[0] * priceValueFactor;
        else if (priceData.Type == PriceType.InfinityHeart)
            HeartSystem.Instance.AddInfinityHeartTime(priceData.Value[0] * priceValueFactor);
    }
}
public enum RewardState
{
    Unlocked,
    Locking,
    ReachMilestone,
    Miss
}
