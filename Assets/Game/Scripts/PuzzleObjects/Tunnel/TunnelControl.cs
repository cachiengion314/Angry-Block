using Unity.Mathematics;
using UnityEngine;

public class TunnelControl : MonoBehaviour, ITrigger, IDirectionBlock, IColorBlock, ISpriteRend
{
    [SerializeField] SpriteRenderer bodyRenderer;
    public Transform blockParent;
    DirectionValue _directionValue;
    public int2 Direction { get; private set; }
    public int Index;

    public int GetColorValue()
    {
        throw new System.NotImplementedException();
    }

    public int2 GetDirection()
    {
        return Direction;
    }

    public DirectionValue GetDirectionValue()
    {
        return _directionValue;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public int GetIndex()
    {
        return Index;
    }

    public void Initialize(DirectionBlockData[] directionBlockDatas)
    {
        var tunnelGird = LevelManager.Instance.BottomGrid.ConvertIndexToGridPos(Index);
        var blockGird = tunnelGird + Direction;
        var blockIndex = LevelManager.Instance.BottomGrid.ConvertGridPosToIndex(blockGird);

        for (int i = 0; i < directionBlockDatas.Length; i++)
        {
            var directionBlockData = directionBlockDatas[i];
            var directionBlock = LevelManager.Instance.SpawnDirectionBlockAt(blockIndex, blockParent);
            directionBlock.SetIndex(blockIndex);
            directionBlock.SetColorValue(directionBlockData.ColorValue);
            directionBlock.SetDirectionValue(directionBlockData.DirectionValue);
            directionBlock.SetAmmunition(directionBlockData.Ammunition);
            directionBlock.gameObject.SetActive(false);
        }
    }

    public void OnTrigger<T>(T data)
    {
        if (data is int2 gridPosBlockAhead)
        {
            var tunnelGird = LevelManager.Instance.BottomGrid.ConvertIndexToGridPos(Index);
            if (!gridPosBlockAhead.Equals(tunnelGird + Direction)) return;

            // anim kick hoad
            if (blockParent.childCount <= 0) return;
            var dirBlock = blockParent.GetChild(0);
            if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) return;
            dirBlock.gameObject.SetActive(true);
            LevelManager.Instance.SetDirectionBlocks(colorBlock.GetIndex(), dirBlock.gameObject);
            dirBlock.SetParent(LevelManager.Instance.SpawnedParent);
        }
    }

    public void RemoveBlockAt(int index)
    {
        if (index >= blockParent.childCount) return;
        var dirBlock = blockParent.GetChild(index);
        dirBlock.gameObject.SetActive(true);
        dirBlock.SetParent(LevelManager.Instance.SpawnedParent);
    }

    public void SetColorValue(int colorValue)
    {
        throw new System.NotImplementedException();
    }

    public void SetDirectionValue(DirectionValue directionValue)
    {
        if (directionValue == DirectionValue.Right)
        {
            Direction = new int2(1, 0);
            bodyRenderer.sprite = RendererSystem.Instance.GetTunnelSprite(0);
        }
        else if (directionValue == DirectionValue.Up)
        {
            Direction = new int2(0, 1);
            bodyRenderer.sprite = RendererSystem.Instance.GetTunnelSprite(1);
        }
        else if (directionValue == DirectionValue.Left)
        {
            Direction = new int2(-1, 0);
            bodyRenderer.sprite = RendererSystem.Instance.GetTunnelSprite(2);
        }
        else if (directionValue == DirectionValue.Down)
        {
            Direction = new int2(0, -1);
            bodyRenderer.sprite = RendererSystem.Instance.GetTunnelSprite(3);
        }
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public SpriteRenderer GetBodyRenderer()
    {
        return bodyRenderer;
    }

    public int GetSortingOrder()
    {
        return bodyRenderer.sortingOrder;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        bodyRenderer.sortingOrder = sortingOrder;
        foreach (Transform child in blockParent)
        {
            if (!child.TryGetComponent(out ISpriteRend spriteRend)) continue;
            spriteRend.SetSortingOrder(sortingOrder - Direction.y);
        }
    }
}
