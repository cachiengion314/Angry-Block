using System;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;

public partial class TrayControl : MonoBehaviour, IPoolItemControl
{
  /// <summary>
  /// Uuser has placed the tray in the wrong way.
  /// So we should return the tray back to the previous position
  /// </summary>
  public void PlacedWrongAt(float3 droppedPos)
  {
    currPos = new float3(1, 1, 1) * 999;
    EnableWoodTrays();

    transform.position = CurrentWorldPos;
  }

  /// <summary>
  /// User has just placed this tray successfully. 
  /// So we should clear its placement in the AvailableTrays array
  /// </summary>
  public void PlacedSuccessfullyAt(float3 droppedPos, Action _onCompleted = null)
  {
    SoundManager.Instance.PlayPlacedTraySfx();

    ClearPlacementInAvailableTrays();

    transform.position = droppedPos;
    CurrentWorldPos = droppedPos;

    IsPlaced = true;

    if (ItemManager.Instance.HasGoldenTrayAt(droppedPos))
    {
      ChangeSkinTrayTo(1);
    }
    else
    {
      ChangeSkinTrayTo(0);
    }

    _onCompleted?.Invoke();
    ItemManager.Instance.ChangeMagicNestStatus();
  }

  public void PlacedAt(float3 droppedPos, Action _onCompleted = null)
  {

    transform.position = droppedPos;
    CurrentWorldPos = droppedPos;

    IsPlaced = true;

    if (ItemManager.Instance.HasGoldenTrayAt(droppedPos))
    {
      ChangeSkinTrayTo(1);
    }
    else
    {
      ChangeSkinTrayTo(0);
    }

    _onCompleted?.Invoke();
  }
}