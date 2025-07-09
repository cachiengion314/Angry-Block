using System;
using DG.Tweening;
using HoangNam;
using UnityEngine;
using UnityEngine.Pool;

public partial class BeverageFridgeControl : MonoBehaviour, IPoolItemControl
{
  public void InjectPool(ObjectPool<GameObject> beverageFridgePool, ObjectPool<GameObject> other = null)
  {
    _beverageFridgePool = beverageFridgePool;
  }

  public void InitFrom(BeverageFridgeData beverageFridgeData)
  {
    _posCellInLayer = new(beverageFridgeData.PosCellInLayer);
    _beverageFridgeData = beverageFridgeData;

    SpawnCups(beverageFridgeData.ColorCups);
  }

  private void SpawnCups(int[] colorCups)
  {
    _amountCup = colorCups.Length;

    for (int i = 0; i < cupParent1.childCount; i++)
    {
      var cup = cupParent1.GetChild(i);
      cup.gameObject.SetActive(true);
      cup.transform.localRotation = Quaternion.identity;
      cup.GetComponent<SpriteRenderer>().sprite = cupSprites[colorCups[i] - 1];
    }

    for (int i = 0; i < cupParent2.childCount; i++)
    {
      var cup = cupParent2.GetChild(i);
      cup.gameObject.SetActive(true);
      cup.transform.localRotation = Quaternion.identity;
      cup.GetComponent<SpriteRenderer>().sprite = cupSprites[colorCups[i] - 1];
    }
  }

  public void SetSortingOrder(int sortingOrder)
  {
    bodyRenderer.sortingOrder = sortingOrder;
    glassRenderer.sortingOrder = sortingOrder + 3;
    bgPushRenderer.sortingOrder = sortingOrder + 1;
    skePushAnim.GetComponent<MeshRenderer>().sortingOrder = sortingOrder + 3;

    for (int i = 0; i < cupParent1.childCount; i++)
    {
      var cup = cupParent1.GetChild(i).GetComponent<SpriteRenderer>();
      cup.sortingOrder = sortingOrder + 2;
    }

    for (int i = 0; i < cupParent2.childCount; i++)
    {
      var cup = cupParent2.GetChild(i).GetComponent<SpriteRenderer>();
      cup.sortingOrder = sortingOrder + 2;
    }
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    _amountCup--;
    ShowStateOfHealthBaseOn(_amountCup);

    if (_amountCup > 0)
    {
      TrySpawnCupFromFridge();
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    ItemManager.Instance.BeverageFridgesGrid[index] = 0;
    ItemManager.Instance.RemoveBeverageFridge(this);
    TrySpawnCupFromFridge();

    return
    LeanTween.delayedCall(gameObject, 0.5f, () =>
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
    if (value <= 0) value = 0;
    cupParent1.GetChild(value).gameObject.SetActive(false);
    cupParent2.GetChild(value).gameObject.SetActive(false);

    PlayAnimPushCup();
  }

  private void PlayAnimPushCup()
  {
    DOTween.Kill(transform);
    var duration = skePushAnim.Skeleton.Data.FindAnimation("animation").Duration;
    skePushAnim.AnimationState.SetAnimation(0, "animation", false);

    DOVirtual.DelayedCall(
      duration,
      () =>
      {
        skePushAnim.AnimationState.SetEmptyAnimation(0, 0);
      }
    ).SetTarget(transform);
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.BeverageFridgesGrid[index] = 0;
    ItemManager.Instance.RemoveBeverageFridge(this);

    Release();
  }

  public void FullyRemoveFromTable()
  {
    ItemManager.Instance.TryRemoveNeighborBeverageFridges(transform.position, false);
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      _beverageFridgePool.Release(gameObject);
    }
  }
}
