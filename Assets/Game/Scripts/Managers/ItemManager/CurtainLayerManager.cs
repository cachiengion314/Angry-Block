using System.Collections.Generic;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public partial class ItemManager : MonoBehaviour
{
  [Header("CurtainLayer Dependences")]
  [SerializeField] GameObject curtainLayerPrefab;
  [SerializeField] SkeletonAnimation skeExplosionCup;

  [Header("CurtainLayer Pooling")]
  ObjectPool<GameObject> curtainLayerPool;

  int[] curtainLayersGrid;
  public int[] CurtainLayersGrid { get { return curtainLayersGrid; } }

  [HideInInspector] public List<CurtainLayerControl> CurtainLayers = new();

  private void InitCurtainLayerPool()
  {
    curtainLayerPool = new ObjectPool<GameObject>(
        CreateCurtainLayerPool,
        OnTakeObjFromPool,
        OnReturnObjFromPool,
        OnDestroyPoolObj,
        true,
        trayGrid.GridSize.x * trayGrid.GridSize.y,
        trayGrid.GridSize.x * trayGrid.GridSize.y
    );

    curtainLayersGrid = new int[24];
  }

  GameObject CreateCurtainLayerPool()
  {
    GameObject _obj = Instantiate(curtainLayerPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(coverLetPool);
    return _obj;
  }

  public void SpawnCurtainLayers(CurtainLayerData[] curtainLayerDatas)
  {
    curtainLayersGrid = new int[24];

    for (int i = 0; i < curtainLayerDatas.Length; i++)
    {
      var curtainLayerData = curtainLayerDatas[i];
      SpawnCurtainLayer(curtainLayerData);
    }
  }

  private void SpawnCurtainLayer(CurtainLayerData curtainLayerData)
  {
    // Calculate Pos
    var pos = new float3(0, 0, 0);
    foreach (var id in curtainLayerData.PosCellInLayer)
    {
      var posCell = trayGrid.ConvertIndexToWorldPos(id);
      pos += posCell;
    }
    pos /= curtainLayerData.PosCellInLayer.Length;

    var curtainLayer = curtainLayerPool.Get();
    curtainLayer.transform.position = pos;

    var curtainLayerControl = curtainLayer.GetComponent<CurtainLayerControl>();
    var indexToCalculateSortingLayer = curtainLayerData.IndexToCalculateSortingLayer;
    int orderLayer = trayGrid.GridSize.y * 20 - indexToCalculateSortingLayer % trayGrid.GridSize.y * 20;
    orderLayer += 6;

    curtainLayerControl.InitFrom(curtainLayerData, orderLayer);
    CurtainLayers.Add(curtainLayerControl);

    InitCurtainLayer(curtainLayerData.PosCellInLayer);
    HideCoffeeBoardsIn(curtainLayerData.PosCellInLayer);
    HideCoverLetsIn(curtainLayerData.PosCellInLayer);
    HideCupBoardsIn(curtainLayerData.PosCellInLayer);
    HideGiftBoxesIn(curtainLayerData.PosCellInLayer);
    HideGoldenTraysIn(curtainLayerData.PosCellInLayer);
    HideGrassesIn(curtainLayerData.PosCellInLayer);
    HideGrillersIn(curtainLayerData.PosCellInLayer);
    HideIceBoxesIn(curtainLayerData.PosCellInLayer);
    HideMachineCreamsIn(curtainLayerData.PosCellInLayer);
    HideMoneyBagsIn(curtainLayerData.PosCellInLayer);
    HidePlantPotsIn(curtainLayerData.PosCellInLayer);
    HideWoodBoxesIn(curtainLayerData.PosCellInLayer);
    HideMagicNestsIn(curtainLayerData.PosCellInLayer);
    HideLeavesFlowersIn(curtainLayerData.PosCellInLayer);
    HideFlowerPotsIn(curtainLayerData.PosCellInLayer);
    HideBeverageFridgesIn(curtainLayerData.PosCellInLayer);
  }

  public SkeletonAnimation SpawnSkeExplosionCupAt(float3 pos)
  {
    var ske = Instantiate(skeExplosionCup, transform);
    ske.transform.position = pos;

    return ske;
  }

  /// <summary>
  /// ColorIndex: -1: all colors
  /// </summary>
  /// <param name="colorIndex"></param>
  /// <returns></returns> <summary>
  /// 
  /// </summary>
  /// <param name="colorIndex"></param>
  /// <returns></returns>
  public CurtainLayerControl FindCurtainLayerWith(int colorIndex)
  {
    for (int i = 0; i < CurtainLayers.Count; i++)
    {
      if (CurtainLayers[i].ColorIndex == colorIndex || CurtainLayers[i].ColorIndex == -1)
      {
        return CurtainLayers[i];
      }
    }

    return null;
  }

  /// <summary>
  /// Use for booster because this func dont check color
  /// </summary>
  /// <param name="worldPos"></param>
  /// <returns></returns>
  public CurtainLayerControl FindCurtainLayerWith(float3 worldPos)
  {
    var gridPos = trayGrid.ConvertWorldPosToGridPos(worldPos);
    if (trayGrid.IsGridPosOutsideAt(gridPos)) return null;

    var id = trayGrid.ConvertGridPosToIndex(gridPos);

    for (int i = 0; i < CurtainLayers.Count; i++)
    {
      for (int j = 0; j < CurtainLayers[i].PosCellInLayer.Count; j++)
      {
        if (CurtainLayers[i].PosCellInLayer[j] == id)
        {
          return CurtainLayers[i];
        }
      }
    }

    return null;
  }

  int FindUpCurtainLayerValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return curtainLayersGrid[upIndex];
  }

  private void InitCurtainLayer(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; i++)
    {
      CurtainLayersGrid[indexes[i]] = 1;
    }
  }

  public void RemoveCurtainLayer(CurtainLayerControl curtainLayerControl)
  {
    var indexes = curtainLayerControl.PosCellInLayer;

    for (int i = 0; i < indexes.Count; i++)
    {
      CurtainLayersGrid[indexes[i]] = 0;
    }

    CurtainLayers.Remove(curtainLayerControl);
  }

  public void RemoveCurtainLayersAt(int x)
  {
    if (CurtainLayers == null || CurtainLayers.Count == 0) return;

    var seq = LeanTween.sequence();
    List<CurtainLayerControl> curtainLayerNeedRemoves = new();

    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var index = trayGrid.ConvertGridPosToIndex(new int2(x, y));
      if (curtainLayersGrid[index] == 0) continue;

      var pos = trayGrid.ConvertGridPosToWorldPos(new int2(x, y));
      var curtainLayer = FindCurtainLayerWith(pos);
      if (curtainLayer == null) continue;
      if (curtainLayerNeedRemoves.Contains(curtainLayer)) continue;

      curtainLayerNeedRemoves.Add(curtainLayer);
    }

    for (int i = 0; i < curtainLayerNeedRemoves.Count; i++)
    {
      var curtainLayer = curtainLayerNeedRemoves[i];
      curtainLayer.FullyRemoveFromTable();
      curtainLayer.UpdateAmount();
    }
  }

  private void ClearCurtainLayers()
  {
    for (int i = 0; i < CurtainLayers.Count; i++)
    {
      if (CurtainLayers[i] == null) continue;

      CurtainLayers[i].Release();
      RemoveCurtainLayer(CurtainLayers[i]);
    }
  }

  public bool HasCurtainLayerAt(float3 pos)
  {
    var curtainLayer = FindCurtainLayerWith(pos);

    if (curtainLayer != null)
    {
      return true;
    }

    return false;
  }

  public void OnTouchCurtainLayerAt(float3 pos)
  {
    var curtainLayer = FindCurtainLayerWith(pos);

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, curtainLayer);
      curtainLayer.UpdateAmount();
    }
  }
}