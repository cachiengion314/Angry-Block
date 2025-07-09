using UnityEngine;
using System.Collections.Generic;
using System;
using HoangNam;
using UnityEngine.SceneManagement;


public class DailyTaskManager : MonoBehaviour
{
  public static DailyTaskManager Instance { get; private set; }
  [SerializeField] public TaskData _taskData;
  public TaskDataJsonBase _taskDataJsonBase;
  public List<TaskDataJsonBase.TaskJson> allPickTasksTest;

  public bool needResetReward = false;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      allPickTasksTest = new List<TaskDataJsonBase.TaskJson>();
    }
  }

  void OnDestroy()
  {

  }

  public void CheckIsExistingData()
  {
    SaveSystem.ChecFileExist(
         "TaskData", () =>
         {
           ChangeScriptableObjectToJson();
         }, () =>
         {
           LoadDataJsonTask();
         }
       );
    CheckVersoinApplicationDataTask();
  }

  public void CheckVersoinApplicationDataTask()
  {
    string _version = Application.version;

    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DATA_DAILY_TASK, "") == "")
    {
      ChangeScriptableObjectToJson();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DATA_DAILY_TASK, _version);
      return;
    }

    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DATA_DAILY_TASK, "") != _version)
    {
      ChangeScriptableObjectToJson();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DATA_DAILY_TASK, _version);
    }

  }
  void RessetIsPick()
  {
    needResetReward = true;

    foreach (var task in _taskDataJsonBase.TasksTodayJson)
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

    Dictionary<string, List<TaskDataJsonBase.TaskJson>> tasksByType = new();

    // Debug.Log("---0.11");
    foreach (var task in _taskDataJsonBase.TasksTodayJson)
    {
      // Debug.Log("---0.1");
      if (task.enumTypeTaskJson != "None")
      {
        // Debug.Log("---1");
        if (!tasksByType.ContainsKey(task.enumTypeTaskJson)) // `task._idTask` có thể được thay bằng thuộc tính `Type` nếu có
        {
          // Debug.Log("---2");
          tasksByType[task.enumTypeTaskJson] = new List<TaskDataJsonBase.TaskJson>();
        }
        // Debug.Log("---3");
        tasksByType[task.enumTypeTaskJson].Add(task);
      }
    }
    Debug.Log("----" + tasksByType.Count);
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
    SaveDataJsonTask();
  }


  public void CheckDataFirebase()
  {
    LoadDataJsonTask();
    var taskTodaydataFirebases = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.daily_task.TasksTodayJson;
    var localTasks = _taskDataJsonBase.TasksTodayJson;

    if (taskTodaydataFirebases == null || taskTodaydataFirebases.Length == 0)
    {
      return;
    }

    if (taskTodaydataFirebases.Length != localTasks.Count)
    {
      ChangeFirebaseToJsonDataToday();
      RessetIsPick();

      return;
    }


    // ✅ Tạo danh sách key của local (để so nhanh hơn)
    HashSet<string> localKeys = new HashSet<string>();
    foreach (var task in localTasks)
    {
      localKeys.Add(task.strKeyTaskJson?.Trim().ToLower());
    }


    for (int i = 0; i < taskTodaydataFirebases.Length; i++)
    {
      var taskDataFirebase = taskTodaydataFirebases[i];
      string keyFirebase = taskDataFirebase.strKeyTaskJson?.Trim().ToLower();


      var localTask = localTasks.Find(t => t.strKeyTaskJson == taskDataFirebase.strKeyTaskJson);

      if (!localKeys.Contains(keyFirebase))
      {
        // ✅ Hoàn toàn không tồn tại key này trong local, thêm mới

        localTasks.Add(taskDataFirebase);
        RessetIsPick();
      }
      else
      {

        // ✅ Task đã có, so sánh từng trường và cập nhật nếu khác
        bool isUpdated = false;

        if (localTask.strNameTaskJson != taskDataFirebase.strNameTaskJson)
        {
          localTask.strNameTaskJson = taskDataFirebase.strNameTaskJson;

          isUpdated = true;
        }

        if (localTask.targetValueJson != taskDataFirebase.targetValueJson)
        {
          localTask.targetValueJson = taskDataFirebase.targetValueJson;

          isUpdated = true;
        }

        if (localTask._isLoopTask != taskDataFirebase._isLoopTask)
        {
          localTask._isLoopTask = taskDataFirebase._isLoopTask;
          RessetIsPick();
          isUpdated = true;
        }

        if (localTask.enumTypeTaskJson != taskDataFirebase.enumTypeTaskJson)
        {
          localTask.enumTypeTaskJson = taskDataFirebase.enumTypeTaskJson;
          isUpdated = true;
        }

        if (localTask.enumTypeChildListeningJson != taskDataFirebase.enumTypeChildListeningJson)
        {

          localTask.enumTypeChildListeningJson = taskDataFirebase.enumTypeChildListeningJson;

          isUpdated = true;
        }
        if (localTask._isSuccess == true)
        {
          localTask.currentValueJson = localTask.targetValueJson;
        }
        if (isUpdated)
        {

        }
      }
    }
    // Cập nhật lại vào local nếu cần
    _taskDataJsonBase.TasksTodayJson = localTasks;
    SaveDataJsonTask();


  }
  void OnEnable()
  {
    foreach (enumListeningDataDailyTask evt in Enum.GetValues(typeof(enumListeningDataDailyTask)))
    {
      if (evt == enumListeningDataDailyTask.None) continue; // Bỏ qua enum "None"
      EventActionManager.StartListening<enumListeningDataDailyTask, MissonDataDailyTask>(evt, UpdateProgressTask);
    }

  }

  void OnDisable()
  {
    foreach (enumListeningDataDailyTask evt in Enum.GetValues(typeof(enumListeningDataDailyTask)))
    {
      if (evt == enumListeningDataDailyTask.None) continue;
      EventActionManager.StopListening<enumListeningDataDailyTask, MissonDataDailyTask>(evt, UpdateProgressTask);
    }

  }

  public void LoadDataScriptableObjectToJson()
  {
    // Tạo mới _taskDataJsonBase
    _taskDataJsonBase = new TaskDataJsonBase();

    // Chuyển dữ liệu từ _taskData sang _taskDataJsonBase
    foreach (var task in _taskData.AllTasks)
    {
      _taskDataJsonBase.TasksTodayJson.Add(new TaskDataJsonBase.TaskJson
      {

        strNameTaskJson = task.strNameTask,
        currentValueJson = task.currentValue,
        targetValueJson = task.targetValue,
        _isSuccess = task._isSuccess,
        _isPick = task._isPick,
        _isLoopTask = task._isLoopTask,
        _isShowNotif = task._isShowNotif,
        enumTypeTaskJson = task.enumTypeTask.ToString(),
        enumTypeChildListeningJson = task.enumTypeChildListening.ToString(),
        strKeyTaskJson = task.keyTaskToday

      });
    }
    // Lưu dữ liệu vào JSON
    // 
  }
  void ChangeJsonToTaskDataJsonBase()
  {

  }

  public void ChangeScriptableObjectToJson()
  {
    // Tạo mới _taskDataJsonBase
    _taskDataJsonBase = new TaskDataJsonBase();

    // Chuyển dữ liệu từ _taskData sang _taskDataJsonBase
    foreach (var task in _taskData.AllTasks)
    {
      _taskDataJsonBase.TasksTodayJson.Add(new TaskDataJsonBase.TaskJson
      {

        strNameTaskJson = task.strNameTask,
        currentValueJson = task.currentValue,
        targetValueJson = task.targetValue,
        _isSuccess = task._isSuccess,
        _isPick = task._isPick,
        _isLoopTask = task._isLoopTask,
        _isShowNotif = task._isShowNotif,
        enumTypeTaskJson = task.enumTypeTask.ToString(),
        enumTypeChildListeningJson = task.enumTypeChildListening.ToString(),
        // strKeyTaskJson = task.enumTypeChildListening.ToString() + "_" + task.targetValue
        strKeyTaskJson = task.keyTaskToday

      });
    }
    // Lưu dữ liệu vào JSON
    SaveDataJsonTask();
  }

  public void ChangeFirebaseToJsonDataToday()
  {
    // Tạo mới _taskDataJsonBase

    _taskDataJsonBase = new TaskDataJsonBase();
    var taskTodaydataFirebases = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.daily_task.TasksTodayJson;
    if (taskTodaydataFirebases == null || taskTodaydataFirebases.Length == 0)
    {
      ChangeScriptableObjectToJson();
      return;
    }
    for (int i = 0; i < taskTodaydataFirebases.Length; i++)
    {
      _taskDataJsonBase.TasksTodayJson.Add(new TaskDataJsonBase.TaskJson
      {
        strNameTaskJson = taskTodaydataFirebases[i].strNameTaskJson,
        currentValueJson = taskTodaydataFirebases[i].currentValueJson,
        targetValueJson = taskTodaydataFirebases[i].targetValueJson,
        _isSuccess = taskTodaydataFirebases[i]._isSuccess,
        _isPick = taskTodaydataFirebases[i]._isPick,
        _isLoopTask = taskTodaydataFirebases[i]._isLoopTask,
        _isShowNotif = taskTodaydataFirebases[i]._isShowNotif,
        enumTypeTaskJson = taskTodaydataFirebases[i].enumTypeTaskJson.ToString(),
        enumTypeChildListeningJson = taskTodaydataFirebases[i].enumTypeChildListeningJson.ToString(),
        strKeyTaskJson = taskTodaydataFirebases[i].strKeyTaskJson

      });

    }
    // Lưu dữ liệu vào JSON
    SaveDataJsonTask();
  }

  public void SaveDataJsonTask()
  {
    SaveSystem.SaveAndEncrypt(_taskDataJsonBase, "TaskData", "1");
  }

  public void LoadDataJsonTask()
  {
    if (FirebaseSetup.Instance.TaskDataJsonBase == null)
      _taskDataJsonBase = SaveSystem.LoadAndDecrypt<TaskDataJsonBase>("TaskData", "1");
  }

  void UpdateProgressTask(MissonDataDailyTask missonData)
  {
    foreach (var task in _taskDataJsonBase.TasksTodayJson)
    {
      if (task != null)
      {
        if (task._isPick == true)
        {
          if (task.enumTypeChildListeningJson == missonData.KeyType)
          {
            task.currentValueJson += missonData.UpProgess;

            if (task.currentValueJson >= task.targetValueJson)
            {
              task.currentValueJson = task.targetValueJson;
              if (task._isShowNotif == false)
              {
                if (SceneManager.GetActiveScene().name == "Gameplay")
                {
                  int iconTaskId = 0;
                  if (Enum.TryParse(typeof(enumListeningDataDailyTask), task.enumTypeChildListeningJson, out object result))
                  {
                    iconTaskId = (int)result - 1; //(vì có none đứng đầu)
                  }
            
                  task._isShowNotif = true;
             
                }

              }
            }
          }
        }
      }
    }

    SaveDataJsonTask();
  }

  public TaskDataJsonBase.TaskJson GetTask(string keyString)
  {
    if (_taskDataJsonBase == null || _taskDataJsonBase.TasksTodayJson == null)
    {
      Debug.LogWarning("TaskDataJsonBase chưa được khởi tạo.");
      return null;
    }

    return _taskDataJsonBase.TasksTodayJson.Find(t => t.strKeyTaskJson == keyString);
  }
  [System.Serializable]
  public class TaskDataJsonBase
  {
    public List<TaskJson> TasksTodayJson = new List<TaskJson>();

    [Serializable]
    public class TaskJson
    {   // ID của task
      public string strKeyTaskJson; // Tên KeyTask
      public string strNameTaskJson; // Tên task
      public int currentValueJson;   // Giá trị hiện tại
      public int targetValueJson;    // Giá trị mục tiêu
      public bool _isSuccess;     // Trạng thái đã nhận
      public bool _isPick;        // Trạng thái chọn
      public bool _isShowNotif;   // ID nhóm nhiệm vụ
      public bool _isLoopTask;
      public string enumTypeTaskJson;   // ID nhóm nhiệm vụ
      public string enumTypeChildListeningJson;   // ID nhóm nhiệm vụ
    }
  }

}
