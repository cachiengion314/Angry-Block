using System.Collections.Generic;
using DG.Tweening;
using HoangNam;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
  private void VisualizeMetaProgressReward(
    List<RewardData> _metaRewardNeedClaims,
    List<RewardData> _metaRewardNextClaims,
    List<AnimMetaProgressData> _animMetaProgressDatas,
    int amountMetaNeedClaim
  )
  {
    Sequence seq = DOTween.Sequence();

    var currentTimeAnim = 1f;
    var timeMoveItem = 1f;
    var timeRunProgress = 1f;
    var timeShowReward = 1f;
    var timeMoveNewReward = 1f;
    var timeRemoveReward = 0.5f;
    var timeDelay = 2;

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        LobbyPanel.Instance.ProgerssSteakBtnSystem.MoveItemToStartPos(timeMoveItem, amountMetaNeedClaim);
      }
    );

    currentTimeAnim += timeMoveItem;

    for (int i = 0; i < _animMetaProgressDatas.Count; i++)
    {
      var animMetaProgressData = _animMetaProgressDatas[i];

      var typeMetaReward = animMetaProgressData.AnimMetaProgressType;
      var startValue = (float)animMetaProgressData.StartValue;
      var endValue = (float)animMetaProgressData.EndValue;
      var target = (float)animMetaProgressData.Target;

      if (typeMetaReward == AnimMetaProgressType.None)
      {
        seq.InsertCallback(
          currentTimeAnim,
          () =>
          {
            Utility.Print("Run progress and don't have reward");

            var currentValue = startValue;
            DOTween.To(
              () => currentValue,
              (value) => currentValue = value,
              endValue,
              timeRunProgress
            ).OnUpdate(
              () =>
              {
                LobbyPanel.Instance.ProgerssSteakBtnSystem.SetTextProgerssBar((int)currentValue, (int)target);
                LobbyPanel.Instance.ProgerssSteakBtnSystem.SetValueProgerssBar(currentValue, target);
              }
            );
          }
        );

        currentTimeAnim += timeRunProgress;
        continue;
      }

      if (typeMetaReward == AnimMetaProgressType.ClaimReward)
      {
        var metaRewardNeedClaim = _metaRewardNeedClaims[i];

        seq.InsertCallback(
          currentTimeAnim,
          () =>
          {
            Utility.Print("Run progress");

            var currentValue = startValue;
            DOTween.To(
              () => currentValue,
              (value) => currentValue = value,
              endValue,
              timeRunProgress
            ).OnUpdate(
              () =>
              {
                LobbyPanel.Instance.ProgerssSteakBtnSystem.SetTextProgerssBar((int)currentValue, (int)target);
                LobbyPanel.Instance.ProgerssSteakBtnSystem.SetValueProgerssBar(currentValue, target);
              }
            );
          }
        );

        currentTimeAnim += timeRunProgress;

        seq.InsertCallback(
          currentTimeAnim,
          () =>
          {
            Utility.Print("Show reward to center of screen");
            LobbyPanel.Instance.ProgerssSteakBtnSystem.MoveRewardToCenterOfScreen(metaRewardNeedClaim, timeShowReward);
          }
        );

        currentTimeAnim += timeShowReward;

        if (i < _metaRewardNextClaims.Count)
        {
          currentTimeAnim += timeDelay;

          var metaRewardNextClaim = _metaRewardNextClaims[i];
          seq.InsertCallback(
            currentTimeAnim,
            () =>
            {
              Utility.Print("Show reward from center of screen to start Pos");
              LobbyPanel.Instance.ProgerssSteakBtnSystem.MoveRewardToStartPos(metaRewardNextClaim, timeMoveNewReward);
            }
          );

          currentTimeAnim += timeMoveNewReward;
        }

        continue;
      }

      if (typeMetaReward == AnimMetaProgressType.EndEvent)
      {
        seq.InsertCallback(
          currentTimeAnim,
          () =>
          {
            Utility.Print("Hide Meta Progress Lobby");
            LobbyPanel.Instance.ProgerssSteakBtnSystem.RemoveReward();
          }
        );

        currentTimeAnim += timeRemoveReward;

        seq.InsertCallback(
          currentTimeAnim,
          () =>
          {
            Utility.Print("Hide Meta Progress Lobby");
            LobbyPanel.Instance.ProgerssSteakBtnSystem.OffMetaProgressEvent();
          }
        );
        continue;
      }
    }

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        LobbyPanel.Instance.PanelFullScreen.gameObject.SetActive(false);
      }
    );
  }
}