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
        effectedBlocks.Add(obj);
      }
    }
  }

  void TouchControlling(DirectionBlockControl directionBlock)
  {
    if (directionBlock == null) return;

    var seq = DOTween.Sequence();
    var startDuration = 0.0f;

    if (!IsFirstLineMatchWith(directionBlock.GetColorValue(), out var firstLineMatched))
    {
      var emptyWaitingSlotIndex = FindEmptySlotIndex(_waitingSlots);
      MoveTo(
        emptyWaitingSlotIndex, directionBlock, _waitingSlots, _waitingPositions,
        out DirectionBlockControl needMovingObj1,
        out Vector3[] destinations1
      );
      // MoveTo animation
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
    // MoveTo animation
    var moveDuration = destinations2.Length * .1f;
    seq.Insert(
      startDuration,
      needMovingObj2.transform
        .DOPath(destinations2, moveDuration)
        .SetEase(Ease.InQuad)
        .OnComplete(() => { })
    );

    FireTo(firstLineMatched, directionBlock, out var effectedBlocks);
    // Fire animation
    var fireDeltaDuration = .25f;
    for (int x = 0; x < effectedBlocks.Count; ++x)
    {
      seq.InsertCallback(
        startDuration + moveDuration + x * fireDeltaDuration,
        () =>
        {
          print("InvokeFireAnimationAt");
          directionBlock.InvokeFireAnimationAt(Vector3.up, fireDeltaDuration);
        }
      );
    }
  }
}