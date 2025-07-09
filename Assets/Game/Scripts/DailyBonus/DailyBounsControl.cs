using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBounsControl : MonoBehaviour
{
    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI textAmount;
    [SerializeField] GameObject greenBtn;
    [SerializeField] GameObject grayBtn;
    [SerializeField] GameObject tickPanl;
    public void Init(RewardData reward)
    {
        iconImg.sprite = reward.Img;
        if (reward.Type == PriceType.Coin) textAmount.text = reward.Value[0].ToString();
        else if (reward.Type == PriceType.InfinityHeart) textAmount.text = $"{reward.Value[0]}m";
        else textAmount.text = $"x{reward.Value[0]}";
    }

    public void NotAccepted()
    {
        tickPanl.SetActive(false);
        grayBtn.SetActive(true);
        greenBtn.SetActive(false);
    }

    public void Accepted()
    {
        tickPanl.SetActive(false);
        grayBtn.SetActive(false);
        greenBtn.SetActive(true);
    }

    public void Complete()
    {
        tickPanl.SetActive(true);
        grayBtn.SetActive(false);
        greenBtn.SetActive(false);
    }
}
