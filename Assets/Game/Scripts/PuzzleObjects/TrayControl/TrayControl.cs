using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using HoangNam;

/// <summary>
/// Control tray
/// </summary>
[RequireComponent(typeof(DragAndDrop))]
public partial class TrayControl : MonoBehaviour, IPoolItemControl
{
  [Header("External dependencies")]
  ObjectPool<GameObject> cupPool;
  [SerializeField] DragAndDrop dragAndDrop;
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] Sprite[] traySprites;

  [Header("Internal dependencies")]
  [SerializeField] Collider2D col;
  public Collider2D Col { get { return col; } }
  [SerializeField] Transform lidParent;
  [SerializeField] Transform woodTrayParent;
  public Transform WoodTrayParent { get { return woodTrayParent; } }
  [SerializeField] Transform cupPosParent;
  public Transform CupPosParent { get { return cupPosParent; } }
  [SerializeField] Transform cupParent;
  public Transform CupParent { get { return cupParent; } }

  [Header("Setting")]
  float3 currPos;
  public float3 CurrentWorldPos;
  public float3 CurrDraggingDir { get; private set; }
  SpriteRenderer _renderer;
  int _originalSortingOrder;
  public bool IsPlaced;
  public int AvailableTrayIndex;

  private void Awake()
  {
    _renderer = GetComponentInChildren<SpriteRenderer>();
    _originalSortingOrder = _renderer.sortingOrder;
  }

  private void OnEnable()
  {
    currPos = new float3(1, 1, 1) * 999;
    col.enabled = true;
    _renderer.sortingOrder = _originalSortingOrder;
    transform.localScale = new float3(1, 1, 0);
    transform.eulerAngles = new float3(0, 0, 0);
    for (int i = 0; i < lidParent.childCount; ++i)
    {
      Destroy(lidParent.GetChild(i).gameObject);
    }

    ChangeSkinTrayTo(0);
    IsPlaced = false;
  }

  void Start()
  {
    dragAndDrop.onDragBegan += DragAndDrop_onDragBegan;
    dragAndDrop.onDragMoving += DragAndDrop_onDragMoving;
    dragAndDrop.onDropped += DragAndDrop_onDropped;
    dragAndDrop.onDetected += DragAndDrop_onDetected;
  }

  [BurstCompile]
  void Update()
  {
#if UNITY_EDITOR
    DrawGrabedBlock();
#endif
  }

  /// <summary>
  /// only for debug
  /// </summary>
  void DrawGrabedBlock()
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    int2 gridPos = trayGrid.ConvertWorldPosToGridPos(new float3(transform.position.x, transform.position.y, 0));
    float3 worldPos = trayGrid.ConvertGridPosToWorldPos(gridPos);
    if (trayGrid.IsPosOutsideAt(worldPos)) return;

    
  }

  void OnDestroy()
  {
    dragAndDrop.onDragBegan -= DragAndDrop_onDragBegan;
    dragAndDrop.onDragMoving -= DragAndDrop_onDragMoving;
    dragAndDrop.onDropped -= DragAndDrop_onDropped;
    dragAndDrop.onDetected -= DragAndDrop_onDetected;
  }

  void DragAndDrop_onDragBegan(float3 beginPos)
  {
    SoundManager.Instance.PlayPickedTraySfx();
    DisableWoodTrays();
  }

  void DragAndDrop_onDragMoving(float3 movingPos)
  {
    DisableWoodTrays();
  }

  void DragAndDrop_onDropped(float3 droppedPos)
  {

  }

  private void DragAndDrop_onDetected(float3 pos)
  {
    if (IsCupsTweening()) return;
    if (IsIceTrayAt(pos)) return;
    if (IsGrillerAt(pos)) return;
    if (IsCoverLetTrayAt(pos)) return;
    if (ItemManager.Instance.HasGiftBoxAt(pos)) return;
    if (ItemManager.Instance.HasCoffeeBoardAt(pos)) return;
    if (ItemManager.Instance.HasPlantPotAt(pos)) return;
    if (ItemManager.Instance.HasCurtainLayerAt(pos)) return;

    PowerItemPanel.Instance.TryPickSwappingTray(this);
    ItemManager.Instance.TrySelfRemovedByHammerAt(pos, this);

    if (PowerItemPanel.Instance.IsTriggeredHammer) return;
    if (PowerItemPanel.Instance.IsTriggeredRocket) return;
    if (PowerItemPanel.Instance.IsTriggerSwap) return;
    if (PowerItemPanel.Instance.IsTriggerRefresh) return;
    DisableWoodTrays();
  }

  public bool IsCupsTweening()
  {
    for (int i = 0; i < cupParent.childCount; ++i)
      if (LeanTween.isTweening(cupParent.GetChild(i).gameObject)) return true;
    return false;
  }

  private bool IsIceTrayAt(float3 pos)
  {
    var iceTray = ItemManager.Instance.FindIceTray(pos);

    if (iceTray != null)
    {
      return true;
    }

    return false;
  }

  private bool IsCoverLetTrayAt(float3 pos)
  {
    var coverLetTray = ItemManager.Instance.FindCoverLetTray(pos);

    if (coverLetTray != null)
    {
      return true;
    }

    return false;
  }

  private bool IsGrillerAt(float3 pos)
  {
    var griller = ItemManager.Instance.GetGrillerAt(pos);

    if (griller != null)
    {
      return true;
    }

    return false;
  }

  public ColorIndex GetDelegateColor()
  {
    if (cupParent.childCount == 0) return ColorIndex.None;
    return cupParent.GetChild(0).GetComponent<CupControl>().ColorIndex;
  }

  public void SpawnCups(ColorIndex[] colors = null, int _amount = 0, bool _isStaticCups = false)
  {
    if (cupParent.childCount > 0) return;
    bool shouldChangeIndex
      = UnityEngine.Random.Range(0f, 1f) < ItemManager.Instance.CupsMixedRate / 100f;
    if (_isStaticCups)
    {
      shouldChangeIndex = true;
    }
  }

  public void SpawnDefaultCupsFrom(ColorIndex[] colors, int _startId = 0)
  {
    for (int i = _startId; i < (colors.Length + _startId); ++i)
    {
      var colorIdx = (int)colors[i - _startId];

      var cup = cupPool.Get().GetComponent<CupControl>();
      cup.InjectPool(cupPool);
      cup.SetColorIndex((ColorIndex)colorIdx);

      cup.InitPostAt(
        cupPosParent.GetChild(i).position,
        (1 - dragAndDrop.PairIndex) * cupPosParent.childCount + -10 + i + 1
      );
      cup.transform.SetParent(cupParent);
      cup.transform.localRotation = Quaternion.identity;
    }
  }

  public void SpawnLidsWithAnim()
  {
    for (int i = 0; i < cupParent.childCount; ++i)
    {
      var cupPos = cupParent.GetChild(i).position;
      var cup = cupParent.GetChild(i);

      var lidPos = cupParent.GetChild(i).position + Vector3.up * .17f;
      var lid = EffectManager.Instance.SpawnLidAt(lidPos);
      lid.transform.localScale = new Vector3(0, 0, 0);

      LeanTween.delayedCall(gameObject, i * .05f, () =>
      {
        LeanTween.move(cup.gameObject, cupPos + Vector3.up * .4f, .2f).setLoopPingPong(1).setEaseLinear();
        LeanTween.scale(cup.gameObject, new Vector3(1, 1.4f, 1), .2f).setLoopPingPong(1).setEaseLinear();

        LeanTween.move(lid, lidPos + Vector3.up * .5f, .2f).setLoopPingPong(1).setEaseLinear();
        LeanTween.scale(lid, new Vector3(1, 1, 1), .4f).setEaseLinear();
      });
      lid.transform.SetParent(lidParent);
    }
  }

  public LTDescr PackingAndMoveOutFromTableWithAnim(GameObject animator)
  {
    return LeanTween.move(
        animator,
        animator.transform.position + Vector3.up * 2.5f,
        ItemManager.Instance.SlowFactor * 5.8f
      )
        .setEaseOutBack()
        .setOnComplete(() =>
        {
          LeanTween.rotateZ(animator,
            -10,
            ItemManager.Instance.SlowFactor * 2
          ).setOnComplete(
            () =>
            {
              LeanTween.rotateZ(animator,
                20,
                ItemManager.Instance.SlowFactor * 3
              );
            }
          );

          LeanTween.move(
            animator,
            animator.transform.position + Vector3.right * 5f,
            ItemManager.Instance.SlowFactor * 5.8f
          )
          .setEase(LeanTweenType.easeInBack)
          .setOnComplete(() =>
          {
            transform.SetParent(null);
            ClearCupImgs();
            Release();
            Destroy(animator);

            ItemManager.Instance.IsPacking = false;
          });
        });
  }

  public void ClearCupsGridWorldValue()
  {
    for (int i = 0; i < cupParent.childCount; ++i)
    {
      var cup = cupParent.GetChild(i).GetComponent<CupControl>();
      cup.RemoveFromTable();
    }
  }

  public bool IsFullOfCups()
  {
    return cupParent.childCount == cupPosParent.childCount;
  }

  public bool IsEmpty()
  {
    return cupParent.childCount == 0;
  }

  public List<float3> FindCupPositions()
  {
    List<float3> l = new();
    for (int i = 0; i < cupPosParent.childCount; ++i)
    {
      l.Add(cupPosParent.GetChild(i).position);
    }
    return l;
  }

  public List<CupControl> FindCups()
  {
    List<CupControl> cups = new();
    foreach (Transform cupChild in cupParent)
    {
      cups.Add(cupChild.GetComponent<CupControl>());
    }
    return cups;
  }

  public void ClearCupImgs()
  {
    List<Transform> list = new();
    foreach (Transform cup in cupParent)
    {
      list.Add(cup);
    }
    foreach (Transform cup in list)
    {
      cup.SetParent(null);
      cup.GetComponent<CupControl>().Release();
    }
  }

  public void EnableWoodTrays()
  {
    for (int i = 0; i < woodTrayParent.childCount; ++i)
      woodTrayParent.GetChild(i).gameObject.SetActive(true);
  }

  public void DisableWoodTrays()
  {
    for (int i = 0; i < woodTrayParent.childCount; ++i)
      woodTrayParent.GetChild(i).gameObject.SetActive(false);
  }

  public void ClearWoodTrays()
  {
    for (int i = 0; i < woodTrayParent.childCount; ++i)
      Destroy(woodTrayParent.GetChild(0).gameObject);
  }

  public void ClearPlacementInAvailableTrays()
  {

  }

  public int GetCupSortingOrder()
  {
    if (cupParent.childCount == 0) return -1;
    return cupParent.GetChild(0).GetComponent<CupControl>().Renderer.sortingOrder;
  }

  public bool HasNeighbors()
  {
    return (dragAndDrop.NeighborTrays != null) && (dragAndDrop.NeighborTrays.Length > 0);
  }

  public bool IsNeighborWith(TrayControl otherTray)
  {
    if (GetInstanceID() == otherTray.GetInstanceID()) return true;
    var neighbors = FindNeighborTraysAt(transform.position);
    for (int i = 0; i < neighbors.Count; ++i)
    {
      TrayControl neighbor = neighbors[i];
      if (otherTray.GetInstanceID() == neighbor.GetInstanceID()) return true;
    }
    return false;
  }

  public void SetUpOrderMoveUp(int id)
  {
    _renderer.sortingOrder = 999 + id * 5;

    for (int i = 0; i < cupParent.childCount; ++i)
    {
      var cup = cupParent.GetChild(i).GetComponentInChildren<SpriteRenderer>();

      if (i > 2)
      {
        cup.sortingOrder = 1001 + id * 5;
        continue;
      }

      cup.sortingOrder = 1000 + id * 5;
    }
  }

  /// <summary>
  /// IndexSkin: (0: normal, 1: golden)
  /// </summary>
  /// <param name="indexSkin"></param> <summary>
  /// 
  /// </summary>
  /// <param name="indexSkin"></param>
  public void ChangeSkinTrayTo(int indexSkin)
  {
    bodyRenderer.sprite = traySprites[indexSkin];
  }

  #region Pool Item
  public void InjectPool(ObjectPool<GameObject> trayPool, ObjectPool<GameObject> cupPool)
  {
    this.cupPool = cupPool;
  }

  public void RemoveFromTable()
  {
    ClearCupsGridWorldValue();
    ClearCupImgs();
    Release();
  }

  public LTDescr RemoveFromTableWithAnim()
  {
    return ItemManager.Instance.ScaleTrayTo(
        float3.zero, gameObject, ItemManager.Instance.SlowFactor * 3.3f,
        () =>
        {
          ClearCupImgs();
          Release();
        }).setEaseInBack();
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      if (dragAndDrop.NeighborTrays != null) dragAndDrop.NeighborTrays = null;
      dragAndDrop.PairIndex = 0;
      ClearWoodTrays();
    }
  }

  public void FullyRemoveFromTable()
  {
    RemoveFromTable();
  }

  #endregion
}