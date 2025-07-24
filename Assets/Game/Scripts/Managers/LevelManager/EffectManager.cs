using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  float3 _shakeCenterPos;
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
    _shakeCenterPos = shakeCenter;

    for (int i = 0; i < _colorBlocks.Length; ++i)
    {
      var obj = _colorBlocks[i];
      if (obj == null) continue;
      if (obj.gameObject.activeSelf == false) continue;
      if (!obj.TryGetComponent<IMoveable>(out var moveableComp)) continue;

      var startPos = topGrid.ConvertIndexToWorldPos(i);
      var distFromCenterPos = startPos - _shakeCenterPos;
      // we have vector field F(x,y) = 1/r^2 * [x y] / √(x^2 + y^2) * 12
      var distSq = math.lengthsq(distFromCenterPos);
      var F = 1 / distSq * math.normalize(distFromCenterPos) * 12;
      var targetPos = 0f + F;

      var path = new float3[] { 0, targetPos, 0 };
      moveableComp.SetPath(path);
      _needShakingObjs.Add(obj.gameObject);
    }
  }

  void ShakeTopGridUpdate()
  {
    for (int i = 0; i < _needShakingObjs.Count; ++i)
    {
      var obj = _needShakingObjs[i];
      if (obj == null) continue;
      if (!obj.TryGetComponent<ISpriteRend>(out var rendComp)) continue;
      if (!obj.TryGetComponent<IMoveable>(out var moveableComp)) continue;
      if (DOTween.IsTweening(rendComp.GetBodyRenderer())) continue;

      var currentPos = rendComp.GetBodyRenderer().transform.localPosition;
      var path = moveableComp.GetPath();
      if (path == null) continue;

      if (!_needMovingObjPathIndexes.ContainsKey(obj.GetInstanceID()))
        _needMovingObjPathIndexes.Add(obj.GetInstanceID(), 0);
      var currentIdx = _needMovingObjPathIndexes[obj.GetInstanceID()];

      var distFromShakeCenterPos = obj.transform.position - (Vector3)_shakeCenterPos;
      InterpolatePathUpdate(
        currentPos,
        currentIdx,
        path,
        topGridShakeSpeed + math.pow(math.E, -.01f * distFromShakeCenterPos.y),
        out var t,
        out var nextPos,
        out var nextIdx
      );
      rendComp.GetBodyRenderer().transform.localPosition = nextPos;
      _needMovingObjPathIndexes[obj.GetInstanceID()] = nextIdx;
      if (t < 1) continue;

      moveableComp.SetPath(null);
      _needMovingObjPathIndexes[obj.GetInstanceID()] = 0;
      _needShakingObjs.Remove(obj);
    }
  }
}