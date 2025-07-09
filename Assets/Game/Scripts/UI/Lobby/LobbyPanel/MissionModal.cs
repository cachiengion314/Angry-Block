using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mission Modal
/// </summary> <summary>
/// 
/// </summary>
public partial class LobbyPanel : MonoBehaviour
{
  [Header("Mission Modal")]
  [SerializeField] RectTransform missionModal;
  [SerializeField] Image dailyGoalBtn;
  [SerializeField] Image weeklyQuestBtn;
  [SerializeField] Sprite[] dailyGoalRenderers;
  [SerializeField] Sprite[] weeklyQuestRenderers;
  [SerializeField] CanvasGroup[] canvasGroups;

  private int _currentTabMission = 0;

  public void ClickMissionModal()
  {
    _currentTabMission = 0;
    SoundManager.Instance.PlayPressBtnSfx();
    ShowMissionModal();
    UpdateTabMission();

    foreach (var canvasGroup in canvasGroups)
    {
      DOTween.Kill(canvasGroup.transform);

      canvasGroup.alpha = 0;
      canvasGroup.DOFade(1, 1).SetTarget(canvasGroup.transform);
    }
  }

  public void ClickDailyTaskBtn()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (_currentTabMission == 0) return;

    _currentTabMission = 0;
    UpdateTabMission();
  }

  bool _isShowingNoticeLockWeekly = false;
  public void ClickWeeklyTaskBtn()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    if (GameManager.Instance.CurrentLevel < DailyWeeklyManager.Instance.LevelUnLock)
    {
      if (_isShowingNoticeLockWeekly) return;
      _isShowingNoticeLockWeekly = true;

      DOVirtual.DelayedCall(2f, () =>
      {
        _isShowingNoticeLockWeekly = false;
      });

      ShowNotifyWith("UNLOCK AT LEVEL " + (DailyWeeklyManager.Instance.LevelUnLock + 1));
      return;
    }
    if (_currentTabMission == 1) return;

    _currentTabMission = 1;
    UpdateTabMission();
  }

  public void ClickCloseMissionModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    HideMissionModal();
  }

  private void UpdateTabMission()
  {
    switch (_currentTabMission)
    {
      case 0:
        ShowDailyTask();
        HideDailyWeekly();
        break;
      case 1:
        HideDailyTask();
        ShowDailyWeekly();
        break;
    }
  }

  private void HideMissionModal()
  {
    missionModal.gameObject.SetActive(false);
  }

  private void HideDailyTask()
  {
    dailyGoalBtn.sprite = dailyGoalRenderers[1];
    _dailyTask.gameObject.SetActive(false);
  }

  private void HideDailyWeekly()
  {
    weeklyQuestBtn.sprite = weeklyQuestRenderers[1];
    _dailyWeekly.gameObject.SetActive(false);
  }

  private void ShowMissionModal()
  {
    missionModal.gameObject.SetActive(true);
  }

  private void ShowDailyTask()
  {
    dailyGoalBtn.sprite = dailyGoalRenderers[0];
    _dailyTask.gameObject.SetActive(true);
  }

  private void ShowDailyWeekly()
  {
    weeklyQuestBtn.sprite = weeklyQuestRenderers[0];
    _dailyWeekly.gameObject.SetActive(true);
  }
}