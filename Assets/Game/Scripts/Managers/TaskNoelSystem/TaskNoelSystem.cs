using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskNoelSystem : MonoBehaviour
{
  public static TaskNoelSystem Instance { get; private set; }

  [Header("External Dependences")]
  [SerializeField] TaskNoelControl[] taskNoelControls;
  [SerializeField] Sprite ticketImg;
  [SerializeField] Image warningTask;

  private TaskNoelData[] _taskNoelDatas;

  #region Lifecycle Function
  private void OnEnable()
  {
    GameManager.onChangeAmountSpin += OnChangeAmountSpin;
    TaskNoelControl.onClaimedReward += OnClaimedReward;
  }

  private void Start()
  {
    StartCoroutine(IEStart());
  }

  IEnumerator IEStart()
  {
    yield return new WaitUntil(() => GameManager.Instance != null);

    Instance = this;

    if (!GameManager.Instance.IsEvented()) yield break;

    Load();
    InitTasks();

    yield return null;
    OnLoginDay();
    OnCompleteDailyGoals();
    OnPlayGame();

    CheckWarning();
  }

  private void InitTasks()
  {
    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];
      taskNoelControl.Init();
    }
  }

  private void OnDisable()
  {
    GameManager.onChangeAmountSpin -= OnChangeAmountSpin;
    TaskNoelControl.onClaimedReward -= OnClaimedReward;
  }

  #endregion

  #region Load/Save Function
  public void Save()
  {
    var _taskNoelDatas = new TaskNoelData[taskNoelControls.Length];

    var taskNoelDatas = new TaskNoels()
    {
      TaskNoelDatas = new TaskNoelData[taskNoelControls.Length]
    };

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelData = taskNoelControls[i].GetTaskNoelData();
      taskNoelDatas.TaskNoelDatas[i].CurrentProgress = taskNoelData.CurrentProgress;
      taskNoelDatas.TaskNoelDatas[i].PhaseNoelDatas = taskNoelData.PhaseNoelDatas;

      _taskNoelDatas[i] = taskNoelData;
    }

    HoangNam.SaveSystem.SaveWith(taskNoelDatas, KeyString.NAME_TASKNOEL_DATA);
  }

  public void Load()
  {
    var _taskNoelDatas = new TaskNoelData[taskNoelControls.Length];

    var taskNoelDatas = HoangNam.SaveSystem.LoadWith<TaskNoels>(KeyString.NAME_TASKNOEL_DATA);

    if (taskNoelDatas.TaskNoelDatas == null)
    {
      Save();
      return;
    }

    for (int i = 0; i < _taskNoelDatas.Length; i++)
    {
      var taskNoelData = _taskNoelDatas[i];
      taskNoelData.CurrentProgress = taskNoelDatas.TaskNoelDatas[i].CurrentProgress;
      taskNoelData.PhaseNoelDatas = taskNoelDatas.TaskNoelDatas[i].PhaseNoelDatas;

      _taskNoelDatas[i] = taskNoelData;
      taskNoelControls[i].SetTaskNoelData(_taskNoelDatas[i]);
    }
  }

  #endregion

  #region Find Function
  public TaskNoelControl[] FindTaskNoelControlWith(TYPETASKNOEL typeTaskNoel)
  {
    List<TaskNoelControl> results = new();

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var typeTask = taskNoelControls[i].GetTypeTaskNoel();

      if (typeTask == typeTaskNoel)
      {
        results.Add(taskNoelControls[i]);
      }
    }

    return results.ToArray();
  }

  #endregion

  #region Event Function
  public void OnChangeAmountSpin()
  {
    if (!GameManager.Instance.IsEvented()) return;

    var taskNoelControls = FindTaskNoelControlWith(TYPETASKNOEL.USETICKETNOEL);

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];
      var taskNoelData = taskNoelControl.GetTaskNoelData();

      if (taskNoelControl.GetIsRewarded()) continue;
      if (taskNoelControl.IsCompletedCurrentPhase()) continue;

      taskNoelData.CurrentProgress++;

      taskNoelControls[i].SetTaskNoelData(taskNoelData);
    }

    OnCompletedMileStone();
  }

  public void OnPlayGame()
  {
    var taskNoelControls = FindTaskNoelControlWith(TYPETASKNOEL.PLAYGAME);

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];
      var taskNoelData = taskNoelControl.GetTaskNoelData();

      if (taskNoelControl.GetIsRewarded()) continue;
      if (taskNoelControl.IsCompletedCurrentPhase()) continue;

      taskNoelData.CurrentProgress = PlayerPrefs.GetInt("AmountPlayGame", 0);

      taskNoelControls[i].SetTaskNoelData(taskNoelData);
    }

    CheckWarning();
    Save();
  }

  public void OnCompleteDailyGoals()
  {
    var taskNoelControls = FindTaskNoelControlWith(TYPETASKNOEL.DAILYGOAL);

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];
      var taskNoelData = taskNoelControl.GetTaskNoelData();

      if (taskNoelControl.GetIsRewarded()) continue;
      if (taskNoelControl.IsCompletedCurrentPhase()) continue;

      taskNoelData.CurrentProgress = GameManager.Instance.AmountCompletedQuest;

      taskNoelControls[i].SetTaskNoelData(taskNoelData);
    }

    CheckWarning();
    Save();
  }

  public void OnLoginDay()
  {
    var taskNoelControls = FindTaskNoelControlWith(TYPETASKNOEL.LOGIN);

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];
      var taskNoelData = taskNoelControl.GetTaskNoelData();

      if (taskNoelControl.GetIsRewarded()) continue;
      if (taskNoelControl.IsCompletedCurrentPhase()) continue;

      taskNoelData.CurrentProgress = CalculateDaysLogin();

      taskNoelControls[i].SetTaskNoelData(taskNoelData);
    }

    CheckWarning();
    Save();
  }

  public void OnCompletedMileStone()
  {
    var taskNoelControls = FindTaskNoelControlWith(TYPETASKNOEL.MILESTONE);

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];
      var taskNoelData = taskNoelControl.GetTaskNoelData();

      if (taskNoelControl.GetIsRewarded()) continue;
      if (taskNoelControl.IsCompletedCurrentPhase()) continue;

      var currentSpinNoel = GameManager.Instance.CurrentSpinNoel;
      var giftControl = ProgressSystem.Instance.GetGiftControlAt(0);
      var targetGift = giftControl.spinMax;

      if (currentSpinNoel < targetGift) continue;

      taskNoelData.CurrentProgress++;

      taskNoelControls[i].SetTaskNoelData(taskNoelData);
    }

    CheckWarning();
    Save();
  }

  public void OnClaimedReward()
  {
    ShowPanel.Instance.ShowImgWith(ticketImg, "", "x1", Color.yellow, false);
    CheckWarning();
    Save();
  }

  #endregion

  #region Handle Function
  private int CalculateDaysLogin()
  {
    var currentDay = DateTime.Today.Day;
;
    var results = 0;

    for (int i = currentDay; i > 0; i--)
    {
      var isLogin = GameManager.Instance.GetLoginDayAt(i);

      if (!isLogin)
      {
        break;
      }

      results++;
    }

    return results;
  }

  public void CheckWarning()
  {
    HideWarning();

    for (int i = 0; i < taskNoelControls.Length; i++)
    {
      var taskNoelControl = taskNoelControls[i];

      if (taskNoelControl.IsCompletedCurrentPhase() && !taskNoelControl.GetIsRewarded())
      {
        ShowWarning();
        break;
      }
    }
  }

  private void ShowWarning()
  {
    warningTask.gameObject.SetActive(true);
  }

  private void HideWarning()
  {
    warningTask.gameObject.SetActive(false);
  }

  #endregion
}

public struct TaskNoels
{
  public TaskNoelData[] TaskNoelDatas;
}
