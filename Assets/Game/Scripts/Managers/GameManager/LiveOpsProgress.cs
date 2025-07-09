using UnityEngine;

/// <summary>
/// LiveOpsProgress
/// </summary>
public partial class GameManager : MonoBehaviour
{
  public void PlayAnimClaimEggHuntProgress()
  {
    if (_moveEggTicketsAmountNeedClaim == 0) return;

    MoveEggTicketsAmount += _moveEggTicketsAmountNeedClaim;
    LobbyPanel.Instance.PlayAnimClaimEggHuntProgress();
  }
}