using System;
using System.Collections.Generic;
using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public class BalloonControl : MonoBehaviour
{
  [Header("")]
  [SerializeField] BoxCollider2D colBalloon;
  [SerializeField] LayerMask balloonLayer;
  [SerializeField] SkeletonAnimation skeAnim;

  private readonly float3 _defaultPos = new(0, -14, 0);

  private bool _isFlying = false;
  public bool IsFlying { get { return _isFlying; } }

  void Update()
  {
    PressControl();
  }

  bool _isClicked;
  void PressControl()
  {
    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

      switch (touch.phase)
      {
        case TouchPhase.Began:
          Collider2D _col = Physics2D.OverlapPoint(touchPos, balloonLayer);

          if (colBalloon == _col)
          {
            if (_isClicked) return;

            LeanTween.pause(gameObject);
            LevelPlayAds.Instance.ShowRewardedAd(() =>
            {
              UnityMainThreadDispatcher.Instance().Enqueue(() =>
              {
                _isClicked = true;
                LeanTween.cancel(gameObject);

                GameManager.Instance.ProgressBalloon -= 200;

                if (skeAnim.AnimationState != null)
                {
                  skeAnim.AnimationState.SetAnimation(0, "click", false);
                  var duration = skeAnim.Skeleton.Data.FindAnimation("click").Duration;

                  DOVirtual.DelayedCall(duration,
                    () =>
                    {
                      gameObject.SetActive(false);
                      transform.position = _defaultPos;
                      _isClicked = false;
                      ClaimReward();
                    }
                  );
                }
                else
                {
                  gameObject.SetActive(false);
                  transform.position = _defaultPos;
                  _isClicked = false;
                  ClaimReward();
                }
              });
            },
            "PressBalloon",
            () =>
            {
              LeanTween.delayedCall(gameObject, 1.5f, () =>
              {
                LeanTween.resume(gameObject);
                _isClicked = false;
              });

              if (!_isClicked)
              {
                _isClicked = true;
              }
            });
          }
          break;
      }
    }
  }

  private void ClaimReward()
  {
    var idItem = UnityEngine.Random.Range(0, 2);

    if (idItem == 0)
    {
      EffectManager.Instance.SpawnEfxCoinAt(Vector3.zero, 1,
        () =>
        {
          _isFlying = false;
          Debug.Log("Is Flying: " + _isFlying);
        }
      );
      return;
    }

    GameManager.Instance.CurrentTicket++;
    EffectManager.Instance.SpawnEfxTicketAt(Vector3.zero, 1,
      () =>
      {
        _isFlying = false;
        Debug.Log("Is Flying: " + _isFlying);
      }
    );
  }

  public void Move()
  {
    transform.position = _defaultPos;

    var currentPos = transform.position;
    var targetPoses = CalculateTargetPos(currentPos);

    if (targetPoses == null || targetPoses.Count < 4) { return; }

    _isFlying = true;
    skeAnim.AnimationState.SetAnimation(0, "loop", true);
    LeanTween.moveSpline(gameObject, targetPoses.ToArray(), 25)
      .setEase(LeanTweenType.linear)
      .setTarget(transform)
      .setOnComplete(() =>
      {
        _isFlying = false;
      });
  }

  private List<Vector3> CalculateTargetPos(Vector3 currentPos)
  {
    var cam = Camera.main;
    float height = cam.orthographicSize * 2f + cam.transform.position.y;
    float width = cam.orthographicSize * cam.aspect * 2f;

    List<Vector3> targetPos = new();
    Vector3 _currentPos = currentPos;
    targetPos.Add(_currentPos);

    while (_currentPos.y <= height)
    {
      Vector3 _nextPos = _currentPos;

      if (_currentPos.x < 0)
      {
        _nextPos.x = UnityEngine.Random.Range(1f, width / 2 - 0.5f);
        _nextPos.y = UnityEngine.Random.Range(_currentPos.y + 2, _currentPos.y + 6);
        targetPos.Add(_nextPos);
        _currentPos = _nextPos;
        continue;
      }

      _nextPos.x = UnityEngine.Random.Range(0.5f - width / 2, -1f);
      _nextPos.y = UnityEngine.Random.Range(_currentPos.y + 2, _currentPos.y + 6);
      targetPos.Add(_nextPos);
      _currentPos = _nextPos;
    }

    return targetPos;
  }
}
