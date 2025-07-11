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

  ColorBlockControl[] FindFirstLineColorBlocks()
  {
    var arr = new ColorBlockControl[topGrid.GridSize.x];
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
      arr[x] = _colorBlocks[idx];
    }
    return arr;
  }

  bool IsFirstRowMatchWith(in int colorValue, out List<ColorBlockControl> firstLineMatched)
  {
    var firstLine = FindFirstLineColorBlocks();
    firstLineMatched = new List<ColorBlockControl>();
    for (int i = 0; i < firstLine.Length; ++i)
    {
      var obj = firstLine[i];
      if (obj == null) continue;
      if (obj.GetColorValue() == colorValue)
        firstLineMatched.Add(obj);
    }

    return firstLineMatched.Count > 0;
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

  bool IsFirstRowFull(out int notFullX)
  {
    notFullX = -1;
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      if (IsColmunEmptyAt(x)) continue;
      var grid = new int2(x, 0);
      var index = topGrid.ConvertGridPosToIndex(grid);
      var obj = _colorBlocks[index];
      if (obj != null) continue;
      notFullX = x;
      return false;
    }
    return true;
  }
}