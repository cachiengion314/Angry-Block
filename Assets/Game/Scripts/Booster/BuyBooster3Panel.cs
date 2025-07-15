using UnityEngine;

public class BuyBooster3Panel : MonoBehaviour
{
    int price = 100;
    public void BuyBooster3()
    {
        if (GameManager.Instance.CurrentCoin >= price)
        {
            GameManager.Instance.CurrentCoin -= price;
            GameManager.Instance.Booster3++;
        }
    }
}
