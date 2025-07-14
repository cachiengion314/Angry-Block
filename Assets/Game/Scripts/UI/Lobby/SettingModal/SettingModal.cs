using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sych.ShareAssets.Runtime;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;


public class SettingModal : MonoBehaviour
{
  [Header("Internal")]
  [SerializeField] Image hapticNegativeBarImg;
  [SerializeField] Image soundNegativeBarImg;
  [SerializeField] Image musicNegativeBarImg;
  // [SerializeField] Image hapticNegativeBarImgAndroid;
  // [SerializeField] Image soundNegativeBarImgAndroid;
  // [SerializeField] Image musicNegativeBarImgAndroid;
  [SerializeField] RectTransform hackPanel;
  [SerializeField] TMP_Text versionText;
  [SerializeField] TMP_Text versionTextAndroid;
  [SerializeField] ShopModal shopModal;

  [Header("Fill UrlGame")]
  [SerializeField] string _urlGame;

  private void Start()
  {
    UpdateUI();
    versionText.text = "Version " + Application.version;
    // versionTextAndroid.text = "Version " + Application.version;
  }

  void UpdateUI()
  {
    if (!GameManager.Instance.IsSoundOn)
    {
      soundNegativeBarImg.gameObject.SetActive(true);
      // soundNegativeBarImgAndroid.gameObject.SetActive(true);
    }
    else
    {
      soundNegativeBarImg.gameObject.SetActive(false);
      // soundNegativeBarImgAndroid.gameObject.SetActive(false);
    }

    if (!GameManager.Instance.IsMusicOn)
    {
      musicNegativeBarImg.gameObject.SetActive(true);
      // musicNegativeBarImgAndroid.gameObject.SetActive(true);
    }
    else
    {
      musicNegativeBarImg.gameObject.SetActive(false);
      // musicNegativeBarImgAndroid.gameObject.SetActive(false);
    }

    if (!GameManager.Instance.IsHapticOn)
    {
      hapticNegativeBarImg.gameObject.SetActive(true);
      // hapticNegativeBarImgAndroid.gameObject.SetActive(true);
    }
    else
    {
      hapticNegativeBarImg.gameObject.SetActive(false);
      // hapticNegativeBarImgAndroid.gameObject.SetActive(false);
    }
  }

  void TurnOnMainThemeMusic()
  {
    GameManager.Instance.IsMusicOn = true;
    musicNegativeBarImg.gameObject.SetActive(false);
    // musicNegativeBarImgAndroid.gameObject.SetActive(false);
  }

  void TurnOffMainThemeMusic()
  {
    GameManager.Instance.IsMusicOn = false;
    musicNegativeBarImg.gameObject.SetActive(true);
    // musicNegativeBarImgAndroid.gameObject.SetActive(true);
  }

  void TurnOnSound()
  {
    GameManager.Instance.IsSoundOn = true;
    soundNegativeBarImg.gameObject.SetActive(false);
    // soundNegativeBarImgAndroid.gameObject.SetActive(false);
  }

  void TurnOffSound()
  {
    GameManager.Instance.IsSoundOn = false;
    soundNegativeBarImg.gameObject.SetActive(true);
    // soundNegativeBarImgAndroid.gameObject.SetActive(true);
  }

  void TurnOnHaptic()
  {
    GameManager.Instance.IsHapticOn = true;
    hapticNegativeBarImg.gameObject.SetActive(false);
    // hapticNegativeBarImgAndroid.gameObject.SetActive(false);
  }

  void TurnOffHaptic()
  {
    GameManager.Instance.IsHapticOn = false;
    hapticNegativeBarImg.gameObject.SetActive(true);
    // hapticNegativeBarImgAndroid.gameObject.SetActive(true);
  }

  public void ToggleHackPanel()
  {
    if (hackPanel.gameObject.activeSelf) hackPanel.gameObject.SetActive(false);
    else hackPanel.gameObject.SetActive(true);
  }

  public void ToggleMainThemeMusic()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!GameManager.Instance.IsMusicOn)
    {
      TurnOnMainThemeMusic();
      return;
    }
    TurnOffMainThemeMusic();
  }

  public void ToggleSound()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!GameManager.Instance.IsSoundOn)
    {
      TurnOnSound();
      return;
    }
    TurnOffSound();
  }

  public void ToggleHaptic()
  {
    SoundManager.Instance.PlayPressBtnSfx();

    if (!GameManager.Instance.IsHapticOn)
    {
      TurnOnHaptic();
      return;
    }
    TurnOffHaptic();
  }

  public void RestorePurchase()
  {
    // if (GameManager.Instance.IsRemoveAds) return;
    shopModal.RestorePurchase();
  }


  int amount = 20;


  public void UnlockHack()
  {
    var isOnCheat = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.cheat_setting.ios.isOn;

#if UNITY_ANDROID
    isOnCheat = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.cheat_setting.android.isOn;
#endif

    if (Debug.isDebugBuild) isOnCheat = true;
    if (!isOnCheat) return;

    if (Debug.isDebugBuild)
    {
      amount = 1;
    }

    amount--;
    if (amount <= 0)
    {
      hackPanel.gameObject.SetActive(true);
    }
  }

  public void SendEmail()
  {
    string email = "cmzsoft.vn@gmail.com";
    string subject = EscapeURL("Yêu cầu hỗ trợ");
    string body = EscapeURL("Xin chào, tôi cần hỗ trợ về sản phẩm của bạn...");

    Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
  }

  string EscapeURL(string url)
  {
    return UnityWebRequest.EscapeURL(url).Replace("+", "%20");
  }

  public void ClickShare
  ()
  {
    // iOS chia sẻ
    // Tạo danh sách các mục cần chia sẻ
    string item = "Link Game: " + _urlGame;

    Share.Item(item, success =>
    {
      Debug.Log($"Chia sẻ {(success ? "thành công" : "thất bại")}");
    });

  }

  public void BackHome()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
  }

}
