using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class LeavesFlowerControl : MonoBehaviour, IPoolItemControl
{
  ObjectPool<GameObject> leavesFlowerPool;

  public void FullyRemoveFromTable()
  {
  
  }

  public void InjectPool(ObjectPool<GameObject> leavesFlowerPool, ObjectPool<GameObject> other = null)
  {
    this.leavesFlowerPool = leavesFlowerPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      leavesFlowerPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.LeavesFlowerGrid[index] = 0;
    ItemManager.Instance.LeavesFlowers[index] = null;

    Release();
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.LeavesFlowerGrid[index] = 0;
    ItemManager.Instance.LeavesFlowers[index] = null;

    return AnimationManager.Instance.ScaleTo(0, gameObject, .1f,
      () =>
      {
        Release();
      });
  }
}
