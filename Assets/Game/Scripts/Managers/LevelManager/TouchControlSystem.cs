using System.Collections.Generic;
using Lean.Touch;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Header("Touch Control System")]
  bool _isUserScreenTouching;
  public bool IsUserScreenTouching { get { return _isUserScreenTouching; } }

  void SubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown += OnFingerDown;
    LeanTouch.OnGesture += OnGesture;
    LeanTouch.OnFingerInactive += OnFingerInactive;
  }

  void UnsubscribeTouchEvent()
  {
    LeanTouch.OnFingerDown -= OnFingerDown;
    LeanTouch.OnGesture -= OnGesture;
    LeanTouch.OnFingerInactive -= OnFingerInactive;
  }

  private void OnFingerDown(LeanFinger finger)
  {
    _isUserScreenTouching = true;

    Vector2 startTouchPos = Camera.main.ScreenToWorldPoint(finger.ScreenPosition);
    Collider2D[] colliders = Physics2D.OverlapPointAll(startTouchPos);

    if (IsTriggerBooster1)
      OnTriggerBooster1(FindDirectionBlockIn(colliders));
    else
      TouchControlling(FindDirectionBlockIn(colliders));
  }

  void OnGesture(List<LeanFinger> list)
  {
    _isUserScreenTouching = true;
  }

  private void OnFingerInactive(LeanFinger finger)
  {
    _isUserScreenTouching = false;
  }
}