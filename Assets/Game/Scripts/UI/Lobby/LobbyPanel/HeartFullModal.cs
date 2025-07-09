using UnityEngine;

/// <summary>
/// HeartFull Modal
/// </summary> <summary>
/// 
/// </summary>
public partial class LobbyPanel : MonoBehaviour
{
  [Header("HeaderFull Modal")]
  [SerializeField] RectTransform heartFullRect;

  private void InitHeartFullRect()
  {
    heartFullRect.gameObject.SetActive(false);
  }

  public void CloseHeartFullModal()
  {
    CloseModal(heartFullRect);
  }

  public void OpenHeartFullModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    OpenModal(heartFullRect.transform);
  }
}