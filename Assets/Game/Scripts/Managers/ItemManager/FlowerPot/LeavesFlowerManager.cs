using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// LeavesFlower Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("LeavesFlower Dependences")]
  [SerializeField] GameObject leavesFlowerPrefab;

  [Header("LeavesFlower Pooling")]
  ObjectPool<GameObject> leavesFlowerPool;

  int[] _leavesFlowerGrid;
  public int[] LeavesFlowerGrid { get { return _leavesFlowerGrid; } }

  [HideInInspector] public GameObject[] LeavesFlowers;

  private void InitLeavesFlowerPool()
  {
    leavesFlowerPool = new ObjectPool<GameObject>(
        CreateLeavesFlowerPool,
        OnTakeObjFromPool,
        OnReturnObjFromPool,
        OnDestroyPoolObj,
        true,
        trayGrid.GridSize.x * trayGrid.GridSize.y,
        trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateLeavesFlowerPool()
  {
    GameObject _obj = Instantiate(leavesFlowerPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(leavesFlowerPool);
    return _obj;
  }

  public void SpawnLeavesFlower(int[] grid)
  {
    if (grid.Length == 0) return;

    _leavesFlowerGrid = new int[grid.Length];
    for (int i = 0; i < grid.Length; ++i)
    {
      _leavesFlowerGrid[i] = grid[i];
    }

    LeavesFlowers = new GameObject[grid.Length];
  }

  public GameObject SpawnLeavesFlowerAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var leavesFlower = leavesFlowerPool.Get();

    leavesFlower.transform.position = pos;
    LeavesFlowers[index] = leavesFlower;
    LeavesFlowerGrid[index] = 1;
    leavesFlower.GetComponentInChildren<SkeletonAnimation>()
      .GetComponent<MeshRenderer>().sortingOrder = -9 + index / 6;

    return leavesFlower;
  }

  public void RemoveLeavesFlowersAt(int x)
  {
    if (LeavesFlowers == null || LeavesFlowers.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = LeavesFlowers[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;
      if (grillerGrid[index] > 0) continue;

      var leavesFlower = obj.GetComponent<LeavesFlowerControl>();
      if (LeanTween.isTweening(leavesFlower.gameObject)) continue;

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

          leavesFlower.FullyRemoveFromTable();
        })
      );
    }
  }

  public void TryRemoveLeavesFlowerUnder(float3 pos, out int leavesFlowerValue)
  {
    leavesFlowerValue = 0;
    var trayGrid = TrayGrid;
    var index = trayGrid.ConvertWorldPosToIndex(pos);
    if (
      CurtainLayersGrid[index] == 0 &&
      LeavesFlowerGrid != null &&
      LeavesFlowerGrid[index] > 0
    )
    {
      leavesFlowerValue = 19;
      if (LeavesFlowers[index] != null)
      {
        var theme = RendererSystem.Instance.GetCurrentTheme();
        LeavesFlowers[index]
          .GetComponent<LeavesFlowerControl>().RemoveFromTableWithAnim();
      }
    }
  }

  public void ClearLeavesFlowers()
  {
    for (int i = 0; i < LeavesFlowers.Length; ++i)
    {
      var obj = LeavesFlowers[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      var leavesFlower = obj.GetComponent<LeavesFlowerControl>();
      leavesFlower.RemoveFromTable();
    }
  }

  private void OnTouchLeavesFlowerAt(float3 pos)
  {
    var leavesFlower = GetLeavesFlowerAt(pos).GetComponent<LeavesFlowerControl>();


    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, leavesFlower);

      var index = TrayGrid.ConvertWorldPosToIndex(pos);
    }
  }

  public bool HasLeavesFlowerAt(float3 pos)
  {
    var leavesFlower = GetLeavesFlowerAt(pos);

    if (leavesFlower != null)
    {
      return true;
    }

    return false;
  }

  public GameObject GetLeavesFlowerAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > LeavesFlowers.Length - 1) return null;

    return LeavesFlowers[index];
  }

  public List<int> FindEmptyLeavesForPot() // use to spawn grass from plant pot
  {
    List<int> emptyLeaves = new();

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

      emptyLeaves.Add(i);
    }

    return emptyLeaves;
  }

  public void HideLeavesFlowersIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (LeavesFlowers[index] == null) continue;
      LeavesFlowers[index].transform.localScale = float3.zero;
    }
  }

  public void ShowLeavesFlowersIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (LeavesFlowers[index] == null) continue;
      LeavesFlowers[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}