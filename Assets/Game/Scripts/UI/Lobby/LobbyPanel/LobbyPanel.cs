using System;
using System.Collections;
using DG.Tweening;
using Firebase.Analytics;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class LobbyPanel : MonoBehaviour
{
  public static LobbyPanel Instance { get; private set; }
  [SerializeField] RectTransform settingModal;
  [SerializeField] RectTransform goalCompletedNotify;
  [SerializeField] RectTransform showCanvas;

  private void Start()
  {
    if (Instance == null) Instance = this;
    else Destroy(gameObject);
  }

  private void OnDestroy()
  {
    DOTween.KillAll();
  }

  void OpenModal(Transform panel)
  {
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenModal");
  }

  void CloseModal(Transform panel)
  {
    panel.GetComponentInChildren<Animator>().Play("CloseModal");
    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      panel.gameObject.SetActive(false);
    });
  }

  bool IsShowingNotify = false;

  void OpenNotify(RectTransform panel)
  {
    if (IsShowingNotify) return;
    IsShowingNotify = true;
    panel.gameObject.SetActive(true);
    panel.GetComponentInChildren<Animator>().Play("OpenNotify");

    var m_CurrentClipInfo = panel.GetComponentInChildren<Animator>().GetCurrentAnimatorClipInfo(0);
    var m_CurrentClipLength = m_CurrentClipInfo[0].clip.length;
    LeanTween.delayedCall(gameObject, m_CurrentClipLength, () =>
    {
      IsShowingNotify = false;
      Destroy(panel.gameObject);
    });
  }

  public void ShowNotifyWith(string content)
  {
    var _goalCompletedNotify = Instantiate(goalCompletedNotify, showCanvas);
    _goalCompletedNotify.GetComponent<GoalCompletedNotify>().ShowNotify(content);
    OpenNotify(_goalCompletedNotify);
  }

  public void LoadSceneWithDelay(string sceneName)
  {
    SoundManager.Instance.PlayPressBtnSfx();
    SceneManager.LoadScene(sceneName);
  }

  public void LoadSceneAt(int i)
  {
    SceneManager.LoadScene(i);
  }

  public void LoadSceneWith(string nameScene)
  {
    SceneManager.LoadScene(nameScene);
  }

  public void ToggleSettingPanel()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!settingModal.gameObject.activeSelf)
    {
      OpenModal(settingModal);
    }
    else
    {
      CloseModal(settingModal);
    }
  }
}
