using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// FlowerPot Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("FlowerPot Dependences")]
  [SerializeField] GameObject flowerPotPrefab;

  [Header("FlowerPot Pooling")]
  ObjectPool<GameObject> flowerPotPool;

  int[] _flowerPotsGrid;
  public int[] FlowerPotsGrid { get { return _flowerPotsGrid; } }

  [HideInInspector] public GameObject[] FlowerPots;

  private void InitFlowerPotPool()
  {
    flowerPotPool = new ObjectPool<GameObject>(
      CreateFlowerPotsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateFlowerPotsPool()
  {
    GameObject _obj = Instantiate(flowerPotPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(flowerPotPool);
    return _obj;
  }

  public void SpawnFlowerPots(int[] grids)
  {
    if (grids.Length == 0) return;

    _flowerPotsGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _flowerPotsGrid[i] = grids[i];
    }

    FlowerPots = new GameObject[grids.Length];
    for (int i = 0; i < _flowerPotsGrid.Length; ++i)
    {
      if (_flowerPotsGrid[i] == 0) continue;
      SpawnFlowerPotAt(i);
    }
  }

  private void SpawnFlowerPotAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var flowerPot = flowerPotPool.Get();

    flowerPot.transform.position = pos;
    flowerPot.transform.localScale = new float3(1, 1, 1);
    FlowerPots[index] = flowerPot;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    flowerPot.GetComponent<FlowerPotControl>().ShowStateOfHealthBaseOn(FlowerPotsGrid[index]);
    flowerPot.GetComponent<FlowerPotControl>().SetSortingOrder(orderLayer);
  }

  public void TryRemoveNeighborFlowerPots(float3 pos, bool _hasFindNeighbor = true)
  {
    var trayGrid = TrayGrid;

    List<GameObject> neighborFlowerPots = new();
    if (_hasFindNeighbor)
    {
      neighborFlowerPots = FindNeighborFlowerPotsAt(pos);
    }
    else
    {
      var flowerPot = GetFlowerPotAt(pos);
      if (flowerPot != null)
      {
        neighborFlowerPots.Add(flowerPot);
      }
    }

    for (int i = 0; i < neighborFlowerPots.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborFlowerPots[i].transform.position);

      if (
        CurtainLayersGrid[index] == 0 &&
        FlowerPotsGrid != null &&
        FlowerPotsGrid[index] > 0
      )
      {
        if (FlowerPots[index] != null)
        {
          FlowerPots[index]
            .GetComponent<FlowerPotControl>().RemoveFromTableWithAnim();
        }
      }
    }
  }

  public void RemoveFlowerPotAt(int x)
  {
    if (FlowerPots == null || FlowerPots.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = FlowerPots[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var flowerPot = obj.GetComponent<FlowerPotControl>();
      if (LeanTween.isTweening(flowerPot.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          // EffectManager.Instance.SpawnHammerExplosiveAt(pos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { Destroy(boomSkeleton.gameObject); });

          flowerPot.FullyRemoveFromTable();
        })
      );
    }
  }

  public void ClearFlowerPots()
  {
    for (int i = 0; i < FlowerPots.Length; ++i)
    {
      var obj = FlowerPots[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var flowerPot = obj.GetComponent<FlowerPotControl>();
      flowerPot.RemoveFromTable();
    }
  }

  public GameObject GetFlowerPotAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > FlowerPots.Length - 1) return null;

    return FlowerPots[index];
  }

  public List<GameObject> FindNeighborFlowerPotsAt(float3 worldPos)
  {
    List<GameObject> linkedFlowerPots = new();
  
    return linkedFlowerPots;
  }

  public bool HasFlowerPotAt(float3 pos)
  {
    var plantPot = GetFlowerPotAt(pos);

    if (plantPot != null)
    {
      return true;
    }

    return false;
  }

  public List<float3> FindEmptyLeavesForPotAt(float3 pos)
  {
    var result = new List<float3>();
    var gridPos = TrayGrid.ConvertWorldPosToGridPos(pos);
    if (TrayGrid.IsGridPosOutsideAt(gridPos)) return null;

  

    return result;
  }

  private void OnTouchFlowerPotAt(float3 pos)
  {
    var plantPot = GetFlowerPotAt(pos).GetComponent<FlowerPotControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, plantPot);
    }
  }

  int FindUpFlowerPotValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _plantPotesGrid[upIndex];
  }

  public void HideFlowerPotsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (FlowerPots[index] == null) continue;
      FlowerPots[index].transform.localScale = float3.zero;
    }
  }

  public void ShowFlowerPotsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (FlowerPots[index] == null) continue;
      FlowerPots[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}