using System;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
  // const int MAX_REVIVE_TODAY_COUNT = 1;
  // [SerializeField] Button retryBtn;
  // [SerializeField] Button reviveBtn;

  // private void OnEnable()
  // {
  //   reviveBtn.interactable = true;
  //   retryBtn.interactable = true;
  //   // BalloonSystem.Instance.HideBalloon();

  //   if (GameManager.Instance.ReviveCount >= MAX_REVIVE_TODAY_COUNT)
  //   {
  //     retryBtn.gameObject.SetActive(true);
  //     reviveBtn.gameObject.SetActive(false);
  //   }
  //   else
  //   {
  //     retryBtn.gameObject.SetActive(true);
  //     reviveBtn.gameObject.SetActive(true);
  //   }
  // }

  //   public void Replay()
  //   {
  //     SoundManager.Instance.PlayPressBtnSfx();

  //     if (!HeartSystem.Instance.TryDecreaseHeart(1))
  //     {

  //       return;
  //     }

  //     HeartSystem.Instance.TryIncreaseHeart(1);
  //     GameManager.Instance.RetryCount++;
  //     // LogEvent
  //     GameManager.Instance.TimePhaseEnd = DateTime.Now;
  //     var duration_end = (int)(GameManager.Instance.TimePhaseEnd - GameManager.Instance.TimePhaseStart).TotalSeconds;
  //     GameManager.Instance.TimePhaseStart = GameManager.Instance.TimePhaseEnd;

  //     if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
  //     {
  //       FirebaseAnalytics.LogEvent(KeyString.FIREBASE_END_LEVEL,
  //         new Parameter[]
  //         {
  //           new ("result", "lose"),
  //           new ("duration_end", duration_end)
  //         });
  //     }

  //     if (GameManager.Instance.IsEvented())
  //     {
  //       PlayerPrefs.SetInt("AmountPlayGame", PlayerPrefs.GetInt("AmountPlayGame", 0) + 1);
  //     }

  //     var paramKeys = new string[2];
  //     paramKeys[0] = "IngameLevel";
  //     paramKeys[1] = "RetryCount";
  //     var values = new int[2];
  //     values[0] = GameManager.Instance.CurrentLevel + 1;
  //     values[1] = GameManager.Instance.RetryCount;

  //     if (GameManager.Instance.RetryCount % 2 == 0)
  //     {
  //       LevelPlayAds.Instance.ShowInterstitialAd(
  //         (info) =>
  //         {

  //         },
  //         "Replay",
  //         (info) =>
  //         {
  //           SceneManager.LoadScene("Gameplay");
  //         },
  //         () =>
  //         {
  //           SceneManager.LoadScene("Gameplay");
  //         }
  //       );

  //       return;
  //     }
  //     SceneManager.LoadScene("Gameplay");
  //   }

  //   public void Revive()
  //   {
  //     HeartSystem.Instance.TryIncreaseHeart(1);
  //     SoundManager.Instance.PlayPressBtnSfx();
  //     GameManager.Instance.ReviveCount++;

  //     reviveBtn.interactable = false;
  //     retryBtn.interactable = false;
  // #if !UNITY_EDITOR
  //     LevelPlayAds.Instance.ShowRewardedAd(() =>
  //       {
  //         BalloonSystem.Instance.ShowBalloonWithDelay(0.4f);
  //         LevelManager.Instance.ClearAndContinueGameplay();
  //         var data = new MissonDataDailyTask(enumListeningDataDailyTask.Revive,1);
  //         EventActionManager.TriggerEvent(enumListeningDataDailyTask.Revive, data);

  //         var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.ReviveWeekly, 1);
  //         EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.ReviveWeekly, dataWeekly);
  //       },
  //       "Revive",
  //       () =>
  //       {
  //         retryBtn.interactable = true;
  //         StartCoroutine(EffectManager.Instance.IEDelayShow(reviveBtn, 1.5f));

  //         GameplayPanel.Instance.ShowNotifyWith("ADS NOT READY");
  //       }
  //     );
  // #else
  //     BalloonSystem.Instance.ShowBalloonWithDelay(0.4f);

  //     var data = new MissonDataDailyTask(enumListeningDataDailyTask.Revive, 1);
  //     EventActionManager.TriggerEvent(enumListeningDataDailyTask.Revive, data);

  //     var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.ReviveWeekly, 1);
  //     EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.ReviveWeekly, dataWeekly);
  // #endif
  //   }

  public void PlayOn()
  {
    GameplayPanel.Instance.ToggleOutOfSpaceModal();
  }
  public void LevelFail()
  {
    GameplayPanel.Instance.OutOfSpaceModal.gameObject.SetActive(false);
    GameplayPanel.Instance.ToggleLevelFailedModal();
  }
}
