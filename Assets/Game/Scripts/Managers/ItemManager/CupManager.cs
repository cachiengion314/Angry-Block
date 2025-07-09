using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Cup Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("Cup Dependences")]
  [SerializeField] GameObject cupPrefab;

  [Header("Cup Pooling")]
  ObjectPool<GameObject> cupPool;

  private void InitCupPool()
  {
    cupPool = new ObjectPool<GameObject>(
      CreateCupPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      cupGrid.GridSize.x * cupGrid.GridSize.y,
      cupGrid.GridSize.x * cupGrid.GridSize.y
    );
  }

  GameObject CreateCupPool()
  {
    GameObject _obj = Instantiate(cupPrefab, transform.position, transform.rotation, tableDepot);
    return _obj;
  }
}