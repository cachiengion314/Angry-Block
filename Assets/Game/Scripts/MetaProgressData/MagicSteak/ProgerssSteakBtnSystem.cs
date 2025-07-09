using DG.Tweening;
using TMPro;
using UnityEngine;

public class ProgerssSteakBtnSystem : MonoBehaviour
{
  public static ProgerssSteakBtnSystem Instance { get; private set; }
  [Header("Item")]
  [SerializeField] Transform liveOps;
  [SerializeField] ProgerssControl progerssControl;
  [SerializeField] Transform itemClone;
  [SerializeField] TextMeshProUGUI textAmount;
  [SerializeField] TextMeshProUGUI timeTxt;

  [Header("Position")]
  [SerializeField] Transform upLiveOps;
  [SerializeField] Transform downLiveOps;
  [SerializeField] Transform ItemPos;
  [SerializeField] Transform rewardPos;


  private void Awake()
  {
    Instance = this;

  }

  private void Start()
  {
    CountdownSystem.Instance.OnTimeUpdate += UpdateTime;
    CountdownSystem.Instance.OnEndLoopTimeSpin += UpdateUi;
  }

  private void OnDestroy()
  {
    CountdownSystem.Instance.OnTimeUpdate -= UpdateTime;
    CountdownSystem.Instance.OnEndLoopTimeSpin -= UpdateUi;
  }

  public void UpdateUi(int x)
  {
    var CurrentValue = GameManager.Instance.CurrentMetaProgressTotal;
    var TotalValue = GameManager.Instance.GetCurrentProgressTarget();
    SetValueProgerssBar(CurrentValue, TotalValue);
    SetTextProgerssBar(CurrentValue, TotalValue);
    SetItemImg();
  }

  public void OffMetaProgressEvent()
  {
    // var duration = 0.3f;
    // var endValue = progerssControl.transform.position;
    // endValue.y += 3;

    // Sequence seq = DOTween.Sequence();

    // seq.Append(
    //   progerssControl.transform.DOMove(endValue, duration)
    //                             .SetEase(Ease.Linear)

    // );
    // seq.AppendCallback(() => gameObject.SetActive(false));
    // seq.Append(
    //   liveOps.DOMove(upLiveOps.position, duration)
    //           .SetEase(Ease.Linear)
    // ).OnComplete(
    //   () =>
    //   {
    //     LobbyPanel.Instance.InitPosDefaultLiveOps();
    //   }
    // );
  }

  public void UpdateTime(string strTime)
  {
    timeTxt.text = strTime;
  }

  public void SetValueProgerssBar(float CurrentValue, float TotalValue)
  {
    progerssControl.SetValueProgerssBar(CurrentValue, TotalValue);
  }
  public void SetTextProgerssBar(int CurrentValue, int TotalValue)
  {
    progerssControl.SetTextProgerssBar(CurrentValue, TotalValue);
  }

  public void MoveRewardToCenterOfScreen(RewardData data, float duration)
  {
    progerssControl.SetImage(data);
    var item = progerssControl.GetItem();

    item.transform.position = rewardPos.position;

    item.DOMove(Vector3.zero, duration)
    .SetEase(Ease.Linear)
    .OnComplete(() =>
    {
      if (GameManager.Instance.IsMetaProgressHappenning())
      {
        LobbyPanel.Instance.ToggleDarkPanel();
      }
      if (item.TryGetComponent(out Canvas canvas))
      {
        canvas.sortingOrder = 10;
      }
    });
  }

  public void MoveRewardToStartPos(RewardData data, float duration)
  {
    if (GameManager.Instance.IsMetaProgressHappenning())
    {
      LobbyPanel.Instance.ToggleDarkPanel();
    }
    var item = progerssControl.GetItem();
    if (item.TryGetComponent(out Canvas canvas))
    {
      canvas.sortingOrder = -1;
    }
    progerssControl.SetImage(data);

    item.transform.position = Vector3.zero;

    item.DOMove(rewardPos.position, duration)
    .SetEase(Ease.Linear);
  }

  public void RemoveReward()
  {
    var item = progerssControl.GetItem();
    item.gameObject.SetActive(false);
  }

  public void MoveItemToStartPos(float duration, int amount)
  {
    itemClone.position = Vector2.zero;
    textAmount.text = amount.ToString();
    itemClone.gameObject.SetActive(true);
    itemClone.localScale = Vector3.one * 2;

    var delay = duration / 10;
    duration -= delay;

    Sequence seq = DOTween.Sequence();
    seq.AppendInterval(delay);
    seq.Join(
      itemClone.DOMove(ItemPos.position, duration)
          .SetEase(Ease.Linear)
    );
    seq.Join(
      itemClone.DOScale(Vector3.one / 2, duration)
          .SetEase(Ease.Linear)
    );
    seq.AppendCallback(() => itemClone.gameObject.SetActive(false));
  }

  public void SetItemImg()
  {
    var CurrentMilestone = GameManager.Instance.CurrentMilestone;
    var RewardData = GameManager.Instance.GetRewardStreakAt(CurrentMilestone);
    progerssControl.SetImage(RewardData);
  }
}
