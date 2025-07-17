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
            if(DirectionBlockDatas[i] == null) continue;

            var tile = tiles[i];
            tile.blockType = BlockType.DirectionBlock;
            tile.ColorValue = DirectionBlockDatas[i].ColorValue;
            tile.DirectionValue = DirectionBlockDatas[i].DirectionValue;
            tile.Ammunition = DirectionBlockDatas[i].Ammunition;
            tile.OnValidate();
        }
    }

    void SaveDirectionBlocks()
    {
        levelInformation.InitDirectionBlocks = new DirectionBlockData[tiles.Length];
        for (int i = 0; i < levelInformation.InitDirectionBlocks.Length; i++)
        {
            var tile = tiles[i];
            if (tile.blockType != BlockType.DirectionBlock) continue;
            var directionBlockData = new DirectionBlockData()
            {
                ColorValue = tile.ColorValue,
                DirectionValue = tile.DirectionValue,
                Ammunition = tile.Ammunition,
            };
            levelInformation.InitDirectionBlocks[i] = directionBlockData;
        }
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

        print("Load level successfully");
    }

    [NaughtyAttributes.Button]
    void SaveLevel()
    {
        levelInformation.Index = levelSelected - 1;
        SaveDirectionBlocks();

        HoangNam.SaveSystem.Save(
          levelInformation,
          "Resources/Levels/" + KeyString.NAME_LEVEL_FILE + levelSelected
        );
        print("Save level successfully");
    }
}
