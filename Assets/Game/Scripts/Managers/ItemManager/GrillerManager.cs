using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Griller Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("Griller Dependences")]
  [SerializeField] GameObject grillerPrefab;

  [Header("Griller Pooling")]
  ObjectPool<GameObject> grillerPool;

  int[] grillerGrid;
  public int[] GrillerGrid { get { return grillerGrid; } }

  [HideInInspector] public GameObject[] Grillers;

  private void InitGrillerPool()
  {
    grillerPool = new ObjectPool<GameObject>(
        CreateGrillerPool,
        OnTakeObjFromPool,
        OnReturnObjFromPool,
        OnDestroyPoolObj,
        true,
        trayGrid.GridSize.x * trayGrid.GridSize.y,
        trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateGrillerPool()
  {
    GameObject _obj = Instantiate(grillerPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(grillerPool);
    return _obj;
  }

  public void SpawnGrillers(int[] grid)
  {
    if (grid.Length == 0) return;

    grillerGrid = new int[grid.Length];
    for (int i = 0; i < grid.Length; ++i)
    {
      grillerGrid[i] = grid[i];
    }

    Grillers = new GameObject[grid.Length];
    for (int i = 0; i < grillerGrid.Length; ++i)
    {
      if (grillerGrid[i] == 0) continue;
      SpawnGrillerAt(i);
    }
  }

  private void SpawnGrillerAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var griller = grillerPool.Get();
    griller.transform.position = pos;
    Grillers[index] = griller;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    griller.GetComponentInChildren<MeshRenderer>().sortingOrder = orderLayer;
  }

  public void RemoveGrillers()
  {
    if (Grillers == null || Grillers.Length == 0) return;

    for (int i = 0; i < Grillers.Length; i++)
    {
      var obj = Grillers[i];
      if (obj == null) continue;
      var griller = obj.GetComponent<GrillerControl>();
      if (LeanTween.isTweening(griller.gameObject))
      {
        LeanTween.cancel(griller.gameObject);
      }

      SoundManager.Instance.PlayBoomSfx();
      var pos = obj.transform.position;
      EffectManager.Instance.SpawnHammerExplosiveAt(pos);
      griller.FullyRemoveFromTableWithoutAnim();
    }
  }

  public void TryRemoveNeighborGrillers(float3 pos, out int grillerValue, out int amountBread, bool _hasFindNeighbor = true)
  {
    grillerValue = 0;
    amountBread = 0;
    var trayGrid = TrayGrid;

    List<GameObject> neighborGriller = new();

    if (!_hasFindNeighbor)
    {
      neighborGriller.Add(GetGrillerAt(pos));
    }
    else
    {
      neighborGriller.AddRange(FindNeighborGrillersAt(pos));
    }

    for (int i = 0; i < neighborGriller.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborGriller[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        GrillerGrid != null &&
        GrillerGrid[index] == 1
      )
      {
        grillerValue = 12;
        amountBread++;
        neighborGriller[i].GetComponent<GrillerControl>().RemoveFromTable();
      }
    }
  }

  public GameObject GetGrillerAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > Grillers.Length - 1) return null;

    return Grillers[index];
  }

  public void RemoveGrillerAt(int x)
  {
    if (Grillers == null || Grillers.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = Grillers[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var griller = obj.GetComponent<GrillerControl>();
      // if (LeanTween.isTweening(giftBox.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          // EffectManager.Instance.SpawnHammerExplosiveAt(pos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { boomSkeleton.gameObject.SetActive(false); });

          griller.FullyRemoveFromTable();
        })
      );
    }
  }

  public void ClearGrillers()
  {
    for (int i = 0; i < Grillers.Length; ++i)
    {
      var obj = Grillers[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var griller = obj.GetComponent<GrillerControl>();
      griller.FullyRemoveFromTableWithoutAnim();
    }
  }

  public List<GameObject> FindNeighborGrillersAt(float3 worldPos)
  {
    List<GameObject> linkedGrillers = new();
   
    return linkedGrillers;
  }

  int FindUpGrillerValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return grillerGrid[upIndex];
  }

  public bool HasGrillerAt(float3 pos)
  {
    var griller = GetGrillerAt(pos);

    if (griller != null)
    {
      return true;
    }

    return false;
  }

  private void OnTouchGrillerAt(float3 pos)
  {
    var griller = GetGrillerAt(pos).GetComponent<GrillerControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, griller);
    }
  }

  public void HideGrillersIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (Grillers[index] == null) continue;
      Grillers[index].transform.localScale = float3.zero;
    }
  }

  public void ShowGrillersIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (Grillers[index] == null) continue;
      Grillers[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}