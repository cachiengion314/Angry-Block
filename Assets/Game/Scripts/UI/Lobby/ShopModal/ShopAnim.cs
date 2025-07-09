using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Shop Anim
/// 
/// </summary> <summary>
/// 
/// </summary>
public partial class ShopModal : MonoBehaviour
{
  [Header("Anim trans")]
  [SerializeField] RectTransform scrollContent;
  [SerializeField] RectTransform content;
  [SerializeField] RectTransform roofImg;

  private float3 _posRoofDefault;
  private float3 _posScrollContentDefault;

  public void InitPosDefault()
  {
    _posRoofDefault = roofImg.transform.position;
    _posScrollContentDefault = scrollContent.transform.position;
  }

  public void ShowAnimTrans()
  {
    DOTween.Kill(roofImg.transform);
    DOTween.Kill(scrollContent.transform);

    gameObject.SetActive(true);
    roofImg.transform.position = _posRoofDefault + new float3(0, 1, 0);
    scrollContent.transform.position = _posScrollContentDefault - new float3(0, 1, 0);

    var duration = 0.2f;

    roofImg.transform.DOMoveY(_posRoofDefault.y, duration).SetEase(Ease.Linear);
    scrollContent.transform.DOMoveY(_posScrollContentDefault.y, duration).SetEase(Ease.Linear);
  }

  public void HideAnimTrans()
  {
    DOTween.Kill(roofImg.transform);
    DOTween.Kill(scrollContent.transform);

    roofImg.transform.position = _posRoofDefault;
    scrollContent.transform.position = _posScrollContentDefault;

    var duration = 0.1f;

    roofImg.transform.DOMoveY(_posRoofDefault.y + 1, duration).SetEase(Ease.Linear);
    scrollContent.transform.DOMoveY(_posScrollContentDefault.y - 1, duration).SetEase(Ease.Linear);
    DOVirtual.DelayedCall(duration,
      () =>
      {
        gameObject.SetActive(false);

        var defaultPos = content.transform.position;
        defaultPos.y = 0;
        content.transform.position = defaultPos;
      }
    );
  }
}