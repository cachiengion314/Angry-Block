using HoangNam;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class GridWorld : MonoBehaviour
{
  [Header("Setting")]
  [SerializeField] bool showGridValue;
  [SerializeField] int smallestPossibleValue;
  public int SmallestPossibleValue { get { return smallestPossibleValue; } }
  [Range(0, 5)]
  [SerializeField] int colorIndex;
  float3x3 _originalMatrix;
  public float3x3 OriginalMatrix { get { return _originalMatrix; } }
  float3x3 _rotatedMatrix;
  [Range(-90, 90)]
  [SerializeField] float degAroundX;
  public float DegAroundX { get { return degAroundX; } }
  [SerializeField] int2 gridSize;
  public int2 GridSize { get { return gridSize; } set { gridSize = value; } }
  [SerializeField] float2 scale;
  public float2 Scale { get { return scale; } }
  NativeArray<int> _grid;
  public NativeArray<int> Grid
  {
    get
    {
      return _grid;
    }
    set
    {
      _grid = value;
    }
  }

  public float2 Offset { get; private set; }
  readonly int2[] directions = new int2[] { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };

  private void Update()
  {
#if UNITY_EDITOR
    DrawGrid();
#endif
  }

  public void BakingGridWorld()
  {
    _originalMatrix = HoangNam.Utility.GetMatrixWith(0);
    _rotatedMatrix = HoangNam.Utility.GetMatrixWith(degAroundX);
    _grid = new NativeArray<int>(gridSize.x * gridSize.y, Allocator.Persistent);
    Offset = new Vector2((gridSize.x - 1) / 2f, (gridSize.y - 1) / 2f);
    for (int i = 0; i < _grid.Length; ++i) _grid[i] = GetEmptyValue();
  }

  public void DisposeGridWorld()
  {
    _grid.Dispose();
  }

  public int ConvertGridPosToIndex(in int2 gridPos)
  {
    var index = gridSize.y * gridPos.x + gridPos.y;
    return index;
  }

  public int2 ConvertIndexToGridPos(in int index)
  {
    int x = (int)(uint)math.floor(index / gridSize.y);
    int y = index - (x * gridSize.y);
    return new int2(x, y);
  }

  public float3 ConvertGridPosToWorldPos(in int2 gridPos)
  {
    var A2 = gridPos;
    var O2 = Offset;
    var O2A2 = A2 - O2;
    var A1 = new float2(O2A2.x * scale.x, O2A2.y * scale.y);
    var worldPos = new float3(A1.x, A1.y, 0);
    var _pos = math.mul(worldPos, _rotatedMatrix);
    return _pos + (float3)transform.position;
  }

  public int2 ConvertWorldPosToGridPos(in float3 worldPos)
  {
    var O1A1 = math.mul(worldPos - (float3)transform.position, _originalMatrix);
    var O2A2 = new float2(O1A1.x * 1 / scale.x, O1A1.y * 1 / scale.y);
    var A2 = Offset + new float2(O2A2.x, O2A2.y);
    int xRound = (int)math.round(A2.x);
    int yRound = (int)math.round(A2.y);
    var gridPos = new int2(xRound, yRound);
    return gridPos;
  }

  public int ConvertWorldPosToIndex(in float3 worldPos)
  {
    var grid = ConvertWorldPosToGridPos(worldPos);
    var index = ConvertGridPosToIndex(grid);
    return index;
  }

  public float3 ConvertIndexToWorldPos(in int index)
  {
    var grid = ConvertIndexToGridPos(index);
    var worldPos = ConvertGridPosToWorldPos(grid);
    return worldPos;
  }

  public int GetEmptyValue()
  {
    return smallestPossibleValue - 1;
  }

  public int GetValueAt(int index)
  {
    if (index < 0) return GetEmptyValue();
    if (index > _grid.Length - 1) return GetEmptyValue();
    return _grid[index];
  }

  public int GetValueAt(float3 worldPos)
  {
    var index = ConvertWorldPosToIndex(worldPos);
    if (index < 0) return GetEmptyValue();
    if (index > _grid.Length - 1) return GetEmptyValue();
    return _grid[index];
  }

  public void SetValueAt(float3 worldPos, int value = 1)
  {
    int index = ConvertWorldPosToIndex(worldPos);
    if (index < 0) return;
    if (index > _grid.Length - 1) return;
    _grid[index] = value;
  }

  public void SetValueAt(int index, int value = 1)
  {
    if (index < 0) return;
    if (index > _grid.Length - 1) return;
    _grid[index] = value;
  }

  public bool IsPosOutsideAt(float3 worldPos)
  {
    int2 gridPos = ConvertWorldPosToGridPos(worldPos);
    return IsGridPosOutsideAt(gridPos);
  }

  public bool IsGridPosOutsideAt(int2 gridPos)
  {
    var emptyValue = GetEmptyValue();
    var index = ConvertGridPosToIndex(gridPos);
    if (gridPos.x > gridSize.x - 1 || gridPos.x < 0) return true;
    if (gridPos.y > gridSize.y - 1 || gridPos.y < 0) return true;
    if (_grid[index] == emptyValue - 1) return true;
    return false;
  }

  public bool IsPosOccupiedAt(float3 worldPos)
  {
    int2 gridPos = ConvertWorldPosToGridPos(worldPos);
    return IsGridPosOccupiedAt(gridPos);
  }

  public bool IsGridPosOccupiedAt(int2 gridPos)
  {
    if (IsGridPosOutsideAt(gridPos)) return true;
    var index = ConvertGridPosToIndex(gridPos);
    if (_grid[index] >= smallestPossibleValue) return true;
    return false;
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void OnDrawGizmos()
  {
    if (!showGridValue) return;
    for (int i = 0; i < _grid.Length; ++i)
    {
      var worldPos = ConvertIndexToWorldPos(i);
      var val = _grid[i];
      Utility.DrawString(val.ToString(), worldPos, Utility.GetColorFrom((ColorIndex)colorIndex));
    }
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void DrawGrid()
  {
    if (Grid == null) return;

    for (int i = 0; i < _grid.Length; ++i)
    {
      var worldPos = ConvertIndexToWorldPos(i);
      HoangNam.Utility.DrawQuad(worldPos, scale, degAroundX, (ColorIndex)colorIndex);
      var val = _grid[i];
      if (val < smallestPossibleValue) continue;
      HoangNam.Utility.DrawQuad(worldPos, scale * .9f, degAroundX, (ColorIndex)colorIndex);
    }
  }

  /// <summary>
  /// Only for debugging
  /// </summary>
  void OnDrawGizmosSelected()
  {
    var color = HoangNam.Utility.GetColorFrom((ColorIndex)colorIndex);
    color.a = .35f;
    Gizmos.color = color;
    Gizmos.DrawCube(transform.position, new Vector2(
      gridSize.x * scale.x,
      gridSize.y * scale.y
    ));
  }

  /// <summary>
  /// return world position of neighbor and float3(-1, -1, -1) for invalid neighbor.
  /// </summary>
  /// 
  public float3[] FindNeighborsAt(float3 worldPos)
  {
    float3[] neighbors = new float3[directions.Length];
    var gridPos = ConvertWorldPosToGridPos(worldPos);
    for (int i = 0; i < directions.Length; ++i)
    {
      var dir = directions[i];
      var neighbor = gridPos + dir;
      if (IsGridPosOutsideAt(neighbor))
      {
        neighbors[i] = new float3(-1, -1, -1);
        continue;
      }
      float3 wPos = ConvertGridPosToWorldPos(neighbor);
      neighbors[i] = wPos;
    }
    return neighbors;
  }
}
