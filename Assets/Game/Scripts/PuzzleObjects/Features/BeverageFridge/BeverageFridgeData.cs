using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// BeverageFridge Data
/// </summary> <summary>
/// 
/// </summary>
public partial class BeverageFridgeControl : MonoBehaviour
{
  [Header("SpriteRenderers")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] SpriteRenderer glassRenderer;
  [SerializeField] SpriteRenderer bgPushRenderer;

  [Header("Skeleton Animations")]
  [SerializeField] SkeletonAnimation skePushAnim;

  [Header("Transform")]
  [SerializeField] Transform cupParent1;
  [SerializeField] Transform cupParent2;
  [SerializeField] Transform posStartTrailFly;

  [Header("Sprite Cups")]
  [SerializeField] Sprite[] cupSprites;

  ObjectPool<GameObject> _beverageFridgePool;

  private int _amountCup;
  public int AmountCup
  {
    get { return _amountCup; }
  }

  private List<int> _posCellInLayer;
  public List<int> PosCellInLayer { get { return _posCellInLayer; } }

  private BeverageFridgeData _beverageFridgeData;
}