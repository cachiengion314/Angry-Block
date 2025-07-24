using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class LevelManager
{
  [SerializeField] Transform mergePositions;
  [SerializeField] GameObject[] _mergeSlotBooster3 = new GameObject[3];
  public Action OnTriggerBooster1Success;
  public void OnTriggerBooster1(GameObject directionBlock)
  {
    if (directionBlock == null) return;
    if (!directionBlock.TryGetComponent(out IColorBlock color)) return;
    if (color.GetIndex() == -1) return;

    if (!directionBlock.TryGetComponent<IMoveable>(out var moveable)) return;
    if (!moveable.GetLockedPosition().Equals(0)) return;

    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot == -1 || emptyWaitingSlot > _waitingSlots.Length - 1) return;

    SoundManager.Instance.PlayClickBlockSfx();
    SoundManager.Instance.PlayBlockMoveSfx();
    OnTriggerBooster1Success?.Invoke();
    GameManager.Instance.Booster1--;

    _waitingSlots[emptyWaitingSlot] = directionBlock;
    _directionBlocks[color.GetIndex()] = null;

    SortingWaitSlotAndAddToMovesQueue();

    OnDirectionBlockMove?.Invoke();
    OnTriggerNeighborAt(directionBlock);
  }

  public void OnTriggerBooster2()
  {
    GameManager.Instance.Booster2--;
    var directionBlockAvailables = FindDirectionBlocksNotNullAt(_directionBlocks);
    for (int i = directionBlockAvailables.Length - 1; i >= 0; i--)
    {
      int j = UnityEngine.Random.Range(0, i + 1);
      SwapAt(directionBlockAvailables[i], directionBlockAvailables[j]);
    }
    VisualizeUseTriggerBooster2();
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
    if (IsMergeBooster3SlotsMMoving()) return;
    int colorValue = GetRandomColor();
    if (colorValue == -1) return;
    GameManager.Instance.Booster3--;
    var needBlocks = FindDirectionBlockColorAt(3, colorValue);
    foreach (var mergeableBlock in needBlocks)
    {
      if (mergeableBlock == null) continue;
      var emptyMergeSlot = FindEmptySlotFrom(_mergeSlotBooster3);
      if (emptyMergeSlot == -1) continue;
      _mergeSlotBooster3[emptyMergeSlot] = mergeableBlock;
      OnDirectionBlockMove?.Invoke();
      OnTriggerNeighborAt(mergeableBlock.gameObject);
      if (!mergeableBlock.TryGetComponent(out IColorBlock colorBlock)) return;
      colorBlock.SetIndex(-1);
      SoundManager.Instance.PlayBlockMoveSfx();
    }
    AutoSortingMergeSlotBooster3AndMoves();
  }

  void AutoSortingMergeSlotBooster3AndMoves()
  {
    _needMovingObjs.Clear();
    for (int i = 0; i < _mergeSlotBooster3.Length; ++i)
    {
      var obj = _mergeSlotBooster3[i];
      if (obj == null) continue;
      AddToMoveQueue(i, obj, mergePositions);
    }
  }

  void WaitAndFindMatchedBooter3Update()
  {
    if (IsMergeBooster3SlotsMMoving()) return;
    for (int i = 0; i < _mergeSlotBooster3.Length; ++i)
    {
      if (_mergeSlotBooster3[i] == null) continue;
      var mergeBlock = _mergeSlotBooster3[i];
      if (mergeBlock.activeSelf == false) continue;

      ShouldMergeBooster3Update(mergeBlock);

      if (!mergeBlock.TryGetComponent<IGun>(out var gun)) continue;
      if (gun.GetAmmunition() <= 0) continue;

      /// move to firing slot
      var emptyFiringSlot = FindEmptySlotFrom(_firingSlots);

      var mergeIdx = FindSlotFor(mergeBlock, _mergeSlotBooster3);
      if (mergeIdx < 0 || mergeIdx > _mergeSlotBooster3.Length - 1) continue;

      _mergeSlotBooster3[mergeIdx] = null;
      if (emptyFiringSlot > _firingSlots.Count - 1)
        _firingSlots.Add(mergeBlock);

      var randDir = Vector3.right * UnityEngine.Random.Range(0, 2.4f);
      var targetPos = _firingPositions.GetChild(0).position + randDir;

      mergeBlock.GetComponent<IMoveable>().SetLockedPosition(targetPos);
      mergeBlock.transform.DOMove(targetPos, .2f)
        .OnComplete(() =>
        {
          if (mergeBlock.TryGetComponent<Collider2D>(out var col))
            col.enabled = false;
          mergeBlock.GetComponent<IMoveable>().SetLockedPosition(0);
          SortingWaitSlotAndAddToMovesQueue();
        });
    }
  }

  void ShouldMergeBooster3Update(GameObject mergeBlock)
  {
    if (!mergeBlock.TryGetComponent<IColorBlock>(out var colorBlock)) return;
    if (!mergeBlock.TryGetComponent<IMergeable>(out var mergeable)) return;

    if (!_mergeSlots.ContainsKey(colorBlock.GetColorValue()))
      _mergeSlots.Add(colorBlock.GetColorValue(), new HashSet<GameObject>());
    _mergeSlots[colorBlock.GetColorValue()].Add(mergeBlock);
    if (_mergeSlots[colorBlock.GetColorValue()].Count < 3) return;

    var mergeableBlocks = _mergeSlots[colorBlock.GetColorValue()].ToArray();
    var totalAmmunition = 0;
    for (int i = 0; i < mergeableBlocks.Length; ++i)
      totalAmmunition
        += mergeableBlocks[i]
        .GetComponent<DirectionBlockControl>()
        .GetAmmunition();

    GameObject blast = null;
    var upperPos = mergeableBlocks[1].transform.position + Vector3.up * 2.0f;
    for (int i = 0; i < mergeableBlocks.Length; ++i)
    {
      var mergeableBlock = mergeableBlocks[i];
      var slot = FindSlotFor(mergeableBlock, _mergeSlotBooster3);
      if (slot == -1 || slot > _mergeSlotBooster3.Length - 1) continue;

      _mergeSlotBooster3[slot] = null;
      if (i == 1)
      {
        blast = SpawnBlastBlockAt(upperPos, spawnedParent).gameObject;
        if (blast.TryGetComponent<IColorBlock>(out var blastColor))
        {
          blastColor.SetIndex(-1);
          blastColor.SetColorValue(
            mergeableBlock.GetComponent<IColorBlock>().GetColorValue()
          );
        }
        if (blast.TryGetComponent<IGun>(out var blastGun))
        {
          blastGun.SetAmmunition(totalAmmunition);
        }
        _mergeSlotBooster3[slot] = blast;
        blast.SetActive(false);
      }
    }

    var seq = DOTween.Sequence();
    var startDuration = .0f;

    var mergeDuration = .25f;
    for (int i = 0; i < mergeableBlocks.Length; ++i)
    {
      var mergeableBlock = mergeableBlocks[i];

      var localStartPos = mergeableBlock.transform.position;
      var localUpperPos = mergeableBlock.transform.position + Vector3.up * 2.0f;
      var path = new Vector3[] { localStartPos, localUpperPos, upperPos };
      seq.Insert(
        startDuration,
        mergeableBlock.transform
          .DOPath(path, mergeDuration)
          .OnComplete(() =>
          {
            Destroy(mergeableBlock);
          })
        );
    }
    startDuration += mergeDuration;

    if (blast != null)
    {
      seq.InsertCallback(
        startDuration,
        () =>
        {
          blast.SetActive(true);
          SortingWaitSlotAndAddToMovesQueue();
          OnMergedCollided(blast);
          SoundManager.Instance.PlayMergeBlockSfx();
        }
      );
    }

    _mergeSlots.Remove(colorBlock.GetColorValue());
  }

  HashSet<GameObject> FindDirectionBlockColorAt(int misAmount, int colorValue)
  {
    HashSet<GameObject> directionBlocks = new();

    for (int i = 0; i < _waitingSlots.Length; i++)
    {
      var waittingBlock = _waitingSlots[i];
      if (waittingBlock == null) continue;
      if (!waittingBlock.TryGetComponent(out DirectionBlockControl component)) continue;
      if (!waittingBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
      if (colorBlock.GetColorValue() != colorValue) continue;
      directionBlocks.Add(waittingBlock);
      _waitingSlots[i] = null;
      if (directionBlocks.Count == misAmount) return directionBlocks;
    }

    var directionBlockAvailables = FindDirectionBlocksNotNullAt(_directionBlocks);
    for (int i = 0; i < directionBlockAvailables.Length; i++)
    {
      var dirBlock = directionBlockAvailables[i];
      if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
      if (colorBlock.GetColorValue() != colorValue) continue;
      directionBlocks.Add(dirBlock);
      _directionBlocks[colorBlock.GetIndex()] = null;
      if (directionBlocks.Count == misAmount) return directionBlocks;
    }

    var woodenBlockAvailables = FindWoodenBlocksNotNullAt(_directionBlocks);
    for (int i = 0; i < woodenBlockAvailables.Length; i++)
    {
      var woodenBlock = woodenBlockAvailables[i];
      if (!woodenBlock.TryGetComponent(out WoodenBlockControl component)) continue;
      var blockParent = component.blockParent;
      if (blockParent.childCount == 0) continue;

      var dirBlock = blockParent.GetChild(0);
      if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
      if (colorBlock.GetColorValue() != colorValue) continue;
      directionBlocks.Add(dirBlock.gameObject);
      component.RemoveBlock();
      if (directionBlocks.Count == misAmount) return directionBlocks;
    }

    var iceBlockAvailables = FindIceBlocksNotNullAt(_directionBlocks);
    for (int i = 0; i < iceBlockAvailables.Length; i++)
    {
      var iceBlock = iceBlockAvailables[i];
      if (!iceBlock.TryGetComponent(out IceBlockControl component)) continue;
      var blockParent = component.blockParent;
      if (blockParent.childCount == 0) continue;

      var dirBlock = blockParent.GetChild(0);
      if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) continue;
      if (colorBlock.GetColorValue() != colorValue) continue;
      directionBlocks.Add(dirBlock.gameObject);
      component.RemoveBlock();
      if (directionBlocks.Count == misAmount) return directionBlocks;
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
        if (colorBlock.GetColorValue() != colorValue) continue;
        directionBlocks.Add(dirBlock.gameObject);
        component.RemoveBlockAt(j);
        colorBlock.SetIndex(999);
        if (directionBlocks.Count == misAmount) return directionBlocks;
      }
    }
    return directionBlocks;
  }

  int GetRandomColor()
  {
    var availableColor = FindColorMatchedFor();
    int colorValue = GetRandomColorInMergeSlots(availableColor);
    if (colorValue == -1) colorValue = GetRandomColorInTable(availableColor);
    if (colorValue == -1) colorValue = GetRandomColorInWoodenBlock(availableColor);
    if (colorValue == -1) colorValue = GetRandomColorInIceBlock(availableColor);
    if (colorValue == -1) colorValue = GetRandomColorInTunnel(availableColor);
    if (colorValue == -1) colorValue = GetRandomColorInTable();
    if (colorValue == -1) colorValue = GetRandomColorInWoodenBlock();
    if (colorValue == -1) colorValue = GetRandomColorInIceBlock();
    if (colorValue == -1) colorValue = GetRandomColorInTunnel();
    return colorValue;
  }

  int GetRandomColorInMergeSlots(int[] availableColor)
  {
    List<GameObject> waitingSlots = new();
    for (int i = 0; i < _waitingSlots.Length; i++)
    {
      var waitingSlot = _waitingSlots[i];
      if (waitingSlot == null) continue;
      if (!waitingSlot.TryGetComponent(out DirectionBlockControl component)) continue;
      waitingSlots.Add(_waitingSlots[i]);
    }
    if (waitingSlots.Count == 0) return -1;

    for (int i = 0; i < availableColor.Length; i++)
    {
      var colorValue = availableColor[i];
      for (int j = 0; j < waitingSlots.Count; j++)
      {
        var waitingSlot = waitingSlots[j];
        if (waitingSlot == null) continue;
        if (!waitingSlot.TryGetComponent(out IColorBlock component)) continue;
        if (colorValue == component.GetColorValue()) return colorValue;
      }
    }

    int randomIndex = Random.Range(0, waitingSlots.Count);
    if (!waitingSlots[randomIndex].TryGetComponent(out IColorBlock colorBlock)) return -1;
    return colorBlock.GetColorValue();
  }

  int GetRandomColorInTable(int[] availableColor)
  {
    var directionBlockAvailables = FindDirectionBlocksNotNullAt(_directionBlocks);
    if (directionBlockAvailables.Length == 0) return -1;

    for (int i = 0; i < availableColor.Length; i++)
    {
      var colorValue = availableColor[i];
      for (int j = 0; j < directionBlockAvailables.Length; j++)
      {
        var dirBlock = directionBlockAvailables[j];
        if (dirBlock == null) continue;
        if (!dirBlock.TryGetComponent(out IColorBlock component)) continue;
        if (colorValue == component.GetColorValue()) return colorValue;
      }
    }

    return -1;
  }

  int GetRandomColorInWoodenBlock(int[] availableColor)
  {
    var woodenBlockAvailables = FindWoodenBlocksNotNullAt(_directionBlocks);
    if (woodenBlockAvailables.Length == 0) return -1;

    for (int i = 0; i < availableColor.Length; i++)
    {
      var colorValue = availableColor[i];
      for (int j = 0; j < woodenBlockAvailables.Length; j++)
      {
        var woodenBlock = woodenBlockAvailables[j];
        if (woodenBlock == null) continue;

        if (!woodenBlock.TryGetComponent(out WoodenBlockControl component)) continue;
        var blockParent = component.blockParent;
        if (blockParent.childCount == 0) continue;
        for (int k = 0; k < blockParent.childCount; k++)
        {
          if (!blockParent.GetChild(k).TryGetComponent(out IColorBlock colorBlock)) continue;
          if (colorValue == colorBlock.GetColorValue()) return colorValue;
        }
      }
    }
    return -1;
  }

  int GetRandomColorInIceBlock(int[] availableColor)
  {
    var iceBlockAvailables = FindIceBlocksNotNullAt(_directionBlocks);
    if (iceBlockAvailables.Length == 0) return -1;

    for (int i = 0; i < availableColor.Length; i++)
    {
      var colorValue = availableColor[i];
      for (int j = 0; j < iceBlockAvailables.Length; j++)
      {
        var iceBlock = iceBlockAvailables[j];
        if (iceBlock == null) continue;

        if (!iceBlock.TryGetComponent(out IceBlockControl component)) continue;
        var blockParent = component.blockParent;
        if (blockParent.childCount == 0) continue;
        for (int k = 0; k < blockParent.childCount; k++)
        {
          if (!blockParent.GetChild(k).TryGetComponent(out IColorBlock colorBlock)) continue;
          if (colorValue == colorBlock.GetColorValue()) return colorValue;
        }
      }
    }
    return -1;
  }

  int GetRandomColorInTunnel(int[] availableColor)
  {
    var tunnelAvailables = FindTunnelNotNullAt(_directionBlocks);
    if (tunnelAvailables.Length == 0) return -1;

    for (int i = 0; i < availableColor.Length; i++)
    {
      var colorValue = availableColor[i];
      for (int j = 0; j < tunnelAvailables.Length; j++)
      {
        var tunnel = tunnelAvailables[j];
        if (tunnel == null) continue;

        if (!tunnel.TryGetComponent(out TunnelControl component)) continue;
        var blockParent = component.blockParent;
        if (blockParent.childCount == 0) continue;
        for (int k = 0; k < blockParent.childCount; k++)
        {
          if (!blockParent.GetChild(k).TryGetComponent(out IColorBlock colorBlock)) continue;
          if (colorValue == colorBlock.GetColorValue()) return colorValue;
        }
      }
    }
    return -1;
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