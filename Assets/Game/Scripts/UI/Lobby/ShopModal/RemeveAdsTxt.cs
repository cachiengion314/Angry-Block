using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RemeveAdsTxt : MonoBehaviour
{
    [SerializeField] string productId;
    [SerializeField] TextMeshProUGUI removeAds;
    [SerializeField] Button removeAdsBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Buyed();
    }

    public void Buyed()
    {
        removeAds = GetComponent<TextMeshProUGUI>();
        if (GameManager.Instance.IsRemoveAds){
            removeAdsBtn.enabled = false;
            removeAds.SetText("Purchased");
        }
        else SetText();
    }

    private void SetText()
    {
        string price = IAPManager.Instance.GetLocalizedPrice(productId);
        if (price == null)
            Invoke(nameof(SetText), Time.deltaTime);
        else
            removeAds.SetText(price);
    }
}
