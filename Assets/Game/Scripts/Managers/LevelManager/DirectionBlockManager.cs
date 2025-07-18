using System.Collections.Generic;
using System;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Header("DirectionBlock Manager")]
  /// <summary>
  /// Manager all direction blocks
  /// </summary>
  GameObject[] _directionBlocks;
  public GameObject[] DirectionBlocks { get { return _directionBlocks; } }
  public Action OnDirectionBlockMove;

  public void SetDirectionBlocks(int index, GameObject directionBlock)
  {
    _directionBlocks[index] = directionBlock;
  }

  void TouchControlling(GameObject directionBlock)
  {
    if (directionBlock == null) return;

    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot == -1 || emptyWaitingSlot > _waitingSlots.Length - 1) return;
    if (!IsBlockMove(directionBlock)) return;

    _waitingSlots[emptyWaitingSlot] = directionBlock;
    _directionBlocks[directionBlock.GetComponent<IColorBlock>().GetIndex()] = null;

    AutoSortingWaitingSlots();
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      var block = _waitingSlots[i];
      if (block == null) continue;
      MoveTo(i, block, _waitingSlots, waitingPositions);
    }

    OnDirectionBlockMove?.Invoke();
    OnTriggerNeighborAt(directionBlock);
  }

  GameObject FindDirectionBlockIn(Collider2D[] cols)
  {
    var colorBlock = FindObjIn<IDirectionBlock>(cols);
    if (colorBlock == null) return null;
    return colorBlock.GetGameObject();
  }

  List<GameObject> FindMoveableDirectionBlocks()
  {
    var list = new List<GameObject>();
    for (int x = 0; x < bottomGrid.GridSize.x; ++x)
    {
      for (int y = 0; y < bottomGrid.GridSize.y; ++y)
      {
        var gridPos = new int2(x, y);
        var currentIndex = bottomGrid.ConvertGridPosToIndex(gridPos);
        var dirBlock = _directionBlocks[currentIndex];
        if (dirBlock == null) continue;
        if (dirBlock.TryGetComponent<IDirectionBlock>(out var directionBlock)) continue;

        var nextGridPos = gridPos + directionBlock.GetDirection();
        var nextIdx = bottomGrid.ConvertGridPosToIndex(nextGridPos);
        var nextBlock = _directionBlocks[nextIdx];
        if (nextBlock != null) continue;

        list.Add(nextBlock);
      }
    }
    return list;
  }

  void MoveTo(
    int slotIndex,
    GameObject directionBlock,
    GameObject[] slots,
    Transform positions
  )
  {
    if (!directionBlock.TryGetComponent<IMoveable>(out var moveable)) return;
    if (slotIndex > positions.childCount - 1 || slotIndex < 0)
    {
      print("Slots is not available");
      return;
    }

    var endPos = positions.GetChild(slotIndex).position;
    moveable.SetLockedPosition(endPos);
    directionBlock.transform.DOMove(endPos, .5f)
      .OnComplete(() =>
      {
        moveable.SetLockedPosition(0);
      });
  }

  // Vector3[] PathMove(GameObject directionBlock)
  // {
  //   if (!directionBlock.TryGetComponent(out IDirectionBlock direction)) return null;
  //   if (!directionBlock.TryGetComponent(out IColorBlock color)) return null;
  //   var blockGrid = bottomGrid.ConvertIndexToGridPos(color.GetIndex());
  //   var blockDir = direction.GetDirection();
  //   var pos1 = (bottomGrid.GridSize - blockGrid) * blockDir + blockGrid;
  // }

  bool IsBlockMove(GameObject directionBlock)
  {
    if (!directionBlock.TryGetComponent(out IDirectionBlock direction)) return false;
    if (!directionBlock.TryGetComponent(out IColorBlock color)) return false;
    var blockGrid = bottomGrid.ConvertIndexToGridPos(color.GetIndex());
    var dirBlock = direction.GetDirection();
    var nextBlock = blockGrid + dirBlock;
    while (!bottomGrid.IsGridPosOutsideAt(nextBlock))
    {
      var index = bottomGrid.ConvertGridPosToIndex(nextBlock);
      if (_directionBlocks[index] != null) return false;
      nextBlock += dirBlock;
    }
    return true;
  }
}