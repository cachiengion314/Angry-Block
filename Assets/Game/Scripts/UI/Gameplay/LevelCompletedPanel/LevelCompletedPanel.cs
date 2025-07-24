using DG.Tweening;
using Firebase.Analytics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompletedPanel : MonoBehaviour
{
  [SerializeField] TMP_Text levelText;

  public void Start()
  {
    levelText.text = $"LEVEL {GameManager.Instance.CurrentLevelIndex + 1}";
  }
  public void NextLevel()
  {
    FirebaseAnalytics.LogEvent(KeyString.FIREBASE_COIN_EARN,
  new Parameter[]
  {
        new ("source", "ContinueGame"),
  });
    DOTween.KillAll();
    SoundManager.Instance.PlayPressBtnSfx();
    GameManager.Instance.CurrentCoin += 10;
    GameManager.Instance.CurrentLevelIndex++;
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
  }
}
