using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
  public void PlayOn()
  {
    int coin = 200;
    if (GameManager.Instance.CurrentCoin < coin)
    {
      GameplayPanel.Instance.ShowNotifyWith("NOT ENOUGH COINS");
      return;
    }
    GameplayPanel.Instance.ToggleOutOfSpaceModal();
    GameManager.Instance.CurrentCoin -= coin;
    LevelManager.Instance.UnlockAllSlot();
  }
  public void LevelFail()
  {
    GameplayPanel.Instance.CloseOutOfSpaceModal();
    GameplayPanel.Instance.ToggleLevelFailedModal();
  }
}
