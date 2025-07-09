using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// MachineCream Manager
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  [Header("MachineCream Dependencies")]
  [SerializeField] GameObject machineCreamPref;

  [Header("MachineCream Pooling")]
  ObjectPool<GameObject> machineCreamPool;

  int[] _machineCreamsGrid;
  public int[] MachineCreamsGrid { get { return _machineCreamsGrid; } }

  [HideInInspector] public GameObject[] MachineCreams;

  private void InitMachineCreamsPool()
  {
    machineCreamPool = new ObjectPool<GameObject>(
      CreateMachineCreamsPool,
      OnTakeObjFromPool,
      OnReturnObjFromPool,
      OnDestroyPoolObj,
      true,
      trayGrid.GridSize.x * trayGrid.GridSize.y,
      trayGrid.GridSize.x * trayGrid.GridSize.y
    );
  }

  GameObject CreateMachineCreamsPool()
  {
    GameObject _obj = Instantiate(machineCreamPref, transform.position, transform.rotation, tableDepot);
    _obj.GetComponent<IPoolItemControl>().InjectPool(machineCreamPool);
    return _obj;
  }

  public void SpawnMachineCreams(int[] grids)
  {
    if (grids.Length == 0) return;

    _machineCreamsGrid = new int[grids.Length];
    for (int i = 0; i < grids.Length; ++i)
    {
      _machineCreamsGrid[i] = grids[i];
    }

    MachineCreams = new GameObject[grids.Length];
    for (int i = 0; i < _machineCreamsGrid.Length; ++i)
    {
      if (_machineCreamsGrid[i] == 0) continue;
      SpawnMachineCreamAt(i);
    }
  }

  private void SpawnMachineCreamAt(int index)
  {
    var pos = trayGrid.ConvertIndexToWorldPos(index);

    var machineCream = machineCreamPool.Get();

    machineCream.transform.position = pos;
    machineCream.transform.localScale = new float3(1, 1, 1);
    MachineCreams[index] = machineCream;

    int orderLayer = trayGrid.GridSize.y * 20 - index % trayGrid.GridSize.y * 20;
    orderLayer += 1;
    machineCream.GetComponent<MachineCreamControl>().SetSortingOrder(orderLayer);
    machineCream.GetComponent<MachineCreamControl>().ShowStateOfHealthBaseOn(1, false);
  }

  public void RemoveMachineCreamAt(int x)
  {
    if (MachineCreams == null || MachineCreams.Length == 0) return;
    var seq = LeanTween.sequence();
    for (int y = 0; y < trayGrid.GridSize.y; ++y)
    {
      var gridPos = new int2(x, y);
      var index = trayGrid.ConvertGridPosToIndex(gridPos);
      var obj = MachineCreams[index];
      seq.append(.06f);
      if (obj == null) continue;
      if (curtainLayersGrid[index] > 0) continue;

      var machineCream = obj.GetComponent<MachineCreamControl>();
      if (LeanTween.isTweening(machineCream.gameObject)) continue;

      seq.append(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          SoundManager.Instance.PlayBoomSfx();
          var pos = trayGrid.ConvertGridPosToWorldPos(gridPos);
          var boomSkeleton = EffectManager.Instance.SpawnExplosiveAt(pos);
          var boomAnim = boomSkeleton.Skeleton.Data.FindAnimation("no-thung");
          float duration = boomAnim.Duration;
          LeanTween.delayedCall(gameObject, duration, () => { Destroy(boomSkeleton.gameObject); });

          machineCream.FullyRemoveFromTable();
        })
      );
    }
  }

  public void RemoveMachineCreams()
  {
    if (MachineCreams == null || MachineCreams.Length == 0) return;

    for (int i = 0; i < MachineCreams.Length; i++)
    {
      var obj = MachineCreams[i];
      if (obj == null) continue;
      var machineCream = obj.GetComponent<MachineCreamControl>();
      if (DOTween.IsTweening(machineCream.transform))
      {
        DOTween.Kill(machineCream.gameObject);
      }

      SoundManager.Instance.PlayBoomSfx();
      var pos = obj.transform.position;
      EffectManager.Instance.SpawnHammerExplosiveAt(pos);
      machineCream.RemoveFromTable();
    }
  }

  public void TryRemoveNeighborMachineCreams(float3 pos, out int machineCreamValue, out int amountMachineCream, bool _hasFindNeighbor = true)
  {
    machineCreamValue = 0;
    amountMachineCream = 0;
    var trayGrid = TrayGrid;
    List<GameObject> neighborMachineCreams = new();

    if (_hasFindNeighbor)
    {
      neighborMachineCreams = FindNeighborMachineCreamsAt(pos);
    }
    else
    {
      neighborMachineCreams.Add(GetMachineCreamAt(pos));
    }

    for (int i = 0; i < neighborMachineCreams.Count; ++i)
    {
      var index = trayGrid.ConvertWorldPosToIndex(neighborMachineCreams[i].transform.position);
      if (
        CurtainLayersGrid[index] == 0 &&
        MachineCreamsGrid != null &&
        MachineCreamsGrid[index] > 0
      )
      {
        machineCreamValue = 17;
        amountMachineCream++;

        LeanTween.delayedCall(gameObject, .1f, () =>
        {
          if (MachineCreams[index] == null) return;
          SoundManager.Instance.PlayMachineCreamSfx();
        });
      }
    }
  }

  public void ClearMachineCreams()
  {
    for (int i = 0; i < MachineCreams.Length; ++i)
    {
      var obj = MachineCreams[i];
      if (obj == null) continue;
      // if (LeanTween.isTweening(obj)) continue;

      var machineCream = obj.GetComponent<MachineCreamControl>();
      machineCream.RemoveFromTable();
    }
  }

  public GameObject GetMachineCreamAt(float3 worldPos)
  {
    var index = trayGrid.ConvertWorldPosToIndex(worldPos);
    if (index < 0 || index > MachineCreams.Length - 1) return null;

    return MachineCreams[index];
  }

  public List<GameObject> FindNeighborMachineCreamsAt(float3 worldPos)
  {
    List<GameObject> linkedMachineCreams = new();
  

    return linkedMachineCreams;
  }

  public bool HasMachineCreamAt(float3 pos)
  {
    var machineCream = GetMachineCreamAt(pos);

    if (machineCream != null)
    {
      return true;
    }

    return false;
  }

  private void OnTouchMachineCreamAt(float3 pos)
  {
    var machineCream = GetMachineCreamAt(pos).GetComponent<MachineCreamControl>();

    if (PowerItemPanel.Instance.IsTriggeredHammer)
    {
      TrySelfRemovedByHammerAt(pos, machineCream);
    }
  }

  int FindUpMachineCreamValueAt(int i)
  {
    var gPos = TrayGrid.ConvertIndexToGridPos(i);
    var gUpPos = gPos + new int2(0, 1);
    if (TrayGrid.IsGridPosOutsideAt(gUpPos)) return -1;
    var upIndex = TrayGrid.ConvertGridPosToIndex(gUpPos);
    return _machineCreamsGrid[upIndex];
  }

  public void HideMachineCreamsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (MachineCreams[index] == null) continue;
      MachineCreams[index].transform.localScale = float3.zero;
    }
  }

  public void ShowMachineCreamsIn(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; ++i)
    {
      var index = indexes[i];
      if (MachineCreams[index] == null) continue;
      MachineCreams[index].transform.DOScale(
        Vector3.one,
        0.5f
      );
    }
  }
}