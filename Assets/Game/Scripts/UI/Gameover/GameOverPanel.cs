using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
  public void PlayOn()
  {
    GameplayPanel.Instance.ToggleOutOfSpaceModal();
  }
  public void LevelFail()
  {
    GameplayPanel.Instance.CloseOutOfSpaceModal();
    GameplayPanel.Instance.ToggleLevelFailedModal();
  }
}
