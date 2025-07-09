using System;

[Serializable]
public struct DailyBonusData
{
    public bool IsNull => startTime == null && endTime == null && data == null;
    public string startTime;
    public string endTime;
    public bool[] data;
}
