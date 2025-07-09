public interface ISpriteRenderer
{
  public int GetSortingOrder();
  public void SetInitSortingOrder(int sortingOrder);
  public void SetSortingOrder(int sortingOrder);
  public void ResetSortingOrder();
}