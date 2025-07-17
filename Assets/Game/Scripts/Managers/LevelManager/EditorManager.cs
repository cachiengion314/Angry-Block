using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public enum BlockType
{
  None,
  DirectionBlock,
  WoodenBlock,
  Tunnel,
  IceBlock
}

[Serializable]
public enum DirectionValue
{
  Right,
  Up,
  Left,
  Down,
}

[Serializable]
public class ColorBlockData
{
  public int ColorValue;
  [Range(1, 20)]
  public int Health = 1;
}

[Serializable]
public class DirectionBlockData
{
  public int Index;
  public int ColorValue;
  public DirectionValue DirectionValue;
  [Range(1, 20)]
  public int Ammunition = 5;
}

[Serializable]
public class TunnelData
{
  public int Index;
  public DirectionBlockData[] directionBlockDatas;
  public DirectionValue DirectionValue;
}

[Serializable]
public class LevelInformation
{
  [ViewOnly]
  public int Index;
  public int2 ColorBlocksGridSize;
  /// <summary>
  /// Should doing quad tree stuffs
  /// </summary>
  [Tooltip("Manual place color blocks in grid positions")]
  public ColorBlockData[] InitColorBlocks;
  public int2 DirectionBlocksGridSize;
  public DirectionBlockData[] InitDirectionBlocks;
  public DirectionBlockData[] InitWoodenBlocks;
  public DirectionBlockData[] InitIceBlocks;
  public TunnelData[] InitTunnels;
}

public partial class LevelManager : MonoBehaviour
{
  [Header("Level Editor")]
  [SerializeField] LevelInformation levelInformation;
  [SerializeField][Range(1, 20)] int levelSelected = 1;
  [SerializeField] bool isSelectedMatchedCurrentLevel;
  public bool IsSelectedMatchedCurrentLevel { get { return isSelectedMatchedCurrentLevel; } }

  [NaughtyAttributes.Button]
  void Clear()
  {
    levelInformation.Index = levelSelected - 1;
    levelInformation.ColorBlocksGridSize = new int2();
    levelInformation.InitColorBlocks = new ColorBlockData[0];
    levelInformation.DirectionBlocksGridSize = new int2();
    levelInformation.InitDirectionBlocks = new DirectionBlockData[0];
    levelInformation.InitTunnels = new TunnelData[0];
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
    print("Load level successfully");
  }

  [NaughtyAttributes.Button]
  void SaveLevel()
  {
    levelInformation.Index = levelSelected - 1;
    HoangNam.SaveSystem.Save(
      levelInformation,
      "Resources/Levels/" + KeyString.NAME_LEVEL_FILE + levelSelected
    );
    print("Save level successfully");
  }
}