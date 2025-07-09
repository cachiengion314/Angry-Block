using DG.Tweening;
using Lean.Touch;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// TouchControlSystem
/// </summary>
public partial class LevelSystem : MonoBehaviour
{
  [Header("TouchControlSystem Datas")]
  readonly float _spawnedEggMoveToScreenDeltaLength = .18f;
  readonly float _lowerBoundTouch = -7;
  readonly float _maxTouchVelocity = 50;
  float _upperBound = 0;
  float _lowerBound = -5;
  float _lastFrameVelocity;

  float ComputeAccelerateFrom(float lastFrameVelocity)
  {
    // if obj outside of Centric and its velocity tend to heading at Centric position
    // we need to calibrate accelerate strong (or weak) enough to make it 
    // moving to Centric slowly when its velocity approchead 0
    // At first, we have: integrate a dt = at + B ==> v = at + V0 
    // at v = 0 ==> at + V0 = 0 ==> t = -V0 / a
    // so we can construct quadratic function y = a/2 t^2 + V0t + Offset
    // when obj reach Centric we have y = Centric ==> a/2 t^2 + V0t + Offset = Centric
    // ==> a = -V0^2 / 2 (Centric - Offset)
    var Y0 = itemSystem.EggHolderParent.transform.position.y;
    var Centric = _upperBound;
    var Offset = Y0;

    var V0 = lastFrameVelocity;
    var A = -V0 * V0 / (2 * (Centric - Offset));
    return A;
  }

  float CalculateAccelerateBy(float lastFrameVelocity)
  {
    var A = 0f;
    if (LeanTouch.Fingers.Count == 0)
      A = ComputeAccelerateFrom(lastFrameVelocity);
    if (lastFrameVelocity == 0) A = 0f;
    return A;
  }

  float CalculateVelocityBy(float lastFrameVelocity)
  {
    var Y0 = itemSystem.EggHolderParent.transform.position.y;

    var accelerate = CalculateAccelerateBy(lastFrameVelocity);
    var v = lastFrameVelocity + accelerate * Time.deltaTime;
    if (math.abs(v) < .01f) v = 0;
    if (lastFrameVelocity > 0 && Y0 > _upperBound && LeanTouch.Fingers.Count > 0)
    {
      // since Fingers.Count > 0 it clearly that lastFrameVelocity is user's scrolling speed
      // so velocity should be depended on lastFrameVelocity.
      // which is: coeffency = (another fancy coeffency) * lastFrameVelocity * -sign(lastFrameVelocity)
      // We don't want (y - _upperBound) reach 5 so v = 0 at (y - _upperBound) = 5
      // we have: coeffency * 5 + offset = 0
      // ==> coeffency * 5 = -offset. 
      // ==> offset = -5 * coeffency
      var coeffency = .22f * lastFrameVelocity * -math.sign(lastFrameVelocity);
      v = coeffency * (Y0 - _upperBound) + (-5 * coeffency);
    }
    return v;
  }

  float CalculatePositionBy(float v, float lastFramePosition)
  {
    var y = lastFramePosition + v * Time.deltaTime;
    return y;
  }

  void OnTouchEnd(float2 touchPos, float2 touchingDir)
  {
    if (_isOnMatchingSuccessInvoke) return;
    if (_isOnMoveEggsToScreenInvoke) return;
    if (_hasEggMoveToHolderTweening) return;
    if (_isOnMatchingFailInvoke) return;

    var dirToUpperBound
      = new float3(0, _upperBound, 0)
        - new float3(0, itemSystem.EggHolderParent.transform.position.y, 0);
    _lastFrameVelocity = math.min(math.abs(dirToUpperBound.y) * 12, _maxTouchVelocity);
    _lastFrameVelocity *= math.sign(dirToUpperBound.y);
  }

  void OnTouchMoved(float2 touchPos, float2 touchingDir)
  {
    if (touchPos.y < _lowerBoundTouch) return;

    if (_isOnMatchingSuccessInvoke) return;
    if (_isOnMoveEggsToScreenInvoke) return;
    if (_hasEggMoveToHolderTweening) return;
    if (_isOnMatchingFailInvoke) return;

    var _dir = .8f * touchingDir.y;
    var currentVelocity = _dir;
    if (math.abs(currentVelocity) > 0)
      _lastFrameVelocity = currentVelocity;
    _lastFrameVelocity = math.min(_maxTouchVelocity, _lastFrameVelocity);
  }

  private void OnTouchUpdate()
  {
    if (_isOnMatchingSuccessInvoke) return;
    if (_isOnMoveEggsToScreenInvoke) return;
    if (_hasEggMoveToHolderTweening) return;
    if (_isOnMatchingFailInvoke) return;

    var eggHolderParent = itemSystem.EggHolderParent;
    var Y0 = eggHolderParent.transform.position.y;

    var currentVelocity = CalculateVelocityBy(_lastFrameVelocity);
    var y = CalculatePositionBy(currentVelocity, Y0);
    y = math.clamp(y, _lowerBound, _upperBound + 10);
    if (currentVelocity == 0 && LeanTouch.Fingers.Count == 0) y = _upperBound;

    var nextPos = new Vector3(0, y, 0);
    itemSystem.EggHolderParent.transform.position = nextPos;

    _lastFrameVelocity = currentVelocity;
  }

  void OnTouchBegan(float2 touchPos, Collider2D[] touchedColliders)
  {
    if (_isOnMatchingSuccessInvoke) return;
    if (_isOnMoveEggsToScreenInvoke) return;
    if (_hasEggMoveToHolderTweening) return;
    if (_isOnMatchingFailInvoke) return;

    if (math.abs(_lastFrameVelocity) > 0) return;
    if (itemSystem.EggHolderParent.transform.position.y != _upperBound) return;

    var touchedEgg = itemSystem.FindEggFrom(touchedColliders);
    if (touchedEgg == null) return;
    if (touchedEgg.PlacedIndex > -1) return;
    if (_moveEggTicketsAmount <= 0)
    {
      if (touchedEgg.TryGetComponent<IFeedbackControl>(out var feedbackComp))
      {
        feedbackComp.InjectChannel(touchedEgg.GetInstanceID());
        FeedbackSystem.Instance.PlayRandomShakesAt(
          touchedEgg.GetInstanceID(), 1, .3f
        );
        feedbackComp.InjectChannel(-98);
      }
      OnCannotMoveEgg(touchedEgg);
      return;
    }

    var currHolder = itemSystem.GetFirstEggHolder();
    if (currHolder == null) return;

    var nearestEmptyIndex = currHolder.FindFirstEmptyIndex();

    var seq = DOTween.Sequence();
    var currentAnimDuration = 0f;

    var moveToHolderLength = .24f;
    var removeWrongEggLength = .82f;
    var afterRemoveWrongEggDelayLength = .4f;

    var eggsMoveToDestinationLength = 1.4f;
    var giftPopOutLength = .82f;
    var giftMoveToDestinationLength = .24f;

    if (nearestEmptyIndex > -1)
    {
      FoundEmptySlotIndex(
        nearestEmptyIndex,
        currHolder,
        touchedEgg,
        moveToHolderLength,
        currentAnimDuration,
        ref seq
      );
      currentAnimDuration += moveToHolderLength + Time.deltaTime;
    }

    if (currHolder.CountEmpty() == 0)
    {
      if (
        !HasMatchedBetween(
          GetCurrentDestinationOrder1(),
          currHolder.GetCurrentPlacedOrder(),
          out EggControl[] rightOrder,
          out EggControl[] wrongOrder
        )
      )
      {
        MatchingFail(
          rightOrder,
          wrongOrder,
          currHolder,
          removeWrongEggLength,
          afterRemoveWrongEggDelayLength,
          _spawnedEggMoveToScreenDeltaLength,
          currentAnimDuration,
          ref seq
        );
        return;
      }
      MatchingSuccess(
        eggsMoveToDestinationLength,
        giftPopOutLength,
        giftMoveToDestinationLength,
        _spawnedEggMoveToScreenDeltaLength,
        currHolder,
        currentAnimDuration,
        ref seq
      );
    }
  }
}