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

  void TouchControlling(GameObject directionBlock)
  {
    if (directionBlock == null) return;

    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot == -1 || emptyWaitingSlot > _waitingSlots.Length - 1) return;

    MoveTo(emptyWaitingSlot, directionBlock, _waitingSlots, waitingPositions);
  }

  GameObject FindDirectionBlockIn(Collider2D[] cols)
  {
    var colorBlock = FindObjIn<IDirectionBlock>(cols);
    if (colorBlock == null) return null;
    return colorBlock.GetGameObject();
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
    var startGridPos = bottomGrid.ConvertWorldPosToGridPos(directionBlock.transform.position);
    var nextGridPos = startGridPos + directionBlock.GetComponent<IDirectionBlock>().GetDirection();
    var nextPos = bottomGrid.ConvertGridPosToWorldPos(nextGridPos);
    if (IsPosOccupiedAt(nextPos))
    {
      print("Cannot move due to occupied position");
      return;
    }
    // var moves = bottomGrid.PathFindingTo(endPos, nextPos, new int2[0]);
    // if (moves.Length == 0)
    // {
    //   print("Cannot move due to no available moves");
    //   moves.Dispose();
    //   return;
    // }
    // Trigger WoodenBlock
    HideHiddenWoodenBlockNeighborAt(directionBlock);

    // logic
    _directionBlocks[directionBlock.GetComponent<IColorBlock>().GetIndex()] = null;
    // var index = bottomGrid.ConvertWorldPosToIndex(endPos);
    // directionBlock.GetComponent<IColorBlock>().SetIndex(index);
    // _directionBlocks[index] = directionBlock;
    slots[slotIndex] = directionBlock;
    /// animation
    // var _moves = new Vector3[moves.Length];
    // for (int i = 0; i < moves.Length; ++i)
    // {
    //   var movePos = bottomGrid.ConvertGridPosToWorldPos(moves[i]);
    //   _moves[i] = movePos;
    // }

    moveable.SetLockedPosition(endPos);
    directionBlock.transform.DOMove(endPos, 1)
      .OnComplete(() =>
      {
        moveable.SetLockedPosition(0);
      });

    // moves.Dispose();
  }
}