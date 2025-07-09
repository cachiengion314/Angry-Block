using UnityEngine;
using UnityEngine.UI;

public class TaskNoelModal : MonoBehaviour
{
  [Header("Internal Dependences")]
  [SerializeField] Slider[] progressBars;
  [SerializeField] TaskNoelControl[] taskNoelControls;
  [SerializeField] Button[] collectBtns;
  [SerializeField] Image[] coatings;

  #region Lifecycle Function
  private void OnEnable()
  {
    TaskNoelControl.onClaimedReward += UpdateUIs;

    UpdateUIs();
  }

  private void OnDisable()
  {
    TaskNoelControl.onClaimedReward -= UpdateUIs;
  }

  #endregion

  #region Update UI Function
  private void UpdateUIs()
  {
    for (int i = 0; i < progressBars.Length; i++)
    {
      UpdateUIAt(i);
    }
  }

  public void UpdateUIAt(int index)
  {
    var taskNoelControl = taskNoelControls[index];

    // UI completed task
    if (taskNoelControl.IsCompletedCurrentPhase())
    {
      var isRewarded = taskNoelControl.GetIsRewarded();

      UpdateUICompletedAt(index, isRewarded);
      return;
    }

    // UI uncompleted task
    UpdateUIUncompletedAt(index);
  }

  private void UpdateUICompletedAt(int index, bool isRewarded)
  {
    var progressBar = progressBars[index];
    progressBar.value = 1;

    if (isRewarded)
    {
      UpdateUIRewardedAt(index);
      return;
    }

    UpdateUIUnRewardedAt(index);
  }

  private void UpdateUIRewardedAt(int index)
  {
    var collectBtn = collectBtns[index];
    collectBtn.interactable = false;

    var coating = coatings[index];
    coating.gameObject.SetActive(true);
  }

  private void UpdateUIUnRewardedAt(int index)
  {
    var collectBtn = collectBtns[index];
    collectBtn.interactable = true;

    var coating = coatings[index];
    coating.gameObject.SetActive(false);
  }

  private void UpdateUIUncompletedAt(int index)
  {
    var taskNoelControl = taskNoelControls[index];
    var taskNoelData = taskNoelControl.GetTaskNoelData();
    var currentProgress = taskNoelData.CurrentProgress;
    var targetProgress = taskNoelControl.GetPhaseNoelDataCurrent().TargetProgress;

    var progressBar = progressBars[index];
    progressBar.value = (float)currentProgress / targetProgress;

    var collectBtn = collectBtns[index];
    collectBtn.interactable = false;

    var coating = coatings[index];
    coating.gameObject.SetActive(false);
  }

  #endregion

  public void Close()
  {
    LobbyPanel.Instance.ToggleTaskNoelModal();
  }
}
