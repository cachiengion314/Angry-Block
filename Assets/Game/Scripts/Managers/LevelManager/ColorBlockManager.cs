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

  ColorBlockControl FindFirstBlockMatchedFor(DirectionBlockControl directionBlock)
  {
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
      var obj = _colorBlocks[idx];
      if (obj == null) continue;
      if (!obj.TryGetComponent<IColorBlock>(out var colorBlock)) continue;
      if (!obj.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoPicked() != null && damageable.GetWhoPicked() != directionBlock)
        continue;
      if (damageable.GetWhoLocked() != null) continue; // the damageable block is waiting its dead when locked
      if (!directionBlock.TryGetComponent<IColorBlock>(out var dirColor)) continue;

      if (colorBlock.GetColorValue() == dirColor.GetColorValue()) return obj;
    }
    return null;
  }

  List<ColorBlockControl> FindFirstRowMatchedWith(in int colorValue)
  {
    var firstLineMatched = new List<ColorBlockControl>();
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
      var obj = _colorBlocks[idx];
      if (obj == null) continue;
      if (obj.GetColorValue() == colorValue)
        firstLineMatched.Add(obj);
    }
    return firstLineMatched;
  }

  bool IsColmunEmptyAt(int x)
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

  int FindNeedArrangeCollumn()
  {
    for (int y = 0; y < 1; ++y)
    {
      for (int x = 0; x < topGrid.GridSize.x; ++x)
      {
        if (IsColmunEmptyAt(x)) continue;
        var grid = new int2(x, y);
        var index = topGrid.ConvertGridPosToIndex(grid);
        var obj = _colorBlocks[index];
        if (obj != null) continue;

        return x;
      }
    }
    return -1;
  }

  void ReArrangeTopGridUpdate()
  {
    var needArrangeCollumn = FindNeedArrangeCollumn();
    if (needArrangeCollumn == -1)
    {
      return;
    }
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

      var t = InterpolateMoveUpdate(
        colorBlock.transform,
        topGrid.ConvertIndexToWorldPos(colorBlock.GetIndex()),
        targetPos
      );
      if (t < 1) continue;

      _colorBlocks[colorBlock.GetIndex()] = null;
      _colorBlocks[targetIndex] = colorBlock;
      colorBlock.SetIndex(targetIndex);
    }
  }
}