using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// MoneyBag Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("MoneyBag Dependencies")]
  [SerializeField] GameObject moneyBagPref;

  [Header("MoneyBag Pooling")]
  ObjectPool<GameObject> moneyBagPool;

  int[] _moneyBagsGrid;
  public int[] MoneyBagsGrid { get { return _moneyBagsGrid; } }

  [HideInInspector] public GameObject[] MoneyBags;

  private void InitMoneyBagsPool()
  {
    moneyBagPool = new ObjectPool<GameObject>(
      CreateMoneyBagsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateMoneyBagsPool()
  {
    GameObject _obj = Instantiate(moneyBagPref, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(moneyBagPool);
    return _obj;
  }

  public void SpawnMoneyBags(int[] grids)
  {
    if (grids.Length == 0) return;

    _moneyBagsGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _moneyBagsGrid[i] = grids[i];
    }

    MoneyBags = new GameObject[grids.Length];
    for (int i = 0; i < _moneyBagsGrid.Length; ++i)
    {
      if (_moneyBagsGrid[i] == 0) continue;
      SpawnMoneyBagAt(i);
    }
  }

  private void SpawnMoneyBagAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var moneyBag = moneyBagPool.Get();

    moneyBag.transform.position = pos;
    moneyBag.transform.localScale = new float3(1, 1, 1);
    MoneyBags[index] = moneyBag;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    moneyBag.GetComponent<MoneyBagControl>().SetSortingOrder(orderLayer);
    moneyBag.GetComponent<MoneyBagControl>().ShowStateOfHealthBaseOn(2);
  }

  public void RemoveMoneyBagAt(int x)
  {
    if (MoneyBags == null || MoneyBags.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = MoneyBags[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var moneyBag = obj.GetComponent<MoneyBagControl>();
      if (LeanTween.isTweening(moneyBag.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { Destroy(boomSkeleton.gameObject); });

          moneyBag.FullyRemoveFromTable();
        })
      );
    }
  }

  public void TryRemoveNeighborMoneyBags(float3 pos, out int moneyBagValue, out int amountMoneyBag, bool _hasFindNeighbor = true)
  {
    moneyBagValue = 0;
    amountMoneyBag = 0;
    var trayGrid = TrayGrid;
    List<GameObject> neighborMoneyBags = new();

    if (_hasFindNeighbor)
    {
      neighborMoneyBags = FindNeighborMoneyBagsAt(pos);
    }
    else
    {
      neighborMoneyBags.Add(GetMoneyBagAt(pos));
    }

    for (int i = 0; i < neighborMoneyBags.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborMoneyBags[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        MoneyBagsGrid != null &&
        MoneyBagsGrid[index] > 0
      )
      {
        // if (GiftBoxesGrid[index] == 1)
        // {
        //   moneyBagValue = 13;
        //   amountMoneyBag++;

        //   var theme = RendererSystem.Instance.GetCurrentTheme();
        //   var isDone = LevelProgressBlock.Instance.IsGiftBoxQuestDone(moneyBagValue);
        //   if (!isDone)
        //   {
        //     ShowPanel.Instance.ShowTextAt(
        //       MoneyBags[index].transform.position, "+1", Color.white, theme.giftBoxRenderer, 1.1f
        //     );
        //   }
        // }

        LeanTween.delayedCall(gameObject, .1f, () =>
        {
          if (MoneyBags[index] == null) return;
          SoundManager.Instance.PlayMoneyBagSfx();

          MoneyBags[index]
            .GetComponent<MoneyBagControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public void ClearMoneyBags()
  {
    for (int i = 0; i < MoneyBags.Length; ++i)
    {
      var obj = MoneyBags[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var moneyBag = obj.GetComponent<MoneyBagControl>();
      moneyBag.RemoveFromTable();
    }
  }

  public GameObject GetMoneyBagAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > MoneyBags.Length - 1) return null;

    return MoneyBags[index];
  }

  public List<GameObject> FindNeighborMoneyBagsAt(float3 worldPos)
  {
    List<GameObject> linkedMoneyBags = new();


    return linkedMoneyBags;
  }

  public bool HasMoneyBagAt(float3 pos)
  {
    var moneyBag = GetMoneyBagAt(pos);

    if (moneyBag != null)
    {
      return true;
    }

    return false;
  }

  private void OnTouchMoneyBagAt(float3 pos)
  {
    var moneyBag = GetMoneyBagAt(pos).GetComponent<MoneyBagControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, moneyBag);
    }
  }

  int FindUpMoneyBagValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _moneyBagsGrid[upIndex];
  }

  public void HideMoneyBagsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (MoneyBags[index] == null) continue;
      MoneyBags[index].transform.localScale = float3.zero;
    }
  }

  public void ShowMoneyBagsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (MoneyBags[index] == null) continue;
      MoneyBags[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}