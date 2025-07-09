using UnityEngine;
using System;
using Unity.Mathematics;
using System.Collections.Generic;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DragAndDrop : MonoBehaviour
{
  [Header("Components")]
  private BoxCollider2D _collider;
  private Rigidbody2D _rig;
  [SerializeField] DOTweenAnimation dOTweenAnimation;
  public Rigidbody2D Rig { get { return _rig; } }
  [SerializeField] TrayControl tray;

  [Header("Events")]
  public Action<float3> onDropped;
  public Action<float3> onDragBegan;
  public Action<float3> onDragMoving;
  public Action<float3> onDetected;

  [Header("Settings")]
  [SerializeField] float3 dragOffset;
  float3 _nextPos;
  public Vector2 OriginalColliderSize = new(.65f, .65f);

  [Header("Pair")]
  public DragAndDrop[] NeighborTrays;
  public int PairIndex; // PairIndex == 0 meaning root pair

  private bool _isDragging = false;

  void Start()
  {
    _collider = GetComponent<BoxCollider2D>();
    _rig = GetComponent<Rigidbody2D>();

    OriginalColliderSize = _collider.size;
  }

  void Update()
  {
    DragDropControl();
  }

  bool IsDragAble()
  {
    if (!GameManager.Instance.IsDisableTutorial)
    {
      if (GameManager.Instance.CurrentLevel == 0)
      {

      }
      else if (GameManager.Instance.CurrentLevel == 12)
      {
        if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_REFRESH, 0) == 0)
        {
          return false;
        }
      }
      else if (GameManager.Instance.CurrentLevel == 4)
      {
        if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER, 0) == 0)
        {
          return false;
        }
      }
      else if (GameManager.Instance.CurrentLevel == 7)
      {
        if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, 0) == 0)
        {
          return false;
        }
      }
      else if (GameManager.Instance.CurrentLevel == 9)
      {
        if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_SWAP, 0) == 0)
        {
          return false;
        }
      }
    }

    if (GameManager.Instance.GetGameState() != GameState.Gameplay) return false;
    if (_collider.GetComponent<TrayControl>().IsPlaced) return false;
    if (LeanTween.isTweening(_collider.gameObject)) return false;
    // When user trigger hammer or rocket
    // we should prevent player from dragging trays
    if (PowerItemPanel.Instance.IsTriggeredHammer) return false;
    if (PowerItemPanel.Instance.IsTriggeredRocket) return false;
    if (PowerItemPanel.Instance.IsTriggerSwap) return false;
    return true;
  }

  void DragDropControl()
  {
    if (GameManager.Instance.GetGameState() == GameState.GamepPause)
    {
      TouchRelease();
    }


    if (Input.touchCount <= 0)
    {
      TouchRelease();
    }

    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

      if (BalloonSystem.Instance.HasBalloonAtTouch() && !_isDragging) return;

      switch (touch.phase)
      {
        case TouchPhase.Began:
          Collider2D[] cols = Physics2D.OverlapPointAll(touchPos);

          foreach (Collider2D col in cols)
          {
            if (_collider == col)
            {
              _isDragging = true;
              onDetected?.Invoke(transform.position);
              if (NeighborTrays != null && NeighborTrays.Length > 0)
              {
                for (int i = 0; i < NeighborTrays.Length; ++i)
                {
                  var _neighbor = NeighborTrays[i];
                  _neighbor.onDetected?.Invoke(_neighbor.transform.position);
                }
              }

              if (!IsDragAble()) return;

              _nextPos = new float3(touchPos.x, touchPos.y, 0) + dragOffset;
              _rig.MovePosition(new Vector2(_nextPos.x, _nextPos.y));
              onDragBegan?.Invoke(transform.position);
              AnimationManager.Instance.CurrentBeingDragged = this;
              dOTweenAnimation.DORestart();

              if (NeighborTrays == null || NeighborTrays.Length == 0) return;
              for (int i = 0; i < NeighborTrays.Length; ++i)
              {
                var _neighbor = NeighborTrays[i];
                float3 _neighborOffset = _neighbor.transform.position - transform.position;
                float3 _neighborNextPos = _nextPos + _neighborOffset;

                _neighbor.onDragBegan?.Invoke(_neighbor.transform.position);
                _neighbor.dOTweenAnimation.DORestart();
                _neighbor.Rig.MovePosition(new Vector2(_neighborNextPos.x, _neighborNextPos.y));
              }
            }
          }
          break;

        case TouchPhase.Moved:
          if (this == AnimationManager.Instance.CurrentBeingDragged)
          {
            onDragMoving?.Invoke(transform.position);
            _nextPos = new float3(touchPos.x, touchPos.y, 0) + dragOffset;
            _rig.MovePosition(new Vector2(_nextPos.x, _nextPos.y));

            if (NeighborTrays == null || NeighborTrays.Length == 0) return;
            for (int i = 0; i < NeighborTrays.Length; ++i)
            {
              var _neighbor = NeighborTrays[i];
              float3 _neighborOffset = _neighbor.transform.position - transform.position;
              float3 _neighborNextPos = _nextPos + _neighborOffset;

              _neighbor.onDragMoving?.Invoke(_neighbor.transform.position);
              _neighbor.Rig.MovePosition(new Vector2(_neighborNextPos.x, _neighborNextPos.y));
            }
          }
          break;

        case TouchPhase.Canceled:
          _isDragging = false;
          TouchRelease();
          break;

        case TouchPhase.Ended:
          _isDragging = false;
          TouchRelease();
          break;
      }
    }
  }

  public bool IsPlaceable()
  {
    var droppedPos = transform.position;
    var trayGrid = ItemManager.Instance.TrayGrid;
    var index = trayGrid.ConvertWorldPosToIndex(droppedPos);
    if (
      trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.WoodBoxesGrid != null &&
      ItemManager.Instance.WoodBoxesGrid[index] > 0)
    )
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.GrillerGrid != null &&
      ItemManager.Instance.GrillerGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.GiftBoxesGrid != null &&
      ItemManager.Instance.GiftBoxesGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.CoffeeBoardsGrid != null &&
      ItemManager.Instance.CoffeeBoardsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.PlantPotsGrid != null &&
      ItemManager.Instance.PlantPotsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.CoverLetsGrid != null &&
      ItemManager.Instance.CoverLetsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.CurtainLayersGrid != null &&
      ItemManager.Instance.CurtainLayersGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.MoneyBagsGrid != null &&
      ItemManager.Instance.MoneyBagsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.CupBoardsGrid != null &&
      ItemManager.Instance.CupBoardsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.MachineCreamsGrid != null &&
      ItemManager.Instance.MachineCreamsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.MagicNestsGrid != null &&
      ItemManager.Instance.MagicNestsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.FlowerPotsGrid != null &&
      ItemManager.Instance.FlowerPotsGrid[index] > 0))
    {
      return false;
    }

    if (trayGrid.IsPosOccupiedAt(droppedPos) ||
      (ItemManager.Instance.BeverageFridgesGrid != null &&
      ItemManager.Instance.BeverageFridgesGrid[index] > 0))
    {
      return false;
    }

    return true;
  }

  public void TouchRelease()
  {
    _rig.linearVelocity = Vector3.zero;
    if (this != AnimationManager.Instance.CurrentBeingDragged) return;

    EffectManager.Instance.TurnOffAllRectangleShadows();

    var _isNeighborPlaceable = true;
    if (NeighborTrays != null && NeighborTrays.Length > 0)
    {
      for (int i = 0; i < NeighborTrays.Length; ++i)
      {
        var _neighbor = NeighborTrays[i];
        if (!IsPlaceable() || !_neighbor.IsPlaceable())
        {
          _isNeighborPlaceable = false;
          break;
        }
      }

      if (_isNeighborPlaceable)
      {
        for (int i = 0; i < NeighborTrays.Length; ++i)
        {
          var _neighborTray = NeighborTrays[i].GetComponent<TrayControl>();
          var roundedDroppedPos = FindNearestLandingPosAt(_neighborTray.transform.position);
          _neighborTray.PlacedSuccessfullyAt(roundedDroppedPos);
        }
      }
      else
      {
        for (int i = 0; i < NeighborTrays.Length; ++i)
        {
          var _neighborTray = NeighborTrays[i].GetComponent<TrayControl>();
          var roundedDroppedPos = FindNearestLandingPosAt(_neighborTray.transform.position);
          _neighborTray.PlacedWrongAt(roundedDroppedPos);
        }
      }
    }

    if (_isNeighborPlaceable && IsPlaceable())
    {
      var draggedTray = GetComponent<TrayControl>();
      var roundedDroppedPos = FindNearestLandingPosAt(transform.position);



      draggedTray.PlacedSuccessfullyAt(
        roundedDroppedPos, () =>
        {
          HashSet<TrayControl> _linkedTrays1 = new();
          HashSet<TrayControl> _linkedTrays2 = new();
          var neighborDraggedTrays = draggedTray.FindNeighborTraysAt(roundedDroppedPos);

          for (int i = 0; i < neighborDraggedTrays.Count; ++i)
          {
            if (NeighborTrays != null && NeighborTrays.Length > 0)
            {
              var isContainNeighbor = neighborDraggedTrays[i].GetInstanceID() ==
                  NeighborTrays[0].GetComponent<TrayControl>().GetInstanceID();
              if (isContainNeighbor) continue; // avoid adding its own neighbor
            }
            _linkedTrays1.Add(neighborDraggedTrays[i]);
          }
          // this will make sure that this tray will be placed at "the the last position" of the set
          _linkedTrays1.Add(draggedTray);
          // continue to check its own neighbor
          if (NeighborTrays != null && NeighborTrays.Length > 0)
          {
            var _neighborTray = NeighborTrays[0].GetComponent<TrayControl>();
            var _neighborOfNeighborTrays = _neighborTray.FindNeighborTraysAt(
              _neighborTray.transform.position
            );

            for (int j = 0; j < _neighborOfNeighborTrays.Count; ++j)
            {
              if (_neighborOfNeighborTrays[j].GetInstanceID() == draggedTray.GetInstanceID()) continue;
              _linkedTrays2.Add(_neighborOfNeighborTrays[j]);
            }

            // this will make sure that neighbors will be placed at "the the last position" of the set
            _linkedTrays2.Add(NeighborTrays[0].GetComponent<TrayControl>());

            List<CupControl> _cupTrays1 = draggedTray.FindCups();
            List<CupControl> _cupTrays2 = _neighborTray.FindCups();
            var isSharingCup = false;
            foreach (var cup in _cupTrays2)
            {
              var foundShareColorCup = _cupTrays1.Find(elt => elt.ColorIndex == cup.ColorIndex);
              if (foundShareColorCup)
              {
                isSharingCup = true;
                break;
              }
            }

            if (isSharingCup)
            {

              ClearNeighbors();
              return;
            }
          }

          if (NeighborTrays != null && NeighborTrays.Length > 0)
          {

            ClearNeighbors();
            return;
          }

        }
      );
    }
    else
    {
      GetComponent<TrayControl>().PlacedWrongAt(transform.position);
    }

    AnimationManager.Instance.CurrentBeingDragged = null;
  }

  float3 FindNearestLandingPosAt(float3 pos)
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    int2 gridPos = trayGrid.ConvertWorldPosToGridPos(pos);
    float3 worldPos = trayGrid.ConvertGridPosToWorldPos(gridPos);
    return worldPos;
  }

  // Duy
  private void ClearNeighbors()
  {
    for (int i = 0; i < NeighborTrays.Length; i++)
    {
      ClearNeighborIn(NeighborTrays[i]);
    }

    ClearNeighborIn(this);
  }

  private void ClearNeighborIn(DragAndDrop tray)
  {
    tray.NeighborTrays = null;
  }
}