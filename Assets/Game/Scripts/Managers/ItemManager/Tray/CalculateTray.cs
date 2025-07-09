using System.Collections.Generic;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Calculate Tray
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  /// <summary>
  /// By the way needMoveCupDesPositions is a bunch of positions that have include offset already
  /// </summary>
  public void CalculateTransferCupsBetween(
      List<TrayControl> linkedTrays,
      out List<CupControl> needMoveCups,
      out List<float3> needMoveCupDesPositions
  )
  {
    needMoveCups = new();
    needMoveCupDesPositions = new();
    if (linkedTrays.Count <= 1) return;

    List<List<CupControl>> trayCupList = new();
    List<List<float3>> trayCupPosList = new();
    foreach (var tray in linkedTrays)
    {
      var cups = tray.FindCups();
      trayCupList.Add(cups);
      trayCupPosList.Add(tray.FindCupPositions());
    }

    List<CupControl> originalPlacedCups = new();
    for (int i = 0; i < trayCupList[^1].Count; ++i)
      originalPlacedCups.Add(trayCupList[^1][i]);

    int MAX_CUPS_CAPACITY = trayCupPosList[^1].Count;

    // Exchange batch of cups between trays
    List<CupControl> placedCups = trayCupList[^1];
    for (int i = 0; i < trayCupList.Count - 1; ++i)
    {
      List<CupControl> neighborCups = trayCupList[i];
      var matchedColor = ColorIndex.None;
      foreach (var cup in neighborCups)
      {
        var color = cup.ColorIndex;
        var matchedColorIdx = placedCups.FindIndex((elt) =>
        {
          return elt.ColorIndex == color;
        });
        if (matchedColorIdx == -1) continue;
        matchedColor = placedCups[matchedColorIdx].ColorIndex;
        break;
      }
      if (matchedColor == ColorIndex.None) continue;

      List<CupControl> needMoveCupsOfPlaced = new();
      foreach (var cup in placedCups)
        if (cup.ColorIndex == matchedColor) needMoveCupsOfPlaced.Add(cup);

      List<CupControl> needMoveCupsOfNeighbor = new();
      foreach (var cup in neighborCups)
        if (cup.ColorIndex != matchedColor) needMoveCupsOfNeighbor.Add(cup);

      for (int j = 0; j < needMoveCupsOfPlaced.Count; ++j)
      {
        neighborCups.Add(needMoveCupsOfPlaced[j]);
      }
      for (int j = 0; j < needMoveCupsOfNeighbor.Count; ++j)
      {
        placedCups.Add(needMoveCupsOfNeighbor[j]);
      }

      // Removing 
      List<CupControl> _neighborCups = new();
      foreach (var cup in neighborCups)
      {
        if (needMoveCupsOfNeighbor.Find(elt => elt.UniqueIndex == cup.UniqueIndex)) continue;
        _neighborCups.Add(cup);
      }
      neighborCups = _neighborCups;

      List<CupControl> _placedCups = new();
      foreach (var cup in placedCups)
      {
        if (needMoveCupsOfPlaced.Find(elt => elt.UniqueIndex == cup.UniqueIndex)) continue;
        _placedCups.Add(cup);
      }
      placedCups = _placedCups;

      if (placedCups.Count > MAX_CUPS_CAPACITY)
      {
        List<CupControl> needRemoveFromPlaced = new();
        for (int k = MAX_CUPS_CAPACITY; k < placedCups.Count; ++k)
        {
          needRemoveFromPlaced.Add(placedCups[k]);
        }
        placedCups.RemoveRange(MAX_CUPS_CAPACITY, needRemoveFromPlaced.Count);
        foreach (var cup in needRemoveFromPlaced)
          neighborCups.Add(cup);
      }
      else if (neighborCups.Count > MAX_CUPS_CAPACITY)
      {
        List<CupControl> needRemoveFromNeighbor = new();
        for (int k = MAX_CUPS_CAPACITY; k < neighborCups.Count; ++k)
        {
          needRemoveFromNeighbor.Add(neighborCups[k]);
        }
        neighborCups.RemoveRange(MAX_CUPS_CAPACITY, needRemoveFromNeighbor.Count);
        foreach (var cup in needRemoveFromNeighbor)
          placedCups.Add(cup);
      }

      trayCupList[i] = neighborCups;
    }
    trayCupList[^1] = placedCups;

    // Take all matched colors from other-neighbor trays
    for (int i = 0; i < trayCupList.Count; ++i)
    {
      List<CupControl> currentCups = trayCupList[i];

      if (currentCups.Count == 0) continue;
      if (currentCups.Count == MAX_CUPS_CAPACITY) continue;

      var oneColor = currentCups[0].ColorIndex;
      var isContainOnlyOneColor = true;
      for (int j = 1; j < currentCups.Count; ++j)
      {
        if (oneColor != currentCups[j].ColorIndex)
        {
          isContainOnlyOneColor = false;
          break;
        }
      }
      if (!isContainOnlyOneColor) continue;
      var foundMatchedColor1 = originalPlacedCups.Find(elt => elt.ColorIndex == oneColor);
      var foundMatchedColor2 = placedCups.Find(elt => elt.ColorIndex == oneColor);
      if (!foundMatchedColor1 && !foundMatchedColor2) continue;

      for (int j = 0; j < trayCupList.Count; ++j)
      {
        if (j == i) continue;
        List<CupControl> neighborCups = trayCupList[j];
        if (neighborCups.Count == 0) continue;
        if (neighborCups.Count == MAX_CUPS_CAPACITY) continue;

        List<CupControl> needMoveCupsOfNeighbor = new();
        foreach (var cup in neighborCups)
          if (cup.ColorIndex == oneColor) needMoveCupsOfNeighbor.Add(cup);
        for (int k = 0; k < needMoveCupsOfNeighbor.Count; ++k)
        {
          currentCups.Add(needMoveCupsOfNeighbor[k]);
        }

        // Removing
        List<CupControl> _neighborCups = new();
        foreach (var cup in neighborCups)
        {
          if (needMoveCupsOfNeighbor.Find(elt => elt.UniqueIndex == cup.UniqueIndex)) continue;
          _neighborCups.Add(cup);
        }
        neighborCups = _neighborCups;

        if (currentCups.Count > MAX_CUPS_CAPACITY)
        {
          List<CupControl> needRemoveFromCurrent = new();
          for (int k = MAX_CUPS_CAPACITY; k < currentCups.Count; ++k)
          {
            needRemoveFromCurrent.Add(currentCups[k]);
          }
          currentCups.RemoveRange(MAX_CUPS_CAPACITY, needRemoveFromCurrent.Count);
          foreach (var cup in needRemoveFromCurrent)
            neighborCups.Add(cup);
        }

        trayCupList[j] = neighborCups;
      }

      trayCupList[i] = currentCups;
    }

    // Conclusion task
    for (int i = 0; i < trayCupList.Count; ++i)
    {
      var trayCups = trayCupList[i];
      var trayPosCups = trayCupPosList[i];
      for (int j = 0; j < trayCups.Count; ++j)
      {
        CupControl cup = trayCups[j];
        float3 desPosition = trayPosCups[j] + cup.Offset;

        needMoveCups.Add(cup);
        needMoveCupDesPositions.Add(desPosition);
      }
    }
  }

  void CalculateSwapPositionFor(TrayControl[] trays, out List<TrayControl> needMoveTrays, out List<float3> destinations)
  {
    needMoveTrays = new List<TrayControl>();
    destinations = new List<float3>();

    if (trays[0] != null && trays[1] != null)
    {
      var tray1_currPos = trays[0].transform.position;
      var tray2_currPos = trays[1].transform.position;

      var index1 = trayGrid.ConvertWorldPosToIndex(trays[0].CurrentWorldPos);
      var index2 = trayGrid.ConvertWorldPosToIndex(trays[1].CurrentWorldPos);

      trays[0].CurrentWorldPos = tray2_currPos;
      trays[1].CurrentWorldPos = tray1_currPos;

      needMoveTrays.Add(trays[0]);
      needMoveTrays.Add(trays[1]);
      destinations.Add(tray2_currPos);
      destinations.Add(tray1_currPos);
    }
  }
}
