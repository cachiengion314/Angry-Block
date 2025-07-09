using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// MagicNest Manager
/// </summary> 
public partial class ItemManager : MonoBehaviour
{
  [Header("MagicNest Dependencies")]
  [SerializeField] GameObject magicNestPref;

  [Header("MoneyBag Pooling")]
  ObjectPool<GameObject> magicNestPool;

  int[] _magicNestsGrid;
  public int[] MagicNestsGrid { get { return _magicNestsGrid; } }

  [HideInInspector] public GameObject[] MagicNests;

  private void InitMagicNestsPool()
  {
    magicNestPool = new ObjectPool<GameObject>(
      CreateMagicNestsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateMagicNestsPool()
  {
    GameObject _obj = Instantiate(magicNestPref, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(magicNestPool);
    return _obj;
  }

  public void SpawnMagicNestsWith(int[] grids)
  {
    if (grids.Length == 0) return;

    _magicNestsGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _magicNestsGrid[i] = grids[i];
    }

    MagicNests = new GameObject[grids.Length];
    for (int i = 0; i < _magicNestsGrid.Length; ++i)
    {
      if (_magicNestsGrid[i] == 0) continue;
      SpawnMagicNestAt(i);
    }
  }

  private void SpawnMagicNestAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var magicNest = magicNestPool.Get();

    magicNest.transform.position = pos;
    magicNest.transform.localScale = new float3(1, 1, 1);
    MagicNests[index] = magicNest;
    magicNest.GetComponent<MagicNestControl>().InitMagicNest();

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    magicNest.GetComponent<MagicNestControl>().SetSortingOrder(orderLayer);
  }

  public void ClearMagicNests()
  {
    for (int i = 0; i < MagicNests.Length; ++i)
    {
      var obj = MagicNests[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      var magicNest = obj.GetComponent<MagicNestControl>();
      magicNest.RemoveFromTable();
    }
  }

  public GameObject GetMagicNestAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > MagicNests.Length - 1) return null;

    return MagicNests[index];
  }

  public void TryRemoveNeighborMagicNests(float3 pos, out int magicNestValue, out int amountMagicNest, bool _hasFindNeighbor = true)
  {
    magicNestValue = 0;
    amountMagicNest = 0;
    var trayGrid = TrayGrid;
    List<GameObject> neighborMagicNests = new();

    if (_hasFindNeighbor)
    {
      neighborMagicNests = FindNeighborMagicNestsAt(pos);
    }
    else
    {
      neighborMagicNests.Add(GetMagicNestAt(pos));
    }

    for (int i = 0; i < neighborMagicNests.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborMagicNests[i].transform.position);
      var magicNest = MagicNests[index].GetComponent<MagicNestControl>();

      if (
        CurtainLayersGrid[index] == 0 &&
        MagicNestsGrid != null &&
        MagicNestsGrid[index] > 0
      )
      {
        if (!magicNest.IsOpening()) return;

        magicNestValue = 18;
        amountMagicNest++;

        if (MagicNests[index] == null) return;
        SoundManager.Instance.PlayMagicNestSfx();

        EffectManager.Instance.SpawnWoodSplashAt(magicNest.transform.position);
      }
    }
  }

  public void RemoveMagicNestAt(int x)
  {
    if (MagicNests == null || MagicNests.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = MagicNests[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var magicNest = obj.GetComponent<MagicNestControl>();
      if (LeanTween.isTweening(magicNest.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { Destroy(boomSkeleton.gameObject); });

          magicNest.FullyRemoveFromTable();
        })
      );
    }
  }

  public List<GameObject> FindNeighborMagicNestsAt(float3 worldPos)
  {
    List<GameObject> linkedMagicNests = new();


    return linkedMagicNests;
  }

  private void OnTouchMagicNestAt(float3 pos)
  {
    var magicNest = GetMagicNestAt(pos).GetComponent<MagicNestControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, magicNest);
    }
  }

  int FindUpMagicNestValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _magicNestsGrid[upIndex];
  }

  public bool HasMagicNestAt(float3 pos)
  {
    var magicNest = GetMagicNestAt(pos);

    if (magicNest != null)
    {
      return true;
    }

    return false;
  }

  public void HideMagicNestsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (MagicNests[index] == null) continue;
      MagicNests[index].transform.localScale = float3.zero;
    }
  }

  public void ShowMagicNestsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (MagicNests[index] == null) continue;
      MagicNests[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }

  public void ChangeMagicNestStatus()
  {
    for (int i = 0; i < MagicNests.Length; i++)
    {
      var worldPos = trayGrid.ConvertIndexToWorldPos(i);
      var magicNest = GetMagicNestAt(worldPos);

      if (magicNest == null) continue;
      magicNest.GetComponent<MagicNestControl>().ChangeStatus();
    }
  }
}