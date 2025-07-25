using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [SerializeField] Transform waitingPositions;
  GameObject[] _waitingSlots;
  GameObject[] slotObjects;
  readonly Dictionary<int, HashSet<GameObject>> _mergeSlots = new();

  void InitWaitingSlots(int lockSlot)
  {
    int unlockSlot = waitingPositions.childCount - lockSlot;
    _waitingSlots = new GameObject[unlockSlot];
    slotObjects = new GameObject[waitingPositions.childCount];
    for (int i = 0; i < slotObjects.Length; i++)
    {
      var slot = SpawnSlotAt(waitingPositions.GetChild(i).position, topBlockParent);
      if (i < unlockSlot) slot.UnlockSlot();
      else slot.LockSlot();
      slotObjects[i] = slot.gameObject;
    }
  }

  bool IsLockSlot()
  {
    return _waitingSlots.Length != slotObjects.Length;
  }

  public void UnlockAllSlot()
  {
    for (int i = 0; i < slotObjects.Length; i++)
    {
      if (!slotObjects[i].TryGetComponent(out SlotControl component)) continue;
      if (component.isLock)
      {
        component.UnlockSlot();
        UnlockSlot();
      }
    }
  }

  void UnlockSlot()
  {
    int unlockSlot = _waitingSlots.Length;
    unlockSlot++;
    if (unlockSlot > waitingPositions.childCount)
      unlockSlot = waitingPositions.childCount;
    GameObject[] watingSlotTemp = (GameObject[])_waitingSlots.Clone();

    _waitingSlots = new GameObject[unlockSlot];
    for (int i = 0; i < watingSlotTemp.Length; i++)
      _waitingSlots[i] = watingSlotTemp[i];
  }

  bool IsUnlockSlot(GameObject slot)
  {
    for (int i = 0; i < slotObjects.Length; i++)
    {
      if (!slotObjects[i].TryGetComponent(out SlotControl component)) continue;
      if (component.isLock) return slot == slotObjects[i];
    }
    return false;
  }

  void OnUnlockSlot(GameObject slot)
  {
    if (slot == null) return;
    if (!IsUnlockSlot(slot))
    {
      GameplayPanel.Instance.ShowNotifyWith("UNLOCK IN ORDER");
      return;
    }
    if (GameManager.Instance.CurrentCoin < 100)
    {
      GameplayPanel.Instance.ShowNotifyWith("NOT ENOUGH COINS");
      return;
    }
    if (!slot.TryGetComponent(out SlotControl control)) return;
    GameManager.Instance.CurrentCoin -= 100;
    control.UnlockSlot();
    UnlockSlot();
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
    if (!waitingBlock.TryGetComponent<IMoveable>(out var moveable)) return;
    if (!moveable.GetLockedPosition().Equals(0)) return;
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
    var upperPos = mergeableBlocks[1].transform.position + Vector3.up * .0f;
    for (int i = 0; i < mergeableBlocks.Length; ++i)
    {
      var mergeableBlock = mergeableBlocks[i];
      var slot = FindSlotFor(mergeableBlock, _waitingSlots);
      if (slot == -1 || slot > _waitingSlots.Length - 1) continue;

      _waitingSlots[slot] = null;
      if (i == 1)
      {
        blast = SpawnBlastBlockAt(upperPos, topBlockParent).gameObject;
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
          OnMergedCollided(blast);

          var colorBlock = FindFirstBlockMatchedFor(waitingBlock);
          if (colorBlock != null) return;
          SortingWaitSlotAndAddToMovesQueue();
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

      ShouldMergeUpdate(waitingBlock);

      if (!waitingBlock.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!moveable.GetLockedPosition().Equals(0)) continue;
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

      var randDir = Vector3.right * UnityEngine.Random.Range(0, 5.0f);
      var targetPos = _firingPositions.GetChild(0).position + randDir;

      waitingBlock.GetComponent<IMoveable>().SetLockedPosition(targetPos);
      waitingBlock.transform.DOMove(targetPos, .28f)
        .OnComplete(() =>
        {
          if (waitingBlock.TryGetComponent<Collider2D>(out var col))
            col.enabled = false;
          waitingBlock.GetComponent<IMoveable>().SetLockedPosition(0);
          SortingWaitSlotAndAddToMovesQueue();
        });
    }
  }
}