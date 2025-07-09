using UnityEngine;

public class NoticeDailyBonus : MonoBehaviour
{
    [SerializeField] GameObject noticeDailyBonusObj;
    void Start()
    {
        Show();
        GameManager.onNoticeDailyBounsChange += Show;
    }
    void OnDestroy()
    {
        GameManager.onNoticeDailyBounsChange -= Show;
    }

    void Show()
    {
        noticeDailyBonusObj.SetActive(GameManager.Instance.IsShowNoticeDailyBouns);
    }
}
