using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class PlantPotControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] Sprite[] healthSprites;
  [SerializeField] Color[] healthColors;
  [SerializeField] SpriteRenderer _renderer;
  [SerializeField] BoxCollider2D _collider;
  [SerializeField] Color colorTrail;
  [SerializeField] SkeletonAnimation skeAnim;

  [Header("External dependencies")]
  ObjectPool<GameObject> plantPotPool;

  public void FullyRemoveFromTable()
  {
    ItemManager.Instance.TryRemoveNeighborPlantPots(transform.position, false);
  }

  public void InjectPool(ObjectPool<GameObject> plantPotPool, ObjectPool<GameObject> other = null)
  {
    this.plantPotPool = plantPotPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      plantPotPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.PlantPotsGrid[index] = 0;
    ItemManager.Instance.PlantPots[index] = null;

    Release();
    EffectManager.Instance.SpawnWoodSplashAt(transform.position);
    // gameObject.SetActive(false);
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.PlantPotsGrid[index]--;
    // ShowSkeletonRemove(ItemManager.Instance.IceBoxesGrid[index]);
    ShowStateOfHealthBaseOn(ItemManager.Instance.PlantPotsGrid[index]);

    if (ItemManager.Instance.PlantPotsGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    ItemManager.Instance.PlantPotsGrid[index] = 3;
    ShowStateOfHealthBaseOn(ItemManager.Instance.PlantPotsGrid[index]);
    TrySpawnGrassesWith(3);

    return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
  }

  public void ShowStateOfHealthBaseOn(int value)
  {
    if (value == 0) return;

    if (value == 1)
    {
      skeAnim.AnimationState.SetAnimation(0, "Pot2sang3", false);
    }

    if (value == 2)
    {
      skeAnim.AnimationState.SetAnimation(0, "Pot1sang2", false);
    }

    if (value == 3)
    {
      skeAnim.AnimationState.SetAnimation(0, "Pot3ve1", false);
    }
  }

  private void TrySpawnGrassesWith(int amount)
  {
    List<int> emptyGrasses = new();

    emptyGrasses = ItemManager.Instance.FindEmptyGrassForPot();
    if (emptyGrasses.Count == 0) return;

    List<GameObject> needSpawnGrasses = new();

    // logic
    for (int i = 0; i < emptyGrasses.Count; i++)
    {
      if (i >= amount) break;

      var randomID = UnityEngine.Random.Range(0, emptyGrasses.Count);
      var grass = ItemManager.Instance.SpawnGrassAt(emptyGrasses[randomID]);
      grass.transform.localScale = float3.zero;

      needSpawnGrasses.Add(grass);
      emptyGrasses.RemoveAt(randomID);
    }

    var currentAnimTime = 0;
    // 
    Sequence seq = DOTween.Sequence();

    VisualizeSpawnGrass(ref seq, ref currentAnimTime, needSpawnGrasses);
  }

  private void VisualizeSpawnGrass(
    ref Sequence seq,
    ref int currentAnimTime,
    List<GameObject> needSpawnGrasses
  )
  {
    var duration = 0.6f;

    for (int i = 0; i < needSpawnGrasses.Count; i++)
    {
      var grass = needSpawnGrasses[i];

      seq.InsertCallback(
        currentAnimTime,
        () =>
        {
          EffectManager.Instance.MoveTrailLightObstacleTo(
            grass.transform.position,
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
            grass,
            new float3(1, 1, 1),
            0.5f
          );
        }
      );
    }
  }
}