using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class MachineCreamControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal Dependencies")]
  [SerializeField] SkeletonAnimation skeAnim;

  ObjectPool<GameObject> machineCreamPool;
  private string[] _nameState = new string[] { "create_new", "Idle", "processing" };

  public void InjectPool(ObjectPool<GameObject> machineCreamPool, ObjectPool<GameObject> other = null)
  {
    this.machineCreamPool = machineCreamPool;
  }

  public void SetSortingOrder(int sortingOrder)
  {
    skeAnim.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
  }

  public void ShowStateOfHealthBaseOn(int value, bool isShowText)
  {
    if (DOTween.IsTweening(transform))
    {
      var theme = RendererSystem.Instance.GetCurrentTheme();

      if (isShowText)
      {
        
      }

      return;
    }

    if (value == 0)
    {
      skeAnim.AnimationState.SetAnimation(0, _nameState[2], false);
      var duration = skeAnim.Skeleton.Data.FindAnimation(_nameState[2]).Duration;
      var duration2 = skeAnim.Skeleton.Data.FindAnimation(_nameState[1]).Duration;

      DOVirtual.DelayedCall(duration * 0.6f,
        () =>
        {
          var theme = RendererSystem.Instance.GetCurrentTheme();

          if (isShowText)
          {
         
          }
        }
      ).SetTarget(transform); ;

      DOVirtual.DelayedCall(duration,
        () =>
        {
          skeAnim.AnimationState.SetAnimation(0, _nameState[0], false);
        }
      ).SetTarget(transform);

      DOVirtual.DelayedCall(duration + duration2,
        () =>
        {
          skeAnim.AnimationState.SetAnimation(0, _nameState[1], true);
        }
      ).SetTarget(transform);

      return;
    }

    if (value == 1)
    {
      skeAnim.AnimationState.SetAnimation(0, _nameState[1], true);
    }
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      machineCreamPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.MachineCreamsGrid[index] = 0;
    ItemManager.Instance.MachineCreams[index] = null;

    Release();
  }

  public void FullyRemoveFromTable()
  {
  
    ItemManager.Instance.TryRemoveNeighborMachineCreams(transform.position, out int machineCreamValue, out int amountMachineCream, false);

    var isFullfil = ItemManager.Instance.IsFullfilQuestWith(
      float3.zero,
      0, 0, 0, 0, 0, 0, 0, machineCreamValue, 0, 0,
      0, 0, 0, 0, amountMachineCream, 0,
      (fullfilValue, amount) =>
      {
       
      }
    );
  }
}
