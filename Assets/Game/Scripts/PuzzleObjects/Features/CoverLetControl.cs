using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Collider2D))]
public class CoverLetControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] SkeletonAnimation skeRenderer;
  [SerializeField] BoxCollider2D _collider;

  [Header("External dependencies")]
  ObjectPool<GameObject> coverLetPool;

  private void OnEnable()
  {
    transform.localScale = new float3(1, 1, 0);
    skeRenderer.AnimationState.SetAnimation(0, "idle", true);
  }

  public void InjectPool(ObjectPool<GameObject> converLetPool, ObjectPool<GameObject> other = null)
  {
    this.coverLetPool = converLetPool;
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.CoverLetsGrid[index]--;

    if (ItemManager.Instance.CoverLetsGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    TryMergeTrayNeighbor(index);
    ItemManager.Instance.CoverLetsGrid[index] = 0;
    ItemManager.Instance.CoverLets[index] = null;

    skeRenderer.AnimationState.SetAnimation(0, "open", false);
    var duration = skeRenderer.Skeleton.Data.FindAnimation("open").Duration;
    var timeFly = 1f;

    LeanTween.delayedCall(duration,
      () =>
      {
        SoundManager.Instance.PlayCoverLetSfx();

        transform.DOMoveY(
          transform.position.y + 10,
          timeFly
        );
      }
    );

    duration += timeFly;

    return
    LeanTween.delayedCall(gameObject, duration, () =>
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
    var posWorld = ItemManager.Instance.CoverLets[index].transform.position;
    var trayControl = ItemManager.Instance.FindCoverLetTray(posWorld);
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

    ItemManager.Instance.CoverLetsGrid[index]--;
    if (ItemManager.Instance.CoverLetsGrid[index] > 0)
    {
      return;
    }

    ItemManager.Instance.CoverLetsGrid[index] = 0;
    ItemManager.Instance.CoverLets[index] = null;

    Release();
  }

  public void FullyRemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    ItemManager.Instance.CoverLetsGrid[index] = 0;
    ItemManager.Instance.CoverLets[index] = null;

    Release();
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      coverLetPool.Release(gameObject);
    }
  }
}