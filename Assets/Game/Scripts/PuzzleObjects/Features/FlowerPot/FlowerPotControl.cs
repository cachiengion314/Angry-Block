using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class FlowerPotControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] Color colorTrail;
  [SerializeField] SkeletonAnimation skeAnim;

  [Header("External dependencies")]
  ObjectPool<GameObject> flowerPotPool;

  public void FullyRemoveFromTable()
  {
    ItemManager.Instance.TryRemoveNeighborFlowerPots(transform.position, false);
  }

  public void InjectPool(ObjectPool<GameObject> flowerPotPool, ObjectPool<GameObject> other = null)
  {
    this.flowerPotPool = flowerPotPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      flowerPotPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.FlowerPotsGrid[index] = 0;
    ItemManager.Instance.FlowerPots[index] = null;

    Release();
    EffectManager.Instance.SpawnWoodSplashAt(transform.position);
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.FlowerPotsGrid[index]--;
    // ShowSkeletonRemove(ItemManager.Instance.IceBoxesGrid[index]);
    ShowStateOfHealthBaseOn(ItemManager.Instance.FlowerPotsGrid[index]);

    if (ItemManager.Instance.FlowerPotsGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    // SoundManager.Instance.PlayGr
    ShowStateOfHealthBaseOn(ItemManager.Instance.FlowerPotsGrid[index]);
    TrySpawnLeaves();
    ItemManager.Instance.FlowerPots[index] = null;

    return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
  }

  public void ShowStateOfHealthBaseOn(int value)
  {
    if (value == 0)
    {
      DOTween.Kill(transform);
      SoundManager.Instance.PlayLeavesSpreadOutSfx();
      skeAnim.AnimationState.SetAnimation(0, "2_to_gone", false);
      return;
    }

    if (value == 1)
    {
      DOTween.Kill(transform);
      SoundManager.Instance.PlayFlowerPotBloomingSfx();
      skeAnim.AnimationState.SetAnimation(0, "1_to_2", false);
      var duration = skeAnim.Skeleton.Data.FindAnimation("1_to_2").Duration;

      DOVirtual.DelayedCall(
        duration,
        () =>
        {
          skeAnim.AnimationState.SetAnimation(0, "2_Idle", true);
        }
      ).SetTarget(transform);
      return;
    }

    if (value == 2)
    {
      DOTween.Kill(transform);
      skeAnim.AnimationState.SetAnimation(0, "1_Idle", true);
      return;
    }
  }

  public void SetSortingOrder(int sortingOrder)
  {
    skeAnim.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
  }

  private void TrySpawnLeaves()
  {
    List<float3> emptyLeaves = ItemManager.Instance.FindEmptyLeavesForPotAt(transform.position);
    if (emptyLeaves.Count == 0) return;

    List<GameObject> needSpawnLeaves = new();

    // logic
    for (int i = 0; i < emptyLeaves.Count; i++)
    {
      var id = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(emptyLeaves[i]);
      var leaves = ItemManager.Instance.SpawnLeavesFlowerAt(id);
      leaves.transform.localScale = float3.zero;

      needSpawnLeaves.Add(leaves);
    }

    var currentAnimTime = 0;
    // 
    Sequence seq = DOTween.Sequence();

    VisualizeSpawnLeaves(ref seq, ref currentAnimTime, needSpawnLeaves);
  }

  private void VisualizeSpawnLeaves(
    ref Sequence seq,
    ref int currentAnimTime,
    List<GameObject> needSpawnLeaves
  )
  {
    var duration = 0.6f;

    for (int i = 0; i < needSpawnLeaves.Count; i++)
    {
      var leave = needSpawnLeaves[i];

      seq.InsertCallback(
        currentAnimTime,
        () =>
        {
          EffectManager.Instance.MoveTrailLightObstacleTo(
            leave.transform.position,
            transform.position,
            colorTrail
          );
        }
      );

      seq.InsertCallback(
        currentAnimTime + duration,
        () =>
        {
          LeanTween.scale(
            leave,
            new float3(1, 1, 1),
            0.5f
          );
        }
      );
    }
  }
}
