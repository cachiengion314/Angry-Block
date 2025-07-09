using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VideoAdsControl : MonoBehaviour
{
  [Header("Components")]
  private BoxCollider2D _collider;

  bool _isClicked;

  private void Start()
  {
    _collider = GetComponent<BoxCollider2D>();
  }

  void Update()
  {
    PressControl();
  }

  void PressControl()
  {
    if (Input.touchCount == 1)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

      switch (touch.phase)
      {
        case TouchPhase.Began:
          if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

          if (_collider == Physics2D.OverlapPoint(touchPos))
          {
            if (_isClicked) return;
            if (GameManager.Instance.GetGameState() != GameState.Gameplay) return;
            if (ItemManager.Instance.IsPacking) return;

            LevelPlayAds.Instance.ShowRewardedAd(() =>
            {
              SelfDestroy();
            },
            "VideoAds",
            () =>
            {
              _isClicked = true;
              LeanTween.delayedCall(gameObject, 1.5f, () =>
              {
                _isClicked = false;
              });
            });
          }
          break;
      }
    }
  }

  public void SelfDestroy()
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    var index = trayGrid.ConvertWorldPosToIndex(transform.position);
    if (index < 0 || index > ItemManager.Instance.WoodBoxesGrid.Length - 1) return;
    ItemManager.Instance.WoodBoxesGrid[index] = 0;
    // DailyWeeklyManager.Instance.UpdateTaskProgress(3,1);

    var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.UnlockSlotsWeekly, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.UnlockSlotsWeekly, dataWeekly);

    ItemManager.Instance.WoodBoxes[index] = null;
    Destroy(gameObject);

    var rectangleShadow = EffectManager.Instance.GetRectangleShadowAt(transform.position);
    rectangleShadow.GetComponentInChildren<SpriteRenderer>().color = Color.white;
  }
}
