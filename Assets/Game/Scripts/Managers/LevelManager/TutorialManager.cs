using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public partial class LevelManager
{
    [SerializeField] Tutorial tutorial;
    [SerializeField] Sprite booster1Sprite;
    [SerializeField] Sprite booster2Sprite;
    [SerializeField] Sprite booster3Sprite;
    [SerializeField] Canvas booster1Btn;
    [SerializeField] Canvas booster2Btn;
    [SerializeField] Canvas booster3Btn;
    // dau game
    void StartTutorial1()
    {
        if (GameManager.Instance.CurrentLevelIndex != 0) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_1, 0) == 1) return;
        tutorial.DoArrowMoveAt(KeyString.KEY_TUTORIAL_1, 0, 180);
        tutorial.ShowTutorialPanelAt(KeyString.KEY_TUTORIAL_1, "Match 3 colors");
    }
    void StopTutorial1()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_1)) return;
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_1, 1);
        tutorial.StopTutorial();
    }
    // level 2

    void StartTutorial2()
    {
        if (GameManager.Instance.CurrentLevelIndex != 1) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_2, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowTutorialPanelAt(KeyString.KEY_TUTORIAL_2, "Blocks can be match in any order", true, () => StopTutorial2());
    }

    void StopTutorial2()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_2)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_2, 1);
        tutorial.StopTutorial();
    }
    // booster 1

    void StartTutorial3()
    {
        if (GameManager.Instance.CurrentLevelIndex != 4) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_3, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowReceivePanel(KeyString.KEY_TUTORIAL_3, booster1Sprite, () =>
        {
            StartTutorial3pass2();
            GameManager.Instance.Booster1++;
        });
    }
    void StartTutorial3pass2()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_3)) return;
        booster1Btn.overrideSorting = true;
        tutorial.ShowDarkPanel();
        tutorial.DoArrowMoveAt(KeyString.KEY_TUTORIAL_3, 1, 180);
    }

    public void StopTutorial3()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_3)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        booster1Btn.overrideSorting = false;
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_3, 1);
        tutorial.StopTutorial();
    }
    //booster 2
    void StartTutorial4()
    {
        if (GameManager.Instance.CurrentLevelIndex != 9) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_4, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowReceivePanel(KeyString.KEY_TUTORIAL_4, booster2Sprite, () =>
        {
            StartTutorial4pass2();
            GameManager.Instance.Booster2++;
        });
    }
    void StartTutorial4pass2()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_4)) return;
        booster2Btn.overrideSorting = true;
        tutorial.ShowDarkPanel();
        tutorial.DoArrowMoveAt(KeyString.KEY_TUTORIAL_4, 2, 180);
    }

    public void StopTutorial4()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_4)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        booster2Btn.overrideSorting = false;
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_4, 1);
        tutorial.StopTutorial();
    }
    // booster 3

    void StartTutorial5()
    {
        if (GameManager.Instance.CurrentLevelIndex != 14) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_5, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowReceivePanel(KeyString.KEY_TUTORIAL_5, booster3Sprite, () =>
        {
            StartTutorial5pass2();
            GameManager.Instance.Booster3++;
        });
    }
    void StartTutorial5pass2()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_5)) return;
        booster3Btn.overrideSorting = true;
        tutorial.ShowDarkPanel();
        tutorial.DoArrowMoveAt(KeyString.KEY_TUTORIAL_5, 3, 180);
    }

    public void StopTutorial5()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_5)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        booster3Btn.overrideSorting = false;
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_5, 1);
        tutorial.StopTutorial();
    }
    // tunnel
    void StartTutorial6()
    {
        if (GameManager.Instance.CurrentLevelIndex != 5) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_6, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowTutorialPanelAt(KeyString.KEY_TUTORIAL_6, "Block come out of tunnel");
        tutorial.ShowDarkPanel(() => StopTutorial6());
        ShowTunnel();
    }
    void StopTutorial6()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_6)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_6, 1);
        HideTunnel();
        tutorial.StopTutorial();
    }
    void ShowTunnel()
    {
        for (var i = 0; i < _directionBlocks.Length; i++)
        {
            var tunnel = _directionBlocks[i];
            if (tunnel == null) continue;
            if (!tunnel.TryGetComponent(out TunnelControl component)) continue;

            if (!tunnel.TryGetComponent(out SortingGroup sortingGroup))
                sortingGroup = tunnel.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Notif";
        }
    }

    void HideTunnel()
    {
        for (var i = 0; i < _directionBlocks.Length; i++)
        {
            var tunnel = _directionBlocks[i];
            if (tunnel == null) continue;
            if (!tunnel.TryGetComponent(out TunnelControl component)) continue;
            if (!tunnel.TryGetComponent(out SortingGroup sortingGroup)) continue;
            Destroy(sortingGroup);
        }
    }
    // woodenblock
    void StartTutorial7()
    {
        if (GameManager.Instance.CurrentLevelIndex != 10) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_7, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowTutorialPanelAt(KeyString.KEY_TUTORIAL_7, "Break the box by clear the part");
        tutorial.ShowDarkPanel(() => StopTutorial7());
        ShowWoodenBlock();
    }
    void StopTutorial7()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_7)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_7, 1);
        HideWoodenBlock();
        tutorial.StopTutorial();
    }
    void ShowWoodenBlock()
    {
        for (var i = 0; i < _directionBlocks.Length; i++)
        {
            var tunnel = _directionBlocks[i];
            if (tunnel == null) continue;
            if (!tunnel.TryGetComponent(out WoodenBlockControl component)) continue;

            if (!tunnel.TryGetComponent(out SortingGroup sortingGroup))
                sortingGroup = tunnel.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Notif";
        }
    }

    void HideWoodenBlock()
    {
        for (var i = 0; i < _directionBlocks.Length; i++)
        {
            var tunnel = _directionBlocks[i];
            if (tunnel == null) continue;
            if (!tunnel.TryGetComponent(out WoodenBlockControl component)) continue;
            if (!tunnel.TryGetComponent(out SortingGroup sortingGroup)) continue;
            sortingGroup.sortingLayerName = "Item";
        }
    }
    // Ice Block
    void StartTutorial8()
    {
        if (GameManager.Instance.CurrentLevelIndex != 15) return;
        if (PlayerPrefs.GetInt(KeyString.KEY_TUTORIAL_8, 0) == 1) return;
        GameManager.Instance.SetGameState(GameState.GamepPause);
        tutorial.ShowTutorialPanelAt(KeyString.KEY_TUTORIAL_8, "Break the ice by making moves to free the frozen block.");
        tutorial.ShowDarkPanel(() => StopTutorial8());
        ShowIceBlock();
    }
    void StopTutorial8()
    {
        if (!tutorial.IsTutorialAt(KeyString.KEY_TUTORIAL_8)) return;
        GameManager.Instance.SetGameState(GameState.Gameplay);
        PlayerPrefs.SetInt(KeyString.KEY_TUTORIAL_8, 1);
        HideIceBlock();
        tutorial.StopTutorial();
    }
    void ShowIceBlock()
    {
        for (var i = 0; i < _directionBlocks.Length; i++)
        {
            var tunnel = _directionBlocks[i];
            if (tunnel == null) continue;
            if (!tunnel.TryGetComponent(out IceBlockControl component)) continue;

            if (!tunnel.TryGetComponent(out SortingGroup sortingGroup))
                sortingGroup = tunnel.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "Notif";
        }
    }

    void HideIceBlock()
    {
        for (var i = 0; i < _directionBlocks.Length; i++)
        {
            var tunnel = _directionBlocks[i];
            if (tunnel == null) continue;
            if (!tunnel.TryGetComponent(out IceBlockControl component)) continue;
            if (!tunnel.TryGetComponent(out SortingGroup sortingGroup)) continue;
            sortingGroup.sortingLayerName = "Item";
        }
    }
}