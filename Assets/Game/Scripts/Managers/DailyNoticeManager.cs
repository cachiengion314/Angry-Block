using UnityEngine;
public class DailyNoticeManager : MonoBehaviour
{
  public static DailyNoticeManager Instance { get; private set; }
  public bool _isDailyTask = false;
  public bool _isDailyWeekly = false;
  [Header("---DailyTask---")]
  [SerializeField] GameObject _objNoticeDailyTask;
  [SerializeField] GameObject _objNoticeDailyTaskTab;
  [SerializeField] GameObject _objNoticeDailyTaskPopUp;
  [Header("---DailyWeekly---")]
  [SerializeField] GameObject _objNoticeDailyWeeklyPopup;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }

    _objNoticeDailyTask.SetActive(false);
    _objNoticeDailyTaskTab.SetActive(false);
    _objNoticeDailyTaskPopUp.SetActive(false);
    _objNoticeDailyWeeklyPopup.SetActive(false);

  }
  void OnEnable()
  {
    TimeEventManager.onNextWeek += CheckNextWeekly;
    TimeEventManager.Instance.CheckNextWeekly();

  }
  void OnDisable()
  {
    TimeEventManager.onNextWeek -= CheckNextWeekly;
  }

  void CheckNextWeekly()
  {
    CheckDailyWeekly();
  }

  void Start()
  {
    CheckDailyTask();
    CheckDailyWeekly();
  }

  void CheckDailyTask()
  {
    // RewardThresholds
    for (int i = 0; i < DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson.Count; i++)
    {
      var task = DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson[i];

      if (!task._isPick) continue; // Chỉ tạo các task có _isPick = true
      if (task.currentValueJson >= task.targetValueJson && task._isSuccess == false)
      {
        _isDailyTask = true;
        break;
      }
    }

    int _currentValueTask = PlayerPrefs.GetInt("CurrentValuetodayDailyTask", 0);
    for (int i = 0; i < 3; i++) // i < 3 vì nhiệm vụ hàng ngày có 3 mốc 
    {
      bool isClaimed = PlayerPrefs.GetInt($"RewardDailTaskClaimed_{i}", 0) == 1;

      if (_currentValueTask >= i + 1 && !isClaimed)
      {
        _isDailyTask = true;
      }
    }

  }

  void CheckDailyWeekly()
  {
    foreach (var task in DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson) // check button select
    {
      if (task.currentValueBaseJson >= task.targetValueBaseJson && task.isSuccessBaseJson == false && task.isPickBaseJson == true)
      {
        _isDailyWeekly = true;
        break;
      }

    }

    // 2️ Kiểm tra nếu có phần thưởng chưa nhận nhưng đạt mốc yêu cầu
    int currentValueTask = PlayerPrefs.GetInt("CurrentValuetodayDailyWeeklyTask", 0);
    int[] rewardThresholds = { 3, 6 }; // Đảm bảo đồng bộ với UIDailyWeeklyModal

    for (int i = 0; i < rewardThresholds.Length; i++)
    {
      bool isClaimed = PlayerPrefs.GetInt($"RewardWeeklyClaimed_{i}", 0) == 1;

      if (!isClaimed && currentValueTask >= rewardThresholds[i]) // Nếu chưa nhận & đã đạt mốc
      {
        _isDailyTask = true;
        return;
      }
    }
  }

  void Update()
  {
    if (_isDailyTask)
    {
      _objNoticeDailyTask.SetActive(true);
      _objNoticeDailyTaskTab.SetActive(true);
      _objNoticeDailyTaskPopUp.SetActive(true);
    }
    else
    {
      _objNoticeDailyTask.SetActive(false);
      _objNoticeDailyTaskTab.SetActive(false);
      _objNoticeDailyTaskPopUp.SetActive(false);
    }

    if (_isDailyWeekly)
    {
      _objNoticeDailyTask.SetActive(true);
      _objNoticeDailyTaskTab.SetActive(true);
      _objNoticeDailyWeeklyPopup.SetActive(true);
    }
    else
    {
      _objNoticeDailyWeeklyPopup.SetActive(false);
    }

  }
}
