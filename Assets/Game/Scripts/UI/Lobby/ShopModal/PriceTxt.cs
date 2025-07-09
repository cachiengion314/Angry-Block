using TMPro;
using UnityEngine;

public class PriceTxt : MonoBehaviour
{
    [SerializeField] string productId;
    [SerializeField] TextMeshProUGUI priceTxt;
    private void Start()
    {
        priceTxt = GetComponent<TextMeshProUGUI>();
        SetText();
    }

    private void SetText()
    {
        string price = IAPManager.Instance.GetLocalizedPrice(productId);
        if (price == null)
            Invoke(nameof(SetText), Time.deltaTime);
        else
            priceTxt.SetText(price);
    }
}
