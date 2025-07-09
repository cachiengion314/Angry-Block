using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class CupBoardControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] Transform cupParent;
  [SerializeField] SpriteMask[] spriteMasks;

  ObjectPool<GameObject> cupBoardPool;

  private int _amountCup;
  public int AmountCup
  {
    get { return _amountCup; }
  }

  private List<int> _posCellInLayer;
  public List<int> PosCellInLayer { get { return _posCellInLayer; } }

  public void InitFrom(CupBoardData cupBoardData)
  {
    _posCellInLayer = new(cupBoardData.PosCellInLayer);
    SpawnCups();
  }

  private void SpawnCups()
  {
    _amountCup = cupParent.childCount;

    for (int i = 0; i < cupParent.childCount; i++)
    {
      var cup = cupParent.GetChild(i);
      cup.gameObject.SetActive(true);
      cup.transform.localRotation = Quaternion.identity;
    }
  }

  public void SetSortingOrder(int sortingOrder)
  {
    bodyRenderer.sortingOrder = sortingOrder;

    for (int i = 0; i < cupParent.childCount; i++)
    {
      var cup = cupParent.GetChild(i).GetComponent<CupOfBoardControl>();
      cup.SetSortingOrder(sortingOrder + 1 + i / 2 * 2);
    }

    for (int i = 0; i < spriteMasks.Length; i++)
    {
      var mask = spriteMasks[i];
      mask.frontSortingOrder = sortingOrder + 2 + i * 2;
      mask.backSortingOrder = sortingOrder + i * 2;
    }
  }

  public void FullyRemoveFromTable()
  {
   
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
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    ItemManager.Instance.CupBoardsGrid[index] = 0;
    ItemManager.Instance.RemoveCupBoard(this);

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
    cupParent.GetChild(value).gameObject.SetActive(false);
  }

  public void InjectPool(ObjectPool<GameObject> cupBoardPool, ObjectPool<GameObject> other = null)
  {
    this.cupBoardPool = cupBoardPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      cupBoardPool.Release(gameObject);
    }
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    ItemManager.Instance.CupBoardsGrid[index] = 0;
    ItemManager.Instance.RemoveCupBoard(this);

    Release();
  }
}