using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;

public class CurtainLayerControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [Tooltip("0: 2x2, 1: 2x3, 2: 2x6, 3: 4x2")]
  [SerializeField] SkeletonAnimation[] skeletonAnims;
  [SerializeField] SpriteRenderer bgTargetRenderer;
  [SerializeField] SpriteRenderer targetRenderer;
  [SerializeField] SpriteRenderer tickRenderer;
  [SerializeField] TMP_Text amountTxt;
  [SerializeField] Transform target;
  public Transform Target { get { return target; } }

  [Header("External dependencies")]
  ObjectPool<GameObject> curtainLayerPool;

  private List<int> _posCellInLayer;
  public List<int> PosCellInLayer { get { return _posCellInLayer; } }

  private int _colorIndex;
  public int ColorIndex { get { return _colorIndex; } }

  private int _amount;
  public int Amount { get { return _amount; } }

  private int2 _size;
  public int2 Size { get { return _size; } }

  private string _dirNameCurtain = "curtain_";
  private string[] _nameCurtainColors = new string[9] { "sky blue_", "blue viollet_", "seafoam_", "yellow_", "magenta viollet_", "", "green_", "orange_", "red_" };
  private string _nameCurtainMultiColor = "rainbow_";
  // cherryjuice, moxito, applejuice, orangejuice, rainbow, pinocolada, greentea, cappuchino, mocha

  private void OnEnable()
  {
    transform.localScale = new float3(1, 1, 0);

    for (int i = 0; i < skeletonAnims.Length; i++)
    {
      skeletonAnims[i].AnimationState.SetAnimation(0, "Idle", false);
      skeletonAnims[i].gameObject.SetActive(false);
    }
  }

  public void InjectPool(ObjectPool<GameObject> curtainLayerPool, ObjectPool<GameObject> other = null)
  {
    this.curtainLayerPool = curtainLayerPool;
  }

  public void InitFrom(CurtainLayerData curtainLayerData, int sortingOrder)
  {
    _colorIndex = curtainLayerData.ColorIndex - 1;
    _posCellInLayer = new(curtainLayerData.PosCellInLayer);
    _amount = curtainLayerData.Amount;
    _size = curtainLayerData.Size;

    amountTxt.text = _amount.ToString();
    for (int i = 0; i < skeletonAnims.Length; i++)
    {
      skeletonAnims[i].GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    }

    bgTargetRenderer.sortingOrder = sortingOrder + 1;
    targetRenderer.sortingOrder = sortingOrder + 2;
    tickRenderer.sortingOrder = sortingOrder + 3;
    amountTxt.GetComponent<MeshRenderer>().sortingOrder = sortingOrder + 3;
    tickRenderer.gameObject.SetActive(false);
    amountTxt.gameObject.SetActive(true);

    var size = curtainLayerData.Size;
    SetSkinWith(size);

    if (_colorIndex == -1)
    {
      return;
    }
  }

  private void SetSkinWith(int2 size)
  {
    var skeAnim = GetSkeAnimCurtainWith(size);
    skeAnim.Skeleton.SetSkin(GetCurtainSkinNameWith(size));
    skeAnim.Skeleton.SetSlotsToSetupPose();
    skeAnim.LateUpdate();
    skeAnim.gameObject.SetActive(true);
  }

  private SkeletonAnimation GetSkeAnimCurtainWith(int2 size)
  {
    if (size.x == 2 && size.y == 2)
    {
      return skeletonAnims[0];
    }

    if (size.x == 2 && size.y == 3)
    {
      return skeletonAnims[1];
    }

    if (size.x == 2 && size.y == 6)
    {
      return skeletonAnims[2];
    }

    return skeletonAnims[3];
  }

  private string GetCurtainSkinNameWith(int2 size)
  {
    if (_colorIndex == -1)
    {
      return _dirNameCurtain + _nameCurtainMultiColor + size.x + "x" + size.y;
    }

    return _dirNameCurtain + _nameCurtainColors[_colorIndex] + size.x + "x" + size.y;
  }

  private void TryMergeTrayNeighbors(List<int> poses)
  {
    for (int i = 0; i < poses.Count; i++)
    {
      if (ItemManager.Instance.TrayGrid.GetValueAt(ItemManager.Instance.TrayGrid.ConvertIndexToWorldPos(i)) != 1) continue;

      TryMergeTrayNeighbor(i);
    }
  }

  private void TryMergeTrayNeighbor(int index)
  {
    HashSet<TrayControl> _linkedTrays1 = new();


  }

  public LTDescr RemoveFromTableWithAnim()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1)
      return AnimationManager.Instance.ScaleTo(1, gameObject, .1f);

    ItemManager.Instance.CurtainLayersGrid[index]--;

    if (ItemManager.Instance.CurtainLayersGrid[index] > 0)
    {
      return AnimationManager.Instance.ScaleTo(.7f, gameObject, .1f).setLoopPingPong(1);
    }

    var poses = new List<int>();
    poses.AddRange(_posCellInLayer);

    ItemManager.Instance.CurtainLayersGrid[index] = 0;
    ItemManager.Instance.RemoveCurtainLayer(this);
    tickRenderer.gameObject.SetActive(true);
    amountTxt.gameObject.SetActive(false);

    var currentTimeAnim = 0f;
    var timeDelay = 0.4f;
    var timeScale = 0.5f;

    Sequence seq = DOTween.Sequence();
    currentTimeAnim += timeDelay;

    seq.Insert(
      currentTimeAnim,
      target.transform.DOScale(float3.zero, timeScale)
    );

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        ItemManager.Instance.SpawnSkeExplosionCupAt(target.transform.position);
      }
    );

    currentTimeAnim += timeScale;

    return
    LeanTween.delayedCall(gameObject, currentTimeAnim, () =>
    {
      TryMergeTrayNeighbors(poses);
      GetSkeAnimCurtainWith(_size).AnimationState.SetAnimation(0, "open", false);
      ItemManager.Instance.ShowCoffeeBoardsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowCoverLetsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowCupBoardsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowGiftBoxesIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowGoldenTraysIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowGrassesIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowGrillersIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowIceBoxesIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowMachineCreamsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowMoneyBagsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowPlantPotsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowWoodBoxesIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowMagicNestsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowLeavesFlowersIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowFlowerPotsIn(PosCellInLayer.ToArray());
      ItemManager.Instance.ShowBeverageFridgesIn(PosCellInLayer.ToArray());

    });
  }

  public void RemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    ItemManager.Instance.CurtainLayersGrid[index]--;
    if (ItemManager.Instance.CurtainLayersGrid[index] > 0)
    {
      return;
    }

    ItemManager.Instance.CurtainLayersGrid[index] = 0;
    ItemManager.Instance.RemoveCurtainLayer(this);

    Release();
  }

  public void FullyRemoveFromTable()
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return;

    SubAmount();
    if (IsOutOfAmount())
    {
      RemoveFromTableWithAnim();
    }
  }

  public void Release()
  {
    if (gameObject.activeSelf)
    {
      curtainLayerPool.Release(gameObject);
    }
  }

  public void SubAmount()
  {
    if (_amount == 0) return;

    _amount--;
  }

  public void UpdateAmount()
  {
    amountTxt.text = _amount.ToString();
  }

  public void UpdateAmountWith(int amount)
  {
    amountTxt.text = amount.ToString();
  }

  public bool IsOutOfAmount()
  {
    return _amount == 0;
  }
}