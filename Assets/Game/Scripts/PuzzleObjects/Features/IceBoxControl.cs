using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
public class IceBoxControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] Sprite[] healthSprites;
  [SerializeField] Color[] healthColors;
  [SerializeField] SpriteRenderer _renderer;
  [SerializeField] BoxCollider2D _collider;
  [SerializeField] SkeletonAnimation skeletonAnimation;

  [Header("External dependencies")]
  ObjectPool<GameObject> iceBoxPool;

  private void OnEnable()
  {
    transform.localScale = new float3(1, 1, 0);
    skeletonAnimation.gameObject.SetActive(false);
    _renderer.enabled = true;
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
          Collider2D[] _cols = Physics2D.OverlapPointAll(touchPos);
          foreach (Collider2D col in _cols)
          {
            if (_collider == col)
            {
              if (PowerItemPanel.Instance.IsTriggerSwap) return;
              if (PowerItemPanel.Instance.IsTriggeredHammer)
              {
                var trayControl = ItemManager.Instance.FindIceTray(transform.position);
                trayControl?.FullyRemoveFromTable();

                if (ItemManager.Instance.HasCurtainLayerAt(transform.position)) return;
                ItemManager.Instance.TrySelfRemovedByHammerAt(transform.position, this);
              }
            }
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

  public void ShowSkeletonRemove(int value)
  {
    _renderer.enabled = false;
    skeletonAnimation.gameObject.SetActive(true);

    if (value == 0)
    {
      skeletonAnimation.AnimationName = "vo_banglop3";
      return;
    }
    if (value == 1)
    {
      skeletonAnimation.AnimationName = "vo_banglop2";
      return;
    }
    if (value == 2)
    {
      skeletonAnimation.AnimationName = "vo_banglop1";
      return;
    }
  }

  public void InjectPool(ObjectPool<GameObject> iceBoxPool, ObjectPool<GameObject> other = null)
  {
    this.iceBoxPool = iceBoxPool;
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.IceBoxesGrid[index]--;
    ShowSkeletonRemove(ItemManager.Instance.IceBoxesGrid[index]);
    ShowStateOfHealthBaseOn(ItemManager.Instance.IceBoxesGrid[index]);

    if (ItemManager.Instance.IceBoxesGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    TryMergeTrayNeighbor(index);
    ItemManager.Instance.IceBoxesGrid[index] = 0;
    ItemManager.Instance.IceBoxes[index] = null;

    return
    LeanTween.delayedCall(gameObject, 1f, () =>
    {
      AnimationManager.Instance.ScaleTo(0, gameObject, .1f,
          () =>
          {
            Release();
          });
    });
  }

  private void TryMergeTrayNeighbor(int index)
  {
    HashSet<TrayControl> _linkedTrays1 = new();
    var posWorld = ItemManager.Instance.IceBoxes[index].transform.position;
    var trayControl = ItemManager.Instance.FindIceTray(posWorld);
    if (trayControl == null) return;

    var neighborDraggedTrays = trayControl.FindNeighborTraysAt(trayControl.CurrentWorldPos);
    for (int i = 0; i < neighborDraggedTrays.Count; ++i)
    {
      _linkedTrays1.Add(neighborDraggedTrays[i]);
    }
    // this will make sure that this tray will be placed at "the the last position" of the set
    _linkedTrays1.Add(trayControl);

   
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    ItemManager.Instance.IceBoxesGrid[index]--;
    ShowStateOfHealthBaseOn(ItemManager.Instance.IceBoxesGrid[index]);
    if (ItemManager.Instance.IceBoxesGrid[index] > 0)
    {
      return;
    }

    ItemManager.Instance.IceBoxesGrid[index] = 0;
    ItemManager.Instance.IceBoxes[index] = null;

    Release();
  }

  public void FullyRemoveFromTable()
  {

    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    ItemManager.Instance.IceBoxesGrid[index] = 0;
    ItemManager.Instance.IceBoxes[index] = null;

    Release();
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      iceBoxPool.Release(gameObject);
    }
  }
}
