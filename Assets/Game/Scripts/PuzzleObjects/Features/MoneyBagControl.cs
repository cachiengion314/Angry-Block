using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Pool;

public class MoneyBagControl : MonoBehaviour, IPoolItemControl
{
  private string[] _nameState = new string[4] { "Idle", "open", "Idle_open", "dissapear" };

  [Header("External dependencies")]
  ObjectPool<GameObject> moneyBagPool;

  [Header("Internal Dependencies")]
  [SerializeField]
  SkeletonAnimation skeAnim;

  public void FullyRemoveFromTable()
  {
    ItemManager.Instance.TryRemoveNeighborMoneyBags(transform.position, out int moneyBagValue, out int amountMoneyBag, false);
  }

  public void InjectPool(ObjectPool<GameObject> moneyBagPool, ObjectPool<GameObject> other = null)
  {
    this.moneyBagPool = moneyBagPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      moneyBagPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.MoneyBagsGrid[index] = 0;
    ItemManager.Instance.MoneyBags[index] = null;

    Release();
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.MoneyBagsGrid[index]--;
    // ShowSkeletonRemove(ItemManager.Instance.IceBoxesGrid[index]);
    ShowStateOfHealthBaseOn(ItemManager.Instance.MoneyBagsGrid[index]);

    if (ItemManager.Instance.MoneyBagsGrid[index] > 0)
    {
      return null;
    }

    ItemManager.Instance.MoneyBagsGrid[index] = 0;
    ItemManager.Instance.MoneyBags[index] = null;
    GameManager.Instance.CurrentCoin += 2;

    return null;
  }

  public void SetSortingOrder(int sortingOrder)
  {
    skeAnim.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
  }

  public void ShowStateOfHealthBaseOn(int value)
  {
    DOTween.Kill(transform);

    if (value == 0)
    {
      skeAnim.AnimationState.SetAnimation(0, _nameState[3], false);
      var duration = skeAnim.Skeleton.Data.FindAnimation(_nameState[3]).Duration;

      DOVirtual.DelayedCall(duration * 0.3f,
        () =>
        {
          var theme = RendererSystem.Instance.GetCurrentTheme();

          ShowPanel.Instance.ShowTextAt(
            transform.position, "+" + 2, Color.yellow, null, 1.1f
          );
        }
      ).SetTarget(transform); ;
      return;
    }

    if (value == 1)
    {
      skeAnim.AnimationState.SetAnimation(0, _nameState[1], false);

      var duration = skeAnim.Skeleton.Data.FindAnimation(_nameState[1]).Duration;
      DOVirtual.DelayedCall(duration,
        () =>
        {
          skeAnim.AnimationState.SetAnimation(0, _nameState[2], true);
        }
      ).SetTarget(transform);

      return;
    }

    if (value == 2)
    {
      skeAnim.AnimationState.SetAnimation(0, _nameState[0], true);
    }
  }
}