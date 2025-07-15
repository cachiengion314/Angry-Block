using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
  public void PlayOn()
  {
    GameplayPanel.Instance.ToggleOutOfSpaceModal();
  }
  public void LevelFail()
  {
    GameplayPanel.Instance.OutOfSpaceModal.gameObject.SetActive(false);
    GameplayPanel.Instance.ToggleLevelFailedModal();
  }
}
