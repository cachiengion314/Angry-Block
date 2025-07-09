using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class GrillerControl : MonoBehaviour, IPoolItemControl
{
  [Header("External dependencies")]
  ObjectPool<GameObject> grillerPool;

  public void FullyRemoveFromTable()
  {

    ItemManager.Instance.TryRemoveNeighborGrillers(transform.position, out int grillerValue, out int amountBread, false);
  }

  public void FullyRemoveFromTableWithoutAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    ItemManager.Instance.GrillerGrid[index] = 0;
    ItemManager.Instance.Grillers[index] = null;

    Release();
  }

  public void InjectPool(ObjectPool<GameObject> griller, ObjectPool<GameObject> other = null)
  {
    this.grillerPool = griller;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      grillerPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    skeletonAnimation.AnimationName = "Use_lonuong";

    var duration = skeletonAnimation.Skeleton.Data.FindAnimation("Use_lonuong").Duration / skeletonAnimation.timeScale;
    LeanTween.delayedCall(gameObject, duration / 2,
    () =>
    {
      SoundManager.Instance.PlayClaimBreadSfx();
    });

    LeanTween.delayedCall(gameObject, duration,
    () =>
    {
      skeletonAnimation.AnimationName = "Idle_lonuong";
    });
  }
}
