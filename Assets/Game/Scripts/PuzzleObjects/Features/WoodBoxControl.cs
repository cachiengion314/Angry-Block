using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
public class WoodBoxControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] ParticleSystem woodBreak;
  [SerializeField] Sprite[] healthSprites;
  [SerializeField] Color[] healthColors;
  [SerializeField] SpriteRenderer _renderer;
  [SerializeField] BoxCollider2D _collider;

  [Header("External dependencies")]
  ObjectPool<GameObject> woodBoxPool;

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
            if (ItemManager.Instance.HasCurtainLayerAt(transform.position)) return;
            ItemManager.Instance.TrySelfRemovedByHammerAt(transform.position, this);
          }
          break;
      }
    }
  }

  public void ShowStateOfHealthBaseOn(int value)
  {
    if (value == 1)
    {
      _renderer.sprite = healthSprites[0];
      _renderer.color = healthColors[0];
      return;
    }
    if (value == 2)
    {
      _renderer.sprite = healthSprites[1];
      _renderer.color = healthColors[1];
      return;
    }
    if (value == 3)
    {
      _renderer.sprite = healthSprites[2];
      _renderer.color = healthColors[2];
      return;
    }
  }

  public void InjectPool(ObjectPool<GameObject> woodBoxPool, ObjectPool<GameObject> other = null)
  {
    this.woodBoxPool = woodBoxPool;
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.WoodBoxesGrid[index]--;
    ShowStateOfHealthBaseOn(ItemManager.Instance.WoodBoxesGrid[index]);
    if (ItemManager.Instance.WoodBoxesGrid[index] > 0)
    {
      var _woodBreak = Instantiate(woodBreak, transform.position, Quaternion.identity);
      _woodBreak.Play();
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1); ;
    }

    ItemManager.Instance.WoodBoxesGrid[index] = 0;
    ItemManager.Instance.WoodBoxes[index] = null;

    EffectManager.Instance.SpawnWoodSplashAt(transform.position);

    return AnimationManager.Instance.ScaleTo(0, gameObject, .1f,
        () =>
        {
          Release();
        });
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    ItemManager.Instance.WoodBoxesGrid[index]--;
    ShowStateOfHealthBaseOn(ItemManager.Instance.WoodBoxesGrid[index]);
    if (ItemManager.Instance.WoodBoxesGrid[index] > 0)
    {
      return;
    }

    ItemManager.Instance.WoodBoxesGrid[index] = 0;
    ItemManager.Instance.WoodBoxes[index] = null;

    Release();
  }

  public void FullyRemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;
    ItemManager.Instance.WoodBoxesGrid[index] = 0;
    ItemManager.Instance.WoodBoxes[index] = null;

    Release();
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      woodBoxPool.Release(gameObject);
    }
  }
}
