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
  [SerializeField] GameObject hapticNegativeBarOn;
  [SerializeField] GameObject hapticNegativeBarOff;

  [SerializeField] GameObject soundNegativeBarOn;
  [SerializeField] GameObject soundNegativeBarOff;

  [SerializeField] GameObject musicNegativeBarOn;
  [SerializeField] GameObject musicNegativeBarOff;
  [SerializeField] TMP_Text versionText;

  [Header("Fill UrlGame")]
  [SerializeField] string _urlGame;

  private void Start()
  {
    UpdateUI();
    versionText.text = "Version " + Application.version;
  }

  void UpdateUI()
  {
    soundNegativeBarOn.SetActive(GameManager.Instance.IsSoundOn);
    soundNegativeBarOff.SetActive(!GameManager.Instance.IsSoundOn);
    musicNegativeBarOn.SetActive(GameManager.Instance.IsMusicOn);
    musicNegativeBarOff.SetActive(!GameManager.Instance.IsMusicOn);
    hapticNegativeBarOn.SetActive(GameManager.Instance.IsHapticOn);
    hapticNegativeBarOff.SetActive(!GameManager.Instance.IsHapticOn);
  }

  public void ToggleMainThemeMusic()
  {
    GameManager.Instance.IsMusicOn = !GameManager.Instance.IsMusicOn;
    musicNegativeBarOn.SetActive(GameManager.Instance.IsMusicOn);
    musicNegativeBarOff.SetActive(!GameManager.Instance.IsMusicOn);
    SoundManager.Instance.PlayPressBtnSfx();
  }

  public void ToggleSound()
  {
    GameManager.Instance.IsSoundOn = !GameManager.Instance.IsSoundOn;
    soundNegativeBarOn.SetActive(GameManager.Instance.IsSoundOn);
    soundNegativeBarOff.SetActive(!GameManager.Instance.IsSoundOn);
    SoundManager.Instance.PlayPressBtnSfx();
  }

  public void ToggleHaptic()
  {
    GameManager.Instance.IsHapticOn = !GameManager.Instance.IsHapticOn;
    hapticNegativeBarOn.SetActive(GameManager.Instance.IsHapticOn);
    hapticNegativeBarOff.SetActive(!GameManager.Instance.IsHapticOn);
    SoundManager.Instance.PlayPressBtnSfx();
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
