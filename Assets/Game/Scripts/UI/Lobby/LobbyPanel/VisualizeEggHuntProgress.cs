using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;

public partial class LobbyPanel : MonoBehaviour
{
  [Header("Setting LiveOpsProgress")]
  [SerializeField] GameObject eggHuntProgressPref;
  [SerializeField] Sprite eggHuntProgressSprite;
  [SerializeField] SkeletonGraphic skeEggHuntIcon;

  public void PlayAnimClaimEggHuntProgress()
  {
    var endPos = skeEggHuntIcon.transform.position;
    var startPos = endPos + new Vector3(1.25f, 0.25f, 0);

    MoveLiveOpsProgressWith(startPos, endPos, eggHuntProgressSprite, GameManager.Instance.MoveEggTicketsAmountNeedClaim);
    GameManager.Instance.MoveEggTicketsAmountNeedClaim = 0;
  }

  private void MoveLiveOpsProgressWith(float3 startPos, float3 endPos, Sprite eventProgressImg, int amount)
  {
    Sequence seq = DOTween.Sequence();
    var currentTimeAnim = 0.5f;
    var deltaTimeDelay = 0.1f;
    var timeShowEgg = 0.5f;
    var timeMoveEgg = 1f;

    amount = amount > 5 ? 5 : amount;

    for (int i = 0; i < amount; ++i)
    {
      var timeDelay = i * deltaTimeDelay;
      var eventProgress = Instantiate(eggHuntProgressPref, skeEggHuntIcon.transform.position, Quaternion.identity);
      eventProgress.GetComponentInChildren<SpriteRenderer>().sprite = eventProgressImg;
      eventProgress.transform.position = startPos;
      eventProgress.transform.localScale = float3.zero;

      seq.InsertCallback(
        currentTimeAnim,
        () =>
        {
          eventProgress.transform.DOScale(
            new Vector3(1, 1, 0),
            timeShowEgg
          );
        }
      );

      seq.InsertCallback(
        currentTimeAnim + timeShowEgg + timeDelay,
        () =>
        {
          eventProgress.transform.DOJump(
            endPos,
            1,
            1,
            timeMoveEgg
          );
        }
      );

      seq.InsertCallback(
        currentTimeAnim + timeShowEgg + timeDelay + timeMoveEgg,
        () =>
        {
          eventProgress.SetActive(false);
        }
      );
    }

    seq.InsertCallback(
      currentTimeAnim + timeShowEgg,
      () =>
      {
        ShowPanel.Instance.ShowTextAt(
          new Vector2(startPos.x, startPos.y) - Vector2.up * 0.5f,
          "+" + amount,
          Color.white
        );
      }
    );
  }
}