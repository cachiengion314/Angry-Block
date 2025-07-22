using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sych.ShareAssets.Runtime;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class SettingModal : MonoBehaviour
{
  [Header("Internal")]
  [SerializeField] Image hapticNegativeBarImg;
  [SerializeField] Image soundNegativeBarImg;
  [SerializeField] Image musicNegativeBarImg;
  [SerializeField] RectTransform hackPanel;
  [SerializeField] TMP_Text versionText;
  [SerializeField] TMP_Text versionTextAndroid;

  [Header("Fill UrlGame")]
  [SerializeField] string _urlGame;

  private void Start()
  {
    UpdateUI();
    versionText.text = "Version " + Application.version;
  }

  void UpdateUI()
  {
    if (!GameManager.Instance.IsSoundOn)
    {
      soundNegativeBarImg.gameObject.SetActive(true);
    }
    else
    {
      soundNegativeBarImg.gameObject.SetActive(false);
    }

    if (!GameManager.Instance.IsMusicOn)
    {
      musicNegativeBarImg.gameObject.SetActive(true);
    }
    else
    {
      musicNegativeBarImg.gameObject.SetActive(false);
    }

    if (!GameManager.Instance.IsHapticOn)
    {
      hapticNegativeBarImg.gameObject.SetActive(true);
    }
    else
    {
      hapticNegativeBarImg.gameObject.SetActive(false);
    }
  }

  void TurnOnMainThemeMusic()
  {
    GameManager.Instance.IsMusicOn = true;
    musicNegativeBarImg.gameObject.SetActive(false);
  }

  void TurnOffMainThemeMusic()
  {
    GameManager.Instance.IsMusicOn = false;
    musicNegativeBarImg.gameObject.SetActive(true);
  }

  void TurnOnSound()
  {
    GameManager.Instance.IsSoundOn = true;
    soundNegativeBarImg.gameObject.SetActive(false);
  }

  void TurnOffSound()
  {
    GameManager.Instance.IsSoundOn = false;
    soundNegativeBarImg.gameObject.SetActive(true);
  }

  void TurnOnHaptic()
  {
    GameManager.Instance.IsHapticOn = true;
    hapticNegativeBarImg.gameObject.SetActive(false);
  }

  void TurnOffHaptic()
  {
    GameManager.Instance.IsHapticOn = false;
    hapticNegativeBarImg.gameObject.SetActive(true);
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

  public void UnlockHack()
  {

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
    DOTween.KillAll();
    SoundManager.Instance.PlayPressBtnSfx();
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
  }

}
