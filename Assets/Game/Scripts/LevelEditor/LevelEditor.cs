using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    [Header("Level Editor")]
    [SerializeField] BlockEditor blockEditorPref;
    [SerializeField] GridWorld gridWorld;
    [SerializeField] BlockEditor[] tiles;
    LevelInformation levelInformation;
    [SerializeField][Range(1, 20)] int levelSelected = 1;

    [NaughtyAttributes.Button]
    void CreateGird()
    {
        ClearGrid();
        gridWorld.BakingGridWorld();
        var length = gridWorld.GridSize.x * gridWorld.GridSize.y;
        tiles = new BlockEditor[length];
        for (int i = 0; i < length; i++)
        {
            var tile = Instantiate(blockEditorPref, transform);

            var pos = gridWorld.ConvertIndexToWorldPos(i);
            tile.transform.position = pos;

            tiles[i] = tile;
        }
    }

    void ClearGrid()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    void LoadDirectionBlocks()
    {
        var DirectionBlockDatas = levelInformation.InitDirectionBlocks;
        for (int i = 0; i < DirectionBlockDatas.Length; i++)
        {
            var tile = tiles[DirectionBlockDatas[i].Index];
            tile.blockType = BlockType.DirectionBlock;
            tile.directionBlockData = DirectionBlockDatas[i];
            tile.OnValidate();
        }
    }

    void LoadWoodenBlocks()
    {
        var WoondenBlockDatas = levelInformation.InitWoodenBlocks;
        for (int i = 0; i < WoondenBlockDatas.Length; i++)
        {
            var tile = tiles[WoondenBlockDatas[i].Index];
            tile.blockType = BlockType.WoodenBlock;
            tile.directionBlockData = WoondenBlockDatas[i];
            tile.OnValidate();
        }
    }

    void LoadIceBlocks()
    {
        var IceBlockDatas = levelInformation.InitIceBlocks;
        for (int i = 0; i < IceBlockDatas.Length; i++)
        {
            var tile = tiles[IceBlockDatas[i].Index];
            tile.blockType = BlockType.IceBlock;
            tile.directionBlockData = IceBlockDatas[i];
            tile.OnValidate();
        }
    }

    void LoadTunnel()
    {
        var TunnelDatas = levelInformation.InitTunnels;
        for (int i = 0; i < TunnelDatas.Length; i++)
        {
            var tile = tiles[TunnelDatas[i].Index];
            tile.blockType = BlockType.Tunnel;
            tile.tunnelData = TunnelDatas[i];
            tile.OnValidate();
        }
    }

    void SaveDirectionBlocks()
    {
        List<DirectionBlockData> InitDirectionBlocks = new();
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile.blockType != BlockType.DirectionBlock) continue;
            tile.directionBlockData.Index = i;
            InitDirectionBlocks.Add(tile.directionBlockData);
        }
        levelInformation.InitDirectionBlocks = InitDirectionBlocks.ToArray();
    }

    void SaveWoodenBlocks()
    {
        List<DirectionBlockData> InitWoodenBlocks = new();
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile.blockType != BlockType.WoodenBlock) continue;
            tile.directionBlockData.Index = i;
            InitWoodenBlocks.Add(tile.directionBlockData);
        }
        levelInformation.InitWoodenBlocks = InitWoodenBlocks.ToArray();
    }

    void SaveIceBlocks()
    {
        List<DirectionBlockData> InitIceBlocks = new();
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile.blockType != BlockType.IceBlock) continue;
            tile.directionBlockData.Index = i;
            InitIceBlocks.Add(tile.directionBlockData);
        }
        levelInformation.InitIceBlocks = InitIceBlocks.ToArray();
    }

    void SaveTunnelBlocks()
    {
        List<TunnelData> InitTunnels = new();
        for (int i = 0; i < tiles.Length; i++)
        {
            var tile = tiles[i];
            if (tile.blockType != BlockType.Tunnel) continue;
            tile.tunnelData.Index = i;
            InitTunnels.Add(tile.tunnelData);
        }
        levelInformation.InitTunnels = InitTunnels.ToArray();
    }

    [NaughtyAttributes.Button]
    void Clear()
    {
        ClearGrid();
        levelInformation = new LevelInformation();
    }

    [NaughtyAttributes.Button]
    void LoadLevel()
    {
        LoadLevelFrom(levelSelected);
    }

    public void LoadLevelFrom(int level)
    {
        var _rawLevelInfo = Resources.Load<TextAsset>("Levels/" + KeyString.NAME_LEVEL_FILE + level).text;
        var levelInfo = JsonUtility.FromJson<LevelInformation>(_rawLevelInfo);

        if (levelInfo == null) { print("This level is not existed!"); return; }
        levelInformation = levelInfo;

        gridWorld.GridSize = levelInformation.DirectionBlocksGridSize;
        CreateGird();
        LoadDirectionBlocks();
        LoadWoodenBlocks();
        LoadIceBlocks();
        LoadTunnel();

        print("Load level successfully");
    }

    [NaughtyAttributes.Button]
    void SaveLevel()
    {
        levelInformation = new();
        levelInformation.Index = levelSelected - 1;
        levelInformation.DirectionBlocksGridSize = gridWorld.GridSize;
        SaveDirectionBlocks();
        SaveWoodenBlocks();
        SaveIceBlocks();
        SaveTunnelBlocks();

        HoangNam.SaveSystem.Save(
          levelInformation,
          "Resources/Levels/" + KeyString.NAME_LEVEL_FILE + levelSelected
        );
        print("Save level successfully");
    }
}
