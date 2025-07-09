using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// NeighborTray
/// </summary> <summary>
/// 
/// </summary>
public partial class TrayControl : MonoBehaviour, IPoolItemControl
{
  public List<TrayControl> FindNeighborTraysAt(float3 worldPos)
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    List<TrayControl> linkedTrays = new();

    return linkedTrays;
  }
}