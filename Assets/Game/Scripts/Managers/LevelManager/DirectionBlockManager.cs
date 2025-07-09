using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [SerializeField] int waitingSlotAMount = 7;
  float3[] _waitingPositions;
  DirectionBlockControl[] _waitingSlots;
  DirectionBlockControl[] _directionBlocks;
  public DirectionBlockControl[] DirectionBlocks { get { return _directionBlocks; } }

  void InitWaitingPositions()
  {
    _waitingPositions = new float3[waitingSlotAMount];
    _waitingSlots = new DirectionBlockControl[waitingSlotAMount];

    var y = bottomGrid.GridSize.y - 1;
    var startX = 0;
    for (int x = startX; x < startX + waitingSlotAMount; ++x)
    {
      if (x > bottomGrid.GridSize.x - 1) break;
      var pos = bottomGrid.ConvertGridPosToWorldPos(new int2(x, y));
      _waitingPositions[x] = pos;
    }
  }

  DirectionBlockControl FindDirectionBlockIn(Collider2D[] cols)
  {
    var colorBlock = FindObjIn<DirectionBlockControl>(cols);
    return colorBlock;
  }

  void SetWaitingSlotAt(int waitingIndex, DirectionBlockControl directionBlock)
  {
    if (waitingIndex > _waitingSlots.Length - 1) return;
    _waitingSlots[waitingIndex] = directionBlock;
  }

  float3 GetWaitingPosAt(int waitingIndex)
  {
    if (waitingIndex > _waitingPositions.Length - 1) return new float3(0, 0, 0);
    return _waitingPositions[waitingIndex];
  }

  int FindEmptyWaitingSlotIndex()
  {
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      if (_waitingSlots[i] == null) return i;
    }
    return -1;
  }

  void MoveToWaitingSlot(DirectionBlockControl directionBlock)
  {
    _directionBlocks[directionBlock.GetIndex()] = null;

    var emptyWaitingIndex = FindEmptyWaitingSlotIndex();
    var pos = GetWaitingPosAt(emptyWaitingIndex);
    var index = bottomGrid.ConvertWorldPosToIndex(pos);
    directionBlock.SetIndex(index);
    _directionBlocks[index] = directionBlock;

    directionBlock.transform.position = pos;

    SetWaitingSlotAt(emptyWaitingIndex, directionBlock);
  }

  void Controlling(DirectionBlockControl directionBlock)
  {
    if (directionBlock == null) return;

    MoveToWaitingSlot(directionBlock);
  }
}