using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

enum BirdStatus
{
  Closed,
  Open
}

public class MagicNestControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal Dependencies")]
  [SerializeField] SkeletonAnimation skeMagicNest;

  [Header("External dependencies")]
  ObjectPool<GameObject> magicNestPool;

  private BirdStatus _birdStatus;

  public void InitMagicNest()
  {
    _birdStatus = BirdStatus.Closed;
    skeMagicNest.AnimationState.SetAnimation(0, "out", false);
  }

  public void FullyRemoveFromTable()
  {
    if (IsOpening())
    {
  
    }

    ChangeStatus();
  }

  public void InjectPool(ObjectPool<GameObject> magicNestPool, ObjectPool<GameObject> other = null)
  {
    this.magicNestPool = magicNestPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      magicNestPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.MagicNestsGrid[index] = 0;
    ItemManager.Instance.MagicNests[index] = null;

    Release();
  }

  public void SetSortingOrder(int sortingOrder)
  {
    skeMagicNest.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
  }

  public void ChangeStatus()
  {
    DOTween.Kill(transform);

    if (_birdStatus == BirdStatus.Closed)
    {
      _birdStatus = BirdStatus.Open;
      skeMagicNest.AnimationState.SetAnimation(0, "in", false);

      var duration = skeMagicNest.Skeleton.Data.FindAnimation("in").Duration;
      DOVirtual.DelayedCall(duration,
        () =>
        {
          skeMagicNest.AnimationState.SetAnimation(0, "Idle", true);
        }
      ).SetTarget(transform);

      return;
    }

    _birdStatus = BirdStatus.Closed;
    skeMagicNest.AnimationState.SetAnimation(0, "out", false);
  }

  public bool IsOpening()
  {
    return _birdStatus == BirdStatus.Open;
  }
}