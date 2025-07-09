using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(HeartControl))]
public class HeartModal : MonoBehaviour
{
  [Header("")]
  [SerializeField] Image heartIcon;
  [SerializeField] Image infinityImg;
  [SerializeField] TextMeshProUGUI heartTxt;
  [SerializeField] TextMeshProUGUI timeTxt;
  [SerializeField] Button heartBtn;

  [Header("")]
  [SerializeField] Sprite heartNormal;
  [SerializeField] Sprite heartInfinity;

  private void OnEnable()
  {
    HeartSystem.onBakedHeartSystem += InitHeartModal;
    HeartSystem.onChangeHeart += UpdateHeartModal;
  }

  private void OnDisable()
  {
    HeartSystem.onBakedHeartSystem -= InitHeartModal;
    HeartSystem.onChangeHeart -= UpdateHeartModal;
  }

  private void InitHeartModal()
  {
    UpdateHeartModal();
  }

  private void UpdateHeartModal()
  {
    var heartMax = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.config_game.max_energy;
    heartTxt.gameObject.SetActive(true);

    // TODO Config
    if (heartMax > 5)
    {
      heartTxt.gameObject.SetActive(false);
      timeTxt.gameObject.SetActive(true);
      infinityImg.gameObject.SetActive(true);
      timeTxt.text = "Play!";
      return;
    }
    else
    {
      heartTxt.text = "" + HeartSystem.Instance.CurrentHeart + "/" + 5;
    }

    if (GetComponent<HeartControl>().GetIsInfinityHeart())
    {
      heartBtn.interactable = false;
      var useInfinityHeartTime = HeartSystem.Instance.UseInfinityHeartTime;
      var useTime = TimeSpan.FromSeconds(useInfinityHeartTime);
      timeTxt.gameObject.SetActive(true);
      infinityImg.gameObject.SetActive(true);

      heartIcon.sprite = heartInfinity;
      if (useTime.Hours > 0)
      {
        timeTxt.text = useTime.Hours + "h" + useTime.Minutes + "m" + useTime.Seconds + "s";
      }
      else
      {
        timeTxt.text = useTime.Minutes + "m" + useTime.Seconds + "s";
      }
      heartTxt.gameObject.SetActive(false);
    }
    else if (GetComponent<HeartControl>().IsFull(HeartSystem.Instance.CurrentHeart))
    {
      timeTxt.gameObject.SetActive(true);
      heartTxt.gameObject.SetActive(false);
      infinityImg.gameObject.SetActive(false);
      // heartBtn.interactable = false;
      heartIcon.sprite = heartNormal;
      timeTxt.text = "Play!";
    }
    else
    {
      heartBtn.interactable = true;
      heartIcon.sprite = heartNormal;
      timeTxt.gameObject.SetActive(false);
      infinityImg.gameObject.SetActive(false);
    }
  }

  public void ClickHeart()
  {
    if (SceneManager.GetActiveScene().name == "Lobby")
    {
      if (GetComponent<HeartControl>().IsFull(HeartSystem.Instance.CurrentHeart))
      {
        LobbyPanel.Instance.OpenHeartFullModal();
        return;
      }

      if (FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.config_game.max_energy > 5) return;
      LobbyPanel.Instance.ToggleBuyHeartPanel();
    }
  }
}
