using UnityEngine;

public partial class GameplayPanel : MonoBehaviour
{
  public static GameplayPanel Instance { get; private set; }
  [SerializeField] RectTransform goalCompletedNotify;
  [SerializeField] RectTransform showCanvas;
  public Transform SettingModal;
  public Transform LevelCompleteModal;
  public Transform LevelFiailedModal;
  public Transform OutOfSpaceModal;
  public Transform Booster1Modal;
  public Transform Booster2Modal;
  public Transform Booster3Modal;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }
  private void Start()
  {
    InitBooster();
  }
  private void OnDestroy()
  {
    UnsubscribeBoosterEvent();
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


  public void ToggleSettingModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!SettingModal.gameObject.activeSelf)
    {
      OpenModal(SettingModal);
    }
    else
    {
      CloseModal(SettingModal);
    }
  }

  public void ToggleLevelCompleteModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!LevelCompleteModal.gameObject.activeSelf)
    {
      OpenModal(LevelCompleteModal);
    }
    else
    {
      CloseModal(LevelCompleteModal);
    }
  }

  public void ToggleLevelFailedModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!LevelFiailedModal.gameObject.activeSelf)
    {
      OpenModal(LevelFiailedModal);
    }
    else
    {
      CloseModal(LevelFiailedModal);
    }
  }

  public void ToggleOutOfSpaceModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!OutOfSpaceModal.gameObject.activeSelf)
    {
      OpenModal(OutOfSpaceModal);
    }
    else
    {
      CloseModal(OutOfSpaceModal);
    }
  }

  public void ToggleBooster1Modal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!Booster1Modal.gameObject.activeSelf)
    {
      OpenModal(Booster1Modal);
    }
    else
    {
      CloseModal(Booster1Modal);
    }
  }

  public void ToggleBooster2Modal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!Booster2Modal.gameObject.activeSelf)
    {
      OpenModal(Booster2Modal);
    }
    else
    {
      CloseModal(Booster2Modal);
    }
  }

  public void ToggleBooster3Modal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!Booster3Modal.gameObject.activeSelf)
    {
      OpenModal(Booster3Modal);
    }
    else
    {
      CloseModal(Booster3Modal);
    }
  }

}
