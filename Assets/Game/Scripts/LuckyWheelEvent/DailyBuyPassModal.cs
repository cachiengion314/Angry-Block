using UnityEngine;

public class DailyBuyPassModal : MonoBehaviour
{
  public void Buy()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_WINTERPASS, (isSuccse, bundelId) =>
    {
      if (isSuccse)
      {
        GameManager.Instance.IsUnlockFollowing = true;
        LobbyPanel.Instance.ShowNotifyWith("Purchase successful");
        LobbyPanel.Instance.ToggleDailyBuyPass();
      }
      else
      {
        LobbyPanel.Instance.ShowNotifyWith("purchase failed");
      }
    });
  }
}
