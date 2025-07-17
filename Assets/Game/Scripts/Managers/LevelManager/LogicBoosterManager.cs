using System.Collections.Generic;
using System.Linq;
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

    MoveToSlots(emptyWaitingSlot, directionBlock, _waitingSlots, waitingPositions);
  }

  void MoveToSlots(
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
    // Trigger WoodenBlock
    OnTriggerNeighborAt(directionBlock);

    var duration = 0.3f;
    var endPos = positions.GetChild(slotIndex).position;

    // logic
    if (!directionBlock.TryGetComponent(out IColorBlock colorBlock)) return;
    _directionBlocks[colorBlock.GetIndex()] = null;
    // var index = bottomGrid.ConvertWorldPosToIndex(endPos);
    // colorBlock.SetIndex(index);
    // _directionBlocks[index] = directionBlock;
    slots[slotIndex] = directionBlock;

    moveable.SetLockedPosition(endPos);
    directionBlock.transform.DOMove(endPos, duration)
    .SetEase(Ease.Linear)
    .OnComplete(() => moveable.SetLockedPosition(0));
  }

  public void OnTriggerBooster2()
  {
    var directionBlockAvailables = FindDirectionBlocksNotNullAt(_directionBlocks);
    for (int i = directionBlockAvailables.Length - 1; i >= 0; i--)
    {
      int j = UnityEngine.Random.Range(0, i + 1);
      SwapAt(directionBlockAvailables[i], directionBlockAvailables[j]);
    }
  }

  GameObject[] DirectionBlocksAvailable()
  {
    var setDirectionBlocks = new HashSet<GameObject>(FindDirectionBlocksNotNullAt(_directionBlocks));
    var setWaitingSlots = new HashSet<GameObject>(FindDirectionBlocksNotNullAt(_waitingSlots));
    var setFiringSlots = new HashSet<GameObject>(FindDirectionBlocksNotNullAt(_firingSlots));

    setDirectionBlocks.ExceptWith(setWaitingSlots);
    setDirectionBlocks.ExceptWith(setFiringSlots);

    return setDirectionBlocks.ToArray();
  }

  GameObject[] FindDirectionBlocksNotNullAt(List<GameObject> directionBlocks)
  {
    List<GameObject> listDirectionBlock = new();
    foreach (var directionBlock in directionBlocks)
    {
      if (directionBlock == null) continue;
      listDirectionBlock.Add(directionBlock);
    }
    return listDirectionBlock.ToArray();
  }

  GameObject[] FindDirectionBlocksNotNullAt(GameObject[] directionBlocks)
  {
    List<GameObject> listDirectionBlock = new();
    foreach (var directionBlock in directionBlocks)
    {
      if (directionBlock == null) continue;
      listDirectionBlock.Add(directionBlock);
    }
    return listDirectionBlock.ToArray();
  }

  void SwapAt(GameObject directionBlocks1, GameObject directionBlocks2)
  {
    if (!directionBlocks1.TryGetComponent(out DirectionBlockControl directionBlockControl1)) return;
    if (!directionBlocks2.TryGetComponent(out DirectionBlockControl directionBlockControl2)) return;

    var colorTemp = directionBlockControl1.GetColorValue();
    var ammunitionTemp = directionBlockControl1.GetAmmunition();

    directionBlockControl1.SetColorValue(directionBlockControl2.GetColorValue());
    directionBlockControl1.SetAmmunition(directionBlockControl2.GetAmmunition());

    directionBlockControl2.SetColorValue(colorTemp);
    directionBlockControl2.SetAmmunition(ammunitionTemp);
  }
}