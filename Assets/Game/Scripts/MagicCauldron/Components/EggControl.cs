using MoreMountains.Feedbacks;
using UnityEngine;
using Spine.Unity;

public class EggControl : MonoBehaviour
, ISpriteRenderer
, IDoTweenControl
, IFeedbackControl
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  public SpriteRenderer BodyRenderer { get { return bodyRenderer; } }
  [SerializeField] MMPositionShaker MMPositionShaker;
  [SerializeField] MMRotationShaker MMRotationShaker;
  [SerializeField] GameObject shadowEgg;
  public SkeletonAnimation skeletonAnimation;

  [Header("Datas")]
  int _spawnedIndex = -1;
  public int SpawnedIndex { get { return _spawnedIndex; } }
  int _placedIndex = -1;
  public int PlacedIndex { get { return _placedIndex; } }
  int _colorValue;
  public int ColorValue { get { return _colorValue; } }
  int _initSortingOrder;
  public int InitSortingOrder { get { return _initSortingOrder; } }

  /// <summary>
  /// index where this egg spawned
  /// </summary>
  /// <param name="i"></param>
  public void SetSpawnedIndex(int i)
  {
    _spawnedIndex = i;
  }

  /// <summary>
  /// index where this egg place at the holder
  /// </summary>
  /// <param name="i"></param>
  public void SetPlacedIndex(int i)
  {
    _placedIndex = i;
  }

  public void SetColorValue(int colorValue)
  {
    _colorValue = colorValue;
  }

  public void ChangeTweeningTo(bool onOffValue)
  {
    throw new System.NotImplementedException();
  }

  public void InjectChannel(int channelId)
  {
    MMPositionShaker.Channel = channelId;
    MMRotationShaker.Channel = channelId;
  }

  public bool IsTweening()
  {
    throw new System.NotImplementedException();
  }

  public void ResetSortingOrder()
  {
    SetSortingOrder(_initSortingOrder);
  }

  public void InjectInitSortingOrder(int sortingOrder)
  {
    _initSortingOrder = sortingOrder;
  }

  public void SetInitSortingOrder(int sortingOrder)
  {
    _initSortingOrder = sortingOrder;
    SetSortingOrder(sortingOrder);
  }

  public int GetSortingOrder()
  {
    return bodyRenderer.sortingOrder;
  }

  public void SetSortingOrder(int sortingOrder)
  {
    bodyRenderer.sortingOrder = sortingOrder;

    if (skeletonAnimation.TryGetComponent<MeshRenderer>(out var meshRenderer))
    {
      meshRenderer.sortingOrder = sortingOrder;
    }
  }

  public void ChangeToUILayer()
  {
    bodyRenderer.sortingLayerName = "UI";

    if (skeletonAnimation.TryGetComponent<MeshRenderer>(out var meshRenderer))
    {
      meshRenderer.sortingLayerName = "UI";
    }
  }

  public void SetActitveShadowEgg(bool _isShawdow)
  {
    shadowEgg.SetActive(_isShawdow);
  }

  public void SetEggSortingLayerNameAt(string layerName, int sortingOrder)
  {
    bodyRenderer.sortingLayerName = layerName;
    bodyRenderer.sortingOrder = sortingOrder;
    shadowEgg.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
    shadowEgg.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder - 1;
  }

  public void SetOffCollider()
  {
    this.gameObject.GetComponent<Collider2D>().enabled = false;
  }

  public void SetOnCollider()
  {
    this.gameObject.GetComponent<Collider2D>().enabled = true;
  }
  // public void SetUpAnimEgg(int _index)
  // {
  //   switch (_index)
  //   {
  //     case 0:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_oragen", true); // Chạy anim_1 khi true
  //       break;
  //     case 1:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_blue", true); // Chạy anim_1 khi true
  //       break;
  //     case 2:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_cyan", true); // Chạy anim_1 khi true
  //       break;
  //     case 3:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_green", true); // Chạy anim_1 khi true
  //       break;
  //     case 4:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_yellow", true); // Chạy anim_1 khi true
  //       break;
  //     case 5:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_red", true); // Chạy anim_1 khi true
  //       break;
  //     case 6:
  //       skeletonAnimation.AnimationState.SetAnimation(0, "egg_purple", true); // Chạy anim_1 khi true
  //       break;
  //   }
  //   skeletonAnimation.gameObject.SetActive(false);
  // }

  public void SetSkinByColor(int colorIndex)
  {
    string skinName = colorIndex switch
    {
      0 => "egg_orange",
      1 => "egg_blue",
      2 => "egg_cyan",
      3 => "egg_green",
      4 => "egg_yellow",
      5 => "egg_red",
      6 => "egg_purple",
      _ => "default"
    };

    skeletonAnimation.initialSkinName = skinName;
    skeletonAnimation.Initialize(true); // Refresh lại skeleton để dùng skin mới
    skeletonAnimation.gameObject.SetActive(false);
  }

  public void PlayAnimEgg()
  {
    bodyRenderer.enabled = false;
    skeletonAnimation.gameObject.SetActive(true);
  }

  public void OffAnimEgg()
  {
    bodyRenderer.enabled = true;
    shadowEgg.SetActive(true);
    skeletonAnimation.gameObject.SetActive(false);
  }

  public void SetSortingTutorial()
  {
    bodyRenderer.sortingLayerName = "Notif";
    bodyRenderer.sortingOrder = 10;

    if (skeletonAnimation.TryGetComponent<MeshRenderer>(out var meshRenderer))
    {
      meshRenderer.sortingOrder = 10;
      meshRenderer.sortingLayerName = "Notif";
    }
  }

  public void ResetSortingTutorial()
  {
    bodyRenderer.sortingLayerName = "Item";
    ResetSortingOrder();

    if (skeletonAnimation.TryGetComponent<MeshRenderer>(out var meshRenderer))
    {
      meshRenderer.sortingOrder = 11;
      meshRenderer.sortingLayerName = "Item";
    }
  }
}
