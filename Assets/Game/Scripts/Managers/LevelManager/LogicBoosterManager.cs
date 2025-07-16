using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public partial class LevelManager
{
    public bool IsTriggerBooster1 = false;
    public void OnTriggerBooster1(GameObject directionBlock)
    {
        if (directionBlock == null) return;
        var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
        if (emptyWaitingSlot == -1 || emptyWaitingSlot > _waitingSlots.Length - 1) return;

        IsTriggerBooster1 = false;
        GameManager.Instance.Booster1--;

        MoveToSlots(emptyWaitingSlot, directionBlock, _waitingSlots, _waitingPositions);
    }
    void MoveToSlots(
    int slotIndex,
    GameObject directionBlock,
    GameObject[] slots,
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
        if (!directionBlock.TryGetComponent(out IColorBlock colorBlock)) return;
        _directionBlocks[colorBlock.GetIndex()] = null;
        var index = bottomGrid.ConvertWorldPosToIndex(endPos);
        colorBlock.SetIndex(index);
        _directionBlocks[index] = directionBlock;
        slots[slotIndex] = directionBlock;

        moveable.SetLockedPosition(endPos);
        directionBlock.transform.DOMove(endPos, duration)
        .SetEase(Ease.Linear)
        .OnComplete(() => moveable.SetLockedPosition(0));
    }
}