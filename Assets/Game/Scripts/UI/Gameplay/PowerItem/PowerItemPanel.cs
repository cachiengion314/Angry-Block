using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Analytics;
using HoangNam;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public partial class PowerItemPanel : MonoBehaviour
{
  public static PowerItemPanel Instance { get; private set; }

  [Header("Internal dependencies")]
  [SerializeField] Button xBtn;
  [SerializeField] Image refreshImg;
  [SerializeField] Image hammerImg;
  [SerializeField] Image rocketImg;
  [SerializeField] Image swapImg;
  [SerializeField] TMP_Text[] itemAmountTexts;
  [SerializeField] GameObject[] lockImgs;
  [SerializeField] Button[] buyItemBtns;
  [SerializeField] Button[] buyItemByAdsBtns;
  [SerializeField] RectTransform[] powerItems;
  [SerializeField] GridWorld trayGrid;
  [SerializeField] TutorialManager tutorialManager;
  int[] powerItemAmounts;

  [Header("Settings")]
  public bool IsTriggeredHammer;
  public bool IsTriggeredRocket;
  public bool IsTriggerSwap;
  public bool IsTriggerRefresh;
  public TrayControl[] SwappingTrays;
  LevelDesignObj levelDesignObj;

  // v1.5
  [SerializeField] SortingGroup tableDepotImg;
  [SerializeField] GameObject demoBGImg;
  [SerializeField] Button yesRefreshBtn;
  [SerializeField] Button noRefreshBtn;

  private float3[] posMoveUp;
  private string[] _nameBoosters = { "Hammer", "Rocket", "Swap", "Refresh" };

  private bool[] _isBuyedByAds = new bool[4] { false, false, false, false };

  private void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    powerItemAmounts = new int[] {
      GameManager.Instance.CurrentRefresh,
      GameManager.Instance.CurrentHammer,
      GameManager.Instance.CurrentRocket,
      GameManager.Instance.CurrentSwap,
    };
    SwappingTrays = new TrayControl[2];
    posMoveUp = new float3[2];

    ShowPowerItems();
    UpdateUI();
    demoBGImg.SetActive(false);
    tableDepotImg.enabled = false;
    yesRefreshBtn.gameObject.SetActive(false);
    noRefreshBtn.gameObject.SetActive(false);
  }

  public void DeTrigger()
  {
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;
    // Block  when showing tutorial
    if (GameManager.Instance.CurrentLevel == 4)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER, 0) == 0) return;
    }

    if (GameManager.Instance.CurrentLevel == 7)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, 0) == 0) return;
    }

    if (GameManager.Instance.CurrentLevel == 9)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, 0) == 0) return;
    }

    if (SwappingTrays[0] != null && SwappingTrays[1] != null)
    {
      if (LeanTween.isTweening(SwappingTrays[0].gameObject) && LeanTween.isTweening(SwappingTrays[1].gameObject))
      {
        return;
      }
    }

    _isClickedHammer = true;
    _isClickedSwap = true;
    _isClickedRocket = true;

    SoundManager.Instance.PlayPressBtnSfx();

    StopMoveUp();
    DeTriggerSwap();
    DeTriggerHammer();
    DeTriggerRocket();
    ShowPanel.Instance.HideAllDemo();
    tableDepotImg.enabled = false;
    demoBGImg.SetActive(false);

    LeanTween.delayedCall(gameObject, 1.5f, () =>
    {
      _isClickedHammer = false;
      _isClickedSwap = false;
      _isClickedRocket = false;
    });
  }

  public void DeTriggerSwap()
  {
    IsTriggerSwap = false;
    EffectManager.Instance.TurnOffDarkImg();
    ShowPowerItems();
    ClearSwappingTrays();
  }

  public void DeTriggerHammer()
  {
    IsTriggeredHammer = false;
    EffectManager.Instance.TurnOffDarkImg();
    ShowPowerItems();
  }

  public void DeTriggerRocket()
  {
    IsTriggeredRocket = false;
    EffectManager.Instance.TurnOffDarkImg();
    EffectManager.Instance.TurnOffArrows();
    ShowPowerItems();
  }

  public void ShowPowerItems()
  {
    for (int i = 0; i < powerItems.Length; ++i)
    {
      powerItems[i].gameObject.SetActive(true);
    }
    xBtn.gameObject.SetActive(false);
  }

  public void HidePowerItems()
  {
    for (int i = 0; i < powerItems.Length; ++i)
    {
      powerItems[i].gameObject.SetActive(false);
    }
    xBtn.gameObject.SetActive(true);
  }

  bool _isClickedRespawn;
  public void ReSpawnAvailableTrays()
  {
    if (GameManager.Instance.CurrentRefresh <= 0) return;
    if (IsTriggeredHammer) return;
    if (IsTriggeredRocket) return;
    if (IsTriggerSwap) return;
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;
    // Block Item when showing tutorial
    if (GameManager.Instance.CurrentLevel == 4)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER, 0) == 0) return;
    }
    if (GameManager.Instance.CurrentLevel == 7)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, 0) == 0) return;
    }
    if (GameManager.Instance.CurrentLevel == 9)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, 0) == 0) return;
    }

    if (_isClickedRespawn) return;
    _isClickedRespawn = true;
    IsTriggerRefresh = true;
    StartCoroutine(IEReSpawnAvailableTrays());
  }

  bool _isCompletedRefresh;
  IEnumerator IEReSpawnAvailableTrays()
  {
    _isCompletedRefresh = false;
    ShowPanel.Instance.ShowDemoBoosterAt(0);
    yesRefreshBtn.gameObject.SetActive(true);
    noRefreshBtn.gameObject.SetActive(true);
    tableDepotImg.enabled = true;
    demoBGImg.SetActive(true);
    GameManager.Instance.SetGameState(GameState.GamepPause);
    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH, 0) == 0)
    {
      TutorialManager.Instance.PlayBooster1AnimAt(1);
    }

    yield return new WaitUntil(() => _isCompletedRefresh);

    yesRefreshBtn.gameObject.SetActive(false);
    noRefreshBtn.gameObject.SetActive(false);
    GameManager.Instance.SetGameState(GameState.Gameplay);
    IsTriggerRefresh = false;

    if (_isUseRefresh)
    {
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH, 1);

      AnimationManager.Instance.PlayScale(refreshImg.transform, 2.5f);

      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_REFRESH) == 0)
        PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_REFRESH, 1);
      else
        GameManager.Instance.CurrentRefresh--;

      var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.UsePowerItem, 1);
      EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.UsePowerItem, dataWeekly);

      var data = new MissonDataDailyTask(enumListeningDataDailyTask.Booster1, 1);
      EventActionManager.TriggerEvent(enumListeningDataDailyTask.Booster1, data);


      UpdateAmountUI();

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_SPEND,
           new Parameter[]
           {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("booster_name", "Refresh"),
            new ("placement", "Ingame")
           });
      }
    }

    DeTrigger();
    LeanTween.delayedCall(gameObject, 0.5f, () =>
    {
      _isClickedRespawn = false;
    });
  }

  bool _isUseRefresh;
  public void AcceptUseRefresh()
  {
    _isUseRefresh = true;
    _isCompletedRefresh = true;
  }

  public void RefuseUseRefresh()
  {
    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH, 0) == 0) return;

    _isUseRefresh = false;
    _isCompletedRefresh = true;
  }

  bool _isClickedHammer;
  public void TriggerHammer()
  {
    if (GameManager.Instance.CurrentHammer <= 0) return;
    if (IsTriggeredHammer) return;
    if (IsTriggeredRocket) return;
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;
    // Block Item when showing tutorial
    if (GameManager.Instance.CurrentLevel == 7)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, 0) == 0) return;
    }
    if (GameManager.Instance.CurrentLevel == 9)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, 0) == 0) return;
    }

    if (_isClickedHammer) return;

    HidePowerItems();
    IsTriggeredHammer = true;
    AnimationManager.Instance.PlayScale(hammerImg.transform, 2.5f);
    EffectManager.Instance.TurnOnDarkImg();
    ShowPanel.Instance.ShowDemoBoosterAt(1);
    tableDepotImg.enabled = true;
    demoBGImg.SetActive(true);
  }

  bool _isClickedRocket;
  public void TriggerRocket()
  {
    if (GameManager.Instance.CurrentRocket <= 0) return;
    if (IsTriggeredHammer) return;
    if (IsTriggeredRocket) return;
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;
    // Block Item when showing tutorial
    if (GameManager.Instance.CurrentLevel == 9)
    {
      if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, 0) == 0) return;
    }
    if (_isClickedRocket) return;

    HidePowerItems();
    IsTriggeredRocket = true;
    AnimationManager.Instance.PlayScale(rocketImg.transform, 2.5f);
    EffectManager.Instance.TurnOnDarkImg();
    EffectManager.Instance.TurnOnArrows();
    ShowPanel.Instance.ShowDemoBoosterAt(2);
    tableDepotImg.enabled = true;
    demoBGImg.SetActive(true);
  }

  public bool IsSwappingTraysFull()
  {
    if (!IsTriggerSwap) return false;
    for (int i = 0; i < SwappingTrays.Length; ++i)
      if (SwappingTrays[i] == null) return false;
    return true;
  }

  public void ClearSwappingTrays()
  {
    for (int i = 0; i < SwappingTrays.Length; ++i)
    {
      if (SwappingTrays[i] == null) continue;
      SwappingTrays[i] = null;
    }
  }

  public void StopMoveUp()
  {
    for (int i = 0; i < SwappingTrays.Length; ++i)
    {
      if (SwappingTrays[i] == null) continue;
      if (LeanTween.isTweening(SwappingTrays[i].gameObject))
      {
        LeanTween.cancel(SwappingTrays[i].gameObject);
        LeanTween.move(SwappingTrays[i].gameObject, SwappingTrays[i].CurrentWorldPos, 0.2f);
        SwappingTrays[i].PlacedAt(SwappingTrays[i].CurrentWorldPos);
      }
    }
  }

  bool _isTutorialRefresh = false;
  public void TryPickSwappingTray(TrayControl tray)
  {

  }

  bool _isClickedSwap;
  public void TriggerSwap()
  {
    if (GameManager.Instance.CurrentSwap <= 0) return;
    if (IsTriggeredHammer) return;
    if (IsTriggeredRocket) return;
    if (IsTriggerSwap) return;
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

    if (_isClickedSwap) return;

    IsTriggerSwap = true;
    HidePowerItems();

    AnimationManager.Instance.PlayScale(swapImg.transform, 2.5f);
    EffectManager.Instance.TurnOnDarkImg();
    ShowPanel.Instance.ShowDemoBoosterAt(3);
    tableDepotImg.enabled = true;
    demoBGImg.SetActive(true);
  }

  public void ShowBuyButtonAt(int i)
  {
    // if(GameManager.Instance.CurrentCoin >= 
    string priceStr = buyItemBtns[i].GetComponentInChildren<TMP_Text>().text;
    int.TryParse(priceStr, out int result);

    if (GameManager.Instance.CurrentCoin >= result || _isBuyedByAds[i])
    {
      buyItemBtns[i].gameObject.SetActive(true);
      buyItemByAdsBtns[i].gameObject.SetActive(false);
    }
    else
    {
      buyItemBtns[i].gameObject.SetActive(false);
      buyItemByAdsBtns[i].gameObject.SetActive(true);
    }
  }

  public void DontShowBuyButtonAt(int i)
  {
    buyItemBtns[i].gameObject.SetActive(false);
    buyItemByAdsBtns[i].gameObject.SetActive(false);
  }

  public void ShowLockAt(int i)
  {
    lockImgs[i].SetActive(true);
    if (i == 0) hammerImg.gameObject.SetActive(false);
    if (i == 1) rocketImg.gameObject.SetActive(false);
    if (i == 2) swapImg.gameObject.SetActive(false);
    if (i == 3) refreshImg.gameObject.SetActive(false);
    itemAmountTexts[i].gameObject.SetActive(false);
  }

  public void ShowUnlockAt(int i)
  {
    lockImgs[i].SetActive(false);
    if (i == 0) hammerImg.gameObject.SetActive(true);
    if (i == 1) rocketImg.gameObject.SetActive(true);
    if (i == 2) swapImg.gameObject.SetActive(true);
    if (i == 3) refreshImg.gameObject.SetActive(true);
    itemAmountTexts[i].gameObject.SetActive(true);
  }

  public void UpdateUI()
  {
    for (int i = 0; i < levelDesignObj.powerUnlockItems.Length; ++i)
    {
      if (levelDesignObj.powerUnlockItems[i])
      {
        ShowUnlockAt(i);
        continue;
      }
      ShowLockAt(i);
    }

    if (levelDesignObj.powerUnlockItems.Length == 3)
    {
      ShowLockAt(3);
    }

    UpdateAmountUI();
  }

  public void UpdateBuyItemBtns()
  {
    for (int i = 0; i < buyItemBtns.Length; ++i)
    {
      string priceStr = buyItemBtns[i].GetComponentInChildren<TMP_Text>().text;
      int.TryParse(priceStr, out int result);
      if (GameManager.Instance.CurrentCoin < result)
      {
        buyItemBtns[i].interactable = false;
      }
      else
      {
        buyItemBtns[i].interactable = true;
      }
    }
  }

  public void UpdateAmountUI()
  {
    powerItemAmounts[0] = GameManager.Instance.CurrentHammer;
    powerItemAmounts[1] = GameManager.Instance.CurrentRocket;
    powerItemAmounts[2] = GameManager.Instance.CurrentSwap;
    powerItemAmounts[3] = GameManager.Instance.CurrentRefresh;

    for (int i = 0; i < buyItemBtns.Length; ++i)
    {
      itemAmountTexts[i].text = powerItemAmounts[i].ToString();
      if (powerItemAmounts[i] <= 0)
      {
        ShowBuyButtonAt(i);
        continue;
      }
      DontShowBuyButtonAt(i);
    }
  }

  public void BuyItemFrom(int index)
  {
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

    SoundManager.Instance.PlayPressBtnSfx();
    BalloonSystem.Instance.ShowBalloon();

    string priceStr = buyItemBtns[index].GetComponentInChildren<TMP_Text>().text;
    int.TryParse(priceStr, out int result);
    if (GameManager.Instance.CurrentCoin < result)
    {
      // show free coin popup
      if (GameManager.Instance.GetGameState() != GameState.Gameover)
      {
        GameManager.Instance.SetGameState(GameState.GamepPause);
      }
      // ShowPanel.Instance.HideHomeBtn();
      BalloonSystem.Instance.HideBalloon();
      return;
    }

    ShowPanel.Instance.ShowTextAt(buyItemBtns[index].transform.position, "-" + priceStr, Color.yellow, null, .7f, .7f);

    GameManager.Instance.CurrentCoin -= result;
    if (index == 0)
    {
      GameManager.Instance.CurrentHammer++;
    }
    else if (index == 1)
    {
      GameManager.Instance.CurrentRocket++;
    }
    else if (index == 2)
    {
      GameManager.Instance.CurrentSwap++;
    }
    else
    {
      GameManager.Instance.CurrentRefresh++;
    }

    UpdateAmountUI();
    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
         new Parameter[]
         {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("booster_name", _nameBoosters[index]),
            new ("placement", "BuyIngame_WithCoin")
         });
    }
  }

  public void BuyItemByAdsFrom(int index)
  {
    if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

    SoundManager.Instance.PlayPressBtnSfx();
    LevelPlayAds.Instance.ShowRewardedAd(
      () =>
      {
        ShowPanel.Instance.ShowTextAt(buyItemBtns[index].transform.position, "+1", Color.white, null, .7f, .7f);

        if (index == 0)
        {
          GameManager.Instance.CurrentHammer++;
        }
        else if (index == 1)
        {
          GameManager.Instance.CurrentRocket++;
        }
        else if (index == 2)
        {
          GameManager.Instance.CurrentSwap++;
        }
        else
        {
          GameManager.Instance.CurrentRefresh++;
        }

        _isBuyedByAds[index] = true;
        UpdateAmountUI();
        if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
        {
          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
             new Parameter[]
             {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("booster_name", _nameBoosters[index]),
            new ("placement", "BuyIngame_WithAds")
             });
        }
      },
      "BuyBoosterByAds",
      () =>
      {

      }
    );
  }

  // Duy
  private void TryMergeTrayNeighbor()
  {
    HashSet<TrayControl> _linkedTrays1 = new();
    HashSet<TrayControl> _linkedTrays2 = new();

    if (SwappingTrays[0] == null) return;
    if (SwappingTrays[1] == null) return;

    var neighborDraggedTrays = SwappingTrays[0].FindNeighborTraysAt(SwappingTrays[0].CurrentWorldPos);
    var neighborDraggedTrays2 = SwappingTrays[1].FindNeighborTraysAt(SwappingTrays[1].CurrentWorldPos);

    for (int i = 0; i < neighborDraggedTrays.Count; ++i)
    {
      var neighbor = neighborDraggedTrays[i];
      if (neighbor == SwappingTrays[1]) continue;
      _linkedTrays1.Add(neighborDraggedTrays[i]);
    }
    // this will make sure that this tray will be placed at "the the last position" of the set
    _linkedTrays1.Add(SwappingTrays[0]);

    for (int i = 0; i < neighborDraggedTrays2.Count; ++i)
    {
      var neighbor = neighborDraggedTrays2[i];
      if (neighbor == SwappingTrays[0]) continue;
      _linkedTrays2.Add(neighborDraggedTrays2[i]);
    }
    // this will make sure that this tray will be placed at "the the last position" of the set
    _linkedTrays2.Add(SwappingTrays[1]);
  }

  public void ReceiveFreeItemFrom(int index)
  {
    if (index == 0)
    {
      GameManager.Instance.CurrentRefresh++;
    }
    else if (index == 1)
    {
      GameManager.Instance.CurrentHammer++;
    }
    else if (index == 2)
    {
      GameManager.Instance.CurrentRocket++;
    }
    else
    {
      GameManager.Instance.CurrentSwap++;
    }

    UpdateAmountUI();
  }

  public Image GetPowerItemImgFor(int itemIndex)
  {
    return itemIndex switch
    {
      0 => refreshImg,
      1 => hammerImg,
      2 => rocketImg,
      3 => swapImg,
      _ => null,
    };
  }
}
