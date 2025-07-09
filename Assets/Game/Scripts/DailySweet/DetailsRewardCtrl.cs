using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DetailsRewardCtrl : MonoBehaviour
{
    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI amountTxt;

    public void SetUp(Sprite icon, int amount)
    {
        iconImg.sprite = icon;
        amountTxt.text = amount.ToString();
    }
}
