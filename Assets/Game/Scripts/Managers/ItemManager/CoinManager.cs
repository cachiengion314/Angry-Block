using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Coin Manager
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("Coin Dependences")]
  [SerializeField] GameObject coinPrefab;

  [Header("Coin Pooling")]
  ObjectPool<GameObject> coinPool;

  [HideInInspector] public GameObject[] Coins;

  private void InitCoinPool()
  {
    coinPool = new ObjectPool<GameObject>(
      CreateCoinPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateCoinPool()
  {
    GameObject _obj = Instantiate(coinPrefab, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(coinPool);
    return _obj;
  }

  public GameObject SpawnCoinAt(float3 pos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(pos);
    if (Coins[index]) return Coins[index];

    var coin = coinPool.Get();
    coin.GetComponent<CoinControl>().InjectIndex(index);

    coin.transform.position = pos;
    Coins[index] = coin;
    return coin;
  }

  public void ClearCoins()
  {
    for (int i = 0; i < Coins.Length; ++i)
    {
      var obj = Coins[i];
      if (obj == null) continue;

      var coin = obj.GetComponent<CoinControl>();
      coin.RemoveFromTable();
    }
  }
}