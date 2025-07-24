using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  float3 _shakeCenter;
  readonly List<GameObject> _needShakingObjs = new();
  [Range(0, 10f)]
  [SerializeField] float topGridShakeSpeed;
  [SerializeField] CinemachineImpulseSource impulseSource;

  void ShakeCameraBy(Vector3 force)
  {
    impulseSource.GenerateImpulse(force);
  }

  void AddToShakeQueue(float3 shakeCenter)
  {
    _shakeCenter = shakeCenter;
    var shakeCenterGridPos = topGrid.ConvertWorldPosToGridPos(shakeCenter);

    for (int i = 0; i < _colorBlocks.Length; ++i)
    {
      var obj = _colorBlocks[i];
      if (obj == null) continue;
      if (!obj.TryGetComponent<ISpriteRend>(out var rendComp)) continue;
      if (DOTween.IsTweening(rendComp.GetBodyRenderer().transform)) continue;
      if (!obj.TryGetComponent<IMoveable>(out var moveableComp)) continue;

      var startGridPos = topGrid.ConvertIndexToGridPos(i);
      var distFromCenterGrid = startGridPos - shakeCenterGridPos;
      var targetGridPos = startGridPos + math.normalize(distFromCenterGrid);
      var targetPos = topGrid.ConvertGridPosToWorldPos((int2)targetGridPos);

      moveableComp.SetInitPostion(obj.transform.position);
      moveableComp.SetLockedPosition(targetPos);
      _needShakingObjs.Add(obj.gameObject);
    }
  }

  void ShakeTopGridUpdate()
  {
    var shakeCenterGrid = topGrid.ConvertWorldPosToGridPos(_shakeCenter);

    for (int i = 0; i < _needShakingObjs.Count; ++i)
    {
      var obj = _needShakingObjs[i];
      if (obj == null) continue;
      if (!obj.TryGetComponent<ISpriteRend>(out var rendComp)) continue;
      if (DOTween.IsTweening(rendComp.GetBodyRenderer().transform)) continue;
      if (!obj.TryGetComponent<IMoveable>(out var moveableComp)) continue;

      var startGrid = topGrid.ConvertWorldPosToGridPos(moveableComp.GetInitPostion());
      var distFromCenterGrid = startGrid - shakeCenterGrid;

      var speed = topGridShakeSpeed
          + math.pow(math.E, -arrangeDampSpeed * distFromCenterGrid.x)
          + math.pow(math.E, -arrangeDampSpeed * distFromCenterGrid.y);

      HoangNam.Utility.InterpolateMoveUpdate(
        obj.transform.position,
        moveableComp.GetInitPostion(),
        moveableComp.GetLockedPosition(),
        speed,
        out var t,
        out var nextPos
      );
      obj.transform.position = nextPos;
      if (t < 1) continue;

      var initPos = moveableComp.GetInitPostion();
      var targetPos = moveableComp.GetLockedPosition();
      moveableComp.SetInitPostion(targetPos);
      moveableComp.SetLockedPosition(initPos);
      _needShakingObjs.Remove(obj);
    }
  }
}