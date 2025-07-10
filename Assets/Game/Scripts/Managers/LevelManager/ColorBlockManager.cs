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

  bool IsFirstLineMatchWith(in int colorValue, out List<ColorBlockControl> firstLineMatched)
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
}