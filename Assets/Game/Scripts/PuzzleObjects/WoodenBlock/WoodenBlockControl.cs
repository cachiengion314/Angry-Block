using Unity.Mathematics;
using UnityEngine;

public class WoodenBlockControl : MonoBehaviour, ITrigger, IColorBlock
{
    public Transform blockParent;
    int Index;
    public int GetColorValue()
    {
        throw new System.NotImplementedException();
    }

    public int GetIndex()
    {
        return Index;
    }

    public void Initialize(DirectionBlockData directionBlockData)
    {
        var directionBlock = LevelManager.Instance.SpawnDirectionBlockAt(Index, blockParent);
        directionBlock.SetIndex(Index);
        directionBlock.SetColorValue(directionBlockData.ColorValue);
        directionBlock.SetDirectionValue(directionBlockData.DirectionValue);
        directionBlock.SetAmmunition(directionBlockData.Ammunition);
        directionBlock.gameObject.SetActive(false);
    }

    public void OnTrigger<T>(T data)
    {
        if (data is int2 gridPosBlockAhead)
        {
            if (blockParent.childCount <= 0) return;
            var block = blockParent.GetChild(0);
            if (!block.TryGetComponent(out IDirectionBlock directionBlock)) return;
            var gridPos = LevelManager.Instance.BottomGrid.ConvertIndexToGridPos(Index);
            if (!gridPosBlockAhead.Equals(gridPos + directionBlock.GetDirection())) return;
            block.gameObject.SetActive(true);
            LevelManager.Instance.SetDirectionBlocks(Index, block.gameObject);
            block.SetParent(LevelManager.Instance.SpawnedParent);
            Destroy(gameObject);
        }
    }

    public void SetColorValue(int colorValue)
    {
        throw new System.NotImplementedException();
    }

    public void SetIndex(int index)
    {
        Index = index;
    }
}
