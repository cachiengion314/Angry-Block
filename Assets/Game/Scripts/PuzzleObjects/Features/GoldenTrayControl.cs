using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class GoldenTrayControl : MonoBehaviour, IPoolItemControl
{
  ObjectPool<GameObject> goldenTrayPool;
  [SerializeField] BoxCollider2D _collider;

  private Material _mat;
  private float _timeLoopShinning;

  private void OnEnable()
  {
    transform.localScale = new float3(1, 1, 0);
    _mat = transform.GetComponentInChildren<SpriteRenderer>().material;
    _timeLoopShinning = UnityEngine.Random.Range(3f, 5f);

    PlayAnimShinning();
  }

  private void OnDisable()
  {
    DOTween.Kill(transform);
  }

  public void InjectPool(ObjectPool<GameObject> goldenTrayPool, ObjectPool<GameObject> other = null)
  {
    this.goldenTrayPool = goldenTrayPool;
  }

  public void PlayAnimShinning()
  {
    DOVirtual.DelayedCall(
      _timeLoopShinning,
      () =>
      {
        DOTween.To(
          () => _mat.GetFloat("_Intensity"),
          (value) => _mat.SetFloat("_Intensity", value),
          0.1f,
          0.5f
        )
        .SetLoops(2, LoopType.Yoyo)
        .SetTarget(transform)
        .OnComplete(PlayAnimShinning);
      }
    );
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.GoldenTrayGrid[index] = 0;
    ItemManager.Instance.GoldenTrays[index] = null;

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
      goldenTrayPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.GoldenTrayGrid[index] = 0;
    ItemManager.Instance.GoldenTrays[index] = null;

    Release();
  }

  public void FullyRemoveFromTable()
  {
    RemoveFromTable();
  }
}