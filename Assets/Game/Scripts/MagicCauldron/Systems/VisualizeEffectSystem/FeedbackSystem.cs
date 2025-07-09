using System;
using MoreMountains.Feedbacks;
using UnityEngine;


public class FeedbackSystem : MonoBehaviour
{
  public static FeedbackSystem Instance { get; private set; }

  [SerializeField] MMF_Player RandomRotaionShakeFeedback;
  [SerializeField] MMF_Player PositionUpShakeFeedback;
  [SerializeField] MMF_Player RandomPositionShakeFeedback;
  [SerializeField] MMF_Player UIPositionShakeFeedback;
  [SerializeField] MMF_Player SquashStretchFeedback;

  void Start()
  {
    Instance = this;
  }

  public async void PlayCameraShake(
    float _shakeRange = .12f, float _duration = .44f, Action _onCompleted = null
  )
  {
    PlayRandomShakesAt(120, _shakeRange, _duration, _onCompleted);

    var MMF_PositionShake = UIPositionShakeFeedback.GetFeedbackOfType<MMF_PositionShake>();
    MMF_PositionShake.Channel = 110;
    MMF_PositionShake.ShakeRange = 7;
    MMF_PositionShake.Duration = _duration;

    await UIPositionShakeFeedback.PlayFeedbacksTask();
  }

  public async void PlayRandomShakesAt(
    int channelId,
    float _shakeRange = .5f,
    float _duration = .2f,
    Action _onCompleted = null
  )
  {
    var MMF_PositionShake = RandomPositionShakeFeedback.GetFeedbackOfType<MMF_PositionShake>();
    MMF_PositionShake.Channel = channelId;
    MMF_PositionShake.ShakeRange = _shakeRange;
    MMF_PositionShake.Duration = _duration;

    await RandomPositionShakeFeedback.PlayFeedbacksTask();
    _onCompleted?.Invoke();
  }

  public async void PlayRandomRotationShakesAt(
    int channelId,
    float _shakeRange = 40f,
    float _duration = .2f,
    Action _onCompleted = null
  )
  {
    var MMF_RotationShake = RandomRotaionShakeFeedback.GetFeedbackOfType<MMF_RotationShake>();
    MMF_RotationShake.Channel = channelId;
    MMF_RotationShake.ShakeRange = _shakeRange;
    MMF_RotationShake.Duration = _duration;

    await RandomRotaionShakeFeedback.PlayFeedbacksTask();
    _onCompleted?.Invoke();
  }

  public async void PlayShakesAt(
    int channelId,
    Vector2 shakeMainDir,
    float _shakeRange = .5f,
    float _duration = .2f,
    Action _onCompleted = null
  )
  {
    var MMF_PositionShake = PositionUpShakeFeedback.GetFeedbackOfType<MMF_PositionShake>();

    MMF_PositionShake.Channel = channelId;
    MMF_PositionShake.ShakeRange = _shakeRange;
    MMF_PositionShake.Duration = _duration;
    MMF_PositionShake.ShakeMainDirection = shakeMainDir;

    await PositionUpShakeFeedback.PlayFeedbacksTask();
    _onCompleted?.Invoke();
    MMF_PositionShake.Channel = -999;
  }

  public async void PlaySquashStretchFor(
    Transform obj, float _duration = .2f, Action _onCompleted = null
  )
  {
    var MMF_SquashAndStretch = SquashStretchFeedback.GetFeedbackOfType<MMF_SquashAndStretch>();
    MMF_SquashAndStretch.AnimateScaleDuration = _duration;
    MMF_SquashAndStretch.SquashAndStretchTarget = obj;

    await SquashStretchFeedback.PlayFeedbacksTask();
    _onCompleted?.Invoke();
  }
}