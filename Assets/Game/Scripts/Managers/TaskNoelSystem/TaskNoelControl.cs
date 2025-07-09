using System;
using UnityEngine;

public enum TYPETASKNOEL
{
  LOGIN,
  USETICKETNOEL,
  MILESTONE,
  DAILYGOAL,
  PLAYGAME
}

public class TaskNoelControl : MonoBehaviour
{
  public static Action onClaimedReward;

  [Header("External Dependences")]
  [SerializeField] TaskNoelData taskNoelData;
  [SerializeField] TYPETASKNOEL typeTaskNoel;

  private int _currentPhaseIndex = 0;
  private int _maxPhaseIndex = 0;

  #region Lifecycle Function
  public void Init()
  {
    _maxPhaseIndex = taskNoelData.PhaseNoelDatas.Length - 1;
  }

  #endregion

  #region Check Function
  public bool IsMaxPhase()
  {
    if (_currentPhaseIndex < _maxPhaseIndex)
    {
      return false;
    }

    return true;
  }

  public bool IsCompletedCurrentPhase()
  {
    var currentProgress = taskNoelData.CurrentProgress;
    var currentTarget = taskNoelData.PhaseNoelDatas[_currentPhaseIndex].TargetProgress;

    if (currentProgress < currentTarget)
    {
      return false;
    }

    return true;
  }

  #endregion

  #region Handle Function
  public void NextPhase()
  {
    _currentPhaseIndex++;
  }

  public void CollectReward()
  {
    var currentTicketNoel = GameManager.Instance.CurrentTicketNoel;
    currentTicketNoel += taskNoelData.PhaseNoelDatas[_currentPhaseIndex].AmountTicketNoel;

    GameManager.Instance.CurrentTicketNoel = currentTicketNoel;
    taskNoelData.PhaseNoelDatas[_currentPhaseIndex].IsRewarded = true;

    onClaimedReward?.Invoke();
  }

  #endregion

  #region Try Function
  public bool TryNextPhase()
  {
    if (IsMaxPhase()) return false;

    NextPhase();
    return true;
  }

  #endregion

  #region Set Function
  public void SetTaskNoelData(TaskNoelData taskNoelDatas)
  {
    this.taskNoelData = taskNoelDatas;
  }

  #endregion

  #region Get Function
  public int GetTargetProgressCurrent()
  {
    return taskNoelData.PhaseNoelDatas[_currentPhaseIndex].TargetProgress;
  }

  public int GetAmountTicketNoelCurrent()
  {
    return taskNoelData.PhaseNoelDatas[_currentPhaseIndex].AmountTicketNoel;
  }

  public bool GetIsRewarded()
  {
    return taskNoelData.PhaseNoelDatas[_currentPhaseIndex].IsRewarded;
  }

  public TaskNoelData GetTaskNoelData()
  {
    return taskNoelData;
  }

  public PhaseNoelData GetPhaseNoelDataCurrent()
  {
    return taskNoelData.PhaseNoelDatas[_currentPhaseIndex];
  }

  public TYPETASKNOEL GetTypeTaskNoel()
  {
    return typeTaskNoel;
  }

  #endregion
}

[Serializable]
public struct TaskNoelData
{
  public int CurrentProgress;
  public PhaseNoelData[] PhaseNoelDatas;
}

[Serializable]
public struct PhaseNoelData
{
  public bool IsRewarded;
  public int TargetProgress;
  public int AmountTicketNoel;
}
