using UnityEngine;

public interface ISpriteRend
{
  public SpriteRenderer GetBodyRenderer();
  public int GetSortingOrder();
  public void SetSortingOrder(int sortingOrder);
}