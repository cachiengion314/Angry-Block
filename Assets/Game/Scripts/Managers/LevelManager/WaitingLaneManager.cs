using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [SerializeField] Transform waitingPositions;
  GameObject[] _waitingSlots;
  readonly Dictionary<int, float> _waitingTimers = new();
  readonly Dictionary<int, HashSet<GameObject>> _mergeSlots = new();
  readonly float KEY_FOUND_MATCHED_DURATION = .2f;

  void InitWaitingSlots()
  {
    _waitingSlots = new GameObject[waitingPositions.childCount];
  }

  void AutoSortingWaitingSlots()
  {
    var hashtable = new Dictionary<int, HashSet<GameObject>>();
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      var block = _waitingSlots[i];
      if (block == null) continue;
      if (!block.TryGetComponent<IColorBlock>(out var colorBlock)) continue;
      if (!hashtable.ContainsKey(colorBlock.GetColorValue()))
        hashtable.Add(colorBlock.GetColorValue(), new HashSet<GameObject>());
      hashtable[colorBlock.GetColorValue()].Add(block);
    }
    var hashArr = hashtable.ToArray();
    var list = new List<GameObject>();
    for (int i = 0; i < hashArr.Length; ++i)
    {
      var gameObjSet = hashArr[i].Value.ToArray();
      for (int j = 0; j < gameObjSet.Length; ++j)
      {
        list.Add(gameObjSet[j]);
      }
    }
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      _waitingSlots[i] = null;
      if (i > list.Count - 1) continue;
      _waitingSlots[i] = list[i];
    }
  }

  void AutoSortingWaitingSlotAndMoves()
  {
    AutoSortingWaitingSlots();
    _needMovingObjs.Clear();
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      var obj = _waitingSlots[i];
      if (obj == null) continue;
      AddToMoveQueue(i, obj, waitingPositions);
    }
  }

  bool IsWaitingSlotsMMoving()
  {
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      var block = _waitingSlots[i];
      if (block == null) continue;
      if (!block.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) return true;
    }
    return false;
  }

  void ShouldMergeUpdate(GameObject waitingBlock)
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
      var slot = FindSlotFor(mergeableBlock, _waitingSlots);
      if (slot == -1 || slot > _waitingSlots.Length - 1) continue;

      _waitingSlots[slot] = null;
      Destroy(mergeableBlock);

      if (i == 1)
      {
        var blastPos = waitingPositions.GetChild(slot).position;
        var blast = SpawnBlastBlockAt(blastPos, spawnedParent);
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
        _waitingSlots[slot] = blast.gameObject;
      }
    }

    AutoSortingWaitingSlotAndMoves();

    _mergeSlots.Remove(colorBlock.GetColorValue());
  }

  void WaitAndFindMatchedUpdate()
  {
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      if (_waitingSlots[i] == null) continue;
      var waitingBlock = _waitingSlots[i];
      if (!waitingBlock.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) continue;

      ShouldMergeUpdate(waitingBlock);

      if (!waitingBlock.TryGetComponent<IGun>(out var gun)) continue;
      if (gun.GetAmmunition() <= 0) continue;
      if (!waitingBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;
      var colorBlock = FindFirstBlockMatchedFor(waitingBlock);
      if (colorBlock == null) continue;

      if (!_waitingTimers.ContainsKey(waitingBlock.GetInstanceID()))
        _waitingTimers.Add(waitingBlock.GetInstanceID(), 0f);
      _waitingTimers[waitingBlock.GetInstanceID()] += Time.deltaTime;
      if (_waitingTimers[waitingBlock.GetInstanceID()] < KEY_FOUND_MATCHED_DURATION)
        continue;

      _waitingTimers[waitingBlock.GetInstanceID()] = 0f;
      /// move to firing slot
      var emptyFiringSlot = FindEmptySlotFrom(_firingSlots);

      var waitingIdx = FindSlotFor(waitingBlock, _waitingSlots);
      if (waitingIdx < 0 || waitingIdx > _waitingSlots.Length - 1) continue;

      _waitingSlots[waitingIdx] = null;
      if (emptyFiringSlot > _firingSlots.Count - 1)
        _firingSlots.Add(waitingBlock);
      else
        _firingSlots[emptyFiringSlot] = waitingBlock;

      var targetPos = _firingPositions.GetChild(0).position;

      waitingBlock.GetComponent<IMoveable>().SetLockedPosition(targetPos);
      waitingBlock.transform.DOMove(targetPos, .2f)
        .OnComplete(() =>
        {
          waitingBlock.GetComponent<IMoveable>().SetLockedPosition(0);
        });
    }
  }
}