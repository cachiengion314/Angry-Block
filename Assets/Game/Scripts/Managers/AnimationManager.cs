using System;
using MoreMountains.Feedbacks;
using Unity.Mathematics;
using UnityEngine;

///
/// <summary>
/// Animation game logic
/// </summary>
///
public class AnimationManager : MonoBehaviour
{
  public static AnimationManager Instance { get; private set; }

  [Header("Feedbacks")]
  public MMF_Player PositionFeedback;
  public MMF_Player ShakeFeedback;
  public MMF_Player ShakeCoinFeedback;
  public MMF_Player RendererFeedback;

  [Header("Settings")]
  // public DragAndDrop CurrentBeingDragged;
  [Range(0, 100)]
  [SerializeField] float tweenSlowFactor;

  private void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else Destroy(gameObject);
  }

  public async void PlayShakesCoinAt(int channelId = 1, float shakeRange = .5f, Action onCompleted = null)
  {
    var MMF_PositionShake = ShakeCoinFeedback.GetFeedbackOfType<MMF_PositionShake>();
    MMF_PositionShake.Channel = channelId;
    MMF_PositionShake.ShakeRange = shakeRange;

    await ShakeCoinFeedback.PlayFeedbacksTask();
    onCompleted?.Invoke();
  }

  public async void PlayShakesAt(int channelId = 1, float shakeRange = .5f, Action onCompleted = null)
  {
    var MMF_PositionShake = ShakeFeedback.GetFeedbackOfType<MMF_PositionShake>();
    MMF_PositionShake.Channel = channelId;
    MMF_PositionShake.ShakeRange = shakeRange;

    await ShakeFeedback.PlayFeedbacksTask();
    onCompleted?.Invoke();
  }

  public async void PlayScale(Transform obj, float _intensitive = 2, Action onCompleted = null)
  {
    var MMF_Scale = PositionFeedback.GetFeedbackOfType<MMF_Scale>();
    MMF_Scale.AnimateScaleTarget = obj;
    MMF_Scale.RemapCurveOne = _intensitive;

    await PositionFeedback.PlayFeedbacksTask();
    onCompleted?.Invoke();
  }

  public LTDescr MoveTo(float3 desPos, float3 fromPos, GameObject moveObj, float slowFactor = .1f, Action onCompleted = null)
  {
    moveObj.transform.position = fromPos;
    return LeanTween.move(moveObj, desPos, slowFactor * tweenSlowFactor)
      .setOnComplete(() =>
      {
        onCompleted?.Invoke();
      });
  }

  public LTDescr ScaleTo(float3 desSize, GameObject scaleObj, float slowFactor = .1f, Action onCompleted = null)
  {
    return LeanTween.scale(scaleObj, desSize, slowFactor * tweenSlowFactor)
      .setOnComplete(() =>
      {
        onCompleted?.Invoke();
      });
  }

  public LTDescr ValueWith(GameObject obj, float to, float from, float slowFactor = 1f, Action<float> onUpdate = null)
  {
    return LeanTween.value(obj, from, to, slowFactor * tweenSlowFactor)
      .setOnUpdate((float val) =>
      {
        onUpdate?.Invoke(val);
      });
  }
}
