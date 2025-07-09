using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using HoangNam;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public partial class BeverageFridgeControl : MonoBehaviour, IPoolItemControl
{
  private void TrySpawnCupFromFridge()
  {
    var colorIndex = (ColorIndex)_beverageFridgeData.ColorCups[_amountCup];
    int amountCupNeedSpawn = 2;

    SpawnCupInTrayHas(colorIndex, ref amountCupNeedSpawn);
    if (amountCupNeedSpawn == 0) return;

    SpawnNewTrayRandomWith(colorIndex, ref amountCupNeedSpawn);
    if (amountCupNeedSpawn == 0) return;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="colorIndex"></param>
  /// <param name="amountCupNeedSpawn"></param>
  /// TODO
  private void SpawnCupInTrayHas(ColorIndex colorIndex, ref int amountCupNeedSpawn)
  {
    var posNotOnlyHasColor = ItemManager.Instance.FindPosTrayHas(colorIndex);
    if (posNotOnlyHasColor.Count == 0) return;

    var index = UnityEngine.Random.Range(0, posNotOnlyHasColor.Count);
    var neighborPosCanPlaceTray = new List<float3>();

    bool hasPlacedTray = false;
    while (posNotOnlyHasColor.Count > 0)
    {
      neighborPosCanPlaceTray = ItemManager.Instance.FindPosNeighborCanPlaceTrayFrom(posNotOnlyHasColor[index]);
      if (neighborPosCanPlaceTray.Count == 0)
      {
        posNotOnlyHasColor.RemoveAt(index);
        index = UnityEngine.Random.Range(0, posNotOnlyHasColor.Count);
        continue;
      }

      hasPlacedTray = true;
      break;
    }
    if (!hasPlacedTray) return;

    ColorIndex[] colors = new ColorIndex[amountCupNeedSpawn];
    for (int i = 0; i < amountCupNeedSpawn; i++)
    {
      colors[i] = colorIndex;
    }
    amountCupNeedSpawn = 0;

    var idxNeighbor = UnityEngine.Random.Range(0, neighborPosCanPlaceTray.Count);
    VisualizeSpawnNewTrayAt(neighborPosCanPlaceTray[idxNeighbor], colors);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="colorIndex"></param>
  /// <param name="amountCupNeedSpawn"></param> <summary>
  /// 
  /// </summary>
  /// <param name="colorIndex"></param>
  /// <param name="amountCupNeedSpawn"></param>
  private void SpawnNewTrayRandomWith(ColorIndex colorIndex, ref int amountCupNeedSpawn)
  {
    var posCanPlaceTray = ItemManager.Instance.FindPosCanPlaceTray();

    if (posCanPlaceTray.Count == 0) return;
    var index = UnityEngine.Random.Range(0, posCanPlaceTray.Count);

    ColorIndex[] colors = new ColorIndex[amountCupNeedSpawn];
    for (int i = 0; i < amountCupNeedSpawn; i++)
    {
      colors[i] = colorIndex;
    }
    amountCupNeedSpawn = 0;

    VisualizeSpawnNewTrayAt(posCanPlaceTray[index], colors);
  }

  private void TryMergeTrayNeighborFrom(float3 pos)
  {
    HashSet<TrayControl> _linkedTrays1 = new();


  }

  private void VisualizeSpawnNewTrayAt(float3 pos, ColorIndex[] colors)
  {
    Sequence seq = DOTween.Sequence();
    var currentTimeAnim = 0f;
    var timeMoveTrail = 0.6f;
    var timeDelayMoveTrail = 0.4f;
    var timeScaleTray = 0.2f;

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        ItemManager.Instance.TrayGrid.SetValueAt(pos, 1000);
      }
    );

    currentTimeAnim += timeDelayMoveTrail;

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        
      }
    );
    currentTimeAnim += timeMoveTrail;

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {


      }
    );

    currentTimeAnim += timeScaleTray;
    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        ItemManager.Instance.TrayGrid.SetValueAt(pos, 1);
        TryMergeTrayNeighborFrom(pos);
      }
    );
  }
}