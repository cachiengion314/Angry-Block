using TMPro;
using UnityEngine;

public class BoosterCtrl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI amountBooster;
    [SerializeField] GameObject UnlockBooster;
    [SerializeField] GameObject LockBooster;
    [SerializeField] GameObject EmptyBooster;

    public void SetAmountBooster(int amount)
    {
        amountBooster.text = amount.ToString();
    }

    public void Unlock()
    {
        UnlockBooster.SetActive(true);
        LockBooster.SetActive(false);
        EmptyBooster.SetActive(false);
    }

    public void Lock()
    {
        UnlockBooster.SetActive(false);
        LockBooster.SetActive(true);
        EmptyBooster.SetActive(false);
    }

    public void Empty()
    {
        UnlockBooster.SetActive(false);
        LockBooster.SetActive(false);
        EmptyBooster.SetActive(true);
    }
}
