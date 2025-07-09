using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// GiftBoxManager
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("PlantPot Dependences")]
  [SerializeField] GameObject plantPotPrefab;

  [Header("PlantPot Pooling")]
  ObjectPool<GameObject> plantPotPool;

  int[] _plantPotesGrid;
  public int[] PlantPotsGrid { get { return _plantPotesGrid; } }

  [HideInInspector] public GameObject[] PlantPots;

  private void InitPlantPotsPool()
  {
    plantPotPool = new ObjectPool<GameObject>(
      CreatePlantPotsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreatePlantPotsPool()
  {
    GameObject _obj = Instantiate(plantPotPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(plantPotPool);
    return _obj;
  }

  public void SpawnPlantPots(int[] grids)
  {
    if (grids.Length == 0) return;

    _plantPotesGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _plantPotesGrid[i] = grids[i];
    }

    PlantPots = new GameObject[grids.Length];
    for (int i = 0; i < _plantPotesGrid.Length; ++i)
    {
      if (_plantPotesGrid[i] == 0) continue;
      SpawnPlanePotAt(i);
    }
  }

  private void SpawnPlanePotAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var plantPot = plantPotPool.Get();

    plantPot.transform.position = pos;
    plantPot.transform.localScale = new float3(1, 1, 1);
    PlantPots[index] = plantPot;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    plantPot.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderLayer;
    plantPot.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Pot1", false);
    plantPot.GetComponentInChildren<SkeletonAnimation>().GetComponent<MeshRenderer>().sortingOrder = orderLayer;
  }

  public void TryRemoveNeighborPlantPots(float3 pos, bool _hasFindNeighbor = true)
  {
    var trayGrid = TrayGrid;

    List<GameObject> neighborPlantPots = new();
    if (_hasFindNeighbor)
    {
      neighborPlantPots = FindNeighborPlantPotsAt(pos);
    }
    else
    {
      var plantPot = GetPlantPotAt(pos);
      if (plantPot != null)
      {
        neighborPlantPots.Add(plantPot);
      }
    }

    for (int i = 0; i < neighborPlantPots.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborPlantPots[i].transform.position);

      if (
        CurtainLayersGrid[index] == 0 &&
        PlantPotsGrid != null &&
        PlantPotsGrid[index] > 0
      )
      {
        if (PlantPots[index] != null)
        {
          SoundManager.Instance.PlayPlantPotSfx();


          PlantPots[index]
            .GetComponent<PlantPotControl>().RemoveFromTableWithAnim();
        }
      }
    }
  }

  public void RemovePlantPotAt(int x)
  {
    if (PlantPots == null || PlantPots.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = PlantPots[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var plantPot = obj.GetComponent<PlantPotControl>();
      if (LeanTween.isTweening(plantPot.gameObject)) continue;

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

          plantPot.FullyRemoveFromTable();
        })
      );
    }
  }

  public void ClearPlantPots()
  {
    for (int i = 0; i < PlantPots.Length; ++i)
    {
      var obj = PlantPots[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var plantPot = obj.GetComponent<PlantPotControl>();
      plantPot.RemoveFromTable();
    }
  }

  public GameObject GetPlantPotAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > PlantPots.Length - 1) return null;

    return PlantPots[index];
  }

  public List<GameObject> FindNeighborPlantPotsAt(float3 worldPos)
  {
    List<GameObject> linkedPlantPots = new();
  

    return linkedPlantPots;
  }

  public bool HasPlantPotAt(float3 pos)
  {
    var plantPot = GetPlantPotAt(pos);

    if (plantPot != null)
    {
      return true;
    }

    return false;
  }

  private void OnTouchPlantPotAt(float3 pos)
  {
    var plantPot = GetPlantPotAt(pos).GetComponent<PlantPotControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, plantPot);
    }
  }

  int FindUpPlantPotValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _plantPotesGrid[upIndex];
  }

  public void HidePlantPotsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (PlantPots[index] == null) continue;
      PlantPots[index].transform.localScale = float3.zero;
    }
  }

  public void ShowPlantPotsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (PlantPots[index] == null) continue;
      PlantPots[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}