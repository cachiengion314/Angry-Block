using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public enum Direction
{
  Right,
  Up,
  Left,
  Down,
}

[Serializable]
public class DirectionBlockData
{
  public int ColorValue;
  public Direction Direction;
}

[Serializable]
public class LevelInformation
{
  public int Index;
  public int2 ColorBlocksGridSize;
  /// <summary>
  /// Should doing quad tree stuffs
  /// </summary>
  [Tooltip("Manual place color blocks in grid positions")]
  public int[] InitColorBlocks;
  public int2 DirectionBlocksGridSize;
  public DirectionBlockData[] InitDirectionBlocks;
}

public partial class LevelManager : MonoBehaviour
{
  [Header("Level Editor")]
  [SerializeField] LevelInformation levelInformation;

  [Header("Level Editor")]
  [SerializeField][Range(1, 20)] int levelSelected = 1;
  [SerializeField] bool isSelectedMatchedCurrentLevel;
  public bool IsSelectedMatchedCurrentLevel { get { return isSelectedMatchedCurrentLevel; } }

  [NaughtyAttributes.Button]
  void Clear()
  {
    levelInformation.Index = levelSelected - 1;
    levelInformation.ColorBlocksGridSize = new int2();
    levelInformation.InitColorBlocks = new int[0];
    levelInformation.DirectionBlocksGridSize = new int2();
    levelInformation.InitDirectionBlocks = new DirectionBlockData[0];
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