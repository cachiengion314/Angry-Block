using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Header("DirectionBlock Manager")]
  [SerializeField] int waitingSlotAMount = 7;
  float3[] _waitingPositions;
  DirectionBlockControl[] _waitingSlots;
  [SerializeField] int firingSlotAMount = 4;
  float3[] _firingPositions;
  DirectionBlockControl[] _firingSlots;
  /// <summary>
  /// Manager all direction blocks
  /// </summary>
  DirectionBlockControl[] _directionBlocks;
  public DirectionBlockControl[] DirectionBlocks { get { return _directionBlocks; } }

  void InitFiringPositions()
  {
    _firingPositions = new float3[firingSlotAMount];
    _firingSlots = new DirectionBlockControl[firingSlotAMount];

    var y = bottomGrid.GridSize.y - 1;
    var startX = 2;
    for (int x = startX; x < startX + firingSlotAMount; ++x)
    {
      if (x > bottomGrid.GridSize.x - 1) break;
      var pos = bottomGrid.ConvertGridPosToWorldPos(new int2(x, y));
      _firingPositions[x - startX] = pos;
    }
  }

  void InitWaitingPositions()
  {
    _waitingPositions = new float3[waitingSlotAMount];
    _waitingSlots = new DirectionBlockControl[waitingSlotAMount];

    var y = bottomGrid.GridSize.y - 1 - 1;
    var startX = 1;
    for (int x = startX; x < startX + waitingSlotAMount; ++x)
    {
      if (x > bottomGrid.GridSize.x - 1) break;
      var pos = bottomGrid.ConvertGridPosToWorldPos(new int2(x, y));
      _waitingPositions[x - startX] = pos;
    }
  }

  DirectionBlockControl FindDirectionBlockIn(Collider2D[] cols)
  {
    var colorBlock = FindObjIn<DirectionBlockControl>(cols);
    return colorBlock;
  }

  int FindEmptySlotIndex(DirectionBlockControl[] slots)
  {
    for (int i = 0; i < slots.Length; ++i)
    {
      if (slots[i] == null) return i;
    }
    return -1;
  }

  bool IsPosOccupiedAt(float3 pos)
  {
    if (bottomGrid.IsPosOutsideAt(pos)) return true;
    var idx = bottomGrid.ConvertWorldPosToIndex(pos);
    if (_directionBlocks[idx] != null) return true;
    return false;
  }

  void MoveTo(
    in int slotIndex,
    in DirectionBlockControl directionBlock,
    in DirectionBlockControl[] slots,
    in float3[] positions,
    out DirectionBlockControl needMovingObj,
    out Vector3[] destinations
  )
  {
    needMovingObj = directionBlock;
    destinations = new Vector3[0];

    if (slotIndex > positions.Length - 1 || slotIndex < 0)
    {
      print("Slots is not available");
      return;
    }

    var endPos = positions[slotIndex];
    var startGridPos = bottomGrid.ConvertWorldPosToGridPos(directionBlock.transform.position);
    var nextGridPos = startGridPos + directionBlock.Direction;
    var nextPos = bottomGrid.ConvertGridPosToWorldPos(nextGridPos);
    if (IsPosOccupiedAt(nextPos))
    {
      print("Cannot move due to occupied position");
      return;
    }
    var moves = bottomGrid.PathFindingTo(endPos, nextPos, new int2[0]);
    if (moves.Length == 0)
    {
      print("Cannot move due to no available moves");
      moves.Dispose();
      return;
    }

    // logic
    _directionBlocks[directionBlock.GetIndex()] = null;
    var index = bottomGrid.ConvertWorldPosToIndex(endPos);
    directionBlock.SetIndex(index);
    slots[slotIndex] = directionBlock;
    _directionBlocks[index] = directionBlock;

    /// animation
    var _moves = new Vector3[moves.Length];
    for (int i = 0; i < moves.Length; ++i)
    {
      var movePos = bottomGrid.ConvertGridPosToWorldPos(moves[i]);
      _moves[i] = movePos;
    }
    destinations = _moves;
    moves.Dispose();
  }

  void FireTo(
    List<ColorBlockControl> firstLineMatched,
    DirectionBlockControl directionBlock,
    out List<ColorBlockControl> effectedBlocks
  )
  {
    effectedBlocks = new List<ColorBlockControl>();
    for (int x = 0; x < firstLineMatched.Count; ++x)
    {
      var obj = firstLineMatched[x];
      if (obj == null) continue;
      if (directionBlock.GetAmmunition() <= 0) break;

      if (obj.TryGetComponent<IDamageable>(out var damageable))
      {
        directionBlock.SetAmmunition(directionBlock.GetAmmunition() - 1);
        damageable.SetHealth(damageable.GetHealth() - 1);
        if (damageable.IsDead())
        {
          _colorBlocks[obj.GetIndex()] = null;
        }
        effectedBlocks.Add(obj);
      }
    }
  }

  void ArrangeAt(
    in int collumn,
    out List<ColorBlockControl> needMovingObjs,
    out List<float3> destinations
  )
  {
    needMovingObjs = new List<ColorBlockControl>();
    destinations = new List<float3>();

    for (int y = 0; y < topGrid.GridSize.y; ++y)
    {
      var grid = new int2(collumn, y);
      var idx = topGrid.ConvertGridPosToIndex(grid);
      var obj = _colorBlocks[idx];
      if (obj == null) continue;
      var downGrid = grid + new int2(0, -1);
      var downIdx = topGrid.ConvertGridPosToIndex(downGrid);
      if (_colorBlocks[downIdx] == null)
      {
        _colorBlocks[obj.GetIndex()] = null;
        _colorBlocks[downIdx] = obj;
        obj.SetIndex(downIdx);
        needMovingObjs.Add(obj);
        destinations.Add(topGrid.ConvertGridPosToWorldPos(downGrid));
      }
    }
  }

  void TouchControlling(DirectionBlockControl directionBlock)
  {
    if (directionBlock == null) return;

    var seq = DOTween.Sequence();
    var startDuration = 0.0f;

    if (!IsFirstRowMatchWith(directionBlock.GetColorValue(), out var firstLineMatched))
    {
      var emptyWaitingSlotIndex = FindEmptySlotIndex(_waitingSlots);
      MoveTo(
        emptyWaitingSlotIndex, directionBlock, _waitingSlots, _waitingPositions,
        out DirectionBlockControl needMovingObj1,
        out Vector3[] destinations1
      );
      // Move to waiting animation
      needMovingObj1.transform
        .DOPath(destinations1, destinations1.Length * .1f)
        .SetEase(Ease.InQuad)
        .OnComplete(() => { });
      return;
    }

    var emptyFiringSlotIndex = FindEmptySlotIndex(_firingSlots);
    MoveTo(
      emptyFiringSlotIndex, directionBlock, _firingSlots, _firingPositions,
      out DirectionBlockControl needMovingObj2,
      out Vector3[] destinations2
    );
    if (destinations2.Length == 0) return;

    // Move to firing position animation
    var moveDuration = destinations2.Length * .1f;
    seq.Insert(
      startDuration,
      needMovingObj2.transform
        .DOPath(destinations2, moveDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() => { })
    );
    startDuration += moveDuration + Time.deltaTime;

    FireTo(firstLineMatched, directionBlock, out var effectedBlocks);
    // Fire animation
    var fireDeltaDuration = .2f;
    var fireDuration = fireDeltaDuration * effectedBlocks.Count;
    for (int x = 0; x < effectedBlocks.Count; ++x)
    {
      var block = effectedBlocks[x];

      seq.InsertCallback(
        startDuration + x * fireDeltaDuration,
        () =>
        {
          directionBlock.InvokeFireAnimationAt(Vector3.up, fireDeltaDuration);
        }
      );

      if (block.IsDead())
      {
        seq.InsertCallback(
          startDuration + x * fireDeltaDuration,
          () =>
          {
            Destroy(block.gameObject);
          }
        );
      }
    }
    startDuration += fireDuration + Time.deltaTime;

    if (directionBlock.GetAmmunition() == 0)
    {
      seq.InsertCallback(startDuration, () => SelfDestruct(directionBlock));
    }
  }

  const string KEY_SELF_DESTRUCT = "self_destruct_";
  void SelfDestruct(DirectionBlockControl directionBlock)
  {
    if (
     _runningAnimations.ContainsKey(KEY_SELF_DESTRUCT + directionBlock.GetInstanceID())
     && _runningAnimations[KEY_SELF_DESTRUCT + directionBlock.GetInstanceID()]
   ) return;

    _runningAnimations[KEY_SELF_DESTRUCT + directionBlock.GetInstanceID()] = true;

    var seq = DOTween.Sequence();
    var startDuration = 0.0f;
    var selfDestructDuration = .3f;

    var firingSlotIdx = -1;
    for (int i = 0; i < _firingSlots.Length; ++i)
    {
      if (_firingSlots[i] != null
         && _firingSlots[i].GetIndex() != directionBlock.GetIndex()
       )
        continue;
      firingSlotIdx = i;
      break;
    }
    _firingSlots[firingSlotIdx] = null;
    _colorBlocks[directionBlock.GetIndex()] = null;

    seq.Insert(
      startDuration,
      directionBlock.transform
        .DOScale(new float3(1.3f, 1.3f, 1.3f), selfDestructDuration / 2f)
        .SetLoops(2, LoopType.Yoyo)
    );
    startDuration += selfDestructDuration + Time.deltaTime;

    seq.InsertCallback(
      startDuration,
      () =>
      {
        Destroy(directionBlock.gameObject);
        _runningAnimations[KEY_SELF_DESTRUCT + directionBlock.GetInstanceID()] = false;
      }
    );
  }

  const string KEY_ARRANGE_COLLUMN = "arrange_collumn_";
  void RearrangeTopGrid()
  {
    if (IsFirstRowFull(out var emptyCol))
    {
      return;
    }
    if (
      _runningAnimations.ContainsKey(KEY_ARRANGE_COLLUMN + emptyCol)
      && _runningAnimations[KEY_ARRANGE_COLLUMN + emptyCol]
    ) return;

    _runningAnimations[KEY_ARRANGE_COLLUMN + emptyCol] = true;
    ArrangeAt(
      emptyCol,
      out List<ColorBlockControl> needMovingObjs3,
      out List<float3> destinations3
     );

    var seq = DOTween.Sequence();
    var startDuration = 0.0f;
    var rearrangeDuration = .3f;

    for (int i = 0; i < needMovingObjs3.Count; ++i)
    {
      var block = needMovingObjs3[i];
      var des = destinations3[i];

      seq.Insert(
        startDuration,
        block.transform
          .DOMove(des, rearrangeDuration)
      );
    }
    startDuration += rearrangeDuration + Time.deltaTime;
    seq.InsertCallback(
      startDuration,
      () =>
      {
        _runningAnimations[KEY_ARRANGE_COLLUMN + emptyCol] = false;
      }
    );
  }
}