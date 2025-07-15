using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompletedPanel : MonoBehaviour
{
  // public static LevelCompletedPanel Instance { get; private set; }
  [SerializeField] TMP_Text levelText;
  // [SerializeField] Transform x2CoinImg;
  // [SerializeField] Button nextBtn;
  [SerializeField] Button nextWithVideoBtn;

  // private void OnEnable()
  // {
  //   levelText.text = $"LEVEL {GameManager.Instance.CurrentLevel + 1}";
  //   nextBtn.interactable = true;
  //   nextWithVideoBtn.interactable = true;
  // }

  // private void Start()
  // {
  //   if (Instance == null)
  //   {
  //     Instance = this;
  //   }
  //   else
  //   {
  //     Destroy(gameObject);
  //   }
  // }

  // void NextLevel()
  // {
  //   GameManager.Instance.ReviveCount = 0;
  //   GameManager.Instance.RetryCount = 0;
  //   // GameManager.Instance.SetGameState(GameState.Gameplay);
  //   // LevelProgressBlock.Instance.GoToNextLevel();

  //   // ShowPanel.Instance.ShowHomeBtn();
  //   // GameplayPanel.Instance.ToggleLevelCompletedModal();
  //   // GameplayPanel.Instance.QuitModal.gameObject.SetActive(false);
  //   // BalloonSystem.Instance.ShowBalloonWithDelay();

  //   SceneManager.LoadScene("Lobby");

  //   if (GameManager.Instance.IsEvented())
  //   {
  //     PlayerPrefs.SetInt("AmountPlayGame", PlayerPrefs.GetInt("AmountPlayGame", 0) + 1);
  //   }
  // }

  // public void GoToNextLevel()
  // {
  //   nextBtn.interactable = false;
  //   nextWithVideoBtn.interactable = false;

  //   if (GameManager.Instance.CurrentLevel < 4)
  //   {
  //     NextLevel();
  //     return;
  //   }

  //   LevelPlayAds.Instance.ShowInterstitialAd(
  //     (info) =>
  //     {

  //     },
  //     "NextLevel",
  //     (info) =>
  //     {
  //       NextLevel();
  //     },
  //     () =>
  //     {
  //       NextLevel();
  //     }
  //   );
  // }

  // public void GoToNextLevelWithReward()
  // {
  //   nextBtn.interactable = false;
  //   nextWithVideoBtn.interactable = false;

  //   LevelPlayAds.Instance.ShowRewardedAd(() =>
  //   {
  //     EffectManager.Instance.EmissingCoinsWithParticleTo(
  //       ShowPanel.Instance.GlobalCoinImg.transform, 10, x2CoinImg.transform, () =>
  //       {
  //         SoundManager.Instance.PlayCoinReceiveSfx();
  //         GameManager.Instance.CurrentCoin += 10;
  //         // DailyTaskManager.Instance.UpdateTaskProgress(8, 10);

  //         var data = new MissonDataDailyTask(enumListeningDataDailyTask.CollectCoin, 10);
  //         EventActionManager.TriggerEvent(enumListeningDataDailyTask.CollectCoin, data);

  //         // DailyWeeklyManager.Instance.UpdateTaskProgress(0, 10);

  //         var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.CollectCoinWeekly, 10);
  //         EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.CollectCoinWeekly, dataWeekly);
  //         NextLevel();
  //       }
  //     );
  //   },
  //   "GoToNextLevelWithAds",
  //   () =>
  //   {
  //     nextBtn.interactable = true;
  //     StartCoroutine(EffectManager.Instance.IEDelayShow(nextWithVideoBtn, 1.5f));
  //   });
  // }

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

    LevelPlayAds.Instance.ShowInterstitialAd(
      (info) =>
      {

      },
      "NextLevel",
      (info) =>
      {
        NextLevel();
      },
      () =>
      {
        NextLevel();
      }
    );
  }

  public void RewardX2Btn()
  {
    LevelPlayAds.Instance.ShowRewardedAd(() =>
    {
      NextLevel();
    },
    "GoToNextLevelWithAds",
    () =>
    {
      StartCoroutine(EffectManager.Instance.IEDelayShow(nextWithVideoBtn, 1.5f));
    });
  }
}
