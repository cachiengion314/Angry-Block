using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeeklyData", menuName = "ScriptableObjects/WeeklyData", order = 1)]

public class WeeklyData : ScriptableObject
{
    [Serializable]
    public class TaskWeeklyday
    {
        public string keyTaskWeekly;
        public string strNameTask; // Tên task
        public int currentValue;   // Giá trị hiện tại
        public int targetValue;    // Giá trị mục tiêu
        public bool isShowNotif;     // Trạng thái đã nhận
        public bool isSuccess;     // Trạng thái đã nhận
        public bool isPick;     // Trạng thái ngày hôm này được nhiệm vụ được chọn để sinh ra
        public bool isLoopTask;
        public TypeTask enumTypeTask;     // Trạng thái ngày hôm này được nhiệm vụ được chọn để sinh ra
        public enumListeningDataDailyWeekly enumTypeChildWeeklyListening;

    }
    public List<TaskWeeklyday> AllTasks;

    public enum TypeTask
    {
        None,
        CollectCoinWeekly,
        Collect1000CupsWeekly,
        CompleteLevelWeekly,
        UnlockSlotsWeekly,
        UsePowerItem,
        ReviveWeekly

    }
}
