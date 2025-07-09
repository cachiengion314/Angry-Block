using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using Firebase.Analytics;
using HoangNam;

public class UiDailyWeeklyTasks : MonoBehaviour
{
  [SerializeField] GameObject _prefabTask; // con 0 là bg , 1 là text tile ; 2 là slider , 3 ;là button
  [SerializeField] GameObject _parentTask;
  [Header("---AlLTask---")]
  [SerializeField] Slider _sliderTaskAll;
  [SerializeField] int _allTaskTarget;
  [SerializeField] int _currentValueTask;
  [SerializeField] GameObject[] _Notifys;
  [Header("---ShowReward---")]
  [SerializeField] Button[] _arrayReward;  // có 3 phần quà . nggười con index 0 = đã nhận , 1 = có thể nhận , 2 default 
  [SerializeField] GameObject _objShowReward;
  [SerializeField] GameObject _objContentParent;
  [SerializeField] private int[] _rewardThresholds = { 3, 6 }; // Mốc nhận quà (phải bằng với _arrayReward.lecght)

  List<DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask> unfinishedWeeklyTasks;
  List<DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask> completedWeeklyTasks;
  List<GameObject> allPickWeeklyTasks;
  List<float> _listYTagerDefault;
  private float _spacing = 0f;

  void Awake()
  {
    TimeEventManager.onNextWeek += ResetWeeklyMissions;

    CheckVersoinApplication();
    CheckFirebase();
    this.transform.gameObject.SetActive(false);
  }
  void OnEnable()
  {
    _listYTagerDefault = new();
    unfinishedWeeklyTasks = new();
    completedWeeklyTasks = new();
    allPickWeeklyTasks = new();

    // if (DailyWeeklyManager.Instance != null)
    // {
    //   CheckUpdateDataTaskVersion();
    // }
    // CheckReset();

    CreatPrefabTask();
    SetupAnimShowReward();
    SetupSliderTaskAll();
  }
  void OnDisable()
  {
    UpdateRewardButtonState();
    foreach (var task in DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson) // check button select
    {
      if (task.currentValueBaseJson >= task.targetValueBaseJson && task.isSuccessBaseJson == false && task.isPickBaseJson == true)
      {
        if (DailyNoticeManager.Instance != null)
        {
          DailyNoticeManager.Instance._isDailyWeekly = true;
        }
        break;
      }

    }
  }

  public void CheckVersoinApplication()
  {
    string _version = Application.version;

    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DAILY_WEEKLY, "") == "")
    {
      // DailyWeeklyManager.Instance.LoadDataScriptableObjectToJson();
      // DailyWeeklyManager.Instance.CheckDataFirebase();
      ResetWeeklyMissions();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DAILY_WEEKLY, _version);
      return;
    }

    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DAILY_WEEKLY, "") != _version)
    {
      // DailyWeeklyManager.Instance.LoadDataScriptableObjectToJson();
      // DailyWeeklyManager.Instance.CheckDataFirebase();
      ResetWeeklyMissions();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DAILY_WEEKLY, _version);
    }

  }

  public void CheckFirebase()
  {
    if (DailyWeeklyManager.Instance.needResetReward == true)
    {
      _currentValueTask = 0;
      PlayerPrefs.SetInt(KeyString.KEY_CRURRENT_VALUE_WEEKLY_DAILY_TASK, 0);

      for (int i = 0; i < _arrayReward.Length; i++)
      {
        PlayerPrefs.SetInt($"RewardWeeklyClaimed_{i}", 0);
      }

      DailyWeeklyManager.Instance.needResetReward = false;
    }
  }

  void SetupAnimShowReward()
  {
    // Lặp qua tất cả các con (child) của _objShowReward
    for (int i = 0; i < _objContentParent.transform.childCount; i++)
    {
      Transform child = _objContentParent.transform.GetChild(i);
      child.gameObject.SetActive(false); // Tắt từng GameObject con
    }
    _objContentParent.SetActive(true);
    _objShowReward.SetActive(false);

  }
  void SetUpNotify()
  {
    foreach (GameObject Notify in _Notifys)
    {
      if (Notify.activeSelf) // Nếu Notify đang hiển thị, chạy animation ẩn đi
      {
        Notify.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear)
        .OnComplete(() => Notify.SetActive(false));
      }
    }
  }
  void OnDestroy()
  {
    TimeEventManager.onNextWeek -= ResetWeeklyMissions;
    DailyWeeklyManager.onSave -= CreatPrefabTask;
  }
  void SetupSliderTaskAll()
  {
    _currentValueTask = PlayerPrefs.GetInt(KeyString.KEY_CRURRENT_VALUE_WEEKLY_DAILY_TASK, 0);

    _sliderTaskAll.maxValue = _allTaskTarget;
    _sliderTaskAll.value = _currentValueTask;
    CheckValuesliderAll();
  }

  void CheckValuesliderAll()
  {
    UpdateRewardButtonState(); // Cập nhật trạng thái nút (enable/disable)
    AssignRewardButtonEvents(); // Gán sự kiện cho các nút
  }

  void UpdateRewardButtonState()
  {
    SetDefaultRewardButton(); // Đặt trạng thái mặc định của reward

    for (int i = 0; i < _arrayReward.Length; i++)
    {
      bool isClaimed = PlayerPrefs.GetInt($"RewardWeeklyClaimed_{i}", 0) == 1;

      if (isClaimed)
      {
        // Nếu đã nhận, tắt nút
        _arrayReward[i].interactable = false;
        _arrayReward[i].transform.GetChild(0).gameObject.SetActive(true); // Đã nhận
      }
      else if (_currentValueTask >= _rewardThresholds[i]) // Nếu đạt tiến trình yêu cầu
      {
        _arrayReward[i].interactable = true;
        _arrayReward[i].transform.GetChild(1).gameObject.SetActive(true); // Hiện trạng thái có thể nhận
        if (DailyNoticeManager.Instance != null)
        {
          DailyNoticeManager.Instance._isDailyWeekly = true;
        }
      }
      else
      {
        // _arrayReward[i].interactable = false;
        _arrayReward[i].transform.GetChild(2).gameObject.SetActive(true); // Default
      }
    }
  }

  void SetDefaultRewardButton()
  {
    foreach (Button rewardButton in _arrayReward)
    {
      Transform buttonTransform = rewardButton.gameObject.transform;

      // Lặp qua tất cả các `Child` của nút

      for (int i = 0; i < buttonTransform.childCount; i++)
      {
        buttonTransform.GetChild(i).gameObject.SetActive(false); // Tắt tất cả các `Child`
      }
    }
  }
  void AssignRewardButtonEvents()
  {
    // Gán sự kiện cho các nút phần quà
    for (int i = 0; i < _arrayReward.Length; i++)
    {
      int rewardIndex = i; // Tạo bản sao để tránh lỗi closure
      _arrayReward[i].onClick.RemoveAllListeners(); // Xóa sự kiện cũ
      _arrayReward[i].onClick.AddListener(() => ToggleRewardNoti(rewardIndex)); // Thêm sự kiện mới
    }
  }
  void ToggleRewardNoti(int rewardIndex)
  {
    if (_currentValueTask >= _rewardThresholds[rewardIndex] && PlayerPrefs.GetInt($"RewardWeeklyClaimed_{rewardIndex}", 0) == 0)
    {
      // Nếu đã đạt mốc và chưa nhận, gọi OnRewardClaimed
      OnRewardClaimed(rewardIndex);
    }
    else
    {
      // Nếu không đủ điều kiện nhận quà, toggle noti

      ShowRewardNoti(rewardIndex);
    }
  }
  void ShowRewardNoti(int rewardIndex)
  {
    SetUpNotify();
    GameObject Notif = _arrayReward[rewardIndex].gameObject.transform.GetChild(_arrayReward[rewardIndex].transform.childCount - 1).gameObject;
    if (!Notif.activeSelf)
    {
      Notif.transform.localScale = Vector3.zero;
      Notif.SetActive(true);
      Notif.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear);
    }
    else
    {
      Notif.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear)
 .OnComplete(() => Notif.SetActive(false));
    }
  }
  void OnRewardClaimed(int rewardIndex)
  {
    SoundManager.Instance.PlayRewardDailySfx();
    if (_currentValueTask < _rewardThresholds[rewardIndex])
    {
      Debug.Log("Chưa đủ nhiệm vụ để nhận phần thưởng này!");
      return;
    }
    if (DailyNoticeManager.Instance != null)
    {
      DailyNoticeManager.Instance._isDailyWeekly = false;
    }

    _arrayReward[rewardIndex].interactable = false;
    PlayerPrefs.SetInt($"RewardWeeklyClaimed_{rewardIndex}", 1);

    _arrayReward[rewardIndex].transform.GetChild(0).gameObject.SetActive(true); // Đã nhận
    _arrayReward[rewardIndex].transform.GetChild(1).gameObject.SetActive(false); // Ẩn có thể nhận
    _arrayReward[rewardIndex].transform.GetChild(2).gameObject.SetActive(false); // Ẩn default

    switch (rewardIndex)
    {
      case 0:
        GameManager.Instance.CurrentCoin += 100;
        GameManager.Instance.CurrentRefresh += 1;
        GameManager.Instance.CurrentHammer += 1;
        ShowAnimReward(rewardIndex);

        if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
        {
          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
            new Parameter[]
            {
              new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
              new ("stage", ""),
              new ("booster_name", "Refresh"),
              new ("placement", "DailyWeekly")
            });

          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
            new Parameter[]
            {
              new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
              new ("stage", ""),
              new ("booster_name", "Hammer"),
              new ("placement", "DailyWeekly")
            });
        }

        Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Refresh" + "_DailyWeekly");
        Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Hammer" + "_DailyWeekly");

        break;
      case 1:
        GameManager.Instance.CurrentCoin += 200;
        GameManager.Instance.CurrentRocket += 1;
        GameManager.Instance.CurrentSwap += 1;
        ShowAnimReward(rewardIndex);

        if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
        {
          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
            new Parameter[]
            {
              new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
              new ("stage", ""),
              new ("booster_name", "Rocket"),
              new ("placement", "DailyWeekly")
            });

          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
            new Parameter[]
            {
              new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
              new ("stage", ""),
              new ("booster_name", "Swap"),
              new ("placement", "DailyWeekly")
            });
        }

        Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Rocket" + "_DailyWeekly");
        Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Swap" + "_DailyWeekly");
        break;
        // case 2:
        //     GameManager.Instance.CurrentCoin += 200;
        //     ShowAnimReward(rewardIndex);
        //     break;
    }

    // if (FirebaseSetup.Instance.FirebaseStatusCode)
    // {
    //   // FirebaseAnalytics.LogEvent(KeyString.FIREBASE_WEEKLY_MILESTONE_REACHED + "_" + rewardIndex);
    //   FirebaseAnalytics.LogEvent(KeyString.FIREBASE_WEEKLY_MILESTONE_REACHED,
    //     new Parameter[]
    //     {
    //       new ("id", Utility.FormatIndexToStringXXX(rewardIndex)),
    //     });
    // }
  }
  void ShowAnimReward(int _intShow)
  {
    GameObject Target = _objContentParent.transform.GetChild(_intShow).gameObject;
    Debug.Log(Target.name);
    Target.transform.localScale = Vector3.zero;
    Target.SetActive(true);
    _objShowReward.SetActive(true);
    Target.transform.DOKill();
    // Debug.Log("ShowAnimReward1");
    Target.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear)
    .OnComplete(() =>
    {
      // Debug.Log("ShowAnimReward2");
      Target.GetComponent<Button>().onClick.RemoveAllListeners();
      Target.GetComponent<Button>().onClick.AddListener(() => BtnHideAnimReward(_intShow));
    });
  }
  void BtnHideAnimReward(int _intShow)
  {
    // Debug.Log("BtnHideAnimReward");
    GameObject Target = _objContentParent.transform.GetChild(_intShow).gameObject;
    Target.transform.DOKill();
    Target.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear)
    .OnComplete(() =>
    {
      _objShowReward.SetActive(false);
    });
  }
  public void ResetWeeklyMissions()
  {
    // lưu lại lần cuối update
    // Debug.Log("---ResetNextDay");
    _currentValueTask = 0;
    PlayerPrefs.SetInt(KeyString.KEY_CRURRENT_VALUE_WEEKLY_DAILY_TASK, 0);

    for (int i = 0; i < _arrayReward.Length; i++)
    {
      PlayerPrefs.SetInt($"RewardWeeklyClaimed_{i}", 0);
    }
    // Reset trạng thái _isPick cho tất cả task
    // foreach (var task in DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson)
    // {
    //   task.isPickBaseJson = false; // reset _ispick
    //   task.isSuccessBaseJson = false;
    //   task.isShowNotifBaseJson = false;
    //   task.currentValueBaseJson = 0;
    // }


    foreach (var task in DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson)
    {
      if (task.isLoopTaskBaseJson == true)
      {
        task.isPickBaseJson = true; // reset _ispick
        task.isSuccessBaseJson = false;
        task.isShowNotifBaseJson = false;
      }
      else
      {

        task.isPickBaseJson = false; // reset _ispick
        task.isSuccessBaseJson = false;
        task.currentValueBaseJson = 0;
        task.isShowNotifBaseJson = false;
      }
    }
    // Random lại một task cho mỗi type
    RandomizeTasksPerType();

    // Lưu trạng thái sau khi reset
    DailyWeeklyManager.Instance.SaveDataJsonTask();
    // gọi đến reset data
  }


  void RandomizeTasksPerType()
  {

    Dictionary<string, List<DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask>> tasksByType = new Dictionary<string, List<DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask>>();

    foreach (var taskIsPick in DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson)
    {
      if (taskIsPick.enumTypeTaskBaseJson != "None")
      {
        if (!tasksByType.ContainsKey(taskIsPick.enumTypeTaskBaseJson)) // `task._idTask` có thể được thay bằng thuộc tính `Type` nếu có
        {
          tasksByType[taskIsPick.enumTypeTaskBaseJson] = new List<DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask>();
        }
        tasksByType[taskIsPick.enumTypeTaskBaseJson].Add(taskIsPick);
      }
    }
    Debug.Log("tasksByType.CountUI-----" + tasksByType.Count);
    // Random chọn 1 task từ mỗi type
    foreach (var kvp in tasksByType)
    {
      var taskList = kvp.Value;

      bool hasLoop = taskList.Exists(t => t.isLoopTaskBaseJson);
      if (hasLoop) continue;

      if (taskList.Count > 0)
      {
        int randomIndex = UnityEngine.Random.Range(0, taskList.Count);
        taskList[randomIndex].isPickBaseJson = true; // Đặt task được chọn
      }
    }
    DailyWeeklyManager.Instance.SaveDataJsonTask();
  }

  void CreatPrefabTask()
  {
    // Xóa các prefab cũ (nếu cần thiết)
    foreach (Transform child in _parentTask.transform)
    {
      if (child != null)
      {
        Destroy(child.gameObject);
      }
    }

    if (DailyWeeklyManager.Instance == null)
    {
      return;
    }
    int _indexTaskPick = 0;
    // Tạo các task được chọn (_isPick = true)
    for (int i = 0; i < DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson.Count; i++)
    {
      var task = DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson[i];

      if (!task.isPickBaseJson) continue; // Chỉ tạo các task có _isPick = true
      if (task.isSuccessBaseJson)
      {
        completedWeeklyTasks.Add(task);
      }
      else
      {
        unfinishedWeeklyTasks.Add(task);
      }

    }

    foreach (var task in unfinishedWeeklyTasks)     // Tạo các task đã hoàn thành sau
    {
      GameObject newTaskPrefab = Instantiate(_prefabTask, _parentTask.transform);

      // Tính toán vị trí Y của task mới
      float posY = -_indexTaskPick * newTaskPrefab.GetComponent<RectTransform>().rect.height + _spacing;
      newTaskPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
      _listYTagerDefault.Add(posY);
      // Cập nhật giao diện
      UpdateTaskUI(newTaskPrefab, task);
      allPickWeeklyTasks.Add(newTaskPrefab);
      _indexTaskPick++;
    }

    // Tạo các task đã hoàn thành sau
    foreach (var task in completedWeeklyTasks)
    {
      GameObject newTaskPrefab = Instantiate(_prefabTask, _parentTask.transform);

      // Tính toán vị trí Y của task mới
      float posY = (-_indexTaskPick * newTaskPrefab.GetComponent<RectTransform>().rect.height + _spacing);
      newTaskPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
      _listYTagerDefault.Add(posY);
      // Cập nhật giao diện
      UpdateTaskUI(newTaskPrefab, task);

      _indexTaskPick++;
      allPickWeeklyTasks.Add(newTaskPrefab);
    }

    RectTransform parentRectTransform = _parentTask.GetComponent<RectTransform>();
    float newHeight = allPickWeeklyTasks.Count * _prefabTask.GetComponent<RectTransform>().rect.height + 60f; // kích thước thằng cha
    parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, newHeight);
  }


  void UpdateTaskUI(GameObject newTaskPrefab, DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask task)
  {
    // Cập nhật các thành phần UI của task
    Image BgTask = newTaskPrefab.GetComponentInChildren<Image>();
    Image imageIconTask = newTaskPrefab.transform.GetChild(0).GetComponent<Image>();
    TMP_Text taskNameText = newTaskPrefab.transform.GetChild(1).GetComponent<TMP_Text>();
    Slider taskProgressSlider = newTaskPrefab.transform.GetChild(2).GetComponent<Slider>();
    TMP_Text currentValueText = taskProgressSlider.transform.GetChild(taskProgressSlider.transform.childCount - 1).GetComponent<TMP_Text>();
    GameObject CheckBox = newTaskPrefab.transform.GetChild(3).gameObject;
    GameObject Tick = CheckBox.transform.GetChild(0).gameObject;
    Button collectButton = newTaskPrefab.transform.GetChild(4).GetComponent<Button>();

    Image BgReceived = newTaskPrefab.transform.GetChild(5).GetComponent<Image>();
    GameObject OBjEffect = newTaskPrefab.transform.GetChild(6).gameObject;
    OBjEffect.SetActive(false);
    if (task.isSuccessBaseJson == true)
    {
      BgReceived.gameObject.SetActive(true);

    }
    else
    {
      BgReceived.gameObject.SetActive(false);
    }

    // Cập nhật giao diện
    if (taskNameText != null)
    {
      taskNameText.text = task.strNameTaskBaseJson;
    }
    if (imageIconTask != null)
    {
      int iconTaskId = 0;

      if (Enum.TryParse(typeof(enumListeningDataDailyWeekly), task.enumTypeChildWeeklyListeningBaseJson, out object result))
      {
        iconTaskId = (int)result - 1; //(vì có none đứng đầu)
      }
    }

    if (taskProgressSlider != null)
    {
      taskProgressSlider.maxValue = task.targetValueBaseJson;
      taskProgressSlider.value = task.currentValueBaseJson;
      currentValueText.text = $"{task.currentValueBaseJson}/{task.targetValueBaseJson}";
    }

    // Cập nhật trạng thái nút thu thập
    collectButton.interactable = !task.isSuccessBaseJson && task.currentValueBaseJson >= task.targetValueBaseJson;
    collectButton.gameObject.SetActive(!task.isSuccessBaseJson);

    UpdateCollectButtonState(collectButton, task, Tick, BgTask, OBjEffect);


    // Gán sự kiện cho nút thu thập
    collectButton.onClick.RemoveAllListeners();
    collectButton.onClick.AddListener(() =>
    {
      ActionButtonTask(collectButton.gameObject, task, Tick, newTaskPrefab, BgReceived, OBjEffect);
    });

  }

  void UpdateCollectButtonState(Button collectButton, DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask task, GameObject _Tick, Image _BgImageTask, GameObject _objEffect)
  {
    if (task.isSuccessBaseJson)
    {
      // Nhiệm vụ đã hoàn thành, nút bị vô hiệu hóa và ẩn
      collectButton.interactable = false;
      collectButton.gameObject.SetActive(false);
      _Tick.SetActive(true);
    }
    else if (task.currentValueBaseJson >= task.targetValueBaseJson)
    {
      // Nhiệm vụ đạt tiến trình yêu cầu, nút có thể nhấn
      _Tick.SetActive(false);
      collectButton.interactable = true;
      collectButton.gameObject.SetActive(true);
      _objEffect.SetActive(true);

      if (DailyNoticeManager.Instance != null)
      {
        DailyNoticeManager.Instance._isDailyWeekly = true;
      }

    }
    else
    {
      // Nhiệm vụ chưa đạt tiến trình yêu cầu, nút bị vô hiệu hóa
      collectButton.interactable = false;
      collectButton.gameObject.SetActive(true); // Vẫn hiển thị nút nhưng không thể nhấn
      _Tick.SetActive(false);
    }
  }

  void ActionButtonTask(GameObject _gameObject, DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask task, GameObject _Tick, GameObject _buttonClick, Image _bgReceived, GameObject _objEffect)
  {
    SoundManager.Instance.PlaySuggesTaksDailySfx();
    if (DailyNoticeManager.Instance != null)
    {
      DailyNoticeManager.Instance._isDailyWeekly = false;
    }
    _gameObject.SetActive(false);
    _currentValueTask++;
    PlayerPrefs.SetInt(KeyString.KEY_CRURRENT_VALUE_WEEKLY_DAILY_TASK, _currentValueTask);

    CheckValuesliderAll();
    UpdatSliderTaskAll();

    AnimClickTaskComplete(_buttonClick, _objEffect, _bgReceived, _Tick);
    // DailyWeeklyManager.Instance.weeklyTaskDataJsonBase.WeeklyTasksBaseJson[_idTask].isSuccessBaseJson = true;
    task.isSuccessBaseJson = true;
    DailyWeeklyManager.Instance.SaveDataJsonTask();

    // if (FirebaseSetup.Instance.FirebaseStatusCode)
    // {
    //   // FirebaseAnalytics.LogEvent(KeyString.FIREBASE_WEEKLY_QUEST_COMPLETE + "_" + _idTask);
    //   FirebaseAnalytics.LogEvent(KeyString.FIREBASE_WEEKLY_QUEST_COMPLETE,
    //     new Parameter[]
    //     {
    //       new ("id", Utility.FormatIndexToStringXXX(_idTask)),
    //     });
    // }
  }
  void AnimClickTaskComplete(GameObject _BtnClick, GameObject _objEffect, Image _bgReceived, GameObject _Tick)
  {

    _Tick.SetActive(true);
    _Tick.transform.localScale = Vector3.one * 1.8f;
    _Tick.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
          _bgReceived.gameObject.SetActive(true);
          _objEffect.SetActive(false);

          // Sau khi hoàn thành thay đổi kích thước, di chuyển button
          RectTransform buttonRect = _BtnClick.GetComponent<RectTransform>();

          // Tính toán vị trí y để di chuyển button xuống cuối cùng
          int taskCount = _parentTask.transform.childCount - 1;  // Tổng số nhiệm vụ trong _parentTask
          float targetY = -(taskCount * _BtnClick.GetComponent<RectTransform>().rect.height - (taskCount - 1) * _spacing);

          // Di chuyển button xuống cuối cùng với một animation
          buttonRect.DOAnchorPosY(targetY, 0.5f) // Di chuyển trong 0.5 giây
                .SetEase(Ease.OutQuad).OnComplete(() =>
                {
                  // Có thể thực hiện thêm hành động sau khi di chuyển
                });


          MoveTasksBelowButton(_BtnClick);
        });

  }

  void MoveTasksBelowButton(GameObject _BtnClick)
  {
    // Lấy RectTransform của button
    RectTransform buttonRect = _BtnClick.GetComponent<RectTransform>();

    // Duyệt qua tất cả các nhiệm vụ và di chuyển các nhiệm vụ dưới nút bấm lên trên
    int index = 0; // Để xác định vị trí của task trong allPickTasks
                   // bool foundButton = false; // Biến kiểm tra nếu đã tìm thấy button
    int btnClickIndex = -1;
    for (int i = 0; i < _parentTask.transform.childCount; i++)
    {
      if (_parentTask.transform.GetChild(i).gameObject == _BtnClick)
      {
        btnClickIndex = i;
        break;
      }
    }

    // Duyệt qua tất cả các nhiệm vụ và di chuyển các nhiệm vụ dưới nút bấm lên trên
    foreach (Transform taskTransform in _parentTask.transform)
    {
      RectTransform taskRect = taskTransform.GetComponent<RectTransform>();

      if (index > btnClickIndex) // Chỉ di chuyển task có index lớn hơn _BtnClick
      {
        // Di chuyển task
        // float newPosY = taskRect.anchoredPosition.y + taskRect.rect.height;
        float newPosY = _listYTagerDefault[index - 1];
        // Debug.Log("newPosY--" + newPosY);
        taskRect.DOAnchorPosY(newPosY, 0.5f).SetEase(Ease.OutQuad);
      }

      index++; // Tăng index để tiếp tục xử lý các task tiếp theo
    }
    _BtnClick.transform.SetSiblingIndex(_parentTask.transform.childCount - 1);
  }

  void UpdatSliderTaskAll()
  {
    _sliderTaskAll.DOKill();
    _sliderTaskAll.DOValue(_currentValueTask, 0.2f) // Tween đến giá trị mới trong 0.5 giây
    .SetEase(Ease.OutQuad); // Tăng tốc mượt mà

  }

}
