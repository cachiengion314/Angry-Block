using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailySweetSystem : MonoBehaviour
{
    public static DailySweetSystem Instance { get; private set; }
    [SerializeField] Transform Exit;
    SpawnDetailsRewardSystem currentDetailsReward;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // text.text = day.ToString();
        Exit.gameObject.SetActive(false);
        StartCoroutine(Init());
        GameManager.onBuyUnlockFollowing += UpdateStateReward;
    }

    private void OnDestroy()
    {
        GameManager.onBuyUnlockFollowing -= UpdateStateReward;
    }

    IEnumerator Init()
    {
        yield return null;
        UpdateStateReward();
    }

    void UpdateStateReward()
    {
        int day = DateTime.Today.Day;
        Transform dayContentParent = SpawnContentSystem.Instance.dayContentParent;
        for (int i = 1; i < dayContentParent.childCount; i++)
        {
            if (dayContentParent.GetChild(i).TryGetComponent(out RewardFreeCtrl rewardFreeCtrl)
                && dayContentParent.GetChild(i).TryGetComponent(out RewardPassCtrl rewardPassCtrl)
                && dayContentParent.GetChild(i).TryGetComponent(out ContentCtrl contentCtrl))
            {
                // Setup RewardFree
                if (GameManager.Instance.GetLoginDayAt(i))
                {
                    if (i <= day)
                    {
                        if (GameManager.Instance.GetRewardFreeDayAt(i)) rewardFreeCtrl.UnLock();
                        else rewardFreeCtrl.ReachMilestone();
                    }
                }
                else
                {
                    if (i < day)
                    {
                        rewardFreeCtrl.Miss();
                    }
                }

                // Setup Content
                if (i == day)
                {
                    contentCtrl.ReachMilestone();
                }
                else if (i < day)
                {
                    contentCtrl.Unlock();
                }
                else if (i > day)
                {
                    contentCtrl.Lock();
                    rewardFreeCtrl.Lock();
                    rewardPassCtrl.Lock();
                }

                // setup RewardPass
                if (GameManager.Instance.IsUnlockFollowing)
                {
                    if (i <= day)
                    {
                        if (GameManager.Instance.GetRewardPassDayAt(i))
                        {
                            rewardPassCtrl.UnLock();
                        }
                        else
                        {
                            rewardPassCtrl.ReachMilestone();
                        }
                    }
                }
                else
                {
                    rewardPassCtrl.Lock();
                }
            }
        }
    }

    public void ClaimAll()
    {
        int day = DateTime.Today.Day;
        Dictionary<PriceType, Sprite> rewardDataImgs = new();
        Dictionary<PriceType, int> amountReward = new();

        Transform dayContentParent = SpawnContentSystem.Instance.dayContentParent;
        for (int i = 1; i < dayContentParent.childCount + 1; i++)
        {
            if (i <= day)
            {
                if (!GameManager.Instance.GetRewardPassDayAt(i) && GameManager.Instance.IsUnlockFollowing)
                {
                    if (dayContentParent.GetChild(i).TryGetComponent(out RewardPassCtrl rewardPassCtrl))
                    {
                        rewardPassCtrl.Receive();
                        rewardPassCtrl.UnLock();
                        foreach (var reward in rewardPassCtrl.RewardDatas)
                        {
                            EarnPriceBy(reward);
                            if (!rewardDataImgs.ContainsKey(reward.Type))
                            {
                                rewardDataImgs.Add(reward.Type, reward.Img);
                            }
                            if (!amountReward.ContainsKey(reward.Type))
                            {
                                amountReward.Add(reward.Type, reward.Value[0]);
                            }
                            else
                            {
                                amountReward[reward.Type] += reward.Value[0];
                            }
                        }
                    }
                }
                if (!GameManager.Instance.GetRewardFreeDayAt(i) && GameManager.Instance.GetLoginDayAt(i))
                {
                    if (dayContentParent.GetChild(i).TryGetComponent(out RewardFreeCtrl rewardFreeCtrl))
                    {
                        rewardFreeCtrl.Receive();
                        rewardFreeCtrl.UnLock();
                        foreach (var reward in rewardFreeCtrl.RewardDatas)
                        {
                            EarnPriceBy(reward);
                            if (!rewardDataImgs.ContainsKey(reward.Type))
                            {
                                rewardDataImgs.Add(reward.Type, reward.Img);
                            }
                            if (!amountReward.ContainsKey(reward.Type))
                            {
                                amountReward.Add(reward.Type, reward.Value[0]);
                            }
                            else
                            {
                                amountReward[reward.Type] += reward.Value[0];
                            }
                        }
                    }
                }
            }
        }

        var rewardDataImgss = new List<Sprite>();
        var amountRewards = new List<string>();
        var colors = new List<Color>();
        var topDescription = new List<string>();

        foreach (var reward in rewardDataImgs.Values)
        {
            rewardDataImgss.Add(reward);
            colors.Add(Color.yellow);
            topDescription.Add(null);
        }

        foreach (var reward in amountReward.Keys)
        {
            if (reward == PriceType.InfinityHeart)
                amountRewards.Add(amountReward[reward] + "m");
            else if (reward == PriceType.Coin)
                amountRewards.Add(amountReward[reward].ToString());
            else
                amountRewards.Add("x" + amountReward[reward]);
        }
        if (amountReward.Count > 0)
        {
            ShowPanel.Instance.ShowImgsWith(
                rewardDataImgss.ToArray(), topDescription.ToArray(),
                amountRewards.ToArray(), colors.ToArray(), false);
        }
    }

    void EarnPriceBy(RewardData priceData, int priceValueFactor = 1)
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

    public void OpenDetails(SpawnDetailsRewardSystem currentDetailsReward)
    {
        Exit.gameObject.SetActive(true);
        this.currentDetailsReward = currentDetailsReward;
    }

    public void CloseDetails()
    {
        Exit.gameObject.SetActive(false);
        currentDetailsReward.HideDetaile();
    }

    // int day
    // {
    //     get { return PlayerPrefs.GetInt("currentday", 1); }
    //     set { PlayerPrefs.SetInt("currentday", value); }
    // }
    // public void Nextday()
    // {
    //     day++;
    //     text.text = day.ToString();
    // }
    // [SerializeField] TextMeshProUGUI text;
    // public void Logined()
    // {
    //     GameManager.Instance.LoginDayAt(day);
    //     UpdateStateReward();
    //     GameManager.onReceivedReward?.Invoke();
    // }
}
