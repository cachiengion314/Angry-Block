using Unity.Mathematics;
using UnityEngine;

public class TunnelControl : MonoBehaviour, ITrigger, IDirectionBlock, IColorBlock
{
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] Transform blockParent;
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
            if (blockParent.childCount > 0)
            {
                var dirBlock = blockParent.GetChild(0);
                if (!dirBlock.TryGetComponent(out IColorBlock colorBlock)) return;
                dirBlock.gameObject.SetActive(true);
                LevelManager.Instance.SetDirectionBlocks(colorBlock.GetIndex(), dirBlock.gameObject);
                dirBlock.SetParent(LevelManager.Instance.SpawnedParent);
            }
        }
    }

    public void SetColorValue(int colorValue)
    {
        throw new System.NotImplementedException();
    }

    public void SetDirectionValue(DirectionValue directionValue)
    {
        var angle90 = 90 / 2f * math.PI / 180f;
        if (directionValue == DirectionValue.Right)
        {
            Direction = new int2(1, 0);
            transform.rotation
              = new Quaternion(0, 0, math.sin(-angle90), math.cos(-angle90));
        }
        else if (directionValue == DirectionValue.Up)
        {
            Direction = new int2(0, 1);
            transform.rotation
              = new Quaternion(0, 0, math.sin(0), math.cos(0));
        }
        else if (directionValue == DirectionValue.Left)
        {
            Direction = new int2(-1, 0);
            transform.rotation
              = new Quaternion(0, 0, math.sin(angle90), math.cos(angle90));
        }
        else if (directionValue == DirectionValue.Down)
        {
            Direction = new int2(0, -1);
            transform.rotation
              = new Quaternion(0, 0, math.sin(2 * angle90), math.cos(2 * angle90));
        }
    }

    public void SetIndex(int index)
    {
        Index = index;
    }
}
