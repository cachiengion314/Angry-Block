using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class LevelTextEffectPanel : MonoBehaviour
{
  [SerializeField] Animator levelEffectAnim;
  [SerializeField] ParticleSystem completedEfx;
  [SerializeField] Image img;
  [SerializeField] SkeletonGraphic levelCompleted;
  [SerializeField] SkeletonGraphic stageCompleted;

  public void ShowTextFrom(int content)
  {

  }

  public void PlayLevelCompletedAnim(Action onCompleted = null)
  {
    completedEfx.Play();

    levelCompleted.gameObject.SetActive(true);
    stageCompleted.gameObject.SetActive(false);

    var startAnim = levelCompleted.Skeleton.Data.FindAnimation("Start");
    float duration = startAnim.Duration;

    LeanTween.delayedCall(gameObject, duration, () =>
    {
      gameObject.SetActive(false);
      onCompleted?.Invoke();
    });
  }

  public void PlayStageCompletedAnim(Action onCompleted = null)
  {
    completedEfx.Play();

    levelCompleted.gameObject.SetActive(false);
    stageCompleted.gameObject.SetActive(true);

    var startAnim = stageCompleted.Skeleton.Data.FindAnimation("Start");
    float duration = startAnim.Duration;

    LeanTween.delayedCall(gameObject, duration, () =>
    {
      gameObject.SetActive(false);
      onCompleted?.Invoke();
    });
  }
}
