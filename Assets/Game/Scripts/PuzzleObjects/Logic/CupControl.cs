using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using HoangNam;

public class CupControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal")]
  [SerializeField] SpriteRenderer _renderer;
  public SpriteRenderer Renderer { get { return _renderer; } }
  [SerializeField] float3 offset;
  public float3 Offset { get { return offset; } }
  ObjectPool<GameObject> cupPool;

  [Header("Settings")]
  ColorIndex colorIndex;
  public ColorIndex ColorIndex { get { return colorIndex; } }
  public int UniqueIndex { get; private set; }
  public float3 CurrentWorldPosWithoutOffset { get; private set; }

  private void OnEnable()
  {
    transform.localScale = new float3(1, 1, 0);
  }

  public TrayControl FindParentBaseOnPos()
  {

    return null;
  }

  public float3 FindParentPos()
  {
    var gridPos = FindParentGridPos();
    var worldPos = ItemManager.Instance.TrayGrid.ConvertGridPosToWorldPos(
        gridPos
    );
    return worldPos;
  }

  public int2 FindParentGridPos()
  {
    var gridPos = ItemManager.Instance.CupGrid.ConvertWorldPosToGridPos(
      CurrentWorldPosWithoutOffset
    );
    var xFactor = ItemManager.Instance.CupGrid.GridSize.x
        / ItemManager.Instance.TrayGrid.GridSize.x;
    var yFactor = ItemManager.Instance.CupGrid.GridSize.y
        / ItemManager.Instance.TrayGrid.GridSize.y;

    int x = (int)math.floor(gridPos.x / xFactor);
    var y = (int)math.floor(gridPos.y / yFactor);
    return new int2(x, y);
  }

  public void InjectPool(ObjectPool<GameObject> cupPool, ObjectPool<GameObject> other = null)
  {
    this.cupPool = cupPool;
  }

  public void SetColorIndex(ColorIndex colorIndex)
  {
    this.colorIndex = colorIndex;
    UniqueIndex = GetInstanceID();
  }

  public void SetSortingOrder(int sortingOrder)
  {
    _renderer.sortingOrder = sortingOrder;
  }

  public void CalculateSortingOrder()
  {
    var index = ItemManager.Instance.CupGrid.GridSize.y
        - ItemManager.Instance.CupGrid.ConvertWorldPosToGridPos(CurrentWorldPosWithoutOffset).y;

    SetSortingOrder(index *= 10);
  }

  public float3 GetPosWithoutOffset()
  {
    return (float3)transform.position - offset;
  }

  public int GetSortingOrder()
  {
    return _renderer.sortingOrder;
  }

  /// <summary>
  /// The cup is not set parent yet so we should use global position
  /// </summary>
  /// <param name="desPos"></param>
  public void InitPostAt(float3 desPos, int sortingOrder)
  {
    _renderer.sortingOrder = sortingOrder;
    transform.position = desPos + offset;
  }

  public void SetGridWorldValueAt(float3 worldPosWithoutOffset, bool _isClearValue = false)
  {
    if (_isClearValue)
    {
      ItemManager.Instance.CupGrid.SetValueAt(worldPosWithoutOffset, (int)ColorIndex.None);
      return;
    }
    CurrentWorldPosWithoutOffset = worldPosWithoutOffset;
    _renderer.sortingOrder
        = ItemManager.Instance.CupGrid.GridSize.y -
        ItemManager.Instance.CupGrid.ConvertWorldPosToGridPos(worldPosWithoutOffset).y;
    ItemManager.Instance.CupGrid.SetValueAt(worldPosWithoutOffset, (int)colorIndex);

    _renderer.sortingOrder *= 10;
  }

  public void Release()
  {
    if (gameObject.activeSelf) cupPool.Release(gameObject);
  }

  public void RemoveFromTable()
  {
    var cupGrid = ItemManager.Instance.CupGrid;
    cupGrid.SetValueAt(CurrentWorldPosWithoutOffset, (int)ColorIndex.None);
  }

  public void FullyRemoveFromTable()
  {
    RemoveFromTable();
  }
}
