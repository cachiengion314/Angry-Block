using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visualize lobby
/// </summary>
public partial class LobbyPanel : MonoBehaviour
{
  [Header("Rect lobby visualize")]
  [SerializeField] RectTransform levelContent;
  [SerializeField] RectTransform[] liveOpsLefts;
  [SerializeField] RectTransform[] liveOpsRights;

  [Header("Vertical")]
  [SerializeField] VerticalLayoutGroup verticalLayoutLeft;
  [SerializeField] VerticalLayoutGroup verticalLayoutRight;

  [Header("Panel All Screen")]
  [SerializeField] RectTransform panelFullScreen;
  public RectTransform PanelFullScreen { get { return panelFullScreen; } }

  private float3 _levelContentDefaultPos;
  private float3[] _posDefaultLefts;
  private float3[] _posDefaultRights;

  private bool _isTweeningLiveOps = false;
  public bool IsTweeningLiveOps { get { return _isTweeningLiveOps; } }

  public void InitPosDefaultLiveOps()
  {
    Canvas.ForceUpdateCanvases();
    verticalLayoutLeft.enabled = false;
    verticalLayoutRight.enabled = false;

    _levelContentDefaultPos = levelContent.transform.position;
    _posDefaultLefts = new float3[liveOpsLefts.Length];
    _posDefaultRights = new float3[liveOpsRights.Length];

    for (int i = 0; i < _posDefaultLefts.Length; i++)
    {
      _posDefaultLefts[i] = liveOpsLefts[i].transform.position;
    }

    for (int i = 0; i < _posDefaultRights.Length; i++)
    {
      _posDefaultRights[i] = liveOpsRights[i].transform.position;
    }

    verticalLayoutLeft.enabled = true;
    verticalLayoutRight.enabled = true;
  }

  public void VisualizeHomeLayout()
  {
    VisualizeLevelContent();
    VisualizeLiveOpsLefts();
    VisualizeLiveOpsRights();
  }

  private void VisualizeLevelContent()
  {
    var duration = 0.5f;
    DOTween.Kill(levelContent.transform);

    levelContent.transform.position = _levelContentDefaultPos + new float3(0, -1, 0) * 2;
    levelContent.transform.DOMove(_levelContentDefaultPos, duration);
  }

  private void VisualizeLiveOpsLefts()
  {
    _isTweeningLiveOps = true;
    var duration = 0.5f;
    int count = 0;

    for (int i = 0; i < liveOpsLefts.Length; i++)
    {
      DOTween.Kill(liveOpsLefts[i].transform);

      liveOpsLefts[i].transform.position = _posDefaultLefts[i] + new float3(-1, 0, 0) * 2;
      liveOpsLefts[i].transform.DOMove(_posDefaultLefts[i], duration).SetTarget(liveOpsLefts[i].transform)
        .OnComplete(
          () =>
          {
            count++;
            if (count == liveOpsLefts.Length)
            {
              _isTweeningLiveOps = false;
            }
          }
        );
    }
  }

  private void VisualizeLiveOpsRights()
  {
    _isTweeningLiveOps = true;
    var duration = 0.5f;
    int count = 0;

    for (int i = 0; i < liveOpsRights.Length; i++)
    {
      DOTween.Kill(liveOpsRights[i].transform);

      liveOpsRights[i].transform.position = _posDefaultRights[i] + new float3(1, 0, 0) * 2;
      liveOpsRights[i].transform.DOMove(_posDefaultRights[i], duration).SetTarget(liveOpsRights[i].transform)
        .OnComplete(
          () =>
          {
            count++;
            if (count == liveOpsRights.Length)
            {
              _isTweeningLiveOps = false;
            }
          }
        );
    }
  }
}