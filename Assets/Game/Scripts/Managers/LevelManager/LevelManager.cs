using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }
  [Header("Level Manager")]
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
    WaitAndFindMatchedUpdate();
    ReArrangeTopGridUpdate();
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
    for (int i = 0; i < bottomGrid.Grid.Length; ++i)
    {
      if (i > initDirectionBlocks.Length - 1) break;
      if (initDirectionBlocks[i] == null) continue;

      var directionBlock = SpawnDirectionBlockAt(i, spawnedParent);
      directionBlock.SetIndex(i);
      directionBlock.SetColorValue(initDirectionBlocks[i].ColorValue);
      directionBlock.SetDirectionValue(initDirectionBlocks[i].DirectionValue);
      directionBlock.SetAmmunition(initDirectionBlocks[i].Ammunition);
      _directionBlocks[i] = directionBlock.gameObject;

      if (initDirectionBlocks[i].IsHidden)
        directionBlock.gameObject.AddComponent<WoodenBlockControl>();

      if (directionBlock.TryGetComponent(out IInitialize initBlock))
        initBlock.Initialize(0);
    }

    for (int i = 0; i < initTunnels.Length; i++)
    {
      var tunnelData = initTunnels[i];
      var tunnel = SpawnTunnelAt(tunnelData.Index, spawnedParent);
      tunnel.Initialize(tunnelData);
      _directionBlocks[tunnelData.Index] = tunnel.gameObject;
    }

    InitWaitingSlots();
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

  float3 Lerp(float3 start, float3 end, float t)
  {
    return (1 - t) * start + t * end;
  }

  void InterpolateMoveUpdate(
    in float3 currentPos,
    in float3 startPos,
    in float3 targetPos,
    in float speed,
    out float t,
    out float3 nextPos
  )
  {
    var distanceFromStart = math.length(currentPos - startPos);
    var totalDistance = ((Vector3)targetPos - (Vector3)startPos).magnitude;
    t = distanceFromStart / totalDistance + speed * 1 / totalDistance * Time.deltaTime;
    nextPos = Lerp(startPos, targetPos, t);
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
}
