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

  void TouchControlling(DirectionBlockControl directionBlock)
  {
    if (directionBlock == null) return;

    var firstRowMatched = FindFirstRowMatchedWith(directionBlock.GetColorValue());
    if (firstRowMatched.Count == 0)
    {
      var emptyWaitingSlotIndex = FindEmptySlotIndex(_waitingSlots);
      MoveTo(
        emptyWaitingSlotIndex, directionBlock, _waitingSlots, _waitingPositions
      );
      return;
    }

    var emptyFiringSlotIndex = FindEmptySlotIndex(_firingSlots);
    if (emptyFiringSlotIndex == -1 || emptyFiringSlotIndex > _firingSlots.Length - 1) return;

    MoveTo(
      emptyFiringSlotIndex, directionBlock, _firingSlots, _firingPositions
    );
  }

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

  int FindSlotIndex(DirectionBlockControl block, DirectionBlockControl[] slots)
  {
    for (int i = 0; i < slots.Length; ++i)
    {
      if (slots[i] == null) continue;
      if (slots[i] == block) return i;
    }
    return -1;
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
    int slotIndex,
    DirectionBlockControl directionBlock,
    DirectionBlockControl[] slots,
    in float3[] positions
  )
  {
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
    _directionBlocks[index] = directionBlock;

    /// animation
    var _moves = new Vector3[moves.Length];
    for (int i = 0; i < moves.Length; ++i)
    {
      var movePos = bottomGrid.ConvertGridPosToWorldPos(moves[i]);
      _moves[i] = movePos;
    }

    directionBlock.transform.DOPath(_moves, 1)
    .OnComplete(() =>
    {
      slots[slotIndex] = directionBlock;
    });

    moves.Dispose();
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

  const string KEY_ARRANGE_COLLUMN = "arrange_collumn_";
  void RearrangeTopGridUpdate()
  {
    var needArrangeCollumn = FindNeedArrangeCollumn();
    if (needArrangeCollumn == -1)
    {
      return;
    }
    if (
      _runningAnimations.ContainsKey(KEY_ARRANGE_COLLUMN + needArrangeCollumn)
      && _runningAnimations[KEY_ARRANGE_COLLUMN + needArrangeCollumn]
    ) return;

    _runningAnimations[KEY_ARRANGE_COLLUMN + needArrangeCollumn] = true;
    ArrangeAt(
      needArrangeCollumn,
      out List<ColorBlockControl> needMovingObjs3,
      out List<float3> destinations3
     );

    var seq = DOTween.Sequence();
    var startDuration = 0.0f;
    var rearrangeDuration = .3f;

    for (int i = 0; i < needMovingObjs3.Count; ++i)
    {
      var block = needMovingObjs3[i];
      if (block == null) continue;
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
        _runningAnimations[KEY_ARRANGE_COLLUMN + needArrangeCollumn] = false;
      }
    );
  }

  void ReArrangeTopGridUpdate()
  {
    var needArrangeCollumn = FindNeedArrangeCollumn();
    if (needArrangeCollumn == -1)
    {
      return;
    }
    for (int y = 0; y < topGrid.GridSize.y; ++y)
    {
      var grid = new int2(needArrangeCollumn, y);
      var currentIndex = topGrid.ConvertGridPosToIndex(grid);
      var colorBlock = _colorBlocks[currentIndex];
      if (colorBlock == null) continue;

      var downGrid = grid + new int2(0, -1);
      var targetIndex = topGrid.ConvertGridPosToIndex(downGrid);
      var targetPos = topGrid.ConvertIndexToWorldPos(targetIndex);
      if (targetIndex < 0 || targetIndex > _colorBlocks.Length - 1) continue;

      colorBlock.transform.position += 2.5f * Time.deltaTime * new Vector3(0, -1, 0);
      var currentPos = colorBlock.transform.position;
      var distance = ((Vector3)targetPos - currentPos).magnitude;

      if (distance > .05f) continue;

      _colorBlocks[colorBlock.GetIndex()] = null;
      _colorBlocks[targetIndex] = colorBlock;
      colorBlock.SetIndex(targetIndex);
    }
  }

  void LockAndFireUpddate()
  {
    for (int i = 0; i < _firingSlots.Length; ++i)
    {
      if (_firingSlots[i] == null) continue;

      var directionBlock = _firingSlots[i];
      if (!directionBlock.TryGetComponent<IGun>(out var gun)) continue;

      if (directionBlock.GetAmmunition() <= 0)
      {
        var idx = FindSlotIndex(directionBlock, _firingSlots);
        _firingSlots[idx] = null;
        Destroy(directionBlock.gameObject);
        continue;
      }

      var colorBlock = FindFirstBlockMatchedWith(directionBlock.GetColorValue());
      if (colorBlock == null)
      {
        // go to waiting slot
        continue;
      }

      if (!colorBlock.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != directionBlock) continue;

      damageable.SetWhoPicked(directionBlock); // picking this target to prevent other interfere
      var rotSpeed = 5;
      var dirToTarget = colorBlock.transform.position - directionBlock.transform.position;
      var targetRad = math.acos(
        math.dot(dirToTarget.normalized, directionBlock.transform.up)
      );
      if (math.abs(targetRad) > .1f)
      {
        var sign = math.sign(
          math.cross(directionBlock.transform.up, dirToTarget).z
        );
        var deltaTargetRad = sign * Time.deltaTime * rotSpeed * targetRad;
        var deltaQuad = new Quaternion(
          0, 0, math.sin(deltaTargetRad / 2f), math.cos(deltaTargetRad / 2f)
        );
        directionBlock.transform.rotation *= deltaQuad;
        continue;
      }
      if (damageable.GetWhoLocked() == directionBlock) continue;

      damageable.SetWhoLocked(directionBlock); // locking target
      directionBlock.SetAmmunition(directionBlock.GetAmmunition() - 1);
      SpawnBulletAt(
        directionBlock.transform.position,
        2.5f * (colorBlock.transform.position - directionBlock.transform.position).normalized,
        1
      );
    }
  }
}