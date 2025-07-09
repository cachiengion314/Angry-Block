using UnityEngine;
using System.Collections.Generic;
using System;
using HoangNam;
using UnityEngine.SceneManagement;

public class DailyWeeklyManager : MonoBehaviour
{
  public static DailyWeeklyManager Instance { get; private set; }
  [SerializeField] public WeeklyData _WeeklyTaskData;
  public WeeklyTaskDataJsonBase weeklyTaskDataJsonBase;
  public static Action onSave;
  public int LevelUnLock;
  public bool needResetReward = false;
  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
  }

  void OnDestroy()
  {

  }

  public void CheckIsExistingData()
  {
    SaveSystem.ChecFileExist(
         "WeeklyTaskData", () =>
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

    Debug.Log("_version0.0--: " + _version);
    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DATA_DAILY_WEEKLY, "") == "")
    {
      ChangeScriptableObjectToJson();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DATA_DAILY_WEEKLY, _version);
      return;
    }

    if (PlayerPrefs.GetString(KeyString.KEY_VERSION_DATA_DAILY_WEEKLY, "") != _version)
    {
      ChangeScriptableObjectToJson();
      PlayerPrefs.SetString(KeyString.KEY_VERSION_DATA_DAILY_WEEKLY, _version);
    }

  }

  void OnEnable()
  {
    foreach (enumListeningDataDailyWeekly evt in Enum.GetValues(typeof(enumListeningDataDailyWeekly)))
    {
      if (evt == enumListeningDataDailyWeekly.None) continue; // Bỏ qua enum "None"
      EventActionManager.StartListening<enumListeningDataDailyWeekly, MissonDataDailyWeekly>(evt, UpdateProgressTask);
    }

  }

  void OnDisable()
  {
    foreach (enumListeningDataDailyWeekly evt in Enum.GetValues(typeof(enumListeningDataDailyWeekly)))
    {
      if (evt == enumListeningDataDailyWeekly.None) continue;
      EventActionManager.StopListening<enumListeningDataDailyWeekly, MissonDataDailyWeekly>(evt, UpdateProgressTask);
    }

  }

  public void CheckDataFirebase()
  {
    LoadDataJsonTask();
    var taskWeeklyDataFirebases = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.daily_weekly.WeeklyTasksBaseJson;
    var localTasks = weeklyTaskDataJsonBase.WeeklyTasksBaseJson;

    if (taskWeeklyDataFirebases == null || taskWeeklyDataFirebases.Length == 0)
    {
      return;
    }

    if (taskWeeklyDataFirebases.Length != localTasks.Count)
    {
      ChangeFirebaseToJsonDataWeekly();
      RessetIsPickWeekly();

      return;
    }


    // ✅ Tạo danh sách key của local (để so nhanh hơn)
    HashSet<string> localKeys = new HashSet<string>();
    foreach (var task in localTasks)
    {
      localKeys.Add(task.strKeyWeeklyBaseJson?.Trim().ToLower());
    }


    for (int i = 0; i < taskWeeklyDataFirebases.Length; i++)
    {
      var taskDataWeeklyFirebase = taskWeeklyDataFirebases[i];
      string keyFirebase = taskDataWeeklyFirebase.strKeyWeeklyBaseJson?.Trim().ToLower();


      var localTask = localTasks.Find(t => t.strKeyWeeklyBaseJson == taskDataWeeklyFirebase.strKeyWeeklyBaseJson);

      if (!localKeys.Contains(keyFirebase))
      {
        // ✅ Hoàn toàn không tồn tại key này trong local, thêm mới

        localTasks.Add(taskDataWeeklyFirebase);

        RessetIsPickWeekly();
      }
      else
      {

        // ✅ Task đã có, so sánh từng trường và cập nhật nếu khác
        bool isUpdated = false;

        if (localTask.strNameTaskBaseJson != taskDataWeeklyFirebase.strNameTaskBaseJson)
        {
          localTask.strNameTaskBaseJson = taskDataWeeklyFirebase.strNameTaskBaseJson;

          isUpdated = true;
        }

        if (localTask.targetValueBaseJson != taskDataWeeklyFirebase.targetValueBaseJson)
        {
          localTask.targetValueBaseJson = taskDataWeeklyFirebase.targetValueBaseJson;

          isUpdated = true;
        }

        if (localTask.isLoopTaskBaseJson != taskDataWeeklyFirebase.isLoopTaskBaseJson)
        {
          localTask.isLoopTaskBaseJson = taskDataWeeklyFirebase.isLoopTaskBaseJson;
          RessetIsPickWeekly();
          isUpdated = true;
        }

        if (localTask.enumTypeTaskBaseJson != taskDataWeeklyFirebase.enumTypeTaskBaseJson)
        {
          localTask.enumTypeTaskBaseJson = taskDataWeeklyFirebase.enumTypeTaskBaseJson;
          isUpdated = true;
        }

        if (localTask.enumTypeChildWeeklyListeningBaseJson != taskDataWeeklyFirebase.enumTypeChildWeeklyListeningBaseJson)
        {

          localTask.enumTypeChildWeeklyListeningBaseJson = taskDataWeeklyFirebase.enumTypeChildWeeklyListeningBaseJson;

          isUpdated = true;
        }
        if (localTask.isSuccessBaseJson == true)
        {
          localTask.currentValueBaseJson = localTask.targetValueBaseJson;
        }
        if (isUpdated)
        {

        }
      }
    }
    // Cập nhật lại vào local nếu cần
    weeklyTaskDataJsonBase.WeeklyTasksBaseJson = localTasks;
    SaveDataJsonTask();

  }


  void RessetIsPickWeekly()
  {
    needResetReward = true;

    foreach (var task in weeklyTaskDataJsonBase.WeeklyTasksBaseJson)
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

    Dictionary<string, List<WeeklyTaskDataJsonBase.WeeklyTask>> tasksByType = new Dictionary<string, List<WeeklyTaskDataJsonBase.WeeklyTask>>();

    foreach (var taskIsPick in weeklyTaskDataJsonBase.WeeklyTasksBaseJson)
    {
      if (taskIsPick.enumTypeTaskBaseJson != "None")
      {
        if (!tasksByType.ContainsKey(taskIsPick.enumTypeTaskBaseJson)) // `task._idTask` có thể được thay bằng thuộc tính `Type` nếu có
        {
          tasksByType[taskIsPick.enumTypeTaskBaseJson] = new List<WeeklyTaskDataJsonBase.WeeklyTask>();
        }
        tasksByType[taskIsPick.enumTypeTaskBaseJson].Add(taskIsPick);
      }
    }


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
    SaveDataJsonTask();
  }


  public void ChangeScriptableObjectToJson()
  {
    // Tạo mới _taskDataJsonBase
    weeklyTaskDataJsonBase = new WeeklyTaskDataJsonBase();

    // Chuyển dữ liệu từ _taskData sang _taskDataJsonBase
    foreach (var task in _WeeklyTaskData.AllTasks)
    {
      weeklyTaskDataJsonBase.WeeklyTasksBaseJson.Add(new WeeklyTaskDataJsonBase.WeeklyTask
      {
        strNameTaskBaseJson = task.strNameTask,
        currentValueBaseJson = task.currentValue,
        targetValueBaseJson = task.targetValue,
        isShowNotifBaseJson = task.isShowNotif,
        isSuccessBaseJson = task.isSuccess,
        isPickBaseJson = task.isPick,
        isLoopTaskBaseJson = task.isLoopTask,
        enumTypeTaskBaseJson = task.enumTypeTask.ToString(),
        enumTypeChildWeeklyListeningBaseJson = task.enumTypeChildWeeklyListening.ToString(),
        strKeyWeeklyBaseJson = task.keyTaskWeekly

      });
    }
    // Lưu dữ liệu vào JSON
    SaveDataJsonTask();
  }

  void ChangeFirebaseToJsonDataWeekly()
  {
    weeklyTaskDataJsonBase = new WeeklyTaskDataJsonBase();
    var taskTodaydataFirebases = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.daily_weekly.WeeklyTasksBaseJson;
    if (taskTodaydataFirebases == null || taskTodaydataFirebases.Length == 0)
    {
      ChangeScriptableObjectToJson();
      return;
    }
    for (int i = 0; i < taskTodaydataFirebases.Length; i++)
    {
      weeklyTaskDataJsonBase.WeeklyTasksBaseJson.Add(new WeeklyTaskDataJsonBase.WeeklyTask
      {
        strNameTaskBaseJson = taskTodaydataFirebases[i].strNameTaskBaseJson,
        currentValueBaseJson = taskTodaydataFirebases[i].currentValueBaseJson,
        targetValueBaseJson = taskTodaydataFirebases[i].targetValueBaseJson,
        isShowNotifBaseJson = taskTodaydataFirebases[i].isShowNotifBaseJson,
        isSuccessBaseJson = taskTodaydataFirebases[i].isSuccessBaseJson,
        isPickBaseJson = taskTodaydataFirebases[i].isPickBaseJson,
        isLoopTaskBaseJson = taskTodaydataFirebases[i].isLoopTaskBaseJson,
        enumTypeTaskBaseJson = taskTodaydataFirebases[i].enumTypeTaskBaseJson.ToString(),
        enumTypeChildWeeklyListeningBaseJson = taskTodaydataFirebases[i].enumTypeChildWeeklyListeningBaseJson.ToString(),
        strKeyWeeklyBaseJson = taskTodaydataFirebases[i].strKeyWeeklyBaseJson

      });

    }
    // Lưu dữ liệu vào JSON
    SaveDataJsonTask();

  }

  public void SaveDataJsonTask()
  {
    SaveSystem.SaveAndEncrypt(weeklyTaskDataJsonBase, "WeeklyTaskData", "1");
    onSave?.Invoke();
  }

  public void LoadDataJsonTask()
  {
    if (FirebaseSetup.Instance.weeklyTaskDataJsonBase == null)
      weeklyTaskDataJsonBase = SaveSystem.LoadAndDecrypt<WeeklyTaskDataJsonBase>("WeeklyTaskData", "1");
  }

  void UpdateProgressTask(MissonDataDailyWeekly missonData)
  {
    if (GameManager.Instance.CurrentLevel < LevelUnLock) return;

    foreach (var task in weeklyTaskDataJsonBase.WeeklyTasksBaseJson)
    {
      if (task != null)
      {
        if (task.isPickBaseJson == true)
        {
          if (task.enumTypeChildWeeklyListeningBaseJson == missonData.KeyType)
          {
            task.currentValueBaseJson += missonData.UpProgess;

            if (task.currentValueBaseJson >= task.targetValueBaseJson)
            {
              task.currentValueBaseJson = task.targetValueBaseJson;
              if (task.isShowNotifBaseJson == false)
              {
                if (SceneManager.GetActiveScene().name == "Gameplay")
                {
                  int iconTaskId = 0;
                  if (Enum.TryParse(typeof(enumListeningDataDailyWeekly), task.enumTypeChildWeeklyListeningBaseJson, out object result))
                  {
                    iconTaskId = (int)result - 1; //(vì có none đứng đầu)
                  }
                  task.isShowNotifBaseJson = true;
                }

              }
            }
          }
        }
      }
    }

    SaveDataJsonTask();

  }

  public void LoadDataScriptableObjectToJson()
  {
    // Tạo mới _taskDataJsonBase
    weeklyTaskDataJsonBase = new WeeklyTaskDataJsonBase();

    // Chuyển dữ liệu từ _taskData sang _taskDataJsonBase
    foreach (var task in _WeeklyTaskData.AllTasks)
    {
      weeklyTaskDataJsonBase.WeeklyTasksBaseJson.Add(new WeeklyTaskDataJsonBase.WeeklyTask
      {
        strNameTaskBaseJson = task.strNameTask,
        currentValueBaseJson = task.currentValue,
        targetValueBaseJson = task.targetValue,
        isShowNotifBaseJson = task.isShowNotif,
        isSuccessBaseJson = task.isSuccess,
        isPickBaseJson = task.isPick,
        enumTypeTaskBaseJson = task.enumTypeTask.ToString(),
        isLoopTaskBaseJson = task.isLoopTask,
        enumTypeChildWeeklyListeningBaseJson = task.enumTypeChildWeeklyListening.ToString(),
        strKeyWeeklyBaseJson = task.keyTaskWeekly

      });
    }
    // Lưu dữ liệu vào JSON
    // 
  }

  [System.Serializable]
  public class WeeklyTaskDataJsonBase
  {
    public List<WeeklyTask> WeeklyTasksBaseJson = new();

    [Serializable]
    public class WeeklyTask
    {
      public string strKeyWeeklyBaseJson; // Tên KeyTask
      public string strNameTaskBaseJson; // Tên task
      public int currentValueBaseJson;   // Giá trị hiện tại
      public int targetValueBaseJson;    // Giá trị mục tiêu
      public bool isShowNotifBaseJson; // Đã hiện notif chưa
      public bool isSuccessBaseJson;     // Trạng thái đã nhận
      public bool isPickBaseJson;        // Trạng thái chọn
      public bool isLoopTaskBaseJson;
      public string enumTypeTaskBaseJson;   // ID nhóm nhiệm vụ
      public string enumTypeChildWeeklyListeningBaseJson;
    }
  }

}
