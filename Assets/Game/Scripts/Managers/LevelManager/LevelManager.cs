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
    FindNeedArrangeCollumns();
    SpawnColorBlocksUpdate();
    FindNeedArrangeCollumnAndUpdate();
    WaitAndFindMatchedUpdate();
    BulletPositionsUpdate();
    LockAndFireTargetUpddate();
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
    var initWoodenBlocks = levelInformation.InitWoodenBlocks;
    var initIceBlock = levelInformation.InitIceBlocks;
    var initTunnels = levelInformation.InitTunnels;

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

  float3 Lerp(float3 start, float3 end, float t)
  {
    return (1 - t) * start + t * end;
  }

  void InterpolateMoveUpdate(
    in float3 currentPos,
    in float3 startPos,
    in float3 targetPos,
    in float speed,
    out float _t,
    out float3 nextPos
  )
  {
    var distanceFromStart = math.length(currentPos - startPos);
    var totalDistance = ((Vector3)targetPos - (Vector3)startPos).magnitude;
    var t = distanceFromStart / totalDistance + speed * 1 / totalDistance * Time.deltaTime;
    _t = math.min(t, 1);
    nextPos = Lerp(startPos, targetPos, _t);
  }

  bool IsPosOccupiedAt(float3 pos)
  {
    if (bottomGrid.IsPosOutsideAt(pos)) return true;
    var idx = bottomGrid.ConvertWorldPosToIndex(pos);
    if (_directionBlocks[idx] != null) return true;
    return false;
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
    for (int i = 0; i < slots.Count; ++i)
    {
      if (slots[i] == null) return i;
    }
    return slots.Count;
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
}
