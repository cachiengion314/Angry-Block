using TMPro;
using UnityEngine;

public class ContentCtrl : MonoBehaviour
{
    [SerializeField] RectTransform panelLock;
    [SerializeField] RectTransform ImgActive;
    [SerializeField] TextMeshProUGUI dayTxt;

    public void SetDay(int day)
    {
        dayTxt.text = day.ToString();
    }

    public void Lock()
    {
        panelLock.gameObject.SetActive(true);
        ImgActive.gameObject.SetActive(false);
    }
    public void Unlock()
    {
        panelLock.gameObject.SetActive(false);
        ImgActive.gameObject.SetActive(false);
    }

    public void ReachMilestone()
    {
        panelLock.gameObject.SetActive(false);
        ImgActive.gameObject.SetActive(true);
    }
}
