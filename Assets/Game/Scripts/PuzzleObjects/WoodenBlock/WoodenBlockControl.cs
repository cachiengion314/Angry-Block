using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class WoodenBlockControl : MonoBehaviour, ITrigger, IColorBlock, ISpriteRend
{
    [SerializeField] SortingGroup sortingGroup;
    [SerializeField] SpriteRenderer bodyRenderer;
    public Transform blockParent;
    int Index;

    public SpriteRenderer GetBodyRenderer()
    {
        return bodyRenderer;
    }

    public int GetColorValue()
    {
        throw new System.NotImplementedException();
    }

    public int GetIndex()
    {
        return Index;
    }

    public int GetSortingOrder()
    {
        return bodyRenderer.sortingOrder;
    }

    public void Initialize(DirectionBlockData directionBlockData)
    {
        var directionBlock = LevelManager.Instance.SpawnDirectionBlockAt(Index, blockParent);
        directionBlock.SetIndex(Index);
        directionBlock.SetColorValue(directionBlockData.ColorValue);
        directionBlock.SetDirectionValue(directionBlockData.DirectionValue);
        directionBlock.SetAmmunition(directionBlockData.Ammunition);
        if (!directionBlock.TryGetComponent(out Collider2D col)) return;
        col.enabled = false;
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
            if (!block.TryGetComponent(out Collider2D col)) return;
            col.enabled = true;
            LevelManager.Instance.SetDirectionBlocks(Index, block.gameObject);
            block.SetParent(LevelManager.Instance.SpawnedParent);
            Destroy(gameObject);
        }
    }

    public void RemoveBlock()
    {
        if (blockParent.childCount <= 0) return;
        var block = blockParent.GetChild(0);
        if (!block.TryGetComponent(out Collider2D col)) return;
        col.enabled = true;
        block.SetParent(LevelManager.Instance.SpawnedParent);
        LevelManager.Instance.SetDirectionBlocks(Index, null);
        Destroy(gameObject);
    }

    public void SetColorValue(int colorValue)
    {
        throw new System.NotImplementedException();
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        sortingGroup.sortingOrder = sortingOrder;
        foreach (Transform child in blockParent)
        {
            if (!child.TryGetComponent(out ISpriteRend spriteRend)) continue;
            spriteRend.SetSortingOrder(sortingOrder);
        }
    }
}
