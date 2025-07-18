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

    _waitingSlots[emptyWaitingSlot] = directionBlock;
    _directionBlocks[directionBlock.GetComponent<IColorBlock>().GetIndex()] = null;

    AutoSortingWaitingSlots();

    IsTriggerBooster1 = false;
    GameManager.Instance.Booster1--;

    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      var block = _waitingSlots[i];
      if (block == null) continue;
      MoveTo(i, block, _waitingSlots, waitingPositions);
    }

    OnDirectionBlockMove?.Invoke();
    OnTriggerNeighborAt(directionBlock);
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

  GameObject[] FindDirectionBlocksNotNullAt(GameObject[] directionBlocks)
  {
    List<GameObject> listDirectionBlock = new();
    foreach (var directionBlock in directionBlocks)
    {
      if (directionBlock == null) continue;
      if (!directionBlock.TryGetComponent(out DirectionBlockControl directionBlockControl1)) continue;
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