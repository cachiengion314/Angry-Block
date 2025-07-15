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
    GameManager.Instance.TimePhaseEnd = DateTime.Now;
      var duration_end = (int)(GameManager.Instance.TimePhaseEnd - GameManager.Instance.TimePhaseStart).TotalSeconds;
      GameManager.Instance.TimePhaseStart = GameManager.Instance.TimePhaseEnd;

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_END_LEVEL,
          new Parameter[]
          {
            new ("result", "lose"),
            new ("duration_end", duration_end)
          });
      }
        SceneManager.LoadScene(KeyString.NAME_SCENE_GAMEPLAY);
    }

    public void BackHome()
    {
    GameManager.Instance.TimePhaseEnd = DateTime.Now;
      var duration_end = (int)(GameManager.Instance.TimePhaseEnd - GameManager.Instance.TimePhaseStart).TotalSeconds;
      GameManager.Instance.TimePhaseStart = GameManager.Instance.TimePhaseEnd;

      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(KeyString.FIREBASE_END_LEVEL,
          new Parameter[]
          {
            new ("result", "lose"),
            new ("duration_end", duration_end)
          });
      }
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
    }
}
