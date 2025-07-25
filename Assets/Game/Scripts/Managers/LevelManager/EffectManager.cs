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

  void ShakeBottomGrid(
    Vector3 shakeCenterPos,
    float _dampValue = .7f,
    float _defaultStrength = .2f
  )
  {
    for (int i = 0; i < _directionBlocks.Length; ++i)
    {
      var obj = _directionBlocks[i];
      if (obj == null) continue;

      if (!obj.TryGetComponent<ISpriteRend>(out var rendComp)) continue;
      if (!obj.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (moveable.GetPath() != null) continue;
      if (DOTween.IsTweening(rendComp.GetBodyRenderer().transform)) continue;

      var distFromShakeCenterPos = obj.transform.position - shakeCenterPos;
      var magnitude = math.pow(math.E, -_dampValue * math.abs(distFromShakeCenterPos.y));

      var strength = _defaultStrength + magnitude;
      rendComp.GetBodyRenderer().transform.DOShakePosition(.3f, strength);
    }
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
      var magnitude = 1 / distSq * math.normalize(distFromCenterPos) * 16;
      var targetPos = 0f + magnitude;

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
      if (DOTween.IsTweening(rendComp.GetBodyRenderer().transform)) continue;

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
        topGridShakeSpeed + math.pow(math.E, -.01f * math.abs(distFromShakeCenterPos.y)),
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

  void VisualizeStartColorBlock()
  {
    Sequence seq = DOTween.Sequence();
    var space = 0.03f;
    var duration = 1f;
    for (int i = 0; i < _colorBlocks.Length; i++)
    {
      var block = _colorBlocks[i];
      if (block == null) continue;
      var blockPos = topGrid.ConvertIndexToWorldPos(i);
      var blockGrid = topGrid.ConvertIndexToGridPos(i);

      block.transform.position = blockPos + new float3(0f, 7f, 0f);

      seq.Insert(space * blockGrid.x,
      block.transform.DOMove(blockPos, duration)
      .SetEase(Ease.OutBounce));
    }
  }

  void VisualizeStartDirBlock()
  {
    Sequence seq = DOTween.Sequence();
    var space = 0.03f;
    var duration = 0.06f;
    for (int i = 0; i < _directionBlocks.Length; i++)
    {
      var block = _directionBlocks[i];
      if (block == null) continue;
      var blockPos = bottomGrid.ConvertIndexToWorldPos(i);
      var blockGrid = bottomGrid.ConvertIndexToGridPos(i);

      block.transform.position = blockPos + new float3(0f, 3f, 0f);
      block.SetActive(false);

      seq.InsertCallback(space * i, () => block.SetActive(true));
      seq.Insert(space * i,
      block.transform.DOMove(blockPos, duration)
      .SetEase(Ease.Linear));
    }
  }
}