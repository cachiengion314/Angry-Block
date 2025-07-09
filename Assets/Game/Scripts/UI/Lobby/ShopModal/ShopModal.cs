using System;
using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class ShopModal : MonoBehaviour
{
  public static Action onWatchFreeCoinAds;

  [Header("")]
  [SerializeField] TMP_Text coinText;
  [SerializeField] Image coinImg;
  [SerializeField] Button buyBtnIOS;
  [SerializeField] Button buyBtnAnroid;
  [SerializeField] Button watchAdsBtnIOS;
  [SerializeField] Button watchAdsBtnAndroid;
  [SerializeField] RemeveAdsTxt remeveAdsTxt;
  [SerializeField] Sprite coinSprite;
  [SerializeField] Image iconAdsFreeCoin;

  public int[] earnCoins;

  [Header("FreeCoin Remote")]
  [SerializeField] TMP_Text coinReceivedTxt;

  private const int AMOUNT_FREECOIN_NOADS = 1;

  private void Awake()
  {
    LobbyPanel.onBuyRemoveAdsSucceed += UpdateRemovedAdsTxt;
    FirebaseSetup.onUpdateRemote += UpdateFreeCoinUI;
  }

  private void OnEnable()
  {
    buyBtnIOS.interactable = true;
    buyBtnAnroid.interactable = true;
    watchAdsBtnIOS.interactable = true;
    watchAdsBtnAndroid.interactable = true;

    if (SceneManager.GetActiveScene().name != "LevelEditor")
    {
      UpdateAdsIcon();
    }
  }

  private void Start()
  {
    UpdateFreeCoinUI();
  }

  private void OnDestroy()
  {
    LobbyPanel.onBuyRemoveAdsSucceed -= UpdateRemovedAdsTxt;
    FirebaseSetup.onUpdateRemote -= UpdateFreeCoinUI;
  }

  private void UpdateFreeCoinUI()
  {
    var coinReceive = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.free_coin.ios.coin_received;

#if UNITY_ANDROID
    coinReceive = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.free_coin.android.coin_received;
#endif

    coinReceivedTxt.text = coinReceive.ToString() + " Coins";
  }

  public void BuyBundle()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_BUNDLE, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentRefresh += 5;
      GameManager.Instance.CurrentHammer += 3;
      GameManager.Instance.CurrentRocket += 2;
      GameManager.Instance.CurrentSwap += 2;
      ShowNotify("PAYMENT SUCCEED");

      if (SceneManager.GetActiveScene().name == "Gameplay")
      {
        PowerItemPanel.Instance.UpdateAmountUI();
      }
    });
  }

  public void RemoveAds()
  {
    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_REMOVEADS, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.IsRemoveAds = true;
      ShowNotify("PAYMENT SUCCEED");
      remeveAdsTxt.Buyed();
      LevelPlayAds.Instance.HideBanner();

      if (SceneManager.GetActiveScene().name == "Lobby")
      {
        LobbyPanel.Instance.HideRemoveAdsIcon();
        LobbyPanel.Instance.InitPosDefaultLiveOps();
      }

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(
          "removeads",
          new Parameter[]{
            new ("type", "infinity")
          }
        );
      }
    });
  }

  public void FreeCoin()
  {
    watchAdsBtnIOS.interactable = false;

    var limitFreeCoinAds = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.free_coin.ios.limit_watch;
    var coinReceive = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.free_coin.ios.coin_received;

#if UNITY_ANDROID
    limitFreeCoinAds = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.free_coin.android.limit_watch;
    coinReceive = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.free_coin.android.coin_received;
#endif

    if (GameManager.Instance.FreeCoinTodayCount < AMOUNT_FREECOIN_NOADS)
    {
      earnCoins[0] = coinReceive;
      EarnRandomCoins();
      SoundManager.Instance.PlayClaimDailyRewardSfx();
      GameManager.Instance.FreeCoinTodayCount++;
      onWatchFreeCoinAds?.Invoke();
      UpdateAdsIcon();
      watchAdsBtnIOS.interactable = true;

      if (SceneManager.GetActiveScene().name == "Gameplay")
      {
        if (GameManager.Instance.GetGameState() != GameState.Gameover)
        {
          BalloonSystem.Instance.ShowBalloonWithDelay(2);
          return;
        }
        return;
      }

      return;
    }

    if (GameManager.Instance.FreeCoinTodayCount >= (limitFreeCoinAds + AMOUNT_FREECOIN_NOADS))
    {
      if (SceneManager.GetActiveScene().name == "Lobby")
      {
        LobbyPanel.Instance.ShowNotifyWith("REACHED LIMIT TODAY");
      }
      StartCoroutine(EffectManager.Instance.IEDelayShow(watchAdsBtnIOS, 1.5f));
      return;
    }

    LevelPlayAds.Instance.ShowRewardedAd(() =>
    {
      earnCoins[0] = coinReceive;
      EarnRandomCoins();
      SoundManager.Instance.PlayClaimDailyRewardSfx();
      GameManager.Instance.FreeCoinTodayCount++;
      onWatchFreeCoinAds?.Invoke();
      UpdateAdsIcon();
      // #if UNITY_IOS
      watchAdsBtnIOS.interactable = true;
      // #elif UNITY_ANDROID
      //       watchAdsBtnAndroid.interactable = true;
      // #endif

      if (SceneManager.GetActiveScene().name == "Gameplay")
      {
        if (GameManager.Instance.GetGameState() != GameState.Gameover)
        {
          BalloonSystem.Instance.ShowBalloonWithDelay(2);
          return;
        }
        return;
      }
    },
    "FreeCoin",
    () =>
    {
      // #if UNITY_IOS
      StartCoroutine(EffectManager.Instance.IEDelayShow(watchAdsBtnIOS, 1.5f));
      // #elif UNITY_ANDROID
      //       StartCoroutine(EffectManager.Instance.IEDelayShow(watchAdsBtnAndroid, 1.5f));
      // #endif

      ShowNotify("ADS NOT READY");
    });
  }

  public void EarnRandomCoins()
  {
    int randCoins = earnCoins[UnityEngine.Random.Range(0, earnCoins.Length)];

    RewardData priceData = ScriptableObject.CreateInstance<RewardData>();
    priceData.Img = coinSprite;
    priceData.Value = new int[] { randCoins };
    priceData.Type = PriceType.Coin;

    Debug.Log("Show Panel");
    ShowPanel.Instance
      .InjectPriceData(priceData, ShowPanel.Instance.GlobalCoinImg.transform)
      .ShowImgWith(priceData.Img, "", " " + priceData.Value[0], Color.yellow, false);
    GameManager.Instance.CurrentCoin += randCoins;
  }

  public void MiniCoin()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_MINICOIN, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentCoin += 400;
      ShowNotify("PAYMENT SUCCEED");
    });
  }

  public void SmallCoin()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_SMALLCOIN, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentCoin += 1700;
      ShowNotify("PAYMENT SUCCEED");
    });
  }

  public void MediumCoin()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_MEDIUMCOIN, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentCoin += 4800;
      ShowNotify("PAYMENT SUCCEED");
    });
  }

  public void BigCoin()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_BIGCOIN, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentCoin += 10000;
      ShowNotify("PAYMENT SUCCEED");
    });
  }

  public void SuperCoin()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_SUPERCOIN, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentCoin += 20000;
      ShowNotify("PAYMENT SUCCEED");
    });
  }

  public void MegaCoin()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_MEGACOIN, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentCoin += 40000;
      ShowNotify("PAYMENT SUCCEED");
    });
  }

  public void DouleValuePack()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_DOUBLEVALUEPACK, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotify("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.CurrentHammer += 5;
      GameManager.Instance.CurrentRocket += 5;
      GameManager.Instance.CurrentSwap += 5;
      GameManager.Instance.CurrentRefresh += 5;
      HeartSystem.Instance.AddInfinityHeartTime(120);
      GameManager.Instance.CurrentCoin += 3000;
      ShowNotify("PAYMENT SUCCEED");

      if (SceneManager.GetActiveScene().name == "Gameplay")
      {
        PowerItemPanel.Instance.UpdateAmountUI();
      }
    });
  }

  public void RestorePurchase()
  {
    IAPManager.Instance.RestorePurchases((isSucceed, id) =>
    {
      if (isSucceed)
      {
        if (id == (IAPManager.Prefix + KeyString.KEY_IAP_REMOVEADS))
        {
          GameManager.Instance.IsRemoveAds = true;
          remeveAdsTxt.Buyed();

          if (SceneManager.GetActiveScene().name == "Lobby")
          {
            LobbyPanel.Instance.HideRemoveAdsIcon();
            LobbyPanel.Instance.InitPosDefaultLiveOps();
          }
        }
        if (id == (IAPManager.Prefix + KeyString.KEY_IAP_WINTERPASS))
        {
          GameManager.Instance.IsUnlockFollowing = true;
        }
        if (id == (IAPManager.Prefix + KeyString.KEY_IAP_REMOVEADS_SUBSCRIPTION))
        {
          GameManager.Instance.IsRemoveAds7d = true;
        }
        ShowNotify("RESTORE SUCCEED");
      }
      else
      {
        ShowNotify("NO TRANSACTION");
      }
    });
  }

  public void Close()
  {
    LeanTween.delayedCall(gameObject, 0.4f, () =>
    {
      ShowPanel.Instance.ShowHomeBtn();

      if (GameManager.Instance.GetGameState() == GameState.Gameover)
      {
        return;
      }
      else
      {
        GameManager.Instance.SetGameState(GameState.Gameplay);
      }

      BalloonSystem.Instance.ShowBalloon();
    });
  }

  void ShowNotify(string message)
  {
    if (SceneManager.GetActiveScene().name == KeyString.NAME_SCENE_LOBBY)
      LobbyPanel.Instance.ShowNotifyWith(message);
  }

  void UpdateRemovedAdsTxt()
  {
    remeveAdsTxt.Buyed();
  }

  public void UpdateAdsIcon()
  {
    if (GameManager.Instance.FreeCoinTodayCount < AMOUNT_FREECOIN_NOADS)
    {
      iconAdsFreeCoin.gameObject.SetActive(false);
      return;
    }

    iconAdsFreeCoin.gameObject.SetActive(true);
  }
}
