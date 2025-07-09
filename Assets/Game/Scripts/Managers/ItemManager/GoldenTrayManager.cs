using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public partial class ItemManager : MonoBehaviour
{
  [Header("GoldenTray Dependences")]
  [SerializeField] GameObject goldenTrayPrefab;

  [Header("GoldenTray Pooling")]
  ObjectPool<GameObject> goldenTrayPool;

  int[] _goldenTrayGrid;
  public int[] GoldenTrayGrid { get { return _goldenTrayGrid; } }

  [HideInInspector] public GameObject[] GoldenTrays;

  private void InitGoldenTraysPool()
  {
    goldenTrayPool = new ObjectPool<GameObject>(
      CreateGoldenTraysPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateGoldenTraysPool()
  {
    GameObject _obj = Instantiate(goldenTrayPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(goldenTrayPool);
    return _obj;
  }

  public void SpawnGoldenTrays(int[] grids)
  {
    if (grids.Length == 0) return;

    _goldenTrayGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _goldenTrayGrid[i] = grids[i];
    }

    GoldenTrays = new GameObject[grids.Length];
    for (int i = 0; i < _goldenTrayGrid.Length; ++i)
    {
      if (_goldenTrayGrid[i] == 0) continue;
      SpawnGoldenTrayAt(i);
    }
  }

  private GameObject SpawnGoldenTrayAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var goldenTray = goldenTrayPool.Get();

    goldenTray.transform.position = pos;
    GoldenTrays[index] = goldenTray;
    GoldenTrayGrid[index] = 1;

    return goldenTray;
  }

  public void ClearGoldenTrays()
  {
    for (int i = 0; i < GoldenTrays.Length; ++i)
    {
      var obj = GoldenTrays[i];
      if (obj == null) continue;
      if (LeanTween.isTweening(obj)) continue;

      var goldenTray = obj.GetComponent<GoldenTrayControl>();
      goldenTray.RemoveFromTable();
    }
  }

  public GameObject GetGoldenTrayAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > GoldenTrays.Length - 1) return null;

    return GoldenTrays[index];
  }

  public void TryRemoveGoldenTrayAt(float3 pos, out int goldenTrayValue)
  {
    goldenTrayValue = 0;
    var index = trayGrid.ConvertWorldPosToIndex(pos);
    var theme = RendererSystem.Instance.GetCurrentTheme();

    if (HasGoldenTrayAt(pos))
    {
      goldenTrayValue = 15;
     

  
    }
  }

  public bool HasGoldenTrayAt(float3 pos)
  {
    var goldenTray = GetGoldenTrayAt(pos);

    if (goldenTray != null)
    {
      return true;
    }

    return false;
  }

  public void HideGoldenTraysIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (GoldenTrays[index] == null) continue;
      GoldenTrays[index].transform.localScale = float3.zero;
    }
  }

  public void ShowGoldenTraysIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (GoldenTrays[index] == null) continue;
      GoldenTrays[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}