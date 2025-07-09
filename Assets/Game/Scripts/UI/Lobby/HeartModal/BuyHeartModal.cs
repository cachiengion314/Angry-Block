using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(HeartControl))]
public class BuyHeartModal : MonoBehaviour
{
  [Header("")]
  [SerializeField] Image heartIcon;
  [SerializeField] TextMeshProUGUI heartTxt;
  [SerializeField] TextMeshProUGUI timeTxt;
  [SerializeField] Button refillBtn;
  [SerializeField] Button freeCoinBtn;

  [Header("Settings")]
  [SerializeField] int refillCost;

  private void OnEnable()
  {
    HeartSystem.onBakedHeartSystem += InitBuyHeartModal;
    HeartSystem.onChangeHeart += UpdateBuyHeartModal;
  }

  private void OnDisable()
  {
    HeartSystem.onBakedHeartSystem -= InitBuyHeartModal;
    HeartSystem.onChangeHeart -= UpdateBuyHeartModal;
  }

  public void InitBuyHeartModal()
  {
    UpdateBuyHeartModal();
  }

  private void UpdateBuyHeartModal()
  {
    heartTxt.gameObject.SetActive(true);
    heartTxt.text = "" + HeartSystem.Instance.CurrentHeart;

    if (GetComponent<HeartControl>().GetIsInfinityHeart())
    {
      return;
    }

    if (GetComponent<HeartControl>().IsFull(HeartSystem.Instance.CurrentHeart))
    {
      refillBtn.interactable = false;
      freeCoinBtn.interactable = false;
      timeTxt.text = "Play!";
      return;
    }

    refillBtn.interactable = true;
    freeCoinBtn.interactable = true;
    var distanceTime = DateTime.Now - HeartSystem.Instance.LastedTimeHeart;
    var deltaTime = (HeartSystem.Instance.MinutePerHearts * 60) - distanceTime.TotalSeconds;
    var _timeText = TimeSpan.FromSeconds(deltaTime);

    timeTxt.text = _timeText.Minutes + "m" + _timeText.Seconds + "s";
  }

  #region Handle
  public void Refill()
  {
    if (GameManager.Instance.CurrentCoin < refillCost)
    {
      if (SceneManager.GetActiveScene().name == "Lobby")
      {
        LobbyPanel.Instance.ToggleBuyHeartPanel();
        LeanTween.delayedCall(gameObject, 0.4f, () =>
        {
          HomeLayout.Instance.OpenShopModal();
        });
        return;
      }

      ShowPanel.Instance.HideHomeBtn();
      return;
    }

    var heartRefill = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.refill_heart.ios.refill_amount;

#if UNITY_ANDROID
    heartRefill = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.refill_heart.android.refill_amount;
#endif

    if (!HeartSystem.Instance.TryIncreaseHeart(heartRefill))
    {
      return;
    }

    GameManager.Instance.CurrentCoin -= refillCost;

    if (SceneManager.GetActiveScene().name == "Lobby")
    {
      LobbyPanel.Instance.ToggleBuyHeartPanel();
    }

    if (SceneManager.GetActiveScene().name == "Gameplay")
    {
      // HeartSystem.Instance.TryDecreaseHeart(1);
      SceneManager.LoadScene("Gameplay");
    }
  }

  bool _isClicked;
  public void FreeHeart()
  {
    freeCoinBtn.interactable = false;
    if (_isClicked) return;

    LevelPlayAds.Instance.ShowRewardedAd(() =>
    {
      freeCoinBtn.interactable = true;
      _isClicked = false;

      if (HeartSystem.Instance.TryIncreaseHeart(1))
      {
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
          LobbyPanel.Instance.ToggleBuyHeartPanel();
          return;
        }
      }

      // HeartSystem.Instance.TryDecreaseHeart(1);
      SceneManager.LoadScene("Gameplay");
    },
    "FreeHeart",
    () =>
    {
      LeanTween.delayedCall(gameObject, 1.5f, () =>
      {
        _isClicked = false;
        freeCoinBtn.interactable = true;
      });

      if (!_isClicked)
      {
        _isClicked = true;
        ShowNotify("ADS NOT READY");
      }
    });
  }

  public void Close()
  {
    if (SceneManager.GetActiveScene().name == "Lobby")
    {
      LobbyPanel.Instance.ToggleBuyHeartPanel();
      return;
    }

    SceneManager.LoadScene("Lobby");
  }

  #endregion

  void ShowNotify(string message)
  {
    if (SceneManager.GetActiveScene().name == KeyString.NAME_SCENE_LOBBY)
      LobbyPanel.Instance.ShowNotifyWith(message);
  }
}
