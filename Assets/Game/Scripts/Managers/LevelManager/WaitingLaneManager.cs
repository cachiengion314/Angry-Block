using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [SerializeField] Transform waitingPositions;
  GameObject[] _waitingSlots;
  readonly Dictionary<int, HashSet<GameObject>> _mergeSlots = new();

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

  void SortingWaitSlotAndAddToMovesQueue()
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

  bool IsWaitingSlotsMTweening()
  {
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      var block = _waitingSlots[i];
      if (block == null) continue;
      if (DOTween.IsTweening(block.transform)) return true;
    }
    return false;
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

  bool IsMergeBooster3SlotsMMoving()
  {
    for (int i = 0; i < _mergeSlotBooster3.Length; ++i)
    {
      var block = _mergeSlotBooster3[i];
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
      totalAmmunition
        += mergeableBlocks[i]
        .GetComponent<DirectionBlockControl>()
        .GetAmmunition();

    GameObject blast = null;
    var upperPos = mergeableBlocks[1].transform.position + Vector3.up * 2.0f;
    for (int i = 0; i < mergeableBlocks.Length; ++i)
    {
      var mergeableBlock = mergeableBlocks[i];
      var slot = FindSlotFor(mergeableBlock, _waitingSlots);
      if (slot == -1 || slot > _waitingSlots.Length - 1) continue;

      _waitingSlots[slot] = null;
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
        _waitingSlots[slot] = blast;
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

  void WaitAndFindMatchedUpdate()
  {
    for (int i = 0; i < _waitingSlots.Length; ++i)
    {
      if (_waitingSlots[i] == null) continue;
      var waitingBlock = _waitingSlots[i];
      if (waitingBlock.activeSelf == false) continue;
      if (!waitingBlock.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) continue;

      ShouldMergeUpdate(waitingBlock);

      if (!waitingBlock.TryGetComponent<IGun>(out var gun)) continue;
      if (gun.GetAmmunition() <= 0) continue;
      if (!waitingBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;
      var colorBlock = FindFirstBlockMatchedFor(waitingBlock);
      if (colorBlock == null) continue;

      /// move to firing slot
      var emptyFiringSlot = FindEmptySlotFrom(_firingSlots);

      var waitingIdx = FindSlotFor(waitingBlock, _waitingSlots);
      if (waitingIdx < 0 || waitingIdx > _waitingSlots.Length - 1) continue;

      _waitingSlots[waitingIdx] = null;
      if (emptyFiringSlot > _firingSlots.Count - 1)
        _firingSlots.Add(waitingBlock);

      var randDir = Vector3.right * UnityEngine.Random.Range(0, 2.4f);
      var targetPos = _firingPositions.GetChild(0).position + randDir;

      waitingBlock.GetComponent<IMoveable>().SetLockedPosition(targetPos);
      waitingBlock.transform.DOMove(targetPos, .2f)
        .OnComplete(() =>
        {
          waitingBlock.GetComponent<IMoveable>().SetLockedPosition(0);
          SortingWaitSlotAndAddToMovesQueue();
        });
    }
  }
}