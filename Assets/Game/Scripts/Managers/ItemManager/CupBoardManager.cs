using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// CupBoard Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("CupBoard Dependencies")]
  [SerializeField] GameObject cupBoardPref;

  [Header("MoneyBag Pooling")]
  ObjectPool<GameObject> cupBoardPool;

  int[] _cupBoardsGrid;
  public int[] CupBoardsGrid { get { return _cupBoardsGrid; } }

  [HideInInspector] public List<CupBoardControl> CupBoards = new();

  private void InitCupBoardsPool()
  {
    cupBoardPool = new ObjectPool<GameObject>(
      CreateCupBoardsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );

    _cupBoardsGrid = new int[24];
  }

  private GameObject CreateCupBoardsPool()
  {
    GameObject _obj = Instantiate(cupBoardPref, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(cupBoardPool);
    return _obj;
  }

  public void SpawnCupBoards(CupBoardData[] cupBoardDatas)
  {
    _cupBoardsGrid = new int[24];

    for (int i = 0; i < cupBoardDatas.Length; i++)
    {
      var cupBoardData = cupBoardDatas[i];
      SpawnCupBoard(cupBoardData);
    }
  }

  private void SpawnCupBoard(CupBoardData cupBoardData)
  {
    var pos = new float3(0, 0, 0);
    foreach (var id in cupBoardData.PosCellInLayer)
    {
      var posCell = trayGrid.ConvertIndexToWorldPos(id);
      pos += posCell;
    }
    pos /= cupBoardData.PosCellInLayer.Length;

    var cupBoard = cupBoardPool.Get();
    cupBoard.transform.position = pos;
    cupBoard.transform.localScale = new float3(1, 1, 1);

    var cupBoardControl = cupBoard.GetComponent<CupBoardControl>();
    var indexToCalculateSortingLayer = cupBoardData.IndexToCalculateSortingLayer;
    int orderLayer = trayGrid.GridSize.y * 20 - indexToCalculateSortingLayer % trayGrid.GridSize.y * 20;
    orderLayer += 1;

    cupBoardControl.InitFrom(cupBoardData);
    cupBoardControl.SetSortingOrder(orderLayer);
    CupBoards.Add(cupBoardControl);

    InitCupBoard(cupBoardData.PosCellInLayer);
  }

  private void InitCupBoard(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; i++)
    {
      CupBoardsGrid[indexes[i]] = 1;
    }
  }

  public void RemoveCupBoard(CupBoardControl cupBoardControl)
  {
    var indexes = cupBoardControl.PosCellInLayer;

    for (int i = 0; i < indexes.Count; i++)
    {
      CupBoardsGrid[indexes[i]] = 0;
    }

    CupBoards.Remove(cupBoardControl);
  }

  public void RemoveCupBoardsAt(int x)
  {
    if (CupBoards == null || CupBoards.Count == 0) return;

    var seq = LeanTween.sequence();
    List<CupBoardControl> cupBoardNeedRemoves = new();

    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var index = trayGrid.ConvertGridPosToIndex(new int2(x, y));
      if (_cupBoardsGrid[index] == 0) continue;

      var pos = trayGrid.ConvertGridPosToWorldPos(new int2(x, y));
      var cupBoard = FindCupBoardWith(pos);
      if (cupBoard == null) continue;
      if (cupBoardNeedRemoves.Contains(cupBoard)) continue;

      cupBoardNeedRemoves.Add(cupBoard);
    }

    for (int i = 0; i < cupBoardNeedRemoves.Count; i++)
    {
      var cupBoard = cupBoardNeedRemoves[i];
      cupBoard.FullyRemoveFromTable();
    }
  }

  public void TryRemoveNeighborCupBoards(float3 pos, out int cupBoardValue, out int amountcupBoard, bool _hasFindNeighbor = true)
  {
    cupBoardValue = 0;
    amountcupBoard = 0;
    var trayGrid = TrayGrid;
    List<GameObject> neighborCupBoards = new();

    if (_hasFindNeighbor)
    {
      neighborCupBoards = FindNeighborCupBoardsAt(pos);
    }
    else
    {
      neighborCupBoards.Add(FindCupBoardWith(pos).gameObject);
    }

    for (int i = 0; i < neighborCupBoards.Count; ++i)
    {
      var cupBoard = neighborCupBoards[i];
      var index = trayGrid.ConvertWorldPosToIndex(neighborCupBoards[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        CupBoardsGrid != null &&
        CupBoardsGrid[index] > 0
      )
      {
        cupBoardValue = 16;
        amountcupBoard++;

        var theme = RendererSystem.Instance.GetCurrentTheme();


        LeanTween.delayedCall(gameObject, .1f, () =>
        {
          if (cupBoard == null) return;
          SoundManager.Instance.PlayCupBoardSfx();

          EffectManager.Instance.SpawnCupBreakSplashAt(cupBoard.transform.position);

          cupBoard
            .GetComponent<CupBoardControl>().RemoveFromTableWithAnim();
        });
      }
    }
  }

  public void ClearCupBoards()
  {
    for (int i = 0; i < CupBoards.Count; ++i)
    {
      var obj = CupBoards[i];
      if (obj == null) continue;

      var cupBoard = obj.GetComponent<CupBoardControl>();
      cupBoard.RemoveFromTable();
    }
  }

  public List<GameObject> FindNeighborCupBoardsAt(float3 worldPos)
  {
    List<GameObject> linkedCupBoards = new();

    return linkedCupBoards;
  }

  public bool HasCupBoardAt(float3 pos)
  {
    var cupBoard = FindCupBoardWith(pos);

    if (cupBoard != null)
    {
      return true;
    }

    return false;
  }

  public void OnTouchCupBoardAt(float3 pos)
  {
    var cupBoard = FindCupBoardWith(pos);

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, cupBoard);
    }
  }

  public CupBoardControl FindCupBoardWith(float3 worldPos)
  {
    var gridPos = trayGrid.ConvertWorldPosToGridPos(worldPos);
    if (trayGrid.IsGridPosOutsideAt(gridPos)) return null;

    var id = trayGrid.ConvertGridPosToIndex(gridPos);

    for (int i = 0; i < CupBoards.Count; i++)
    {
      for (int j = 0; j < CupBoards[i].PosCellInLayer.Count; j++)
      {
        if (CupBoards[i].PosCellInLayer[j] == id)
        {
          return CupBoards[i];
        }
      }
    }

    return null;
  }

  int FindUpCupBoardValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);

    return _cupBoardsGrid[upIndex];
  }

  public void HideCupBoardsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      var cupBoard = FindCupBoardWith(trayGrid.ConvertIndexToWorldPos(index));

      if (cupBoard == null) continue;

      cupBoard.transform.localScale = float3.zero;
    }
  }

  public void ShowCupBoardsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      var cupBoard = FindCupBoardWith(trayGrid.ConvertIndexToWorldPos(index));

      if (cupBoard == null) continue;
      cupBoard.transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}