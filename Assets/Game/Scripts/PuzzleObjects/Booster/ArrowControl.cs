using Firebase.Analytics;
using HoangNam;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ArrowControl : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField] LayerMask balloonLayer;

  private BoxCollider2D _collider;

  int _index;

  public void InjectIndex(int index)
  {
    _index = index;
  }

  private void Start()
  {
    _collider = GetComponent<BoxCollider2D>();
  }

  void Update()
  {
    PressControl();
  }

  public void RemovePlacedTrays()
  {
    if (GameManager.Instance.CurrentRocket <= 0) return;
    if (!PowerItemPanel.Instance.IsTriggeredRocket) return;
    PowerItemPanel.Instance.IsTriggeredRocket = false;

    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_ROCKET) == 0)
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_ROCKET, 1);
    else
      GameManager.Instance.CurrentRocket--;

    var data = new MissonDataDailyTask(enumListeningDataDailyTask.Booster3, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyTask.Booster3, data);

    var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.UsePowerItem, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.UsePowerItem, dataWeekly);

    PowerItemPanel.Instance.UpdateAmountUI();
    PowerItemPanel.Instance.DeTrigger();

    var toPos = transform.position + Vector3.up * 8.0f;
    EffectManager.Instance.SpawnRocketAt(transform.position + Vector3.right * .16f, toPos, out float time);

    LeanTween.delayedCall(gameObject, 1.5f, () =>
    {
      ItemManager.Instance.RemoveWoodBoxesAt(_index);
      ItemManager.Instance.RemoveIceBoxesAt(_index);
      ItemManager.Instance.RemoveGrassesAt(_index);
      ItemManager.Instance.RemoveGrillerAt(_index);
      ItemManager.Instance.RemoveGiftBoxAt(_index);
      ItemManager.Instance.RemoveCoffeeBoardAt(_index);
      ItemManager.Instance.RemovePlantPotAt(_index);
      ItemManager.Instance.RemoveCoverLetsAt(_index);
      ItemManager.Instance.RemoveCurtainLayersAt(_index);
      ItemManager.Instance.RemoveMoneyBagAt(_index);
      ItemManager.Instance.RemoveCupBoardsAt(_index);
      ItemManager.Instance.RemoveMachineCreamAt(_index);
      ItemManager.Instance.RemoveMagicNestAt(_index);
      ItemManager.Instance.RemoveLeavesFlowersAt(_index);
      ItemManager.Instance.RemoveFlowerPotAt(_index);
      ItemManager.Instance.RemoveBeverageFridgesAt(_index);
    });

    EffectManager.Instance.TurnOffArrows();
    EffectManager.Instance.TurnOffDarkImg();

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_SPEND,
         new Parameter[]
         {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("booster_name", "Rocket"),
            new ("placement", "Ingame")
         });
    }
  }

  void PressControl()
  {
    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

      Collider2D _col = Physics2D.OverlapPoint(touchPos, balloonLayer);
      if (_col != null) return;

      switch (touch.phase)
      {
        case TouchPhase.Began:
          if (_collider == Physics2D.OverlapPoint(touchPos))
          {
            PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_ROCKET, 1);
            RemovePlacedTrays();
            return;
          }
          // touching other items will not cause any effect
          break;
      }
    }
  }
}
