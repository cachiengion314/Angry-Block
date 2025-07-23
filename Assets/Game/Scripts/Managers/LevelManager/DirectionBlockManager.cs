using System;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

public partial class LevelManager : MonoBehaviour
{
  [Header("DirectionBlock Manager")]
  /// <summary>
  /// Manager all direction blocks
  /// </summary>
  GameObject[] _directionBlocks;
  readonly List<GameObject> _needMovingObjs = new();
  readonly Dictionary<int, int> _needMovingObjPathIndexes = new();
  public GameObject[] DirectionBlocks { get { return _directionBlocks; } }
  public Action OnDirectionBlockMove;
  [SerializeField][Range(1, 100)] float movingSpeed = 12.5f;

  public void SetDirectionBlocks(int index, GameObject directionBlock)
  {
    _directionBlocks[index] = directionBlock;
  }

  void TouchControlling(GameObject directionBlock)
  {
    if (directionBlock == null) return;

    if (!directionBlock.TryGetComponent(out IColorBlock color)) return;
    if (color.GetIndex() == -1) return;

    if (!directionBlock.TryGetComponent<IMoveable>(out var moveable)) return;
    if (!moveable.GetLockedPosition().Equals(0)) return;

    SoundManager.Instance.PlayClickBlockSfx();
    
    if (!IsBlockMoveable(directionBlock)) return;

    SoundManager.Instance.PlayBlockMoveSfx();

    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot == -1 || emptyWaitingSlot > _waitingSlots.Length - 1) return;

    _waitingSlots[emptyWaitingSlot] = directionBlock;
    _directionBlocks[color.GetIndex()] = null;

    SortingWaitSlotAndAddToMovesQueue();

    OnDirectionBlockMove?.Invoke();
    OnTriggerNeighborAt(directionBlock);
  }

  GameObject FindDirectionBlockIn(Collider2D[] cols)
  {
    var col = FindObjIn<IDirectionBlock>(cols);
    if (col == null) return null;
    return col.gameObject;
  }

  void AddToMoveQueue(
    int slotIndex,
    GameObject directionBlock,
    Transform positions
  )
  {
    if (!directionBlock.TryGetComponent<IMoveable>(out var moveable)) return;
    if (slotIndex > positions.childCount - 1 || slotIndex < 0)
    {
      print("Slots is not available");
      return;
    }
    if (!directionBlock.TryGetComponent(out IColorBlock color)) return;

    var startPos = bottomGrid.ConvertIndexToWorldPos(color.GetIndex());
    var endPos = positions.GetChild(slotIndex).position;
    if (color.GetIndex() == -1)
    {
      /// case: block is already standing at the waiting slot
      moveable.SetInitPostion(directionBlock.transform.position);
      moveable.SetLockedPosition(endPos);
      moveable.SetPath(null);
      _needMovingObjs.Add(directionBlock);
      return;
    }

    var path = CalculatePathFor(directionBlock, endPos);
    if (path == null)
    {
      /// case: block being blocking by others
      print("Cannot move duo to others blocking path!");
      return;
    }

    /// case: block move to the waiting slot
    moveable.SetInitPostion(startPos);
    moveable.SetLockedPosition(endPos);
    moveable.SetPath(path);
    _needMovingObjs.Add(directionBlock);
  }

  float3[] CalculatePathFor(GameObject directionBlock, float3 endPos)
  {
    if (!directionBlock.TryGetComponent(out IDirectionBlock direction)) return null;
    if (!directionBlock.TryGetComponent(out IColorBlock color)) return null;
    float3[] path = null;

    var startGrid = bottomGrid.ConvertIndexToGridPos(color.GetIndex());
    var startPos = bottomGrid.ConvertGridPosToWorldPos(startGrid);
    var blockDir = direction.GetDirection();
    var pos1 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(new int2(-1, -1));
    var pos2 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(new int2(-1, bottomGrid.GridSize.y));
    var pos3 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(bottomGrid.GridSize);
    var pos5 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(new int2(startGrid.x, bottomGrid.GridSize.y));
    var pos6 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(new int2(bottomGrid.GridSize.x, startGrid.y));
    var pos7 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(new int2(startGrid.x, -1));
    var pos8 = (Vector3)bottomGrid.ConvertGridPosToWorldPos(new int2(-1, startGrid.y));

    if (blockDir.Equals(new int2(0, 1))) path = new float3[3] { startPos, pos5, endPos };
    if (blockDir.Equals(new int2(1, 0))) path = new float3[4] { startPos, pos6, pos3, endPos };
    if (blockDir.Equals(new int2(0, -1))) path = new float3[5] { startPos, pos7, pos1, pos2, endPos };
    if (blockDir.Equals(new int2(-1, 0))) path = new float3[4] { startPos, pos8, pos2, endPos };
    return path;
  }

  bool IsBlockMoveable(GameObject directionBlock)
  {
    if (!directionBlock.TryGetComponent(out IDirectionBlock direction)) return false;
    if (!directionBlock.TryGetComponent(out IColorBlock color)) return false;
    var blockGrid = bottomGrid.ConvertIndexToGridPos(color.GetIndex());
    var dirBlock = direction.GetDirection();
    var nextBlock = blockGrid + dirBlock;
    while (!bottomGrid.IsGridPosOutsideAt(nextBlock))
    {
      var index = bottomGrid.ConvertGridPosToIndex(nextBlock);
      if (_directionBlocks[index] != null) return false;
      nextBlock += dirBlock;
    }
    return true;
  }

  void MovesToWaitingUpdate()
  {
    for (int i = _needMovingObjs.Count - 1; i >= 0; --i)
    {
      var obj = _needMovingObjs[i];

      if (obj == null) continue;
      if (obj.activeSelf == false) continue;
      if (!obj.TryGetComponent<IMoveable>(out var moveable)) continue;
      if (!obj.TryGetComponent<IColorBlock>(out var colorBlock)) continue;

      var currentPos = obj.transform.position;
      var startPos = moveable.GetInitPostion();
      var targetPos = moveable.GetLockedPosition();
      var path = moveable.GetPath();
      if (path == null)
      {
        HoangNam.Utility.InterpolateMoveUpdate(
          currentPos, startPos, targetPos, movingSpeed, out var percent, out var nextPosition
        );
        obj.transform.position = nextPosition;
        if (percent < 1) continue;

        colorBlock.SetIndex(-1);
        moveable.SetLockedPosition(0);
        moveable.SetLockedTarget(null);
        _needMovingObjs.Remove(obj);
        continue;
      }

      if (!_needMovingObjPathIndexes.ContainsKey(obj.GetInstanceID()))
        _needMovingObjPathIndexes.Add(obj.GetInstanceID(), 0);
      var currentIdx = _needMovingObjPathIndexes[obj.GetInstanceID()];

      InterpolatePathUpdate(
        currentPos, currentIdx, path, movingSpeed, out var t, out var nextPos, out var nextIdx
      );
      obj.transform.position = nextPos;
      _needMovingObjPathIndexes[obj.GetInstanceID()] = nextIdx;
      if (t < 1) continue;

      colorBlock.SetIndex(-1);
      moveable.SetLockedPosition(0);
      moveable.SetLockedTarget(null);
      moveable.SetPath(null);
      _needMovingObjPathIndexes[obj.GetInstanceID()] = 0;
      _needMovingObjs.Remove(obj);
    }
  }
}