using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class GrassControl : MonoBehaviour, IPoolItemControl
{
  ObjectPool<GameObject> grassPool;
  [SerializeField] BoxCollider2D _collider;

  private void OnEnable()
  {
    transform.localScale = new float3(1, 1, 0);
  }

  void Update()
  {
    PressControl();
  }

  void PressControl()
  {
    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
      if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

      switch (touch.phase)
      {
        case TouchPhase.Began:
          if (_collider == Physics2D.OverlapPoint(touchPos))
          {
            var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
            if (ItemManager.Instance.HasCurtainLayerAt(transform.position))
            {
              return;
            }

            if (ItemManager.Instance.HasGrillerAt(transform.position))
            {
              return;
            }

            if (ItemManager.Instance.WoodBoxes[index])
            {
              var box = ItemManager.Instance.WoodBoxes[index].GetComponent<WoodBoxControl>();
              ItemManager.Instance.TrySelfRemovedByHammerAt(transform.position, box);
              return;
            }
            ;

            ItemManager.Instance.TrySelfRemovedByHammerAt(transform.position, this);
          }
          break;
      }
    }
  }

  public void InjectPool(ObjectPool<GameObject> grassPool, ObjectPool<GameObject> other = null)
  {
    this.grassPool = grassPool;
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.GrassesGrid[index] = 0;
    ItemManager.Instance.Grasses[index] = null;

    return AnimationManager.Instance.ScaleTo(0, gameObject, .1f,
      () =>
      {
        Release();
      });
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      grassPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    if (ItemManager.Instance.HasCurtainLayerAt(transform.position)) return;
    if (ItemManager.Instance.HasGrillerAt(transform.position)) return;


  }

  public void FullyRemoveFromTable()
  {
    // RemoveFromTable();
  }
}
