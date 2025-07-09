using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class CoffeeBoardControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] Sprite[] healthSprites;
  [SerializeField] Color[] healthColors;
  [SerializeField] SpriteRenderer _renderer;
  [SerializeField] BoxCollider2D _collider;
  [SerializeField] SpriteRenderer box_BRenderer;
  [SerializeField] SpriteRenderer box_FRenderer;
  [SerializeField] SpriteRenderer box_BottomRenderer;
  [SerializeField] SpriteRenderer[] coffeePackFs;
  [SerializeField] SpriteRenderer[] coffeePackBs;
  [SerializeField] Transform coffeeParent;

  [Header("External Dependences")]
  ObjectPool<GameObject> coffeeBoardPool;

  public void FullyRemoveFromTable()
  {
   
    ItemManager.Instance.TryRemoveNeighborCoffeeBoards(transform.position, out int coffeeBoardValue, out int amountCoffeeBoard, false);


  }

  public void InjectPool(ObjectPool<GameObject> coffeeBoardPool, ObjectPool<GameObject> other = null)
  {
    this.coffeeBoardPool = coffeeBoardPool;
  }


  public void Release()
  {
    if (gameObject.activeSelf)
    {
      coffeeBoardPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.CoffeeBoardsGrid[index] = 0;
    ItemManager.Instance.CoffeeBoards[index] = null;

    Release();
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.CoffeeBoardsGrid[index]--;
    // ShowSkeletonRemove(ItemManager.Instance.IceBoxesGrid[index]);
    ShowStateOfHealthBaseOn(ItemManager.Instance.CoffeeBoardsGrid[index]);

    if (ItemManager.Instance.CoffeeBoardsGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    ItemManager.Instance.CoffeeBoardsGrid[index] = 0;
    ItemManager.Instance.CoffeeBoards[index] = null;

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
    coffeeParent.GetChild(value).gameObject.SetActive(false);
  }

  public void ShowCoffeePacks()
  {
    for (int i = 0; i < coffeeParent.childCount; i++)
    {
      coffeeParent.GetChild(i).gameObject.SetActive(true);
    }
  }

  public void SetSortingOrder(int sortingOrder)
  {
    box_BottomRenderer.sortingOrder = sortingOrder;
    box_BRenderer.sortingOrder = sortingOrder + 1;

    for (int i = 0; i < coffeePackBs.Length; i++)
    {
      coffeePackBs[i].sortingOrder = sortingOrder + 2;
    }

    for (int i = 0; i < coffeePackFs.Length; i++)
    {
      coffeePackFs[i].sortingOrder = sortingOrder + 3;
    }

    box_FRenderer.sortingOrder = sortingOrder + 4;
  }
}