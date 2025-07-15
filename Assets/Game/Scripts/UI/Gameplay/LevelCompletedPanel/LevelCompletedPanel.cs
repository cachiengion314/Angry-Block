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
    GameManager.Instance.CurrentCoin += 10;
    GameManager.Instance.CurrentLevel++;
    SceneManager.LoadScene(KeyString.NAME_SCENE_LOBBY);
  }

  public void NextBtn()
  {
    if (GameManager.Instance.CurrentLevel < 4)
    {
      NextLevel();
      return;
    }

    // LevelPlayAds.Instance.ShowInterstitialAd(
    //   (info) =>
    //   {

    //   },
    //   "NextLevel",
    //   (info) =>
    //   {
    //     NextLevel();
    //   },
    //   () =>
    //   {
    //     NextLevel();
    //   }
    // );
  }

  public void RewardX2Btn()
  {
    // LevelPlayAds.Instance.ShowRewardedAd(() =>
    // {
    //   NextLevel();
    // },
    // "GoToNextLevelWithAds",
    // () =>
    // {
    // });
  }
}
