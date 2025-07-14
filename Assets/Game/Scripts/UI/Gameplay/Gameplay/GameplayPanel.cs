using UnityEngine;

public partial class GameplayPanel : MonoBehaviour
{
  public static GameplayPanel Instance { get; private set; }

  [SerializeField] Transform SettingModal;
  [SerializeField] Transform LevelCompleteModal;
  [SerializeField] Transform LevelFiailedModal;
  [SerializeField] Transform OutOfSpaceModal;
  [SerializeField] Transform Booster1Modal;

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
}
