using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public partial class ItemManager : MonoBehaviour
{
  [Header("CoverLet Dependences")]
  [SerializeField] GameObject coverLetPrefab;

  [Header("CoverLet Pooling")]
  ObjectPool<GameObject> coverLetPool;

  int[] coverLetsGrid;
  public int[] CoverLetsGrid { get { return coverLetsGrid; } }

  [HideInInspector] public GameObject[] CoverLets;

  private void InitCoverLetPool()
  {
    coverLetPool = new ObjectPool<GameObject>(
        CreateCoverLetPool,
        OnTakeObjFromPool,
        OnReturnObjFromPool,
        OnDestroyPoolObj,
        true,
        trayGrid.GridSize.x * trayGrid.GridSize.y,
        trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateCoverLetPool()
  {
    GameObject _obj = Instantiate(coverLetPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(coverLetPool);
    return _obj;
  }

  public void SpawnCoverLets(int[] grid)
  {
    if (grid.Length == 0) return;

    coverLetsGrid = new int[grid.Length];
    for (int i = 0; i < grid.Length; ++i)
    {
      coverLetsGrid[i] = grid[i];
    }

    CoverLets = new GameObject[grid.Length];
    for (int i = 0; i < coverLetsGrid.Length; ++i)
    {
      if (coverLetsGrid[i] == 0) continue;
      SpawnCoverLetAt(i, coverLetsGrid[i]);
    }
  }

  private void SpawnCoverLetAt(int index, int value)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);
    var coverLet = coverLetPool.Get();

    coverLet.transform.position = pos;
    CoverLets[index] = coverLet;

    var gridPos = trayGrid.ConvertIndexToGridPos(index);
    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20 + gridPos.x;
    orderLayer += 1;
    coverLet.GetComponentInChildren<SkeletonAnimation>().GetComponent<MeshRenderer>().sortingOrder = orderLayer;
  }

  public void TryRemoveNeighborCoverLets(float3 pos)
  {
    var trayGrid = TrayGrid;
    List<GameObject> neighborCoverLet = FindCoverLetNeighborTraysAt(pos);
    for (int i = 0; i < neighborCoverLet.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborCoverLet[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        CoverLetsGrid != null &&
        CoverLetsGrid[index] > 0
      )
      {
        LeanTween.delayedCall(gameObject, .8f, () =>
        {
          if (CoverLets[index] == null) return;

          CoverLets[index]
            .GetComponent<CoverLetControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public void RemoveCoverLetsAt(int x)
  {
    if (CoverLets == null || CoverLets.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = CoverLets[index];
      seq.append(.07f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;
      var coverLet = obj.GetComponent<CoverLetControl>();
      if (LeanTween.isTweening(coverLet.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { Destroy(boomSkeleton.gameObject); });

          coverLet.FullyRemoveFromTable();
        })
      );
    }
  }

  public GameObject GetCoverLetAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > CoverLets.Length - 1) return null;

    return CoverLets[index];
  }

  public bool HasCoverLetAt(float3 pos)
  {
    var coverLet = GetCoverLetAt(pos);

    if (coverLet != null)
    {
      return true;
    }

    return false;
  }

  public void ClearCoverLets()
  {
    for (int i = 0; i < CoverLets.Length; ++i)
    {
      var obj = CoverLets[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      var coverLet = obj.GetComponent<CoverLetControl>();
      coverLet.FullyRemoveFromTable();
    }
  }

  private void OnTouchCoverLetAt(float3 pos)
  {
    var coverLet = GetCoverLetAt(pos).GetComponent<CoverLetControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      var trayControl = FindCoverLetTray(pos);
      trayControl?.FullyRemoveFromTable();

      TrySelfRemovedByHammerAt(pos, coverLet);
    }
  }

  public void HideCoverLetsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (CoverLets[index] == null) continue;
      CoverLets[index].transform.localScale = float3.zero;
    }
  }

  public void ShowCoverLetsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (CoverLets[index] == null) continue;

      CoverLets[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}