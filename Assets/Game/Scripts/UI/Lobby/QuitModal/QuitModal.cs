using UnityEngine;
using UnityEngine.UI;

public class QuitModal : MonoBehaviour
{
  [SerializeField] Button quitBtn;

  public void Quit()
  {
    quitBtn.interactable = false;
    LobbyPanel.Instance.GoLobby();

#if !UNITY_EDITOR
    if (GameManager.Instance.IsMetaProgressHappenning())
    {
      GameManager.Instance.ResetMetaStreak();
    }
#endif
  }

  public void Close()
  {
    GameManager.Instance.SetGameState(GameState.Gameplay);
    BalloonSystem.Instance.ShowBalloonWithDelay(0.4f);
  }
}
