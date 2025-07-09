using System;

[Serializable]
public class DayRewardData
{
    public bool IsReachMaxPrice;
    public DayData[] DayDatas;
}

[Serializable]
public class DayData
{
    public int DayIndex;
    public int[] CoinValue;
    public int DayActivated = -1; // -1 for not activated
}