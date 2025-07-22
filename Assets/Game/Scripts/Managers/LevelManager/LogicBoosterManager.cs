using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class LevelManager
{
  [SerializeField] Transform mergePositions;
  public Action OnTriggerBooster1Success;
  public void OnTriggerBooster1(GameObject directionBlock)
  {
    if (directionBlock == null) return;
    if (!directionBlock.TryGetComponent(out IColorBlock color)) return;
    if (color.GetIndex() == -1) return;

    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot == -1 || emptyWaitingSlot > _waitingSlots.Length - 1) return;

    OnTriggerBooster1Success?.Invoke();
    GameManager.Instance.Booster1--;

    _waitingSlots[emptyWaitingSlot] = directionBlock;
    _directionBlocks[color.GetIndex()] = null;

    AutoSortingWaitingSlotAndMoves();

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
      if (!directionBlock.TryGetComponent(out DirectionBlockControl component)) continue;
      listDirectionBlock.Add(directionBlock);
    }
    return listDirectionBlock.ToArray();
  }

  GameObject[] FindWoodenBlocksNotNullAt(GameObject[] directionBlocks)
  {
    List<GameObject> listWoodenBlock = new();
    foreach (var woodenBlock in directionBlocks)
    {
      if (woodenBlock == null) continue;
      if (!woodenBlock.TryGetComponent(out WoodenBlockControl component)) continue;
      listWoodenBlock.Add(woodenBlock);
    }
    return listWoodenBlock.ToArray();
  }

  GameObject[] FindIceBlocksNotNullAt(GameObject[] directionBlocks)
  {
    List<GameObject> listIceBlock = new();
    foreach (var iceBlock in directionBlocks)
    {
      if (iceBlock == null) continue;
      if (!iceBlock.TryGetComponent(out IceBlockControl component)) continue;
      listIceBlock.Add(iceBlock);
    }
    return listIceBlock.ToArray();
  }

  GameObject[] FindTunnelNotNullAt(GameObject[] directionBlocks)
  {
    List<GameObject> listTunnel = new();
    foreach (var tunnel in directionBlocks)
    {
      if (tunnel == null) continue;
      if (!tunnel.TryGetComponent(out TunnelControl component)) continue;
      listTunnel.Add(tunnel);
    }
    return listTunnel.ToArray();
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

  public void OntriggerBooster3()
  {
    int colorValue = GetRandomColor();
    if (colorValue != -1)
    {
      int count = 0;
      if (_mergeSlots.ContainsKey(colorValue)) count = _mergeSlots[colorValue].Count;

      var misAmount = 3 - count;
      var DirBlocks = FindDirectionBlockColorAt(misAmount, colorValue);
      foreach (GameObject dirBlock in DirBlocks)
      {
        OnTriggerNeighborAt(dirBlock);
        if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) return;
        colorBlock.SetIndex(-1);
        ShouldMergeBooster3(dirBlock);
      }
    }
  }

  void ShouldMergeBooster3(GameObject waitingBlock)
  {
    if (IsWaitingSlotsMMoving()) return;

    if (!waitingBlock.TryGetComponent<IColorBlock>(out var colorBlock)) return;
    if (!waitingBlock.TryGetComponent<IMergeable>(out var mergeable)) return;
    if (!_mergeSlots.ContainsKey(colorBlock.GetColorValue()))
      _mergeSlots.Add(colorBlock.GetColorValue(), new HashSet<GameObject>());
    _mergeSlots[colorBlock.GetColorValue()].Add(waitingBlock);
    if (_mergeSlots[colorBlock.GetColorValue()].Count < 3) return;

    var mergeableBlocks = _mergeSlots[colorBlock.GetColorValue()].ToArray();
    var totalAmmunition = 0;
    for (int i = 0; i < mergeableBlocks.Length; ++i)
      totalAmmunition += mergeableBlocks[i].GetComponent<DirectionBlockControl>().GetAmmunition();
    for (int i = 0; i < mergeableBlocks.Length; ++i)
    {
      var mergeableBlock = mergeableBlocks[i];
      var idx = FindSlotFor(mergeableBlock, _waitingSlots);
      if (idx != -1)
        _waitingSlots[idx] = null;
      Destroy(mergeableBlock);

      if (i == 1)
      {
        var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
        var blastPos = waitingPositions.GetChild(emptyWaitingSlot).position;
        var blast = SpawnBlastBlockAt(blastPos, spawnedParent);
        if (blast.TryGetComponent<IColorBlock>(out var blastColor))
        {
          blastColor.SetColorValue(
            mergeableBlock.GetComponent<IColorBlock>().GetColorValue()
          );
        }
        if (blast.TryGetComponent<IGun>(out var blastGun))
        {
          blastGun.SetAmmunition(totalAmmunition);
        }
        _waitingSlots[emptyWaitingSlot] = blast.gameObject;
      }
    }
    _mergeSlots.Remove(colorBlock.GetColorValue());
  }

  HashSet<GameObject> FindDirectionBlockColorAt(int misAmount, int colorValue)
  {
    HashSet<GameObject> directionBlocks = new();
    var directionBlockAvailables = FindDirectionBlocksNotNullAt(_directionBlocks);
    for (int i = 0; i < directionBlockAvailables.Length; i++)
    {
      var dirBlock = directionBlockAvailables[i];
      if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
      if (colorBlock.GetColorValue() == colorValue) directionBlocks.Add(dirBlock);
      if (directionBlocks.Count == misAmount) return directionBlocks;
    }

    var woodenBlockAvailables = FindWoodenBlocksNotNullAt(_directionBlocks);
    for (int i = 0; i < woodenBlockAvailables.Length; i++)
    {
      var woodenBlock = woodenBlockAvailables[i];
      if (!woodenBlock.TryGetComponent(out WoodenBlockControl component)) continue;
      var blockParent = component.blockParent;
      if (blockParent.childCount == 0) continue;
      for (int j = blockParent.childCount - 1; j >= 0; j--)
      {
        var dirBlock = blockParent.GetChild(j);
        if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
        if (colorBlock.GetColorValue() == colorValue) directionBlocks.Add(dirBlock.gameObject);

        dirBlock.gameObject.SetActive(true);
        dirBlock.SetParent(spawnedParent);
        Destroy(woodenBlock.gameObject);

        if (directionBlocks.Count == misAmount) return directionBlocks;
      }
    }

    var iceBlockAvailables = FindIceBlocksNotNullAt(_directionBlocks);
    for (int i = 0; i < iceBlockAvailables.Length; i++)
    {
      var iceBlock = iceBlockAvailables[i];
      if (!iceBlock.TryGetComponent(out IceBlockControl component)) continue;
      var blockParent = component.blockParent;
      if (blockParent.childCount == 0) continue;
      for (int j = blockParent.childCount - 1; j >= 0; j--)
      {
        var dirBlock = blockParent.GetChild(j);
        if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
        if (colorBlock.GetColorValue() == colorValue) directionBlocks.Add(dirBlock.gameObject);

        dirBlock.gameObject.SetActive(true);
        dirBlock.SetParent(spawnedParent);
        Destroy(iceBlock.gameObject);

        if (directionBlocks.Count == misAmount) return directionBlocks;
      }
    }

    var tunnelAvailables = FindTunnelNotNullAt(_directionBlocks);
    for (int i = 0; i < tunnelAvailables.Length; i++)
    {
      var tunnel = tunnelAvailables[i];
      if (!tunnel.TryGetComponent(out TunnelControl component)) continue;
      var blockParent = component.blockParent;
      if (blockParent.childCount == 0) continue;
      for (int j = blockParent.childCount - 1; j >= 0; j--)
      {
        var dirBlock = blockParent.GetChild(j);
        if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
        if (colorBlock.GetColorValue() == colorValue) directionBlocks.Add(dirBlock.gameObject);

        dirBlock.gameObject.SetActive(true);
        dirBlock.SetParent(spawnedParent);
        colorBlock.SetIndex(999);

        if (directionBlocks.Count == misAmount) return directionBlocks;
      }
    }
    return directionBlocks;
  }

  int GetRandomColor()
  {
    int colorValue = GetRandomColorInMergeSlots();
    if (colorValue == -1) colorValue = GetRandomColorInTable();
    if (colorValue == -1) colorValue = GetRandomColorInWoodenBlock();
    if (colorValue == -1) colorValue = GetRandomColorInIceBlock();
    if (colorValue == -1) colorValue = GetRandomColorInTunnel();
    return colorValue;
  }

  int GetRandomColorInMergeSlots()
  {
    if (_mergeSlots.Count == 0) return -1;
    var keys = _mergeSlots.Keys.ToList();
    int randomIndex = Random.Range(0, keys.Count);
    return keys[randomIndex];
  }

  int GetRandomColorInTable()
  {
    var directionBlockAvailables = FindDirectionBlocksNotNullAt(_directionBlocks);
    if (directionBlockAvailables.Length == 0) return -1;
    int index = Random.Range(0, directionBlockAvailables.Length);
    var directionBlock = directionBlockAvailables[index];
    if (!directionBlock.TryGetComponent(out IColorBlock colorBlock)) return -1;
    return colorBlock.GetColorValue();
  }

  int GetRandomColorInWoodenBlock()
  {
    var woodenBlockAvailables = FindWoodenBlocksNotNullAt(_directionBlocks);
    if (woodenBlockAvailables.Length == 0) return -1;
    int index = Random.Range(0, woodenBlockAvailables.Length);
    var woodenBlock = woodenBlockAvailables[index];
    if (!woodenBlock.TryGetComponent(out WoodenBlockControl component)) return -1;
    var blockParent = component.blockParent;
    if (blockParent.childCount == 0) return -1;
    int index1 = Random.Range(0, blockParent.childCount);
    if (!blockParent.GetChild(index1).TryGetComponent(out IColorBlock colorBlock)) return -1;
    return colorBlock.GetColorValue();
  }

  int GetRandomColorInIceBlock()
  {
    var iceBlockAvailables = FindIceBlocksNotNullAt(_directionBlocks);
    if (iceBlockAvailables.Length == 0) return -1;
    int index = Random.Range(0, iceBlockAvailables.Length);
    var iceBlock = iceBlockAvailables[index];
    if (!iceBlock.TryGetComponent(out IceBlockControl component)) return -1;
    var blockParent = component.blockParent;
    if (blockParent.childCount == 0) return -1;
    int index1 = Random.Range(0, blockParent.childCount);
    if (!blockParent.GetChild(index1).TryGetComponent(out IColorBlock colorBlock)) return -1;
    return colorBlock.GetColorValue();
  }

  int GetRandomColorInTunnel()
  {
    var tunnelAvailables = FindTunnelNotNullAt(_directionBlocks);
    if (tunnelAvailables.Length == 0) return -1;
    int index = Random.Range(0, tunnelAvailables.Length);
    var tunnel = tunnelAvailables[index];
    if (!tunnel.TryGetComponent(out TunnelControl component)) return -1;
    var blockParent = component.blockParent;
    if (blockParent.childCount == 0) return -1;
    int index1 = Random.Range(0, blockParent.childCount);
    if (!blockParent.GetChild(index1).TryGetComponent(out IColorBlock colorBlock)) return -1;
    return colorBlock.GetColorValue();
  }
}