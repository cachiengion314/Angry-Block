
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public struct AnimateMovingData<T>
{
  public T needMovingObj;
  public GameObject needMovingObjParent;
  public Vector3 fromPosition;
  public Vector3 destination;
}

/// <summary>
/// VisualizeSystem
/// </summary>
public partial class LevelSystem : MonoBehaviour
{
  bool _hasEggMoveToHolderTweening;
  bool _isOnMatchingSuccessInvoke;
  bool _isOnMatchingFailInvoke;
  bool _isOnMoveEggsToScreenInvoke;

  void VisualizeMoveToHolder(
    AnimateMovingData<EggControl> data,
    float animLength, float currentAnimDuration, ref Sequence seq
  )
  {
    var currObj = data.needMovingObj;
    var fromPos = data.fromPosition;
    var desPos = data.destination;
    var parent = data.needMovingObjParent;
    Vector3[] movePaths = new Vector3[2] {
        fromPos,
        desPos,
    };

    _hasEggMoveToHolderTweening = true;

    seq.Insert(
      currentAnimDuration,
      currObj.transform.DOPath(
        movePaths,
        animLength,
        PathType.CatmullRom, PathMode.Sidescroller2D
      )
        .SetEase(Ease.Linear)
        .OnComplete(() =>
        {
          _hasEggMoveToHolderTweening = false;

          currObj.SetSortingOrder(
            parent.GetComponent<EggHolderControl>().GetSortingOrder() + 1
          );
          parent.GetComponent<EggHolderControl>().LinkChilds();
          OnCompletedMoveToHolder(currObj);
        })
    );
  }

  void VisualizeMoveEggsToScreen(
    List<AnimateMovingData<EggControl>> datas,
    float animDeltaLength, float currentAnimDuration, ref Sequence seq
  )
  {
    _isOnMoveEggsToScreenInvoke = true;

    var totalLength = animDeltaLength * datas.Count;
    var eachEggWholeLength = .5f * totalLength;
    var eachEggPaddingLength = (totalLength - eachEggWholeLength) / datas.Count;

    for (int i = 0; i < datas.Count; ++i)
    {
      var data = datas[i];
      var currObj = data.needMovingObj;
      var fromPos = data.fromPosition;
      var desPos = data.destination;

      seq.Insert(
        currentAnimDuration + eachEggPaddingLength * i,
        currObj.transform.DOMove(
          desPos,
          eachEggPaddingLength + eachEggWholeLength
        )
          .SetEase(Ease.Linear)
          .OnComplete(() =>
          {
            OnCompletedEggMoveToScreen(currObj);

            currObj.GetComponent<IFeedbackControl>().InjectChannel(
              currObj.GetInstanceID()
            );
            FeedbackSystem.Instance.PlayRandomRotationShakesAt(currObj.GetInstanceID());
            currObj.GetComponent<IFeedbackControl>().InjectChannel(-99);

            _isOnMoveEggsToScreenInvoke = false;
          })
        );
    }
    // seq.InsertCallback(currentAnimDuration + totalLength, () => { Debug.Break(); });
  }
}