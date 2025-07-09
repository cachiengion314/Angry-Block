
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public partial class ItemManager : MonoBehaviour
{
  public LTSeq AssignFullTraysAnimationFrom(LTSeq seq, List<TrayControl> fullTrays, List<TrayControl> emptyTrays)
  {
    foreach (var tray in fullTrays)
    {
      seq.insert(
        LeanTween.delayedCall(gameObject, 0, () =>
        {
          var seq2 = LeanTween.sequence();
          seq2.append(
            LeanTween.delayedCall(gameObject, 0, () =>
            {
              IsPacking = true;
            })
          );

          var idBox = 0;
          var dir = "Skin-khay1";
          var timeCheck = 1.8f;
          var posSpawnBox = tray.transform.position + Vector3.up * .28f;

          if (HasGoldenTrayAt(tray.transform.position))
          {
            idBox = 1;
            dir = "cup";
            timeCheck = 2f;
            posSpawnBox = tray.transform.position + Vector3.up * .2f;
          }

          var delegateColorIndex = (int)tray.GetDelegateColor() - 1;
          var curtainLayer = FindCurtainLayerWith(delegateColorIndex);

          if (curtainLayer != null)
          {
            curtainLayer.SubAmount();
            if (curtainLayer.IsOutOfAmount())
            {

            }
            else
            {

            }
          }

          var gridPos = trayGrid.ConvertWorldPosToGridPos(tray.transform.position);
          tray.gameObject.SetActive(false);
          seq2.append(timeCheck); // exactly parking animation length
        })
      );
    }

    foreach (var tray in emptyTrays)
      seq.insert(tray.RemoveFromTableWithAnim());

    return seq;
  }

  public LTSeq AssignAnimationsFrom(LTSeq seq, List<CupControl> needMoveCups, List<float3> needMoveCupDesPositions)
  {
    var _slowFactor = .12f;
    var maxSortingOrder = 0;
    for (int i = 0; i < needMoveCups.Count; ++i)
    {
      if (needMoveCups[i].GetSortingOrder() <= maxSortingOrder) continue;
      maxSortingOrder = needMoveCups[i].GetSortingOrder();
    }

    for (int i = 0; i < needMoveCups.Count; ++i)
    {
      var cup = needMoveCups[i];
      if (cup.transform.position.Equals(needMoveCupDesPositions[i])) continue;
      cup.SetSortingOrder(maxSortingOrder);

      seq.append(
        LeanTween.delayedCall(gameObject, 0f, () =>
        {
          SoundManager.Instance.PlayTrayTransferSfx();
        })
      );

      seq.append(
        AnimationManager.Instance.MoveTo(
          needMoveCupDesPositions[i], cup.transform.position,
          cup.gameObject, _slowFactor,
          () =>
          {
            cup.CalculateSortingOrder();
          }
        )
      );
    }

    return seq;
  }

  public LTDescr ScaleTrayTo(float3 desSize, GameObject scaleObj, float slowFactor = .1f, Action onCompleted = null)
  {
    return AnimationManager.Instance.ScaleTo(desSize, scaleObj, slowFactor, onCompleted);
  }

  public LTDescr MoveTrayTo(float3 desPos, float3 fromPos, GameObject trayObj, float slowFactor = .1f, Action onCompleted = null)
  {
    if (trayObj.TryGetComponent(out TrayControl component))
      component.CurrentWorldPos = desPos;
    return AnimationManager.Instance.MoveTo(desPos, fromPos, trayObj, slowFactor, onCompleted);
  }
}