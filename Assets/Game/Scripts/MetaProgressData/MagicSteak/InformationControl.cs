using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class InformationControl : MonoBehaviour
{
  [SerializeField] GameObject[] steps;
  [SerializeField] Transform tapContinueTxt;

  public void ShowInformation()
  {
    gameObject.SetActive(true);
    for (int i = 0; i < steps.Length; i++)
    {
      // steps[i].SetActive(false);
      steps[i].transform.localScale = float3.zero;
    }
    tapContinueTxt.gameObject.SetActive(false);

    AnimShow();
  }

  private void AnimShow()
  {
    var currentTimeAnim = 0f;
    float stepAnimDuration = 0.3f;

    Sequence seq = DOTween.Sequence();

    for (int i = 0; i < steps.Length; i++)
    {
      var step = steps[i];

      seq.InsertCallback(
        currentTimeAnim - stepAnimDuration / 2 * i,
        () =>
        {
          step.SetActive(true);
          step.transform.DOScale(
            new float3(1, 1, 1),
            stepAnimDuration
          ).SetEase(Ease.OutBack);
        }
      );

      currentTimeAnim += stepAnimDuration;
    }

    seq.InsertCallback(currentTimeAnim - stepAnimDuration / 2 * (steps.Length - 1),
      () =>
      {
        tapContinueTxt.gameObject.SetActive(true);
      }
    );
  }

  public void Hide()
  {
    gameObject.SetActive(false);
  }
}
