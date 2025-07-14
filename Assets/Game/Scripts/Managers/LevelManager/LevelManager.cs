using System.Collections.Generic;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }

  [Header("Dependencies")]
  [SerializeField] Transform spawnedParent;
  [Header("Grids")]
  [SerializeField] GridWorld topGrid;
  [SerializeField] GridWorld bottomGrid;
  Dictionary<string, bool> _runningAnimations;

  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else Destroy(gameObject);

    SubscribeTouchEvent();

    InitPool();
    LoadLevelFrom(levelSelected);
    SetupCurrentLevel();

    _runningAnimations = new Dictionary<string, bool>();
  }

  void Update()
  {
    ReArrangeTopGridUpdate();
    BulletPositionsUpdate();
    LockAndFireUpddate();
  }

  void OnDestroy()
  {
    UnsubscribeTouchEvent();

    topGrid.DisposeGridWorld();
    bottomGrid.DisposeGridWorld();
    bottomGrid.DisposePathFinding();
  }

  void SetupCurrentLevel()
  {
    topGrid.GridSize = levelInformation.ColorBlocksGridSize;
    bottomGrid.GridSize = levelInformation.DirectionBlocksGridSize;
    topGrid.BakingGridWorld();
    bottomGrid.BakingGridWorld();
    bottomGrid.BakingPathFinding();

    var initColorBlocks = levelInformation.InitColorBlocks;
    var initDirectionBlocks = levelInformation.InitDirectionBlocks;

    _colorBlocks = new ColorBlockControl[topGrid.Grid.Length];
    for (int i = 0; i < topGrid.Grid.Length; ++i)
    {
      if (i > initColorBlocks.Length - 1) break;
      if (initColorBlocks[i] == null) continue;

      var colorBlock = SpawnColorBlockAt(i, spawnedParent);
      colorBlock.SetIndex(i);
      colorBlock.SetColorValue(initColorBlocks[i].ColorValue);
      colorBlock.SetInitHealth(initColorBlocks[i].Health);

      _colorBlocks[i] = colorBlock;
    }

    _directionBlocks = new DirectionBlockControl[bottomGrid.Grid.Length];
    for (int i = 0; i < bottomGrid.Grid.Length; ++i)
    {
      if (i > initDirectionBlocks.Length - 1) break;
      if (initDirectionBlocks[i] == null) continue;

      var directionBlock = SpawnDirectionBlockAt(i, spawnedParent);
      directionBlock.SetIndex(i);
      directionBlock.SetColorValue(initDirectionBlocks[i].ColorValue);
      directionBlock.SetDirectionValue(initDirectionBlocks[i].DirectionValue);
      directionBlock.SetAmmunition(initDirectionBlocks[i].Ammunition);

      _directionBlocks[i] = directionBlock;
    }

    InitFiringPositions();
    InitWaitingPositions();
  }

  public T FindObjIn<T>(Collider2D[] cols)
  {
    for (int i = 0; i < cols.Length; ++i)
    {
      if (cols[i] == null) continue;
      if (cols[i].TryGetComponent<T>(out var comp))
      {
        return comp;
      }
    }
    return default;
  }
}
