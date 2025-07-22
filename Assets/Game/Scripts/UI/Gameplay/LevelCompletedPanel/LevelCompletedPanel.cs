using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompletedPanel : MonoBehaviour
{
  [SerializeField] TMP_Text levelText;

  public void Start()
  {
    levelText.text = $"LEVEL {GameManager.Instance.CurrentLevel + 1}";
  }
  public void NextLevel()
  {
    DOTween.KillAll();
    GameManager.Instance.CurrentCoin += 10;
    GameManager.Instance.CurrentLevel++;
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
  }
}
