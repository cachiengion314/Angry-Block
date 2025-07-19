using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Header("ColorBlock Manager")]
  /// <summary>
  /// Manager all color blocks
  /// </summary>
  ColorBlockControl[] _colorBlocks;
  public ColorBlockControl[] ColorBlocks { get { return _colorBlocks; } }
  [Range(1f, 10f)]
  [SerializeField] float arrangeSpeed = 5.5f;

  ColorBlockControl FindFirstBlockMatchedFor(GameObject blastBlock)
  {
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
      var obj = _colorBlocks[idx];
      if (obj == null) continue;
      if (!obj.TryGetComponent<IColorBlock>(out var colorBlock)) continue;
      if (!obj.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != blastBlock)
        continue;
      if (damageable.GetWhoLocked() != null) continue; // the damageable block is waiting its dead when locked
      if (!blastBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;

      if (colorBlock.GetColorValue() == dirColor.GetColorValue()) return obj;
    }
    return null;
  }

  bool IsAtVisibleBound(GameObject colorBlock)
  {
    if (colorBlock.transform.position.y < 13.5f) return true;
    return false;
  }

  bool IsCollmunEmptyAt(int x)
  {
    for (int y = 0; y < topGrid.GridSize.y; ++y)
    {
      var grid = new int2(x, y);
      var idx = topGrid.ConvertGridPosToIndex(grid);
      var obj = _colorBlocks[idx];
      if (obj != null) return false;
    }
    return true;
  }

  int FindNeedSpawningCollumn()
  {
    var y = topGrid.GridSize.y - 1;
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      if (IsCollmunEmptyAt(x)) continue;
      var grid = new int2(x, y);
      var index = topGrid.ConvertGridPosToIndex(grid);
      var obj = _colorBlocks[index];
      if (obj == null) return x;
    }
    return -1;
  }

  List<int> FindNeedArrangeCollumns()
  {
    var list = new List<int>();
    for (int y = 0; y < 1; ++y)
    {
      for (int x = 0; x < topGrid.GridSize.x; ++x)
      {
        if (IsCollmunEmptyAt(x)) continue;
        var grid = new int2(x, y);
        var index = topGrid.ConvertGridPosToIndex(grid);
        var obj = _colorBlocks[index];
        if (obj != null) continue;

        list.Add(x);
      }
    }
    return list;
  }

  int FindDelegateColorFrom(List<GameObject> moveableBlocks)
  {
    var obj = moveableBlocks[0];
    if (!obj.TryGetComponent<IColorBlock>(out var colorBlock)) return 0;
    return colorBlock.GetColorValue();
  }

  void SpawnColorBlocksUpdate()
  {
    var needSpawningCollumn = FindNeedSpawningCollumn();
    if (needSpawningCollumn == -1)
    {
      return;
    }

    var y = topGrid.GridSize.y - 1;
    var grid = new int2(needSpawningCollumn, y);
    var currentIndex = topGrid.ConvertGridPosToIndex(grid);

    var colorBlock = SpawnColorBlockAt(currentIndex, spawnedParent);
    colorBlock.GetComponent<IColorBlock>().SetIndex(currentIndex);

    var moveableBlocks = FindMoveableDirectionBlocks();
    if (moveableBlocks.Count > 0)
    {
      var delegateColor = FindDelegateColorFrom(moveableBlocks);
      colorBlock.GetComponent<IColorBlock>().SetColorValue(delegateColor);
    }
    else
      colorBlock.GetComponent<IColorBlock>().SetColorValue(0);

    _colorBlocks[currentIndex] = colorBlock;
  }

  void FindNeedArrangeCollumnAndUpdate()
  {
    var needArrangeCollumns = FindNeedArrangeCollumns();
    if (needArrangeCollumns.Count == 0)
    {
      return;
    }

    for (int x = 0; x < needArrangeCollumns.Count; ++x)
    {
      var needArrangeCollumn = needArrangeCollumns[x];
      for (int y = 0; y < topGrid.GridSize.y; ++y)
      {
        var grid = new int2(needArrangeCollumn, y);
        var currentIndex = topGrid.ConvertGridPosToIndex(grid);
        var colorBlock = _colorBlocks[currentIndex];
        if (colorBlock == null) continue;
        if (!colorBlock.TryGetComponent<IMoveable>(out var moveable)) continue;

        var downGrid = grid + new int2(0, -1);
        var targetIndex = topGrid.ConvertGridPosToIndex(downGrid);
        var targetPos = topGrid.ConvertIndexToWorldPos(targetIndex);
        if (topGrid.IsPosOutsideAt(targetPos)) continue;

        InterpolateMoveUpdate(
          colorBlock.transform.position,
          topGrid.ConvertIndexToWorldPos(colorBlock.GetIndex()),
          targetPos,
          updateSpeed * arrangeSpeed,
          out var t,
          out var nextPos
        );

        if (
          IsAtVisibleBound(colorBlock.gameObject)
          && colorBlock.gameObject.activeSelf == false
        )
          colorBlock.gameObject.SetActive(true);

        colorBlock.transform.position = nextPos;
        if (t < 1) continue;

        _colorBlocks[colorBlock.GetIndex()] = null;
        _colorBlocks[targetIndex] = colorBlock;
        colorBlock.SetIndex(targetIndex);
      }
    }
  }
}