using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// CoffeeBoardManager
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("CoffeeBoard Dependences")]
  [SerializeField] GameObject coffeeBoardPrefab;

  [Header("CoffeeBoard Pooling")]
  ObjectPool<GameObject> coffeeBoardPool;

  int[] _coffeeBoardesGrid;
  public int[] CoffeeBoardsGrid { get { return _coffeeBoardesGrid; } }

  [HideInInspector] public GameObject[] CoffeeBoards;

  private void InitCoffeeBoardsPool()
  {
    coffeeBoardPool = new ObjectPool<GameObject>(
      CreateCoffeeBoardsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateCoffeeBoardsPool()
  {
    GameObject _obj = Instantiate(coffeeBoardPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(coffeeBoardPool);
    return _obj;
  }

  public void SpawnCoffeeBoards(int[] grids)
  {
    if (grids.Length == 0) return;

    _coffeeBoardesGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _coffeeBoardesGrid[i] = grids[i];
    }

    CoffeeBoards = new GameObject[grids.Length];
    for (int i = 0; i < _coffeeBoardesGrid.Length; ++i)
    {
      if (_coffeeBoardesGrid[i] == 0) continue;
      SpawnCoffeeBoardAt(i);
    }
  }

  private void SpawnCoffeeBoardAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var coffeeBoard = coffeeBoardPool.Get();

    coffeeBoard.transform.position = pos;
    coffeeBoard.transform.localScale = new float3(1, 1, 1);
    CoffeeBoards[index] = coffeeBoard;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;

    coffeeBoard.GetComponent<CoffeeBoardControl>().ShowCoffeePacks();
    coffeeBoard.GetComponent<CoffeeBoardControl>().SetSortingOrder(orderLayer);
  }

  public void TryRemoveNeighborCoffeeBoards(float3 pos, out int coffeeBoardValue, out int amountCoffeeBoard, bool _hasFindNeighbor = true)
  {
    coffeeBoardValue = 0;
    amountCoffeeBoard = 0;
    var trayGrid = TrayGrid;
    List<GameObject> neighborCoffeeBoards = new();

    if (_hasFindNeighbor)
    {
      neighborCoffeeBoards = FindNeighborCoffeeBoardsAt(pos);
    }
    else
    {
      neighborCoffeeBoards.Add(GetCoffeeBoardAt(pos));
    }

    for (int i = 0; i < neighborCoffeeBoards.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborCoffeeBoards[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        CoffeeBoardsGrid != null &&
        CoffeeBoardsGrid[index] > 0
      )
      {
        coffeeBoardValue = 14;
        amountCoffeeBoard++;

        var theme = RendererSystem.Instance.GetCurrentTheme();

        LeanTween.delayedCall(gameObject, .1f, () =>
        {
          if (CoffeeBoards[index] == null) return;
          SoundManager.Instance.PlayCoffeeBoardSfx();

          EffectManager.Instance.SpawnCoffeeSplashAt(CoffeeBoards[index].transform.position);

          CoffeeBoards[index]
            .GetComponent<CoffeeBoardControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public void RemoveCoffeeBoardAt(int x)
  {
    if (CoffeeBoards == null || CoffeeBoards.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = CoffeeBoards[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var coffeeBoard = obj.GetComponent<CoffeeBoardControl>();
      if (LeanTween.isTweening(coffeeBoard.gameObject)) continue;

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

          coffeeBoard.FullyRemoveFromTable();
        })
      );
    }
  }

  public void ClearCoffeeBoards()
  {
    for (int i = 0; i < CoffeeBoards.Length; ++i)
    {
      var obj = CoffeeBoards[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var coffeeBoard = obj.GetComponent<CoffeeBoardControl>();
      coffeeBoard.RemoveFromTable();
    }
  }

  public GameObject GetCoffeeBoardAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > CoffeeBoards.Length - 1) return null;

    return CoffeeBoards[index];
  }

  public List<GameObject> FindNeighborCoffeeBoardsAt(float3 worldPos)
  {
    List<GameObject> linkedCoffeeBoards = new();
   
    return linkedCoffeeBoards;
  }

  public bool HasCoffeeBoardAt(float3 pos)
  {
    var coffeeBoard = GetCoffeeBoardAt(pos);

    if (coffeeBoard != null)
    {
      return true;
    }

    return false;
  }

  private void OnTouchCoffeeBoardAt(float3 pos)
  {
    var coffeeBoard = GetCoffeeBoardAt(pos).GetComponent<CoffeeBoardControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, coffeeBoard);
    }
  }

  int FindUpCoffeeBoardValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _coffeeBoardesGrid[upIndex];
  }

  public void HideCoffeeBoardsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (CoffeeBoards[index] == null) continue;
      CoffeeBoards[index].transform.localScale = float3.zero;
    }
  }

  public void ShowCoffeeBoardsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (CoffeeBoards[index] == null) continue;

      CoffeeBoards[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}