using UnityEngine;

public class BoosterSystem : MonoBehaviour
{
    [SerializeField] BoosterCtrl booster1Ctrl;
    [SerializeField] BoosterCtrl booster2Ctrl;
    [SerializeField] BoosterCtrl booster3Ctrl;
    void Start()
    {
        VisualeTriggerBooster1();
        VisualeTriggerBooster2();
        VisualeTriggerBooster3();
        GameManager.Instance.OnBooster1Change += VisualeTriggerBooster1;
        GameManager.Instance.OnBooster2Change += VisualeTriggerBooster2;
        GameManager.Instance.OnBooster3Change += VisualeTriggerBooster3;
    }

    void Oestroy()
    {
        GameManager.Instance.OnBooster1Change -= VisualeTriggerBooster1;
        GameManager.Instance.OnBooster2Change -= VisualeTriggerBooster2;
        GameManager.Instance.OnBooster3Change -= VisualeTriggerBooster3;
    }

    public void OnTriggerBooster1()
    {
        if (GameManager.Instance.CurrentLevel > 3)
        {
            if (GameManager.Instance.Booster1 <= 0)
            {
                GameplayPanel.Instance.ToggleBooster1Modal();
            }
            else
            {
                GameManager.Instance.Booster1--;
            }
        }
    }

    public void OnTriggerBooster2()
    {
        if (GameManager.Instance.CurrentLevel > 6)
        {
            if (GameManager.Instance.Booster2 <= 0)
            {
                GameplayPanel.Instance.ToggleBooster2Modal();
            }
            else
            {
                GameManager.Instance.Booster2--;
            }
        }
    }

    public void OnTriggerBooster3()
    {
        if (GameManager.Instance.CurrentLevel > 9)
        {
            if (GameManager.Instance.Booster3 <= 0)
            {
                GameplayPanel.Instance.ToggleBooster3Modal();
            }
            else
            {
                GameManager.Instance.Booster3--;
            }
        }
    }

    void VisualeTriggerBooster1()
    {
        if (GameManager.Instance.CurrentLevel < 3)
        {
            booster1Ctrl.Lock();
        }
        else
        {
            if (GameManager.Instance.Booster1 <= 0)
                booster1Ctrl.Empty();
            else
                booster1Ctrl.Unlock();
        }
    }

    void VisualeTriggerBooster2()
    {
        if (GameManager.Instance.CurrentLevel < 3)
        {
            booster2Ctrl.Lock();
        }
        else
        {
            if (GameManager.Instance.Booster2 <= 0)
                booster2Ctrl.Empty();
            else
                booster2Ctrl.Unlock();
        }
    }

    void VisualeTriggerBooster3()
    {
        if (GameManager.Instance.CurrentLevel < 3)
        {
            booster3Ctrl.Lock();
        }
        else
        {
            if (GameManager.Instance.Booster3 <= 0)
                booster3Ctrl.Empty();
            else
                booster3Ctrl.Unlock();
        }
    }
}
