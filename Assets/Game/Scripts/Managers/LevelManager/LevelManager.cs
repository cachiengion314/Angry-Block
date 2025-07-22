using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }
  [Header("Level Manager")]
  LevelInformation levelInformation;
  [SerializeField][Range(1, 20)] int levelSelected = 1;
  [Header("Dependencies")]
  [SerializeField] Transform spawnedParent;
  public Transform SpawnedParent => spawnedParent;
  [Header("Grids")]
  [SerializeField] GridWorld topGrid;
  [SerializeField] GridWorld bottomGrid;
  public GridWorld BottomGrid => bottomGrid;
  [Range(0f, 2f)]
  [SerializeField] float updateSpeed;

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
  }

  void Update()
  {
    FindNeedArrangeCollumnInUpdate();
    ArrangeColorBlocksUpdate();
    WaitAndFindMatchedUpdate();
    LockAndFireTargetUpddate();
    UpdateWinLevel();
    BulletPositionsUpdate();
    UpdateLoseLevel();
    MovesToWaitingUpdate();
  }

  void OnDestroy()
  {
    UnsubscribeTouchEvent();

    topGrid.DisposeGridWorld();
    bottomGrid.DisposeGridWorld();
    bottomGrid.DisposePathFinding();
  }

  int ConvertPercentToIdx(int percentInt, int gridSize)
  {
    return (int)math.floor(percentInt / 100.0f * gridSize);
  }

  void SetupCurrentLevel()
  {
    topGrid.transform.position = levelInformation.ColorBlocksGridPosition;
    bottomGrid.transform.position = levelInformation.DirectionBlocksGridPosition;
    topGrid.GridSize = levelInformation.ColorBlocksGridSize;
    topGrid.transform.position = levelInformation.ColorBlocksGridPosition;
    bottomGrid.GridSize = levelInformation.DirectionBlocksGridSize;
    bottomGrid.transform.position = levelInformation.DirectionBlocksGridPosition;
    topGrid.BakingGridWorld();
    bottomGrid.BakingGridWorld();
    bottomGrid.BakingPathFinding();

    var colorBlockPartitionDatas = levelInformation.ColorBlockPartitionDatas;
    var initDirectionBlocks = levelInformation.InitDirectionBlocks;
    var initWoodenBlocks = levelInformation.InitWoodenBlocks;
    var initIceBlock = levelInformation.InitIceBlocks;
    var initTunnels = levelInformation.InitTunnels;

    _colorBlocks = new ColorBlockControl[topGrid.Grid.Length];

    for (int i = 0; i < colorBlockPartitionDatas.Length; ++i)
    {
      var partition = colorBlockPartitionDatas[i];
      var percentInX = partition.PercentInX;
      var percentInY = partition.PercentInY;

      var startX = ConvertPercentToIdx(percentInX.x, levelInformation.ColorBlocksGridSize.x);
      var endX = ConvertPercentToIdx(percentInX.y, levelInformation.ColorBlocksGridSize.x);
      var startY = ConvertPercentToIdx(percentInY.x, levelInformation.ColorBlocksGridSize.y);
      var endY = ConvertPercentToIdx(percentInY.y, levelInformation.ColorBlocksGridSize.y);

      for (int y = startY; y < endY; ++y)
      {
        for (int x = startX; x < endX; ++x)
        {
          var gridPos = new int2(x, y);
          var index = topGrid.ConvertGridPosToIndex(gridPos);
          var colorBlock = SpawnColorBlockAt(index, spawnedParent);
          colorBlock.SetIndex(index);
          colorBlock.SetColorValue(partition.ColorValue);
          colorBlock.SetInitHealth(partition.Health);

          if (!IsAtVisibleBound(colorBlock.gameObject))
            colorBlock.gameObject.SetActive(false);

          _colorBlocks[index] = colorBlock;
        }
      }
    }

    _directionBlocks = new GameObject[bottomGrid.Grid.Length];
    for (int i = 0; i < initDirectionBlocks.Length; ++i)
    {
      if (initDirectionBlocks[i] == null) continue;

      var directionBlock = SpawnDirectionBlockAt(initDirectionBlocks[i].Index, spawnedParent);
      directionBlock.SetIndex(initDirectionBlocks[i].Index);
      directionBlock.SetColorValue(initDirectionBlocks[i].ColorValue);
      directionBlock.SetDirectionValue(initDirectionBlocks[i].DirectionValue);
      directionBlock.SetAmmunition(initDirectionBlocks[i].Ammunition);
      _directionBlocks[initDirectionBlocks[i].Index] = directionBlock.gameObject;
    }

    for (int i = 0; i < initWoodenBlocks.Length; ++i)
    {
      var woodenData = initWoodenBlocks[i];
      if (woodenData == null) continue;

      var woodenBlock = SpawnWoondenBlockAt(woodenData.Index, spawnedParent);
      woodenBlock.SetIndex(woodenData.Index);
      woodenBlock.Initialize(woodenData);
      _directionBlocks[woodenData.Index] = woodenBlock.gameObject;
    }

    for (int i = 0; i < initIceBlock.Length; ++i)
    {
      var iceData = initIceBlock[i];
      if (iceData == null) continue;

      var iceBlock = SpawnIceBlockAt(iceData.Index, spawnedParent);
      iceBlock.SetIndex(iceData.Index);
      iceBlock.Initialize(iceData);
      _directionBlocks[iceData.Index] = iceBlock.gameObject;
    }

    for (int i = 0; i < initTunnels.Length; i++)
    {
      var tunnelData = initTunnels[i];
      if (tunnelData == null) continue;

      var tunnel = SpawnTunnelAt(tunnelData.Index, spawnedParent);
      tunnel.SetDirectionValue(tunnelData.DirectionValue);
      tunnel.SetIndex(tunnelData.Index);
      tunnel.Initialize(tunnelData.directionBlockDatas);
      _directionBlocks[tunnelData.Index] = tunnel.gameObject;
    }

    InitWaitingSlots();
    GameManager.Instance.SetGameState(GameState.Gameplay);
  }

  public Collider2D FindObjIn<T>(Collider2D[] cols)
  {
    for (int i = 0; i < cols.Length; ++i)
    {
      if (cols[i] == null) continue;
      if (cols[i].TryGetComponent<T>(out var comp))
      {
        return cols[i];
      }
    }
    return default;
  }

  int FindSlotFor(GameObject block, GameObject[] slots)
  {
    for (int i = 0; i < slots.Length; ++i)
    {
      if (slots[i] == null) continue;
      if (slots[i] == block) return i;
    }
    return -1;
  }

  int FindSlotFor(GameObject block, List<GameObject> slots)
  {
    for (int i = 0; i < slots.Count; ++i)
    {
      if (slots[i] == null) continue;
      if (slots[i] == block) return i;
    }
    return -1;
  }

  int FindEmptySlotFrom(GameObject[] slots)
  {
    for (int i = 0; i < slots.Length; ++i)
    {
      if (slots[i] == null) return i;
    }
    return -1;
  }

  int FindEmptySlotFrom(List<GameObject> slots)
  {
    return slots.Count;
  }

  void InterpolatePathUpdate(
    in float3 currentPos,
    in int currentIdx,
    in float3[] path,
    in float speed,
    out float _t,
    out float3 nextPos,
    out int nextIdx
  )
  {
    _t = 1;
    nextPos = currentPos;
    nextIdx = currentIdx;
    if (currentIdx > path.Length - 1) return;

    var startPos = path[currentIdx];
    var targetIdx = currentIdx + 1;
    if (targetIdx > path.Length - 1) return;

    var targetPos = path[targetIdx];

    HoangNam.Utility.InterpolateMoveUpdate(
      currentPos, startPos, targetPos, speed, out var percent, out var nextPosition
    );
    _t = (currentIdx + percent) / math.max(path.Length - 1, 1);
    nextPos = nextPosition;

    if (percent < 1)
    {
      nextIdx = currentIdx;
      return;
    }

    nextIdx = currentIdx + 1;
  }

  public void RestartLevel()
  {
    SceneManager.LoadScene(KeyString.NAME_SCENE_GAMEPLAY);
  }

  public void LoadLevelFrom(int level)
  {
    var _rawLevelInfo = Resources.Load<TextAsset>("Levels/" + KeyString.NAME_LEVEL_FILE + level).text;
    var levelInfo = JsonUtility.FromJson<LevelInformation>(_rawLevelInfo);

    if (levelInfo == null) { print("This level is not existed!"); return; }
    levelInformation = levelInfo;
    print("Load level " + level + " successfully ");
  }

  void UpdateLoseLevel()
  {
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (IsWaitingSlotsMMoving()) return;
    var emptyWaitingSlot = FindEmptySlotFrom(_waitingSlots);
    if (emptyWaitingSlot != -1) return;
    GameManager.Instance.SetGameState(GameState.Gameover);
    Debug.Log("Lose");
  }

  void UpdateWinLevel()
  {
    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
    if (!IsBoardClear()) return;
    GameManager.Instance.SetGameState(GameState.Gamewin);
    Debug.Log("Win");
  }

  bool IsBoardClear()
  {
    for (int i = 0; i < _colorBlocks.Length; i++)
      if (_colorBlocks[i] != null) return false;
    return true;
  }
}
