using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public struct GridNode
{
  public int Index;
  public int ParentIndex;
  public int2 GridPosition;
  public float HCost;
  public float GCost;
  public float FCost { get { return HCost + GCost; } }
  public int Value;
}

/// <summary>
/// PathFinding
/// </summary>
public partial class GridWorld : MonoBehaviour
{
  [Header("Datas")]
  NativeArray<GridNode> _nodeMap;
  public NativeArray<GridNode> NodeMap { get { return _nodeMap; } }

  public void BakingPathFinding()
  {
    _nodeMap = new NativeArray<GridNode>(gridSize.x * gridSize.y, Allocator.Persistent);
    for (int x = 0; x < GridSize.x; ++x)
    {
      for (int y = 0; y < GridSize.y; ++y)
      {
        var gridPos = new int2(x, y);
        var index = ConvertGridPosToIndex(gridPos);
        var node = new GridNode()
        {
          Index = index,
          ParentIndex = -1,
          GridPosition = gridPos,
          Value = GetEmptyValue()
        };

        _nodeMap[index] = node;
      }
    }
  }

  public void DisposePathFinding()
  {
    _nodeMap.Dispose();
  }

  /// <summary>
  /// excludes is an array that contain all of positions that is not consider 
  /// as obstacles. Note that for performance wise excludes own item is int2 
  /// which is a grid base position
  /// </summary>
  /// <param name="destination"></param>
  /// <param name="startPos"></param>
  /// <param name="excludes"></param>
  /// <returns></returns>
  public NativeList<int2> PathFindingTo(
    in float3 destination,
    in float3 startPos,
    in int2[] excludes
  )
  {
    ResetPathFindingNodeData();

    NativeList<GridNode> availableNodes = new(_nodeMap.Length, Allocator.Temp);
    NativeList<GridNode> visitedNodes = new(_nodeMap.Length, Allocator.Temp);

    if (IsPosOutsideAt(destination)) return new NativeList<int2>(0, Allocator.Temp);
    if (IsPosOutsideAt(startPos)) return new NativeList<int2>(0, Allocator.Temp);

    var destNode = _nodeMap[ConvertWorldPosToIndex(destination)];
    var startNode = _nodeMap[ConvertWorldPosToIndex(startPos)];
    availableNodes.Add(startNode);

    while (availableNodes.Length > 0)
    {
      // standing in a suitable node
      var currentNode = availableNodes[^1];
      var currentIndex = availableNodes.Length - 1;
      for (int i = 0; i < availableNodes.Length; ++i)
      {
        if (availableNodes[i].FCost < currentNode.FCost)
        {
          currentNode = availableNodes[i];
          currentIndex = i;
        }
      }
      availableNodes.RemoveAt(currentIndex);
      visitedNodes.Add(currentNode);

      var neighbors = FindNeighborNodesFor(currentNode, excludes);
      for (int i = 0; i < neighbors.Length; ++i)
      {
        // we should skip the neighbor node that has already placed in visitedNodes
        var hasVisited = false;
        for (int j = 0; j < visitedNodes.Length; ++j)
        {
          if (neighbors[i].Index != visitedNodes[j].Index) continue;
          hasVisited = true;
          break;
        }
        for (int j = 0; j < availableNodes.Length; ++j)
        {
          if (availableNodes[j].Index != neighbors[i].Index) continue;
          hasVisited = true;
          break;
        }
        if (hasVisited) continue;

        var distanceToDest = math.distancesq(destNode.GridPosition, neighbors[i].GridPosition);
        var distanceFromParent = math.distancesq(neighbors[i].GridPosition, currentNode.GridPosition);
        // update new value for neighbor node
        var _neighbor = new GridNode()
        {
          Index = neighbors[i].Index,
          ParentIndex = currentNode.Index,
          GridPosition = neighbors[i].GridPosition,
          HCost = distanceToDest,
          GCost = currentNode.GCost + distanceFromParent,
          Value = neighbors[i].Value
        };
        neighbors[i] = _neighbor;
        _nodeMap[_neighbor.Index] = _neighbor;

        if (distanceToDest == 0) return FindTracingPathFor(_neighbor);

        availableNodes.Add(_neighbor);
      }

      neighbors.Dispose();
    }

    availableNodes.Dispose();
    visitedNodes.Dispose();

    return new NativeList<int2>(0, Allocator.Temp);
  }

  public void ResetPathFindingNodeData()
  {
    for (int x = 0; x < GridSize.x; ++x)
    {
      for (int y = 0; y < GridSize.y; ++y)
      {
        var gridPos = new int2(x, y);
        var index = ConvertGridPosToIndex(gridPos);
        var node = new GridNode()
        {
          Index = index,
          ParentIndex = -1,
          GridPosition = gridPos,
          HCost = 0,
          GCost = 0,
          Value = GetEmptyValue()
        };

        _nodeMap[index] = node;
      }
    }
  }

  NativeList<int2> FindTracingPathFor(GridNode node)
  {
    var maxLength = 64;
    NativeList<int2> foundList = new(maxLength, Allocator.Temp)
    {
      node.GridPosition
    };
    if (node.ParentIndex == -1) return foundList;

    var parentNode = _nodeMap[node.ParentIndex];
    var count = 0;
    while (
      parentNode.Index < _nodeMap.Length && parentNode.Index > -1 ||
      count > maxLength
    )
    {
      foundList.Add(parentNode.GridPosition);
      if (parentNode.ParentIndex == -1) break;
      parentNode = _nodeMap[parentNode.ParentIndex];
      count++;
      if (count > maxLength) print("FindTracingPaths function reach its max capacity!");
    }

    NativeList<int2> foundListReserved = new(maxLength, Allocator.Temp);
    for (int i = foundList.Length - 1; i >= 0; --i) foundListReserved.Add(foundList[i]);
    foundList.Dispose();
    return foundListReserved;
  }

  NativeList<GridNode> FindNeighborNodesFor(in GridNode node, int2[] excludes)
  {
    NativeList<GridNode> found = new(directions.Length, Allocator.Temp);

    for (int i = 0; i < directions.Length; ++i)
    {
      var neighborPos = node.GridPosition + directions[i];
      if (IsGridPosOutsideAt(neighborPos)) continue;
      var _index = ConvertGridPosToIndex(neighborPos);
      var neighbor = _nodeMap[_index];

      if (excludes.Contains(neighborPos))
      {
        found.Add(neighbor);
        continue;
      }

      if (_grid[_index] > GetEmptyValue()) continue;
      if (LevelManager.Instance.DirectionBlocks[_index] != null) continue;

      found.Add(neighbor);
    }

    return found;
  }
}