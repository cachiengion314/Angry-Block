using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Grass Manager
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("Grass Dependences")]
  [SerializeField] GameObject grassPrefab;

  [Header("Grass Pooling")]
  ObjectPool<GameObject> grassPool;

  int[] grassesGrid;
  public int[] GrassesGrid { get { return grassesGrid; } }

  [HideInInspector] public GameObject[] Grasses;

  private void InitGrassPool()
  {
    grassPool = new ObjectPool<GameObject>(
        CreateGrassesPool,
        OnTakeObjFromPool,
        OnReturnObjFromPool,
        OnDestroyPoolObj,
        true,
        trayGrid.GridSize.x * trayGrid.GridSize.y,
        trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateGrassesPool()
  {
    GameObject _obj = Instantiate(grassPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(grassPool);
    return _obj;
  }

  public void SpawnGrasses(int[] grid)
  {
    if (grid.Length == 0) return;

    grassesGrid = new int[grid.Length];
    for (int i = 0; i < grid.Length; ++i)
    {
      grassesGrid[i] = grid[i];
    }

    Grasses = new GameObject[grid.Length];
    for (int i = 0; i < grassesGrid.Length; ++i)
    {
      if (grassesGrid[i] == 0) continue;
      SpawnGrassAt(i);
    }
  }

  public GameObject SpawnGrassAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var grass = grassPool.Get();

    grass.transform.position = pos;
    Grasses[index] = grass;
    GrassesGrid[index] = 1;

    return grass;
  }

  public void RemoveGrassesAt(int x)
  {
    if (Grasses == null || Grasses.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = Grasses[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;
      if (grillerGrid[index] > 0) continue;

      var grass = obj.GetComponent<GrassControl>();
      if (LeanTween.isTweening(grass.gameObject)) continue;

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

          grass.RemoveFromTable();
        })
      );
    }
  }

  public void TryRemoveGrassUnder(float3 pos, out int grassValue)
  {
    grassValue = 0;
    var trayGrid = TrayGrid;
    var index = trayGrid.ConvertWorldPosToIndex(pos);
    if (
      GrassesGrid != null &&
      GrassesGrid[index] > 0
    )
    {
      grassValue = 11; // grasses value for scoring in UI is 11
      if (Grasses[index] != null)
      {
        var theme = RendererSystem.Instance.GetCurrentTheme();
        Grasses[index]
          .GetComponent<GrassControl>().RemoveFromTableWithAnim();
      }
    }
  }

  public void ClearGrasses()
  {
    for (int i = 0; i < Grasses.Length; ++i)
    {
      var obj = Grasses[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      var grass = obj.GetComponent<GrassControl>();
      grass.RemoveFromTable();
    }
  }

  public List<int> FindEmptyGrassForPot() // use to spawn grass from plant pot
  {
    List<int> emptyGrasses = new();

    for (int i = 0; i < GrassesGrid.Length; i++)
    {
      var pos = TrayGrid.ConvertIndexToWorldPos(i);

      if (TrayGrid.GetValueAt(pos) == 999) continue;
      if (HasGiftBoxAt(pos)) continue;
      if (HasCoffeeBoardAt(pos)) continue;
      if (HasPlantPotAt(pos)) continue;
      if (Grillers[i] != null) continue;
      if (Grasses[i] != null) continue;
      // check video ads 
      if (WoodBoxes[i] != null && WoodBoxes[i].TryGetComponent(out VideoAdsControl videoAds)) continue;

      emptyGrasses.Add(i);
    }

    return emptyGrasses;
  }

  public bool HasGrassAt(float3 pos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(pos);

    if (GrassesGrid != null && GrassesGrid[index] > 0)
    {
      return true;
    }

    return false;
  }

  public void HideGrassesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (Grasses[index] == null) continue;
      Grasses[index].transform.localScale = float3.zero;
    }
  }

  public void ShowGrassesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (Grasses[index] == null) continue;
      Grasses[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}