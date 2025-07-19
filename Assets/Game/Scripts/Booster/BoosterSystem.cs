using UnityEngine;

public partial class GameplayPanel
{
    [SerializeField] BoosterCtrl booster1Ctrl;
    [SerializeField] BoosterCtrl booster2Ctrl;
    [SerializeField] BoosterCtrl booster3Ctrl;
    int levelUnlockBooster1 = 0;
    int levelUnlockBooster2 = 0;
    int levelUnlockBooster3 = 0;
    void InitBooster()
    {
        VisualeTriggerBooster1();
        VisualeTriggerBooster2();
        VisualeTriggerBooster3();
        GameManager.Instance.OnBooster1Change += VisualeTriggerBooster1;
        GameManager.Instance.OnBooster2Change += VisualeTriggerBooster2;
        GameManager.Instance.OnBooster3Change += VisualeTriggerBooster3;
    }

    void UnsubscribeBoosterEvent()
    {
        GameManager.Instance.OnBooster1Change -= VisualeTriggerBooster1;
        GameManager.Instance.OnBooster2Change -= VisualeTriggerBooster2;
        GameManager.Instance.OnBooster3Change -= VisualeTriggerBooster3;
    }

    public void OnTriggerBooster1()
    {
        if (GameManager.Instance.CurrentLevel < levelUnlockBooster1) return;
        if (GameManager.Instance.Booster1 <= 0)
            ToggleBooster1Modal();
        else
        {
            var IsTriggerBooster1 = LevelManager.Instance.IsTriggerBooster1;
            LevelManager.Instance.IsTriggerBooster1 = !IsTriggerBooster1;
            //visualize trigger booster
        }
    }

    public void OnTriggerBooster2()
    {
        if (GameManager.Instance.CurrentLevel < levelUnlockBooster2) return;

        if (GameManager.Instance.Booster2 <= 0)
            ToggleBooster2Modal();
        else
        {
            LevelManager.Instance.OnTriggerBooster2();
            //visualize trigger booster
        }
    }

    public void OnTriggerBooster3()
    {
        if (GameManager.Instance.CurrentLevel < levelUnlockBooster3) return;
        if (GameManager.Instance.Booster3 <= 0)
            ToggleBooster3Modal();
        else
        {
            LevelManager.Instance.OntriggerBooster3();
            //visualize trigger booster
        }
    }

    void VisualeTriggerBooster1()
    {
        if (GameManager.Instance.CurrentLevel < levelUnlockBooster1)
        {
            booster1Ctrl.Lock();
            return;
        }

        var amount = GameManager.Instance.Booster1;
        if (amount <= 0)
            booster1Ctrl.Empty();
        else
        {
            booster1Ctrl.Unlock();
            booster1Ctrl.SetAmountBooster(amount);
        }
    }

    void VisualeTriggerBooster2()
    {
        if (GameManager.Instance.CurrentLevel < levelUnlockBooster2)
        {
            booster2Ctrl.Lock();
            return;
        }

        var amount = GameManager.Instance.Booster2;
        if (amount <= 0)
            booster2Ctrl.Empty();
        else
        {
            booster2Ctrl.Unlock();
            booster2Ctrl.SetAmountBooster(amount);
        }
    }

    void VisualeTriggerBooster3()
    {
        if (GameManager.Instance.CurrentLevel < levelUnlockBooster3)
        {
            booster3Ctrl.Lock();
            return;
        }

        var amount = GameManager.Instance.Booster3;
        if (amount <= 0)
            booster3Ctrl.Empty();
        else
        {
            booster3Ctrl.Unlock();
            booster3Ctrl.SetAmountBooster(amount);
        }
    }

    public void BuyBooster1()
    {
        int price = 100;
        if (GameManager.Instance.CurrentCoin < price)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }

        GameManager.Instance.CurrentCoin -= price;
        GameManager.Instance.Booster1++;
    }
    public void BuyBooster2()
    {
        int price = 100;
        if (GameManager.Instance.CurrentCoin < price)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }

        GameManager.Instance.CurrentCoin -= price;
        GameManager.Instance.Booster2++;
    }
    public void BuyBooster3()
    {
        int price = 100;
        if (GameManager.Instance.CurrentCoin < price)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }

        GameManager.Instance.CurrentCoin -= price;
        GameManager.Instance.Booster3++;
    }
}
