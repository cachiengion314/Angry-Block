using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using HoangNam;
using Firebase.Analytics;

public class UIDailyTaskModal : MonoBehaviour
{
  [SerializeField] GameObject _prefabTask; // con 0 là bg , 1 là text tile ; 2 là slider , 3 ;là button
  [SerializeField] GameObject _parentTask;
  [Header("---AlLTask---")]
  [SerializeField] Slider _sliderTaskAll;
  [SerializeField] int _allTaskTarget;
  [SerializeField] int _currentValueTask;
  [SerializeField] Button[] _arrayReward;  // có 3 phần quà . nggười con index 0 = đã nhận , 1 = có thể nhận , 2 default 
  [Header("---ShowReward---")]
  [SerializeField] GameObject _objShowReward;
  [SerializeField] GameObject _objContentParent;
  [SerializeField] private int[] _rewardThresholds = { 1, 2, 3 }; // Mốc nhận quà (phải bằng với _arrayReward.lecght)
  public int[] RewardThresholds => _rewardThresholds;

  [Header("Light Img")]
  [SerializeField] Image[] lightImgs;
  List<DailyTaskManager.TaskDataJsonBase.TaskJson> unfinishedTasks;
  List<DailyTaskManager.TaskDataJsonBase.TaskJson> completedTasks;
  List<DailyTaskManager.TaskDataJsonBase.TaskJson> allPickTasks;
  List<float> _listYTaskTagerDefault;
  private float _spacing = 0f;

  // [SerializeField] VerticalLayoutGroup verticalLayoutGroup;

  /// <summary>
  /// TestCheat
  /// </summary>
  /// 


  void Awake()
  {
    //   

    // CheckIsExistingData();
    CheckVersoinApplication();
    CheckFirebase();

    // CheckToday();
    // CreatPrefabTask();
    this.gameObject.SetActive(false);
  }
  void OnEnable()
  {
    _listYTaskTagerDefault = new();
    allPickTasks = new List<DailyTaskManager.TaskDataJsonBase.TaskJson>();
    unfinishedTasks = new List<DailyTaskManager.TaskDataJsonBase.TaskJson>();
    completedTasks = new List<DailyTaskManager.TaskDataJsonBase.TaskJson>();

    // CheckUpdateDataTaskVersion();
    // CheckFirebase();
    CheckToday();
    CreatPrefabTask();
    SetupAnimShowReward();


  }
  void OnDisable()
  {
    UpdateState_isDayLyTask();
  }
  // void CheckIsExistingData()
  // {
  //   SaveSystem.ChecFileExist(
  //        "TaskData", () =>
  //        {
  //          DailyTaskManager.Instance.ChangeScriptableObjectToJson();
  //          ResetNextDay();
  //        }, () =>
  //        {
  //          DailyTaskManager.Instance.LoadDataJsonTask();
  //        }
  //      );
  // }
  public void CheckFirebase()
  {

    if (DailyTaskManager.Instance.needResetReward == true)
    {
      _currentValueTask = 0;
      PlayerPrefs.SetInt(KeyString.KEY_CRURRENT_VALUE_TODAY_DAILY_TASK, 0);

      for (int i = 0; i < _arrayReward.Length; i++)
      {
        PlayerPrefs.SetInt($"RewardDailTaskClaimed_{i}", 0);
      }

      DailyTaskManager.Instance.needResetReward = false;
    }
  }
  public void CheckVersoinApplication()
  {
    string _version = Application.version;

    // Debug.Log("_version0.0--: " + _version);
    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DAILY_TASK, "") == "")
    {
      Debug.Log("_version1.0--: " + _version);
      // DailyTaskManager.Instance.ChangeScriptableObjectToJson();
      // DailyTaskManager.Instance.LoadDataScriptableObjectToJson();
      // DailyTaskManager.Instance.CheckDataFirebase();
      ResetNextDay();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DAILY_TASK, _version);
      return;
    }

    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DAILY_TASK, "") != _version)
    {
      Debug.Log("_version1.1--: " + _version);
      // DailyTaskManager.Instance.ChangeScriptableObjectToJson();
      // DailyTaskManager.Instance.LoadDataScriptableObjectToJson();
      // DailyTaskManager.Instance.CheckDataFirebase();
      ResetNextDay();

      PlayerPrefs.SetString(KeyString.KEY_VERSION_DAILY_TASK, _version);
    }

  }
  void UpdateState_isDayLyTask()
  {
    UpdateRewardButtonState(); // check button Reward  
    foreach (var task in DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson) // check button select
    {
      if (task.currentValueJson >= task.targetValueJson && task._isSuccess == false && task._isPick == true)
      {
        // Nếu có nhiệm vụ đạt điều kiện, đặt flag thành true
        if (DailyNoticeManager.Instance != null)
        {
          DailyNoticeManager.Instance._isDailyTask = true;
        }
        break;
      }

    }
  }
  // public void CheckUpdateDataTaskVersion() // kiểm tra dữ liệu các nhiệm vụ xem có khớp với dữ lụu trong ScriptableObject
  // {
  //   // Kiểm tra số lượng nhiệm vụ


  //   if (DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson.Count != DailyTaskManager.Instance._taskData.AllTasks.Count)
  //   {
  //     // Nếu số lượng khác nhau hoặc file JSON không tồn tại, chạy lại ChangeScriptableObjectToJson
  //     DailyTaskManager.Instance.ChangeScriptableObjectToJson();
  //     UpdateDataTaskVersion();
  //     return;
  //   }
  //   for (int i = 0; i < DailyTaskManager.Instance._taskData.AllTasks.Count; i++)
  //   {

  //     var scriptableTask = DailyTaskManager.Instance._taskData.AllTasks[i];

  //     var jsonTask = DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson
  //     .Find(t => t.strKeyTaskJson == scriptableTask.enumTypeChildListening.ToString() + "_" + scriptableTask.targetValue);

  //     if (jsonTask == null)// nếu sửa đổi tên sẽ update lại
  //     {
  //       DailyTaskManager.Instance.ChangeScriptableObjectToJson();
  //       UpdateDataTaskVersion();
  //       return;
  //     }
  //     // Nếu không tìm thấy nhiệm vụ hoặc tên nhiệm vụ khác, đồng bộ lại dữ liệu
  //     if (jsonTask.strNameTaskJson != scriptableTask.strNameTask)// nếu sửa đổi tên sẽ update lại
  //     {

  //       DailyTaskManager.Instance.ChangeScriptableObjectToJson();
  //       UpdateDataTaskVersion();
  //       return;
  //     }
  //     if (jsonTask.targetValueJson != scriptableTask.targetValue) //nếu sửa đổi nhiệm vụ sẽ update lại
  //     {
  //       DailyTaskManager.Instance.ChangeScriptableObjectToJson();
  //       UpdateDataTaskVersion();
  //       return;
  //     }
  //   }
  // }
  public void UpdateDataTaskVersion() // update lại dữ liệu
  {
    ResetNextDay();// reset Dữ Liệu
    SetupSliderTaskAll();
  }

  void CheckToday()
  {
    string STRToday = DateTime.Now.ToString("yyyyMMdd");
    if (PlayerPrefs.GetString(KeyString.KEY_TODAY_DAILY_TASK) == "")
    {
      PlayerPrefs.SetString(KeyString.KEY_TODAY_DAILY_TASK, STRToday); // trường hợp ngày đầu tiên chưa được khởi tạo
      ResetNextDay();
    }
    string STRtodayData = PlayerPrefs.GetString(KeyString.KEY_TODAY_DAILY_TASK);

    if (STRtodayData != STRToday)
    {
      PlayerPrefs.SetString(KeyString.KEY_TODAY_DAILY_TASK, STRToday);
      ResetNextDay();// reset Dữ Liệu
    }
    SetupSliderTaskAll();
  }
  void SetupSliderTaskAll()
  {
    _currentValueTask = PlayerPrefs.GetInt(KeyString.KEY_CRURRENT_VALUE_TODAY_DAILY_TASK, 0);

    _sliderTaskAll.maxValue = _allTaskTarget;
    _sliderTaskAll.value = _currentValueTask;
    CheckValuesliderAll();
  }
  void CheckValuesliderAll()
  {
    UpdateRewardButtonState(); // Cập nhật trạng thái nút (enable/disable)
    AssignRewardButtonEvents(); // Gán sự kiện cho các nút
  }

  void SetDefaultRewardButton()
  {
    foreach (Button rewardButton in _arrayReward)
    {
      Transform buttonTransform = rewardButton.gameObject.transform;

      for (int i = 0; i < buttonTransform.childCount; i++)
      {
        buttonTransform.GetChild(i).gameObject.SetActive(false); // Tắt tất cả các `Child`
      }
    }
  }

  void UpdateRewardButtonState()
  {
    SetDefaultRewardButton(); // Đặt trạng thái mặc định của reward

    for (int i = 0; i < _arrayReward.Length; i++)
    {
      bool isClaimed = PlayerPrefs.GetInt($"RewardDailTaskClaimed_{i}", 0) == 1;


      if (isClaimed)
      {
        // Nếu đã nhận, tắt nút
        _arrayReward[i].interactable = false;
        _arrayReward[i].transform.GetChild(0).gameObject.SetActive(true); // Đã nhận
        lightImgs[i].gameObject.SetActive(false);
      }
      else if (_currentValueTask >= _rewardThresholds[i]) // Nếu đạt tiến trình yêu cầu
      {
        _arrayReward[i].interactable = true;
        _arrayReward[i].transform.GetChild(2).gameObject.SetActive(true); // Hiện trạng thái có thể nhận
        lightImgs[i].gameObject.SetActive(true);

        if (DailyNoticeManager.Instance != null)
        {
          DailyNoticeManager.Instance._isDailyTask = true;
        }
      }
      else
      {
        _arrayReward[i].interactable = false;
        _arrayReward[i].transform.GetChild(3).gameObject.SetActive(true); // Default
        lightImgs[i].gameObject.SetActive(false);
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
    if (_currentValueTask >= _rewardThresholds[rewardIndex] && PlayerPrefs.GetInt($"RewardDailTaskClaimed_{rewardIndex}", 0) == 0)
    {
      // Nếu đã đạt mốc và chưa nhận, gọi OnRewardClaimed
      OnRewardClaimed(rewardIndex);
    }
    else
    {
      // Nếu không đủ điều kiện nhận quà, toggle noti

      // ShowRewardNoti(rewardIndex);
    }
  }
  void OnRewardClaimed(int rewardIndex) // button nhận quà
  {
    SoundManager.Instance.PlayRewardDailySfx();
    // SoundSystem.Instance.PlayCollectRewardSfx();
    if (DailyNoticeManager.Instance != null)
    {
      DailyNoticeManager.Instance._isDailyTask = false;
    }
    // Vô hiệu hóa nút sau khi nhận quà
    _arrayReward[rewardIndex].interactable = false;
    PlayerPrefs.SetInt($"RewardDailTaskClaimed_{rewardIndex}", 1);
    // Xử lý logic nhận phần quà ở đây (cộng tiền, vật phẩm, v.v.)
    // Ví dụ:
    // AddCoins(100 * (rewardIndex + 1));
    _arrayReward[rewardIndex].gameObject.transform.GetChild(0).gameObject.SetActive(true);
    lightImgs[rewardIndex].gameObject.SetActive(false);
    _arrayReward[rewardIndex].gameObject.transform.GetChild(2).gameObject.SetActive(false);
    _arrayReward[rewardIndex].gameObject.transform.GetChild(3).gameObject.SetActive(false);
    switch (rewardIndex)
    {
      case 0:
        GameManager.Instance.CurrentCoin += 20;
        ShowAnimReward(rewardIndex);

        break;

      case 1:

        GameManager.Instance.CurrentRefresh += 1;
        ShowAnimReward(rewardIndex);
        if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
        {
          FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_EARN,
             new Parameter[]
             {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("stage", ""),
            new ("booster_name", "Refresh"),
            new ("placement", "DailyTask")
             });
        }

        Utility.Print("Level_" + (GameManager.Instance.CurrentLevel + 1) + "Refresh" + "_DailyTask");

        break;

      case 2:
        GameManager.Instance.CurrentCoin += 50;
        GameManager.Instance.CurrentHammer += 1;
        ShowAnimReward(rewardIndex);

        break;
    }

    // if (FirebaseSetup.Instance.FirebaseStatusCode)
    // {
    //   // FirebaseAnalytics.LogEvent(KeyString.FIREBASE_DAILY_GOAL_REACHED + "_" + rewardIndex);
    //   FirebaseAnalytics.LogEvent(KeyString.FIREBASE_DAILY_GOAL_REACHED,
    //   new Parameter[]
    //   {
    //     new ("id", Utility.FormatIndexToStringXXX(rewardIndex))
    //   });
    // }
  }
  void ResetNextDay()
  {

    if (DailyNoticeManager.Instance != null)
    {
      DailyNoticeManager.Instance._isDailyTask = true;
    }
    _currentValueTask = 0;
    PlayerPrefs.SetInt(KeyString.KEY_CRURRENT_VALUE_TODAY_DAILY_TASK, 0);

    for (int i = 0; i < _arrayReward.Length; i++)
    {
      PlayerPrefs.SetInt($"RewardDailTaskClaimed_{i}", 0);
    }
    // Reset trạng thái _isPick cho tất cả task
    foreach (var task in DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson)
    {
      if (task._isLoopTask == true)
      {
        task._isPick = true; // reset _ispick
        task._isSuccess = false;
        task._isShowNotif = false;
      }
      else
      {
        task._isPick = false; // reset _ispick
        task._isSuccess = false;
        task.currentValueJson = 0;
        task._isShowNotif = false;
      }
    }
    // Random lại một task cho mỗi type
    RandomizeTasksPerType();

    // Lưu trạng thái sau khi reset
    DailyTaskManager.Instance.SaveDataJsonTask();
  }
  void RandomizeTasksPerType()
  {
    // Group các nhiệm vụ theo type
    Dictionary<string, List<DailyTaskManager.TaskDataJsonBase.TaskJson>> tasksByType = new Dictionary<string, List<DailyTaskManager.TaskDataJsonBase.TaskJson>>();

    foreach (var task in DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson)
    {
      if (task.enumTypeTaskJson != "None")
      {
        if (!tasksByType.ContainsKey(task.enumTypeTaskJson)) // `task._idTask` có thể được thay bằng thuộc tính `Type` nếu có
        {
          tasksByType[task.enumTypeTaskJson] = new List<DailyTaskManager.TaskDataJsonBase.TaskJson>();
        }
        tasksByType[task.enumTypeTaskJson].Add(task);
      }
    }

    // Random chọn 1 task từ mỗi type
    foreach (var kvp in tasksByType)
    {
      var taskList = kvp.Value;

      bool hasLoop = taskList.Exists(t => t._isLoopTask);
      if (hasLoop) continue;

      if (taskList.Count > 0)
      {
        int randomIndex = UnityEngine.Random.Range(0, taskList.Count);
        taskList[randomIndex]._isPick = true; // Đặt task được chọn
      }
    }
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

    if (DailyTaskManager.Instance == null)
    {
      return;
    }

    int _indexTaskPick = 0; // index để set vị trí cho newTaskPrefab; (-1 để nhiệm vụ đầu tiên được chọn = 0 (set đúng vị trí))
    for (int i = 0; i < DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson.Count; i++)
    {
      var task = DailyTaskManager.Instance._taskDataJsonBase.TasksTodayJson[i];

      if (!task._isPick) continue; // Chỉ tạo các task có _isPick = true
      if (task._isSuccess)
      {
        completedTasks.Add(task);
      }
      else
      {
        unfinishedTasks.Add(task);
      }

      allPickTasks.Add(task);
    }

    foreach (var task in unfinishedTasks)     // Tạo các task đã hoàn thành sau
    {
      GameObject newTaskPrefab = Instantiate(_prefabTask, _parentTask.transform);

      // Tính toán vị trí Y của task mới
      float posY = -_indexTaskPick * newTaskPrefab.GetComponent<RectTransform>().rect.height + _spacing;
      newTaskPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);

      _listYTaskTagerDefault.Add(posY);
      // Cập nhật giao diện
      UpdateTaskUI(newTaskPrefab, task);

      _indexTaskPick++;
    }

    // Tạo các task đã hoàn thành sau
    foreach (var task in completedTasks)
    {
      GameObject newTaskPrefab = Instantiate(_prefabTask, _parentTask.transform);

      // Tính toán vị trí Y của task mới
      float posY = (-_indexTaskPick * newTaskPrefab.GetComponent<RectTransform>().rect.height + _spacing);
      newTaskPrefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, posY);
      _listYTaskTagerDefault.Add(posY);
      // Cập nhật giao diện
      UpdateTaskUI(newTaskPrefab, task);

      _indexTaskPick++;
    }
    // set height cho thằng cho để kéo đưọc srollview
    RectTransform parentRectTransform = _parentTask.GetComponent<RectTransform>();
    float newHeight = allPickTasks.Count * _prefabTask.GetComponent<RectTransform>().rect.height;
    parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, newHeight);


  }
  void UpdateTaskUI(GameObject newTaskPrefab, DailyTaskManager.TaskDataJsonBase.TaskJson task)
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
    if (task._isSuccess == true)
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
      taskNameText.text = task.strNameTaskJson;
    }
    if (imageIconTask != null)
    {
      int iconTaskId = 0;
      if (Enum.TryParse(typeof(enumListeningDataDailyTask), task.enumTypeChildListeningJson, out object result))
      {
        iconTaskId = (int)result - 1; //(vì có none đứng đầu)
      }
    }

    if (taskProgressSlider != null)
    {
      taskProgressSlider.maxValue = task.targetValueJson;
      taskProgressSlider.value = task.currentValueJson;
      currentValueText.text = $"{task.currentValueJson}/{task.targetValueJson}";
    }

    // Cập nhật trạng thái nút thu thập
    collectButton.interactable = !task._isSuccess && task.currentValueJson >= task.targetValueJson;
    collectButton.gameObject.SetActive(!task._isSuccess);

    UpdateCollectButtonState(collectButton, task, Tick, BgTask, OBjEffect);


    // Gán sự kiện cho nút thu thập
    collectButton.onClick.RemoveAllListeners();
    collectButton.onClick.AddListener(() =>
    {
      int iconTaskId = 0;
      if (Enum.TryParse(typeof(enumListeningDataDailyTask), task.enumTypeChildListeningJson, out object result))
      {
        iconTaskId = (int)result - 1; //(vì có none đứng đầu)
      }
      ActionButtonTask(collectButton.gameObject, task, Tick, newTaskPrefab, BgReceived, OBjEffect);
    });

  }

  void UpdateCollectButtonState(Button collectButton, DailyTaskManager.TaskDataJsonBase.TaskJson task, GameObject _Tick, Image _BgImageTask, GameObject _objEffect)
  {
    if (task._isSuccess)
    {
      // Nhiệm vụ đã hoàn thành, nút bị vô hiệu hóa và ẩn
      collectButton.interactable = false;
      collectButton.gameObject.SetActive(false);
      _Tick.SetActive(true);
    }
    else if (task.currentValueJson >= task.targetValueJson)
    {
      // Nhiệm vụ đạt tiến trình yêu cầu, nút có thể nhấn
      _Tick.SetActive(false);
      collectButton.interactable = true;
      collectButton.gameObject.SetActive(true);

      _objEffect.SetActive(true);
      if (DailyNoticeManager.Instance != null)
      {
        DailyNoticeManager.Instance._isDailyTask = true;
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
  void ActionButtonTask(GameObject _gameObject, DailyTaskManager.TaskDataJsonBase.TaskJson task, GameObject _Tick, GameObject _buttonClick, Image _bgReceived, GameObject _objEffect)
  {
    SoundManager.Instance.PlaySuggesTaksDailySfx();
    if (DailyNoticeManager.Instance != null)
    {
      DailyNoticeManager.Instance._isDailyTask = false;
    }
    _gameObject.SetActive(false);
    // verticalLayoutGroup.enabled = false;
    _currentValueTask++;
    PlayerPrefs.SetInt(KeyString.KEY_CRURRENT_VALUE_TODAY_DAILY_TASK, _currentValueTask);

    CheckValuesliderAll();
    UpdatSliderTaskAll();
    AnimClickTaskComplete(_buttonClick, _objEffect, _bgReceived, _Tick);

    task._isSuccess = true;

    DailyTaskManager.Instance.SaveDataJsonTask();

    // if (FirebaseSetup.Instance.FirebaseStatusCode)
    // {
    //   // FirebaseAnalytics.LogEvent(KeyString.FIREBASE_DAILY_GOAL_COMPLETE, new Parameter[]{new Parameter(_idTask)});
    //   FirebaseAnalytics.LogEvent(KeyString.FIREBASE_DAILY_GOAL_COMPLETE,
    //     new Parameter[]
    //     {
    //       new ("id", Utility.FormatIndexToStringXXX(_idTask)),
    //     });
    // }
  }

  void AnimClickTaskComplete(GameObject _BtnClick, GameObject _objEffect, Image _bgReceived, GameObject _Tick) // di chuyển button click
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
          // float targetY = -(taskCount * _BtnClick.GetComponent<RectTransform>().rect.height - taskCount  * _spacing);

          // Di chuyển button xuống cuối cùng với một animation
          buttonRect.DOAnchorPosY(targetY, 0.5f) // Di chuyển trong 0.5 giây
                .SetEase(Ease.OutQuad).OnComplete(() =>
                {
                  // Có thể thực hiện thêm hành động sau khi di chuyển
                  // verticalLayoutGroup.enabled = true;
                });

          MoveTasksBelowButton(_BtnClick);
        });

  }
  void MoveTasksBelowButton(GameObject _BtnClick) // di chuyển các button còn lại
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
        float newPosY = _listYTaskTagerDefault[index - 1];
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
  void ShowAnimReward(int _intShow)
  {
    GameObject Target = _objContentParent.transform.GetChild(_intShow).gameObject;
    Target.transform.localScale = Vector3.zero;
    Target.SetActive(true);
    _objShowReward.SetActive(true);
    Target.transform.DOKill();
    Target.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear)
    .OnComplete(() =>
    {
      Target.GetComponent<Button>().onClick.RemoveAllListeners();
      Target.GetComponent<Button>().onClick.AddListener(() => BtnHideAnimReward(_intShow));
    });
  }
  void BtnHideAnimReward(int _intShow)
  {
    GameObject Target = _objContentParent.transform.GetChild(_intShow).gameObject;
    Target.transform.DOKill();
    Target.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.Linear)
    .OnComplete(() =>
    {
      _objShowReward.SetActive(false);
    });
  }


}