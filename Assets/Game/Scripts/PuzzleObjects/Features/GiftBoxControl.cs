using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class GiftBoxControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] Sprite[] healthSprites;
  [SerializeField] Color[] healthColors;
  [SerializeField] SpriteRenderer _renderer;
  [SerializeField] BoxCollider2D _collider;
  [SerializeField] SkeletonAnimation skeAnim;

  [Header("External dependencies")]
  ObjectPool<GameObject> giftBoxPool;

  public void FullyRemoveFromTable()
  {
 
  }

  public void InjectPool(ObjectPool<GameObject> giftBoxPool, ObjectPool<GameObject> other = null)
  {
    this.giftBoxPool = giftBoxPool;
  }


  public void Release()
  {
    if (gameObject.activeSelf)
    {
      giftBoxPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.GiftBoxesGrid[index] = 0;
    ItemManager.Instance.GiftBoxes[index] = null;

    Release();
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.GiftBoxesGrid[index]--;
    // ShowSkeletonRemove(ItemManager.Instance.IceBoxesGrid[index]);
    ShowStateOfHealthBaseOn(ItemManager.Instance.GiftBoxesGrid[index]);

    if (ItemManager.Instance.GiftBoxesGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    ItemManager.Instance.GiftBoxesGrid[index] = 0;
    ItemManager.Instance.GiftBoxes[index] = null;

    return
    LeanTween.delayedCall(gameObject, 0.3f, () =>
    {
      AnimationManager.Instance.ScaleTo(0, gameObject, .1f,
          () =>
          {
            Release();
          });
    });
  }

  public void ShowStateOfHealthBaseOn(int value)
  {
    if (value == 0) return;

    if (value == 1)
    {
      skeAnim.AnimationState.SetAnimation(0, "Box2", false);
    }
  }
}