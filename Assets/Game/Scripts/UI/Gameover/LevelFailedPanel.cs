using System;
using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFailedPanel : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI levelText;
  void Start()
  {
    levelText.text = $"LEVEL {GameManager.Instance.CurrentLevel + 1}";
  }

  public void TryAgain()
  {
    SceneManager.LoadScene(KeyString.NAME_SCENE_GAMEPLAY);
  }

  public void BackHome()
  {
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
  }
}
