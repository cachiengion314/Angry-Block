using UnityEngine;

public class CupOfBoardControl : MonoBehaviour
{
  [Header("Internal Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;

  public void SetSortingOrder(int sortingOrder)
  {
    bodyRenderer.sortingOrder = sortingOrder;
  }
}