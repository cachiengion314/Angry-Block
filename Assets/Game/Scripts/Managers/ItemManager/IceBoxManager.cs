using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// IceBox Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("IceBox Dependences")]
  [SerializeField] GameObject iceBoxPrefab;

  [Header("IceBox Pooling")]
  ObjectPool<GameObject> iceBoxPool;

  int[] iceBoxesGrid;
  public int[] IceBoxesGrid { get { return iceBoxesGrid; } }

  [HideInInspector] public GameObject[] IceBoxes;

  private void InitIceBoxPool()
  {
    iceBoxPool = new ObjectPool<GameObject>(
        CreateIceBoxPool,
        OnTakeObjFromPool,
        OnReturnObjFromPool,
        OnDestroyPoolObj,
        true,
        trayGrid.GridSize.x * trayGrid.GridSize.y,
        trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateIceBoxPool()
  {
    GameObject _obj = Instantiate(iceBoxPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(iceBoxPool);
    return _obj;
  }

  public void SpawnIceBoxes(int[] grid)
  {
    if (grid.Length == 0) return;

    iceBoxesGrid = new int[grid.Length];
    for (int i = 0; i < grid.Length; ++i)
    {
      iceBoxesGrid[i] = grid[i];
    }

    IceBoxes = new GameObject[grid.Length];
    for (int i = 0; i < iceBoxesGrid.Length; ++i)
    {
      if (iceBoxesGrid[i] == 0) continue;
      SpawnIceBoxAt(i, iceBoxesGrid[i]);
    }
  }

  private void SpawnIceBoxAt(int index, int value)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var iceBox = iceBoxPool.Get();

    iceBox.transform.position = pos;
    iceBox.GetComponent<IceBoxControl>().ShowStateOfHealthBaseOn(value);
    IceBoxes[index] = iceBox;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    iceBox.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderLayer;
    iceBox.transform.GetChild(1).GetComponent<MeshRenderer>().sortingOrder = orderLayer;
  }

  public void RemoveIceBoxesAt(int x)
  {
    if (IceBoxes == null || IceBoxes.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = IceBoxes[index];
      seq.append(.07f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;
      var box = obj.GetComponent<IceBoxControl>();
      if (LeanTween.isTweening(box.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { Destroy(boomSkeleton.gameObject); });

          box.FullyRemoveFromTable();
        })
      );
    }
  }

  public void TryRemoveNeighborIceBoxes(float3 pos)
  {
    var trayGrid = TrayGrid;
    List<GameObject> neighborIceBox = FindIceNeighborTraysAt(pos);
    for (int i = 0; i < neighborIceBox.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborIceBox[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        IceBoxesGrid != null &&
        IceBoxesGrid[index] > 0
      )
      {
        LeanTween.delayedCall(gameObject, .8f, () =>
        {
          if (IceBoxes[index] == null) return;
          SoundManager.Instance.PlayIceDestroySfx();

          IceBoxes[index]
            .GetComponent<IceBoxControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public GameObject GetIceBoxValueAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > IceBoxes.Length - 1) return null;
    // Duy : Check ice trays
    // var currentLevelDesign = LevelManager.Instance.GetCurrentLevelDesign();
    return IceBoxes[index];
  }

  public void ClearIceBoxes()
  {
    for (int i = 0; i < IceBoxes.Length; ++i)
    {
      var obj = IceBoxes[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      var box = obj.GetComponent<IceBoxControl>();
      box.FullyRemoveFromTable();
    }
  }

  public bool HasIceBoxAt(float3 worldPos)
  {
    var iceBox = GetIceBoxValueAt(worldPos);
    if (iceBox == null) return false;

    return true;
  }

  public void HideIceBoxesIn(int[] indexes)
  {
    if (IceBoxes == null || IceBoxes.Length == 0) return;

    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (IceBoxes[index] == null) continue;
      IceBoxes[index].transform.localScale = float3.zero;
    }
  }

  public void ShowIceBoxesIn(int[] indexes)
  {
    if (IceBoxes == null || IceBoxes.Length == 0) return;

    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (IceBoxes[index] == null) continue;
      IceBoxes[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}