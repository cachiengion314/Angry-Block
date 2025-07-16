using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager
{
    void HideHiddenWoodenBlockNeighborAt(GameObject directionBlock)
    {
        if (!directionBlock.TryGetComponent(out IColorBlock colorBlock)) return;
        var blockPos = bottomGrid.ConvertIndexToWorldPos(colorBlock.GetIndex());
        var blockGrid = bottomGrid.ConvertIndexToGridPos(colorBlock.GetIndex());
        var neighbors = bottomGrid.FindNeighborsAt(blockPos);
        foreach (var neighbor in neighbors)
        {
            var neighborIndex = bottomGrid.ConvertWorldPosToIndex(neighbor);
            var neighborBlock = _directionBlocks[neighborIndex];
            if (neighborBlock == null) continue;
            if (!neighborBlock.TryGetComponent(out WoodenBlockControl woodenBlock)) continue;
            if (!woodenBlock.IsHideHidden(blockGrid)) continue;
            woodenBlock.HideHidden();
            Destroy(woodenBlock);
        }
    }
}