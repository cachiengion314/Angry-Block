using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Pool;

public class CoinControl : MonoBehaviour, IPoolItemControl
{
  [Header("Internal dependencies")]
  [SerializeField] MMPositionShaker MMPositionShaker;
  [SerializeField] BoxCollider2D _collider;
  ObjectPool<GameObject> coinPool;

  [Header("Settings")]
  int _index;
  public int CoinAmount;

  private void OnEnable()
  {
    _collider.enabled = false;
    MMPositionShaker.Channel = GetInstanceID();

    LeanTween.delayedCall(gameObject, 1.4f, () =>
    {
      if (_collider == null) return;
      _collider.enabled = true;
    });
  }

  void Update()
  {
    PressControl();
  }

  public void InjectIndex(int index)
  {
    _index = index;
  }

  public void InjectPool(ObjectPool<GameObject> coinPool, ObjectPool<GameObject> other = null)
  {
    this.coinPool = coinPool;
  }

  public void Release()
  {
    if (gameObject.activeSelf) coinPool.Release(gameObject);
  }

  public void RemoveFromTable()
  {
    ItemManager.Instance.Coins[_index] = null;
    Release();
  }

  void PressControl()
  {
    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
      if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

      switch (touch.phase)
      {
        case TouchPhase.Began:
          if (_collider == Physics2D.OverlapPoint(touchPos))
          {
            TriggerEarnCoin();
            return;
          }
          // touching other items will not cause any effect
          break;
      }
    }
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    TriggerEarnCoin();
  }

  public void TriggerEarnCoin(bool isShowText = true)
  {
    SoundManager.Instance.PlayHandOnCoinSfx();

    // DailyGoalManager.Instance.DailyGoalDatas[(int)DailyGoalType.CollectCoin].CurrentAmount++;
    // DailyTaskManager.Instance.UpdateTaskProgress(8, 1);

    var data = new MissonDataDailyTask(enumListeningDataDailyTask.CollectCoin, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyTask.CollectCoin, data);

    // DailyWeeklyManager.Instance.UpdateTaskProgress(0, 1);
    // DailyGoalManager.Instance.CheckCompletedTodayDailyGoal(DailyGoalType.CollectCoin);

    var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.CollectCoinWeekly, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.CollectCoinWeekly, dataWeekly);

    if (isShowText)
    {
      ShowPanel.Instance.ShowTextAt(
        transform.position, "+" + CoinAmount.ToString(), Color.yellow, null, 1.1f
      );
    }
    EffectManager.Instance.SpawnGoldSplashAt(transform.position);
    EffectManager.Instance.EmissingCoinsWithParticleTo(
        ShowPanel.Instance.GlobalCoinImg.transform, CoinAmount, transform, () =>
        {
          if (CoinAmount == 1)
          {
            SoundManager.Instance.PlayCoinDropSfx();
          }
          else if (CoinAmount > 1)
          {
            SoundManager.Instance.PlayCoinReceiveSfx();
          }
          GameManager.Instance.CurrentCoin += CoinAmount;
        }
      );
    RemoveFromTable();
  }

  public void FullyRemoveFromTable()
  {
    RemoveFromTable();
  }
}
