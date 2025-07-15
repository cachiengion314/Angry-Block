using UnityEngine;

public class BuyBooster1Panel : MonoBehaviour
{
    int price = 100;
    public void BuyBooster1()
    {
        if (GameManager.Instance.CurrentCoin >= price)
        {
            GameManager.Instance.CurrentCoin -= price;
            GameManager.Instance.Booster1++;
        }
    }
}
