using System.Collections.Generic;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Find Tray
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  public List<GameObject> FindIceNeighborTraysAt(float3 worldPos)
  {
    List<GameObject> linkedIceBoxes = new();
 
    return linkedIceBoxes;
  }

  public List<GameObject> FindCoverLetNeighborTraysAt(float3 worldPos)
  {
    List<GameObject> linkedCoverLets = new();
  

    return linkedCoverLets;
  }

  public List<TrayControl> FindNeighborTraysAt(float3 worldPos)
  {
    List<TrayControl> linkedTrays = new();
   
 
    return linkedTrays;
  }

  public void FindFullCupsTrays(out List<TrayControl> fullTrays, out List<TrayControl> emptyTrays)
  {
    fullTrays = new List<TrayControl>();
    emptyTrays = new List<TrayControl>();
  }

  public TrayControl FindIceTray(float3 posWorld)
  {
    if (trayGrid.IsPosOutsideAt(posWorld)) return null;
    if (IceBoxes != null && IceBoxes.Length > 0)
    {
      if (!IceBoxes[trayGrid.ConvertWorldPosToIndex(posWorld)]) return null;
    }

    if (IceBoxes.Length == 0)
    {
      return null;
    }

    return null;
  }

  public TrayControl FindCoverLetTray(float3 posWorld)
  {
    if (trayGrid.IsPosOutsideAt(posWorld)) return null;
    if (CoverLets != null && CoverLets.Length > 0)
    {
      if (!CoverLets[trayGrid.ConvertWorldPosToIndex(posWorld)]) return null;
    }

    if (CoverLets.Length == 0)
    {
      return null;
    }
    return null;
  }

  public List<float3> FindPosTrayHas(ColorIndex colorIndex)
  {
    var posTrays = new List<float3>();

    for (int i = 0; i < TrayGrid.Grid.Length; i++)
    {
      var pos = TrayGrid.ConvertIndexToWorldPos(i);







      bool hasColorNeed = false;


      if (hasColorNeed) posTrays.Add(pos);
    }

    return posTrays;
  }

  public List<float3> FindPosCanPlaceTray()
  {
    var posCanPlaceTrays = new List<float3>();

    for (int i = 0; i < TrayGrid.Grid.Length; i++)
    {
      var pos = TrayGrid.ConvertIndexToWorldPos(i);

      if (!IsPlaceableTrayAt(pos)) continue;
      posCanPlaceTrays.Add(pos);
    }

    return posCanPlaceTrays;
  }

  public List<float3> FindPosNeighborCanPlaceTrayFrom(float3 pos)
  {
    var posCanPlaceTrays = new List<float3>();
    var gridPos = TrayGrid.ConvertWorldPosToGridPos(pos);
    var dirs = new int2[] { new(-1, 0), new(0, 1), new(1, 0), new(0, -1) };

    for (int i = 0; i < dirs.Length; i++)
    {
      var newGridPos = gridPos + dirs[i];
      var newPos = TrayGrid.ConvertGridPosToWorldPos(newGridPos);

      if (!IsPlaceableTrayAt(newPos)) continue;
      posCanPlaceTrays.Add(newPos);
    }

    return posCanPlaceTrays;
  }

  private bool IsPlaceableTrayAt(float3 pos)
  {
    if (TrayGrid.IsPosOutsideAt(pos)) return false;
    if (TrayGrid.GetValueAt(pos) == 999) return false;
    if (HasIceBoxAt(pos)) return false;
    if (HasWoodBoxAt(pos)) return false;
    if (HasVideoAdsAt(pos)) return false;
    if (HasGrillerAt(pos)) return false;
    if (HasGiftBoxAt(pos)) return false;
    if (HasCoffeeBoardAt(pos)) return false;
    if (HasPlantPotAt(pos)) return false;
    if (HasCoverLetAt(pos)) return false;
    if (HasCurtainLayerAt(pos)) return false;
    if (HasMoneyBagAt(pos)) return false;
    if (HasCupBoardAt(pos)) return false;
    if (HasMachineCreamAt(pos)) return false;
    if (HasMagicNestAt(pos)) return false;
    if (HasFlowerPotAt(pos)) return false;
    if (HasBeverageFridgeAt(pos)) return false;

    return true;
  }
}