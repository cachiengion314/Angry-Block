using Firebase.Analytics;
using UnityEngine;
using UnityEngine.Rendering;

public partial class GameplayPanel
{
    [SerializeField] SortingGroup sortingGroup;
    [SerializeField] BoosterCtrl booster1Ctrl;
    [SerializeField] BoosterCtrl booster2Ctrl;
    [SerializeField] BoosterCtrl booster3Ctrl;
    int levelUnlockBooster1 = 4;
    int levelUnlockBooster2 = 9;
    int levelUnlockBooster3 = 14;
    public bool IsTriggerBooster1 { get; private set; }
    void InitBooster()
    {
        VisualeTriggerBooster1();
        VisualeTriggerBooster2();
        VisualeTriggerBooster3();
        GameManager.Instance.OnBooster1Change += VisualeTriggerBooster1;
        GameManager.Instance.OnBooster2Change += VisualeTriggerBooster2;
        GameManager.Instance.OnBooster3Change += VisualeTriggerBooster3;
        LevelManager.Instance.OnTriggerBooster1Success += OnTriggerBooster1;
    }

    void UnsubscribeBoosterEvent()
    {
        GameManager.Instance.OnBooster1Change -= VisualeTriggerBooster1;
        GameManager.Instance.OnBooster2Change -= VisualeTriggerBooster2;
        GameManager.Instance.OnBooster3Change -= VisualeTriggerBooster3;
        LevelManager.Instance.OnTriggerBooster1Success -= OnTriggerBooster1;
    }

    public void OnTriggerBooster1()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        if (GameManager.Instance.CurrentLevelIndex < levelUnlockBooster1) return;
        LevelManager.Instance.StopTutorial3();
        if (GameManager.Instance.Booster1 <= 0)
            ToggleBooster1Modal();
        else
            ToggleUseBooster1();
    }

    void ToggleUseBooster1()
    {
        IsTriggerBooster1 = !IsTriggerBooster1;
        UseBooster1Modal.gameObject.SetActive(IsTriggerBooster1);
        sortingGroup.enabled = IsTriggerBooster1;
    }

    public void OnTriggerBooster2()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        if (GameManager.Instance.CurrentLevelIndex < levelUnlockBooster2) return;
        LevelManager.Instance.StopTutorial4();
        if (GameManager.Instance.Booster2 <= 0)
            ToggleBooster2Modal();
        else
        {
            sortingGroup.enabled = true;
            UseBooster2Modal.gameObject.SetActive(true);
            GameManager.Instance.SetGameState(GameState.GamepPause);
        }
    }

    public void ExitBooster2()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        sortingGroup.enabled = false;
        UseBooster2Modal.gameObject.SetActive(false);
        GameManager.Instance.SetGameState(GameState.Gameplay);
    }

    public void OnTriggerBooster2Success()
    {
        LevelManager.Instance.OnTriggerBooster2();
        ExitBooster2();
    }

    public void OnTriggerBooster3()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        if (GameManager.Instance.CurrentLevelIndex < levelUnlockBooster3) return;
        LevelManager.Instance.StopTutorial5();
        if (GameManager.Instance.Booster3 <= 0)
            ToggleBooster3Modal();
        else
        {
            sortingGroup.enabled = true;
            UseBooster3Modal.gameObject.SetActive(true);
            GameManager.Instance.SetGameState(GameState.GamepPause);
        }
    }

    public void ExitBooster3()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        sortingGroup.enabled = false;
        UseBooster3Modal.gameObject.SetActive(false);
        GameManager.Instance.SetGameState(GameState.Gameplay);
    }

    public void OnTriggerBooster3Success()
    {
        LevelManager.Instance.OntriggerBooster3();
        ExitBooster3();
    }

    void VisualeTriggerBooster1()
    {
        if (GameManager.Instance.CurrentLevelIndex < levelUnlockBooster1)
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
        if (GameManager.Instance.CurrentLevelIndex < levelUnlockBooster2)
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
        if (GameManager.Instance.CurrentLevelIndex < levelUnlockBooster3)
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
        SoundManager.Instance.PlayPressBtnSfx();
        int price = 100;
        if (GameManager.Instance.CurrentCoin < price)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }

        GameManager.Instance.CurrentCoin -= price;
        GameManager.Instance.Booster1++;
        ToggleBooster1Modal();

        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_COIN_USE,
    new Parameter[]
    {
            new ("purpose", "BuyBooster1"),
    });
    }
    public void BuyBooster2()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        int price = 100;
        if (GameManager.Instance.CurrentCoin < price)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }

        GameManager.Instance.CurrentCoin -= price;
        GameManager.Instance.Booster2++;
        ToggleBooster2Modal();

        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_COIN_USE,
new Parameter[]
{
            new ("purpose", "BuyBooster2"),
});
    }
    public void BuyBooster3()
    {
        SoundManager.Instance.PlayPressBtnSfx();
        int price = 100;
        if (GameManager.Instance.CurrentCoin < price)
        {
            ShowNotifyWith("NOT ENOUGH COINS");
            return;
        }

        GameManager.Instance.CurrentCoin -= price;
        GameManager.Instance.Booster3++;
        ToggleBooster3Modal();

        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_COIN_USE,
new Parameter[]
{
            new ("purpose", "BuyBooster3"),
});
    }
}
