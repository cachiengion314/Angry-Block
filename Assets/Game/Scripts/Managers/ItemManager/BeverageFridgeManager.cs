using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// BeverageFridge Manager
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("BeverageFridge Dependencies")]
  [SerializeField] GameObject beverageFridgePref;

  [Header("MoneyBag Pooling")]
  ObjectPool<GameObject> _beverageFridgePool;

  int[] _beverageFridgesGrid;
  public int[] BeverageFridgesGrid { get { return _beverageFridgesGrid; } }

  [HideInInspector] public List<BeverageFridgeControl> BeverageFridges = new();

  private void InitBeverageFridgesPool()
  {
    _beverageFridgePool = new ObjectPool<GameObject>(
      CreateBeverageFridgesPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );

    _beverageFridgesGrid = new int[24];
  }

  private GameObject CreateBeverageFridgesPool()
  {
    GameObject _obj = Instantiate(beverageFridgePref, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(_beverageFridgePool);
    return _obj;
  }

  public void SpawnBeverageFridges(BeverageFridgeData[] beverageFridgeDatas)
  {
    _beverageFridgesGrid = new int[24];

    for (int i = 0; i < beverageFridgeDatas.Length; i++)
    {
      var beverageFridgeData = beverageFridgeDatas[i];
      SpawnBeverageFridge(beverageFridgeData);
    }
  }

  private void SpawnBeverageFridge(BeverageFridgeData beverageFridgeData)
  {
    var pos = new float3(0, 0, 0);
    foreach (var id in beverageFridgeData.PosCellInLayer)
    {
      var posCell = trayGrid.ConvertIndexToWorldPos(id);
      pos += posCell;
    }
    pos /= beverageFridgeData.PosCellInLayer.Length;

    var beverageFridge = _beverageFridgePool.Get();
    beverageFridge.transform.position = pos;
    beverageFridge.transform.localScale = new float3(1, 1, 1);

    var beverageFridgeControl = beverageFridge.GetComponent<BeverageFridgeControl>();
    var indexToCalculateSortingLayer = beverageFridgeData.IndexToCalculateSortingLayer;
    int orderLayer = trayGrid.GridSize.y * 20 - indexToCalculateSortingLayer % trayGrid.GridSize.y * 20;
    orderLayer += 1;

    beverageFridgeControl.InitFrom(beverageFridgeData);
    beverageFridgeControl.SetSortingOrder(orderLayer);
    BeverageFridges.Add(beverageFridgeControl);

    InitBeverageFridge(beverageFridgeData.PosCellInLayer);
  }

  private void InitBeverageFridge(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; i++)
    {
      BeverageFridgesGrid[indexes[i]] = 1;
    }
  }

  public void RemoveBeverageFridge(BeverageFridgeControl beverageFridgeControl)
  {
    var indexes = beverageFridgeControl.PosCellInLayer;

    for (int i = 0; i < indexes.Count; i++)
    {
      BeverageFridgesGrid[indexes[i]] = 0;
    }

    BeverageFridges.Remove(beverageFridgeControl);
  }

  public void RemoveBeverageFridgesAt(int x)
  {
    if (BeverageFridges == null || BeverageFridges.Count == 0) return;

    var seq = LeanTween.sequence();
    List<BeverageFridgeControl> beverageFridgeNeedRemoves = new();

    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var index = trayGrid.ConvertGridPosToIndex(new int2(x, y));
      if (_beverageFridgesGrid[index] == 0) continue;

      var pos = trayGrid.ConvertGridPosToWorldPos(new int2(x, y));
      var beverageFridge = FindBeverageFridgeWith(pos);
      if (beverageFridge == null) continue;
      if (beverageFridgeNeedRemoves.Contains(beverageFridge)) continue;

      beverageFridgeNeedRemoves.Add(beverageFridge);
    }

    for (int i = 0; i < beverageFridgeNeedRemoves.Count; i++)
    {
      var beverageFridge = beverageFridgeNeedRemoves[i];
      beverageFridge.FullyRemoveFromTable();
    }
  }

  public void TryRemoveNeighborBeverageFridges(float3 pos, bool _hasFindNeighbor = true)
  {
    var trayGrid = TrayGrid;
    List<GameObject> neighborBeverageFridges = new();

    if (_hasFindNeighbor)
    {
      neighborBeverageFridges = FindNeighborBeverageFridgesAt(pos);
    }
    else
    {
      neighborBeverageFridges.Add(FindBeverageFridgeWith(pos).gameObject);
    }

    for (int i = 0; i < neighborBeverageFridges.Count; ++i)
    {
      var beverageFridge = neighborBeverageFridges[i];
      var index = trayGrid.ConvertWorldPosToIndex(neighborBeverageFridges[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        BeverageFridgesGrid != null &&
        BeverageFridgesGrid[index] > 0
      )
      {
        LeanTween.delayedCall(gameObject, .1f, () =>
        {
          if (beverageFridge == null) return;

          beverageFridge
            .GetComponent<BeverageFridgeControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public void ClearBeverageFridges()
  {
    for (int i = 0; i < BeverageFridges.Count; ++i)
    {
      var obj = BeverageFridges[i];
      if (obj == null) continue;

      var beverageFridge = obj.GetComponent<BeverageFridgeControl>();
      beverageFridge.RemoveFromTable();
    }
  }

  public List<GameObject> FindNeighborBeverageFridgesAt(float3 worldPos)
  {
    List<GameObject> linkedBeverageFridges = new();
  
    return linkedBeverageFridges;
  }

  public bool HasBeverageFridgeAt(float3 pos)
  {
    var beverageFridge = FindBeverageFridgeWith(pos);

    if (beverageFridge != null)
    {
      return true;
    }

    return false;
  }

  public void OnTouchBeverageFridgeAt(float3 pos)
  {
    var beverageFridge = FindBeverageFridgeWith(pos);

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, beverageFridge);
    }
  }

  public BeverageFridgeControl FindBeverageFridgeWith(float3 worldPos)
  {
    var gridPos = trayGrid.ConvertWorldPosToGridPos(worldPos);
    if (trayGrid.IsGridPosOutsideAt(gridPos)) return null;

    var id = trayGrid.ConvertGridPosToIndex(gridPos);

    for (int i = 0; i < BeverageFridges.Count; i++)
    {
      for (int j = 0; j < BeverageFridges[i].PosCellInLayer.Count; j++)
      {
        if (BeverageFridges[i].PosCellInLayer[j] == id)
        {
          return BeverageFridges[i];
        }
      }
    }

    return null;
  }

  int FindUpBeverageFridgeValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);

    return _beverageFridgesGrid[upIndex];
  }

  public void HideBeverageFridgesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      var beverageFridge = FindBeverageFridgeWith(trayGrid.ConvertIndexToWorldPos(index));

      if (beverageFridge == null) continue;

      beverageFridge.transform.localScale = float3.zero;
    }
  }

  public void ShowBeverageFridgesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      var beverageFridge = FindBeverageFridgeWith(trayGrid.ConvertIndexToWorldPos(index));

      if (beverageFridge == null) continue;
      beverageFridge.transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}