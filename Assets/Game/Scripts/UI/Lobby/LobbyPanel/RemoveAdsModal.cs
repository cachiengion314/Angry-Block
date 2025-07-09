using System;
using Firebase.Analytics;
using HoangNam;
using TMPro;
using UnityEngine;

public partial class LobbyPanel : MonoBehaviour
{
  public static Action onBuyRemoveAdsSucceed;

  [Header("Remove Ads Modal")]
  [SerializeField] RectTransform removeAdsIcon;
  [SerializeField] RectTransform removeAdsModal;
  [SerializeField] TextMeshProUGUI textPrice;
  [SerializeField] TextMeshProUGUI textPurchased;
  [SerializeField] EventDataNguyen eventDataNguyen;

  private void InitRemoveAdsIcon()
  {
    if (GameManager.Instance.IsRemoveAds)
    {
      HideRemoveAdsIcon();
      InitPosDefaultLiveOps();
    }
  }

  public void ClickOpenRemoveAdsModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    OpenModal(removeAdsModal);
  }

  public void ClickCloseRemoveAdsModal()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    CloseModal(removeAdsModal);
  }

  public void ClickBuyRemoveAds()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_REMOVEADS, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotifyWith("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.IsRemoveAds = true;
      ShowNotifyWith("PAYMENT SUCCEED");
      HideRemoveAdsIcon();
      InitPosDefaultLiveOps();
      LevelPlayAds.Instance.HideBanner();
      CloseModal(removeAdsModal);
      onBuyRemoveAdsSucceed?.Invoke();

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

  public void ClickBuyRemoveAds7d()
  {
    if (GameManager.Instance.IsRemoveAds7d) return;
    SoundManager.Instance.PlayPressBtnSfx();

    IAPManager.Instance.PurchaseProduct(KeyString.KEY_IAP_REMOVEADS7D, (isSucceed, notice) =>
    {
      if (!isSucceed)
      {
        ShowNotifyWith("PAYMENT ERROR");
        return;
      }

      GameManager.Instance.IsRemoveAds7d = true;
      InitRemoveAds7d();
      ShowNotifyWith("PAYMENT SUCCEED");
      LevelPlayAds.Instance.HideBanner();
      OnChangeRemoveAds7d();

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(
          "removeads",
          new Parameter[]{
            new ("type", "7d")
          }
        );
      }
    });
  }

  public void InitRemoveAds7d()
  {
    // DateTime currentTime = new DateTime(2025, 5, 1, 11, 0, 0);
    DateTime currentTime = DateTime.Now;
    RemoveAds7d data = new RemoveAds7d();
    InitEvent.Init(currentTime, eventDataNguyen, out DateTime startTime, out DateTime endTime);
    data.startTime = startTime.ToString("yyyy-MM-dd HH:mm");
    data.endTime = endTime.ToString("yyyy-MM-dd HH:mm");
    SaveSystem.SaveWith(data, KeyString.NAME_REMOVEADS7D_DATA);
    Debug.Log("save");
  }

  public void CheckRemoveAds7d()
  {
    bool isPurchased = GameManager.Instance.IsRemoveAds7d;
    if (isPurchased)
    {
      DateTime currentTime = DateTime.Now;
      DailyBonusData data = SaveSystem.LoadWith<DailyBonusData>(KeyString.NAME_REMOVEADS7D_DATA);
      DateTime startTime = DateTime.ParseExact(data.startTime, "yyyy-MM-dd HH:mm", null);
      DateTime endTime = DateTime.ParseExact(data.endTime, "yyyy-MM-dd HH:mm", null);
      GameManager.Instance.IsRemoveAds7d = InitEvent.CheckEvent(currentTime, startTime, endTime);
    }
    OnChangeRemoveAds7d();
  }

  void OnChangeRemoveAds7d()
  {
    bool isPurchased = GameManager.Instance.IsRemoveAds7d;
    if (textPrice == null || textPurchased == null) return;
    textPrice.gameObject.SetActive(!isPurchased);
    textPurchased.gameObject.SetActive(isPurchased);
  }

  public void HideRemoveAdsModal()
  {
    removeAdsModal.gameObject.SetActive(false);
  }

  public void HideRemoveAdsIcon()
  {
    removeAdsIcon.gameObject.SetActive(false);
  }
}

public struct RemoveAds7d
{
  public bool IsNull => startTime == null && endTime == null;
  public string startTime;
  public string endTime;
}