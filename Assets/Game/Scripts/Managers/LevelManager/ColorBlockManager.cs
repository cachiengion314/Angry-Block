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

  ColorBlockControl FindFirstBlockMatchedWith(in int colorValue)
  {
    for (int x = 0; x < topGrid.GridSize.x; ++x)
    {
      var idx = topGrid.ConvertGridPosToIndex(new int2(x, 0));
      var obj = _colorBlocks[idx];
      if (obj == null) continue;
      if (!obj.TryGetComponent<IColorBlock>(out var colorBlock)) continue;
      if (!obj.TryGetComponent<IDamageable>(out var damageable)) continue;
      if (damageable.GetWhoLocked() != null) continue;
      if (colorBlock.GetColorValue() == colorValue) return obj;
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
}