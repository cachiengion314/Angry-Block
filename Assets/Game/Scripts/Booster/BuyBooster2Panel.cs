using UnityEngine;

public class BuyBooster2Panel : MonoBehaviour
{
    int price = 100;
    public void BuyBooster2()
    {
        if (GameManager.Instance.CurrentCoin >= price)
        {
            GameManager.Instance.CurrentCoin -= price;
            GameManager.Instance.Booster2++;
        }
    }
}
