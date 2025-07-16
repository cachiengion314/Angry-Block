using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Range(1, 10)]
  [SerializeField] int waitingSlotAmount = 7;
  float3[] _waitingPositions;
  GameObject[] _waitingSlots;
  readonly Dictionary<int, float> _waitingTimers = new();
  readonly Dictionary<int, HashSet<GameObject>> _mergeSlots = new();
  readonly float _DURATION_NOT_FOUND_MATCHED = 1.2f;
  readonly float _DURATION_FOUND_MATCHED = .7f;

  void InitWaitingPositions()
  {
    _waitingPositions = new float3[waitingSlotAmount];
    _waitingSlots = new GameObject[waitingSlotAmount];

    var y = bottomGrid.GridSize.y - 1 - 1;
    var startX = 1;
    for (int x = startX; x < startX + waitingSlotAmount; ++x)
    {
      if (x > bottomGrid.GridSize.x - 1) break;
      var pos = bottomGrid.ConvertGridPosToWorldPos(new int2(x, y));
      _waitingPositions[x - startX] = pos;
    }
  }

  void ShouldGoWaitingUpdate(GameObject tmpNotFoundMatchedDirBlock)
  {
    if (!_waitingTimers.ContainsKey(tmpNotFoundMatchedDirBlock.GetInstanceID()))
      _waitingTimers.Add(tmpNotFoundMatchedDirBlock.GetInstanceID(), 0f);
    _waitingTimers[tmpNotFoundMatchedDirBlock.GetInstanceID()] += Time.deltaTime;
    if (
      _waitingTimers[tmpNotFoundMatchedDirBlock.GetInstanceID()] < _DURATION_NOT_FOUND_MATCHED
    )
      return;

    _waitingTimers[tmpNotFoundMatchedDirBlock.GetInstanceID()] = 0f;
    // move to waiting slot
    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot < 0 || emptyWaitingSlot > _waitingSlots.Length - 1)
    {
      // gameover should be here
      return;
    }

    var firingIdx = FindSlotFor(tmpNotFoundMatchedDirBlock, _firingSlots);
    if (firingIdx < 0 || firingIdx > _firingSlots.Length - 1) return;

    _firingSlots[firingIdx] = null;
    _waitingSlots[emptyWaitingSlot] = tmpNotFoundMatchedDirBlock;

    var targetPos = _waitingPositions[emptyWaitingSlot];
    var targetIdx = bottomGrid.ConvertWorldPosToIndex(targetPos);
    _directionBlocks[tmpNotFoundMatchedDirBlock.GetComponent<IColorBlock>().GetIndex()] = null;
    _directionBlocks[targetIdx] = tmpNotFoundMatchedDirBlock;
    tmpNotFoundMatchedDirBlock.GetComponent<IColorBlock>().SetIndex(targetIdx);

    tmpNotFoundMatchedDirBlock.GetComponent<IMoveable>().SetLockedPosition(targetPos);
    tmpNotFoundMatchedDirBlock.transform.DOMove(targetPos, .5f)
      .OnComplete(() =>
      {
        tmpNotFoundMatchedDirBlock.GetComponent<IMoveable>().SetLockedPosition(0);
      });
  }

  void ShouldMergeUpdate(GameObject waitingBlock)
  {
    if (!waitingBlock.TryGetComponent<IColorBlock>(out var colorBlock)) return;
    if (!waitingBlock.TryGetComponent<IMergeable>(out var mergeable)) return;

    if (!_mergeSlots.ContainsKey(colorBlock.GetColorValue()))
      _mergeSlots.Add(colorBlock.GetColorValue(), new HashSet<GameObject>());
    _mergeSlots[colorBlock.GetColorValue()].Add(waitingBlock);
    if (_mergeSlots[colorBlock.GetColorValue()].Count == 3)
    {
      print("Should merge invoke");
    }
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
      if (_waitingTimers[waitingBlock.GetInstanceID()] < _DURATION_FOUND_MATCHED)
        return;

      _waitingTimers[waitingBlock.GetInstanceID()] = 0f;
      /// move to firing slot
      var emptyFiringSlot = FindEmptySlotFrom(_firingSlots);
      if (emptyFiringSlot < 0 || emptyFiringSlot > _firingSlots.Length - 1) return;

      var waitingIdx = FindSlotFor(waitingBlock, _waitingSlots);
      if (waitingIdx < 0 || waitingIdx > _waitingSlots.Length - 1) return;

      _waitingSlots[waitingIdx] = null;
      _firingSlots[emptyFiringSlot] = waitingBlock;

      var targetPos = _firingPositions[emptyFiringSlot];
      var targetIdx = bottomGrid.ConvertWorldPosToIndex(targetPos);
      _directionBlocks[waitingBlock.GetComponent<IColorBlock>().GetIndex()] = null;
      _directionBlocks[targetIdx] = waitingBlock;
      waitingBlock.GetComponent<IColorBlock>().SetIndex(targetIdx);

      waitingBlock.GetComponent<IMoveable>().SetLockedPosition(targetPos);
      waitingBlock.transform.DOMove(targetPos, .5f)
        .OnComplete(() =>
        {
          waitingBlock.GetComponent<IMoveable>().SetLockedPosition(0);
        });
    }
  }
}