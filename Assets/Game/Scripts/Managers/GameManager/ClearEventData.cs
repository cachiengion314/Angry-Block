using UnityEngine;
public partial class GameManager : MonoBehaviour
{
  public void ClearDataEventLuckyEggHunt()
  {
    PlayerPrefs.SetInt(KeyString.KEY_CURRENT_EGG_GIFT_INDEX, 0);// nhiệm vụ nhận quà
    PlayerPrefs.SetInt(KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT, KeyString.KEY_DEFAULT_MOVE_EGG_TICKETS_AMOUNT);// lượt đi
    PlayerPrefs.SetInt(KeyString.KEY_CURRENT_NEED_SOLVING_EGG_ORDER_INDEX, 0);
    PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_TUTORIAL_GAMEPLAY_EGG, 0);
    PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_MOVE_TUTORIAL_GAMEPLAY_EGG, 0);
  }
}