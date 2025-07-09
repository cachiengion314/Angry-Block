using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TaskData", menuName = "ScriptableObjects/TaskData", order = 1)]
public class TaskData : ScriptableObject
{
  [Serializable]
  public class Taskday
  {
    public string keyTaskToday;
    public string strNameTask; // Tên task
    public int currentValue;   // Giá trị hiện tại
    public int targetValue;    // Giá trị mục tiêu
    public bool _isSuccess;     // Trạng thái đã nhận
    public bool _isPick;     // Trạng thái ngày hôm này được nhiệm vụ được chọn để sinh ra
    public bool _isShowNotif;
    public bool _isLoopTask;
    public TypeTask enumTypeTask;     // Trạng thái ngày hôm này được nhiệm vụ được chọn để sinh ra

    public enumListeningDataDailyTask enumTypeChildListening;

  }
  public List<Taskday> AllTasks = new List<Taskday>();


  public enum TypeTask
  {
    None,
    Login, // login
    BoosterAndCup, // dùng booster hoặc nhiệm vụ ăn cup
    Other, // các nhiệm vụ khác 

  }

}
