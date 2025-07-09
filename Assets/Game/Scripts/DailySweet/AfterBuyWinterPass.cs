using UnityEngine;

public class AfterBuyWinterPass : MonoBehaviour
{
    private void Start()
    {
        GameManager.onBuyUnlockFollowing += OffGameObject;
        if (GameManager.Instance.IsUnlockFollowing)
        {
            OffGameObject();
        }
    }
    private void OnDestroy()
    {
        GameManager.onBuyUnlockFollowing -= OffGameObject;
    }
    void OffGameObject()
    {
        gameObject.SetActive(false);
    }
}
