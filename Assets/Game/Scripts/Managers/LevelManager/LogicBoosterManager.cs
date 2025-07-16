using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public partial class LevelManager
{
    public bool IsTriggerBooster1 = false;
    public void OnTriggerBooster1(DirectionBlockControl directionBlock)
    {
        if (directionBlock == null) return;
        IsTriggerBooster1 = false;
        GameManager.Instance.Booster1--;

        var emptyWaitingSlotIndex = FindEmptySlotFrom(_waitingSlots);
        MoveToSlots(
          emptyWaitingSlotIndex, directionBlock, _waitingSlots, _waitingPositions
        );
    }
    void MoveToSlots(
    int slotIndex,
    DirectionBlockControl directionBlock,
    DirectionBlockControl[] slots,
    in float3[] positions
  )
    {
        if (!directionBlock.TryGetComponent<IMoveable>(out var moveable)) return;
        if (slotIndex > positions.Length - 1 || slotIndex < 0)
        {
            print("Slots is not available");
            return;
        }
        var duration = 0.3f;
        var endPos = positions[slotIndex];

        // logic
        _directionBlocks[directionBlock.GetIndex()] = null;
        var index = bottomGrid.ConvertWorldPosToIndex(endPos);
        directionBlock.SetIndex(index);
        _directionBlocks[index] = directionBlock;
        slots[slotIndex] = directionBlock;

        moveable.SetLockedPosition(endPos);
        directionBlock.transform.DOMove(endPos, duration)
        .SetEase(Ease.Linear)
        .OnComplete(()=> moveable.SetLockedPosition(0));
    }
}