using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Check Tray
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  public bool IsMatchColorBetween(TrayControl tray1, TrayControl tray2)
  {
    List<CupControl> _cupTrays1 = tray1.FindCups();
    List<CupControl> _cupTrays2 = tray2.FindCups();
    foreach (CupControl cup in _cupTrays2)
    {
      var foundShareColorCup = _cupTrays1.Find(elt => elt.ColorIndex == cup.ColorIndex);
      if (foundShareColorCup)
      {
        return true;
      }
    }
    return false;
  }

  public bool IsPlacedTraysFull()
  {
    for (int i = 0; i < TrayGrid.Grid.Length; i++)
    {
      var valueTray = TrayGrid.Grid[i];
      var valueGiftBox = _giftBoxesGrid[i];
      var valueCoffeeBoard = _coffeeBoardesGrid[i];
      var valuePlantPot = _plantPotesGrid[i];
      var valueMoneyBag = _moneyBagsGrid[i];
      var valueMachineCream = _machineCreamsGrid[i];
      var valueMagicNest = _magicNestsGrid[i];
      var valueFlowerPot = _flowerPotsGrid[i];

      var valueWoodBox = 0;
      var valueGriller = 0;
      var valueCurtainLayer = 0;
      var valueCupBoard = 0;
      var valueBeverageFridge = 0;

      if (valueTray == 1000) valueTray = 0;
      if (woodBoxesGrid != null) { valueWoodBox = woodBoxesGrid[i]; }
      if (grillerGrid != null) valueGriller = grillerGrid[i];
      if (curtainLayersGrid != null) valueCurtainLayer = curtainLayersGrid[i];
      if (_cupBoardsGrid != null) valueCupBoard = _cupBoardsGrid[i];
      if (_beverageFridgesGrid != null) valueBeverageFridge = _beverageFridgesGrid[i];

      if (HasOnlyPairTrays())
      {
        var valueUpGiftBox = FindUpGiftBoxValueAt(i);
        var valueUpCoffeeBoard = FindUpCoffeeBoardValueAt(i);
        var valueUpPlantPot = FindUpPlantPotValueAt(i);
        var valueUpCurtainLayer = FindUpCurtainLayerValueAt(i);
        var valueUpMoneyBag = FindUpMoneyBagValueAt(i);
        var valueUpCupBoard = FindUpCupBoardValueAt(i);
        var valueUpMachineCream = FindUpMachineCreamValueAt(i);
        var valueUpMagicNest = FindUpMagicNestValueAt(i);
        var valueUpFlowerPot = FindUpFlowerPotValueAt(i);
        var valueUpBeverageFridge = FindUpBeverageFridgeValueAt(i);

        var valueUpWoodBox = 0;
        var valueUpGriller = 0;


        if (woodBoxesGrid != null) valueUpWoodBox = FindUpWoodBoxValueAt(i);
        if (grillerGrid != null) valueUpGriller = FindUpGrillerValueAt(i);

        if (valueTray == 0 && valueGiftBox == 0 && valueCoffeeBoard == 0
            && valuePlantPot == 0 && valueWoodBox == 0 && valueGriller == 0
            && valueCurtainLayer == 0 && valueMoneyBag == 0 && valueCupBoard == 0
            && valueMachineCream == 0 && valueMagicNest == 0 && valueFlowerPot == 0
            && valueBeverageFridge == 0
            && valueUpPlantPot == 0 && valueUpWoodBox == 0 && valueUpGriller == 0
            && valueUpCurtainLayer == 0 && valueUpMoneyBag == 0 && valueUpCupBoard == 0
            && valueUpMachineCream == 0 && valueUpMagicNest == 0 && valueUpFlowerPot == 0
            && valueUpBeverageFridge == 0)
          return false;
      }
      else
      {
        if (valueTray == 0 && valueGiftBox == 0 && valueCoffeeBoard == 0
            && valuePlantPot == 0 && valueWoodBox == 0 && valueGriller == 0
            && valueCurtainLayer == 0 && valueMoneyBag == 0 && valueCupBoard == 0
            && valueMachineCream == 0 && valueMagicNest == 0 && valueFlowerPot == 0
            && valueBeverageFridge == 0)
          return false;
      }
    }

    return true;
  }

  public bool HasOnlyPairTrays()
  {
    int pairCount = 0;
    int trayCount = 0;

    if (pairCount > 0 && pairCount == trayCount) return true;
    return false;
  }

  public bool IsPlacedTraysAlmostFull(out List<TrayControl> surroundedTrays)
  {
    surroundedTrays = new List<TrayControl>();

    if (woodBoxesGrid != null)
    {
      var _count1 = 0;
      var _countPosList1 = new List<float3>();
      for (int i = 0; i < TrayGrid.Grid.Length; ++i)
      {
        var valueTray = TrayGrid.Grid[i];
        var valueWoodBox = woodBoxesGrid[i];
        if (HasOnlyPairTrays())
        {

          var valueUpWood = FindUpWoodBoxValueAt(i);


          if (_count1 == 1)
          {
            _countPosList1.Add(trayGrid.ConvertIndexToWorldPos(i));
          }
          if (_count1 > 1) return false;
        }
        else
        {
          if (valueTray == 0 && valueWoodBox == 0)
          {
            _count1++;
            if (_count1 == 1)
            {
              _countPosList1.Add(trayGrid.ConvertIndexToWorldPos(i));
            }
            if (_count1 > 1) return false;
          }
        }
      }
      if (_countPosList1.Count > 0)
      {
        var neighbors1 = FindNeighborTraysAt(_countPosList1[0]);
        surroundedTrays.AddRange(neighbors1);
      }
      return true;
    }

    var _count2 = 0;
    var _countPosList2 = new List<float3>();
    for (int i = 0; i < TrayGrid.Grid.Length; ++i)
    {
      var valueTray = TrayGrid.Grid[i];

      if (HasOnlyPairTrays())
      {


        if (_count2 == 1)
        {
          _countPosList2.Add(trayGrid.ConvertIndexToWorldPos(i));
        }
        if (_count2 > 1) return false;
      }
      else
      {
        if (valueTray == 0)
        {
          _count2++;
          if (_count2 == 1)
          {
            _countPosList2.Add(trayGrid.ConvertIndexToWorldPos(i));
          }
          if (_count2 > 1) return false;
        }
      }
    }
    if (_countPosList2.Count > 0)
    {
      var neighbors2 = FindNeighborTraysAt(_countPosList2[0]);
      surroundedTrays.AddRange(neighbors2);
    }
    return true;
  }

  public bool IsAvailableTrayEmptyAt(int index)
  {
    return false;
  }

  public bool IsAvailableTraysEmpty()
  {
    int count = 0;

    return count == 0;
  }


}