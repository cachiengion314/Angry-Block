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
  [Header("GiftBox Dependences")]
  [SerializeField] GameObject giftBoxPrefab;

  [Header("GiftBox Pooling")]
  ObjectPool<GameObject> giftBoxPool;

  int[] _giftBoxesGrid;
  public int[] GiftBoxesGrid { get { return _giftBoxesGrid; } }

  [HideInInspector] public GameObject[] GiftBoxes;

  private void InitGiftBoxesPool()
  {
    giftBoxPool = new ObjectPool<GameObject>(
      CreateGiftBoxesPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateGiftBoxesPool()
  {
    GameObject _obj = Instantiate(giftBoxPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(giftBoxPool);
    return _obj;
  }

  public void SpawnGiftBoxes(int[] grids)
  {
    if (grids.Length == 0) return;

    _giftBoxesGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _giftBoxesGrid[i] = grids[i];
    }

    GiftBoxes = new GameObject[grids.Length];
    for (int i = 0; i < _giftBoxesGrid.Length; ++i)
    {
      if (_giftBoxesGrid[i] == 0) continue;
      SpawnGiftBoxAt(i);
    }
  }

  private void SpawnGiftBoxAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var giftBox = giftBoxPool.Get();

    giftBox.transform.position = pos;
    giftBox.transform.localScale = new float3(1, 1, 1);
    GiftBoxes[index] = giftBox;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    giftBox.GetComponentInChildren<SpriteRenderer>().sortingOrder = orderLayer;
    giftBox.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Box1", false);
    giftBox.GetComponentInChildren<SkeletonAnimation>().GetComponent<MeshRenderer>().sortingOrder = orderLayer;
  }


  public void RemoveGiftBoxAt(int x)
  {
    if (GiftBoxes == null || GiftBoxes.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = GiftBoxes[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var giftBox = obj.GetComponent<GiftBoxControl>();
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

          giftBox.FullyRemoveFromTable();
        })
      );
    }
  }

  public void TryRemoveNeighborGiftBoxes(float3 pos, out int giftBoxValue, out int amountGiftBox, bool _hasFindNeighbor = true)
  {
    giftBoxValue = 0;
    amountGiftBox = 0;
    var trayGrid = TrayGrid;
    List<GameObject> neighborGiftBoxes = new();

    if (_hasFindNeighbor)
    {
      neighborGiftBoxes = FindNeighborGiftBoxesAt(pos);
    }
    else
    {
      neighborGiftBoxes.Add(GetGiftBoxAt(pos));
    }

    for (int i = 0; i < neighborGiftBoxes.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborGiftBoxes[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        GiftBoxesGrid != null &&
        GiftBoxesGrid[index] > 0
      )
      {
        if (GiftBoxesGrid[index] == 1)
        {
          giftBoxValue = 13;
          amountGiftBox++;

          var theme = RendererSystem.Instance.GetCurrentTheme();

        }

        LeanTween.delayedCall(gameObject, .1f, () =>
        {
          if (GiftBoxes[index] == null) return;
          SoundManager.Instance.PlayGiftBoxSfx();

          GiftBoxes[index]
            .GetComponent<GiftBoxControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public void ClearGiftBoxes()
  {
    for (int i = 0; i < GiftBoxes.Length; ++i)
    {
      var obj = GiftBoxes[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var giftBox = obj.GetComponent<GiftBoxControl>();
      giftBox.RemoveFromTable();
    }
  }

  public GameObject GetGiftBoxAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > GiftBoxes.Length - 1) return null;

    return GiftBoxes[index];
  }

  public List<GameObject> FindNeighborGiftBoxesAt(float3 worldPos)
  {
    List<GameObject> linkedGiftBoxes = new();
   
    return linkedGiftBoxes;
  }

  public bool HasGiftBoxAt(float3 pos)
  {
    var giftBox = GetGiftBoxAt(pos);

    if (giftBox != null)
    {
      return true;
    }

    return false;
  }

  private void OnTouchGiftBoxAt(float3 pos)
  {
    var giftBox = GetGiftBoxAt(pos).GetComponent<GiftBoxControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, giftBox);
    }
  }

  int FindUpGiftBoxValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _giftBoxesGrid[upIndex];
  }

  public void HideGiftBoxesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (GiftBoxes[index] == null) continue;
      GiftBoxes[index].transform.localScale = float3.zero;
    }
  }

  public void ShowGiftBoxesIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (GiftBoxes[index] == null) continue;
      GiftBoxes[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}