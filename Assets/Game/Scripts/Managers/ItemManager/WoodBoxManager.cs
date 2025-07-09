using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// WoodBox Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("WoodBox Dependences")]
  [SerializeField] GameObject woodBoxPrefab;

  [Header("WoodBox Pooling")]
  ObjectPool<GameObject> woodBoxPool;

  int[] woodBoxesGrid;
  public int[] WoodBoxesGrid { get { return woodBoxesGrid; } }

  [HideInInspector] public GameObject[] WoodBoxes;

  private void InitWoodBoxPool()
  {
    woodBoxPool = new ObjectPool<GameObject>(
      CreateWoodBoxPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateWoodBoxPool()
  {
    GameObject _obj = Instantiate(woodBoxPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(woodBoxPool);
    return _obj;
  }

  public void SpawnWoodBoxes(int[] grid)
  {
    if (grid.Length == 0) return;

    woodBoxesGrid = new int[grid.Length];
    for (int i = 0; i < grid.Length; ++i)
    {
      woodBoxesGrid[i] = grid[i];
    }

    WoodBoxes = new GameObject[grid.Length];
    for (int i = 0; i < woodBoxesGrid.Length; ++i)
    {
      if (woodBoxesGrid[i] == 0) continue;
      SpawnWoodBoxAt(i, woodBoxesGrid[i]);
    }
  }

  private void SpawnWoodBoxAt(int index, int value)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    if (value == 111) // 111 mean videoAds
    {
      var vid = Instantiate(videoAdsPrefab, tableDepot);

      vid.transform.position = pos;
      WoodBoxes[index] = vid;
      return;
    }

    var woodBox = woodBoxPool.Get();

    woodBox.transform.position = pos;
    woodBox.GetComponent<WoodBoxControl>().ShowStateOfHealthBaseOn(value);
    WoodBoxes[index] = woodBox;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    woodBox.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderLayer;
  }

  public void RemoveWoodBoxesAt(int x)
  {
    if (WoodBoxes == null || WoodBoxes.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = WoodBoxes[index];
      seq.append(.07f);
      if (obj == null) continue;
      if (woodBoxesGrid[index] == 111) continue; // 111 stand for videoAds
      if (curtainLayersGrid[index] > 0) continue;
      var box = obj.GetComponent<WoodBoxControl>();
      if (LeanTween.isTweening(box.gameObject)) continue;

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

          box.FullyRemoveFromTable();
        })
      );
    }
  }

  public void ClearWoodBoxes()
  {
    for (int i = 0; i < WoodBoxes.Length; ++i)
    {
      var obj = WoodBoxes[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      if (WoodBoxesGrid[i] == 111) // AdsVideo
      {
        obj.GetComponent<VideoAdsControl>().SelfDestroy();
        continue;
      }
      var box = obj.GetComponent<WoodBoxControl>();
      box.FullyRemoveFromTable();
    }
  }

  int FindUpWoodBoxValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return woodBoxesGrid[upIndex];
  }

  public void HideWoodBoxesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (WoodBoxes[index] == null) continue;
      WoodBoxes[index].transform.localScale = float3.zero;
    }
  }

  public void ShowWoodBoxesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (WoodBoxes[index] == null) continue;
      WoodBoxes[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }

  public bool HasWoodBoxAt(float3 pos)
  {
    var woodBox = GetWoodBoxAt(pos);

    if (woodBox != null)
    {
      return true;
    }

    return false;
  }

  public bool HasVideoAdsAt(float3 pos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(pos);
    if (index < 0 || index > WoodBoxes.Length - 1) return false;

    if (WoodBoxesGrid[index] == 111) return true;
    return false;
  }

  public GameObject GetWoodBoxAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > WoodBoxes.Length - 1) return null;

    return WoodBoxes[index];
  }
}