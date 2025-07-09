using UnityEngine;
using System;
using System.Collections.Generic;

public class TimeEventManager : MonoBehaviour
{
  public static TimeEventManager Instance { get; private set; }
  [SerializeField] TimeEventData _timeData;
  public Action<EventData> onTest;
  public List<EventData> ArrayEvent;
  // private DateTime currentTime; // Thời gian hiện tại
  public Dictionary<int, EventData> eventList = new Dictionary<int, EventData>(); // Danh sách sự kiện
  private HashSet<string> triggeredEvents = new HashSet<string>(); // Danh sách sự kiện đã kích hoạt

  public Dictionary<int, List<int>> listIntTurnWeekly; // Danh sách sự kiện Tuần
  public Dictionary<int, List<int>> listIntTurnMonthly; // Danh sách sự kiện Tháng
  public Dictionary<int, List<int>> listIntTurnYearly; // Danh sách sự kiện Năm
  public static Action onNextWeek;
  public static Action onNextMonth;
  public static Action onNextYear;



  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    ArrayEvent = new();
    listIntTurnWeekly = new();
    listIntTurnMonthly = new();
    listIntTurnYearly = new();
    SetUpIndexDaily();
    // CheckNextWeekly();
    // CheckNextMonthly();
    // CheckNextYear();
  }
  void Start()
  {

    InitializeEventsFromData();// Khởi tạo danh sách sự kiện

    CheckEventStatus(); // Kiểm tra sự kiện ngay khi game bắt đầu


  }
  void SetUpIndexDaily()
  {
    // Xóa các danh sách cũ để đảm bảo dữ liệu sạch
    listIntTurnWeekly.Clear();
    listIntTurnMonthly.Clear();
    listIntTurnYearly.Clear();

    foreach (var eventData in _timeData.AllEvents)
    {
      // Xử lý sự kiện tuần
      if (eventData._isTypeEvent == TimeEventData.TypeEvent.TypeEventWeekly)
      {
        int eventTypeKeyWeek = (int)eventData._isTypeWeeklyEvent; // Chuyển enum thành số
        if (!listIntTurnWeekly.ContainsKey(eventTypeKeyWeek))
        {
          listIntTurnWeekly[eventTypeKeyWeek] = new List<int>();
        }
        listIntTurnWeekly[eventTypeKeyWeek].Add(eventData._idIntTypeEvents);
        // Debug.Log($"Weekly Event Type {eventTypeKeyWeek} Total: {listIntTurnWeekly[eventTypeKeyWeek].Count}");
      }

      // Xử lý sự kiện tháng
      if (eventData._isTypeEvent == TimeEventData.TypeEvent.TypeEventMonth)
      {
        int eventTypeKeyMonth = (int)eventData._isTypeMonthEvent; // Chuyển enum thành số
        if (!listIntTurnMonthly.ContainsKey(eventTypeKeyMonth))
        {
          listIntTurnMonthly[eventTypeKeyMonth] = new List<int>();
        }
        listIntTurnMonthly[eventTypeKeyMonth].Add(eventData._idIntTypeEvents);
        // Debug.Log($"Monthly Event Type {eventTypeKeyMonth} Total: {listIntTurnMonthly[eventTypeKeyMonth].Count}");
      }

      // Xử lý sự kiện năm
      if (eventData._isTypeEvent == TimeEventData.TypeEvent.TypeEventYear)
      {
        int eventTypeKeyYear = (int)eventData._isTypeYearEvent; // Chuyển enum thành số
        if (!listIntTurnYearly.ContainsKey(eventTypeKeyYear))
        {
          listIntTurnYearly[eventTypeKeyYear] = new List<int>();
        }
        listIntTurnYearly[eventTypeKeyYear].Add(eventData._idIntTypeEvents);
        // Debug.Log($"Yearly Event Type {eventTypeKeyYear} Total: {listIntTurnYearly[eventTypeKeyYear].Count}");
      }
    }
  }

  public void CheckNextWeekly()
  {
    int IntToday = ConvertTimeTodayToInt(); // lấy ngày hiện tại

    int nextReset = PlayerPrefs.GetInt("NextUpdateWeekly", 0); //lấy ngày tiếp theo
    if (nextReset == 0)
    {
      nextReset = GetNextResetTime(); // nếu chưa khởi tạo dữ liệu ngày tiếp thì sẽ lấy lại khởi tạo lại 
      SaveNextUpdateWeekly(nextReset);
    }
    int lastReset = PlayerPrefs.GetInt("LastUpdateWeekly", 0);

    if (lastReset == 0) // nếu là ngày đầu đăng nhập thì sẽ reset dữ liệu
    {
      onNextWeek?.Invoke();
      Debug.Log("onNextWeek---1");
      AddNewIndexWeekly();
      SaveLastUpdateWeekly(IntToday);
      return;
    }
    if (IntToday >= nextReset) // hôm nay mà >= ngày reset đã lưu thì load ngày mới + lưu lại ngày mới
    {
      // Đã đến lúc reset nhiệm vụ tuần
      onNextWeek?.Invoke();
      Debug.Log("onNextWeek---2");
      SaveLastUpdateWeekly(nextReset);
      AddNewIndexWeekly();

      int NextTurnRest = GetNextResetTime();
      SaveNextUpdateWeekly(NextTurnRest);
    }
  }


  public void CheckNextMonthly()
  {
    int IntToday = ConvertTimeTodayToInt(); // Lấy ngày hiện tại
    int nextMonthlyReset = PlayerPrefs.GetInt("NextUpdateMonthly", 0); // Lấy ngày reset tháng tiếp theo

    if (nextMonthlyReset == 0)
    {
      nextMonthlyReset = GetNextMonthlyResetTime(); // Nếu chưa khởi tạo thì tính lại ngày reset tháng
      SaveNextUpdateMonthly(nextMonthlyReset);
    }

    int lastMonthlyReset = PlayerPrefs.GetInt("LastUpdateMonthly", 0);

    if (lastMonthlyReset == 0) // Nếu là lần đầu đăng nhập, reset dữ liệu
    {
      SaveLastUpdateMonthly(IntToday);
      AddNewIndexMonthly();
      onNextMonth?.Invoke();
      return;
    }

    if (IntToday >= nextMonthlyReset) // Nếu hôm nay đã đến ngày reset tháng
    {
      SaveLastUpdateMonthly(nextMonthlyReset);
      int nextTurnReset = GetNextMonthlyResetTime();
      SaveNextUpdateMonthly(nextTurnReset);
      AddNewIndexMonthly();
      onNextMonth?.Invoke();
    }
  }
  public void CheckNextYear()
  {
    // Debug.Log("nextYearlyReset---1");
    int IntToday = ConvertTimeTodayToInt(); // Lấy ngày hiện tại
    int nextYearlyReset = PlayerPrefs.GetInt("NextUpdateYearly", 0); // Lấy ngày reset năm tiếp theo

    if (nextYearlyReset == 0)
    {
      nextYearlyReset = GetNextYearlyResetTime(); // Nếu chưa có, khởi tạo ngày reset năm mới
      SaveNextUpdateYearly(nextYearlyReset);
    }

    int lastYearlyReset = PlayerPrefs.GetInt("LastUpdateYearly", 0);

    if (lastYearlyReset == 0) // Nếu lần đầu đăng nhập, reset dữ liệu
    {
      AddNewIndexYearly(); // Xoay tua sự kiện năm đầu tiên
      SaveLastUpdateYearly(IntToday);
      // Debug.Log("nextYearlyReset---2");
      onNextYear?.Invoke();

      return;
    }

    if (IntToday >= nextYearlyReset) // Nếu hôm nay đã đến ngày reset năm
    {
      SaveLastUpdateYearly(nextYearlyReset);
      AddNewIndexYearly(); // Xoay tua sự kiện năm

      int nextTurnReset = GetNextYearlyResetTime();
      SaveNextUpdateYearly(nextTurnReset);
      Debug.Log("nextYearlyReset---3");
      onNextYear?.Invoke();
    }
  }


  void AddNewIndexWeekly()
  {
    foreach (var weeklyType in listIntTurnWeekly.Keys)
    {
      List<int> eventIds = listIntTurnWeekly[weeklyType]; // Lấy danh sách sự kiện theo loại tuần
      int totalWeeklyEvents = eventIds.Count;

      if (totalWeeklyEvents == 0)
      {
        // Debug.LogWarning($"Không có sự kiện tuần nào cho loại {weeklyType}");
        continue;
      }

      // Lấy index hiện tại từ PlayerPrefs, mặc định -1 nếu chưa có
      int currentIndex = PlayerPrefs.GetInt($"IndexWeekly_{weeklyType}", -1);

      // Xoay tua: Tăng index, nếu vượt quá danh sách thì quay lại 0
      int newIndex = (currentIndex + 1) % totalWeeklyEvents;

      // Cập nhật lại danh sách
      PlayerPrefs.SetInt($"IndexWeekly_{weeklyType}", newIndex);
      // Debug.Log($"Cập nhật xoay tua {weeklyType}: Index mới = {newIndex}");
    }
  }


  void AddNewIndexMonthly()
  {
    foreach (var monthlyType in listIntTurnMonthly.Keys)
    {
      List<int> eventIds = listIntTurnMonthly[monthlyType];
      int totalMonthlyEvents = eventIds.Count;

      if (totalMonthlyEvents == 0)
      {
        // Debug.LogWarning($"Không có sự kiện tháng nào cho loại {monthlyType}");
        continue;
      }

      int currentIndex = PlayerPrefs.GetInt($"IndexMonthly_{monthlyType}", -1);
      int newIndex = (currentIndex + 1) % totalMonthlyEvents;

      PlayerPrefs.SetInt($"IndexMonthly_{monthlyType}", newIndex);
      // Debug.Log($"Cập nhật xoay tua tháng {monthlyType}: Index mới = {newIndex}");
    }

  }
  void AddNewIndexYearly()
  {
    foreach (var yearlyType in listIntTurnYearly.Keys)
    {
      List<int> eventIds = listIntTurnYearly[yearlyType];
      int totalYearlyEvents = eventIds.Count;

      if (totalYearlyEvents == 0)
      {
        // Debug.LogWarning($"Không có sự kiện tháng nào cho loại {yearlyType}");
        continue;
      }

      int currentIndex = PlayerPrefs.GetInt($"IndexYearly_{yearlyType}", -1);
      int newIndex = (currentIndex + 1) % totalYearlyEvents;

      PlayerPrefs.SetInt($"IndexYearly_{yearlyType}", newIndex);
      // Debug.Log($"Cập nhật xoay tua năm {yearlyType}: Index mới = {newIndex}");
    }

  }

  void InitializeEventsFromData()
  {
    int currentYear = DateTime.Now.Year; // Lấy năm hiện tại
                                         // int currentYear = DateTime.UtcNow.Year; // Lấy năm hiện tại theo UTC
    eventList.Clear(); // Xóa danh sách sự kiện cũ trước khi khởi tạo lại

    foreach (var eventData in _timeData.AllEvents)
    {
      switch (eventData._isTypeEvent)
      {
        case TimeEventData.TypeEvent.TypeEventWeekly:

          if (listIntTurnWeekly.ContainsKey((int)eventData._isTypeWeeklyEvent)) // Kiểm tra xem loại sự kiện tuần này có tồn tại trong danh sách `listIntTurnWeekly` hay không
          {
            // Lấy danh sách ID các sự kiện tuần thuộc loại sự kiện tuần cụ thể (_isTypeWeeklyEvent)
            List<int> weeklyEventIds = listIntTurnWeekly[(int)eventData._isTypeWeeklyEvent];

            // Lấy index hiện tại của loại sự kiện tuần từ PlayerPrefs
            int currentWeeklyIndex = PlayerPrefs.GetInt($"IndexWeekly_{(int)eventData._isTypeWeeklyEvent}", 0);

            // currentWeeklyIndex = Mathf.Clamp(currentWeeklyIndex, 0, weeklyEventIds.Count - 1);

            if (currentWeeklyIndex >= weeklyEventIds.Count)
            {
              // Nếu vượt quá, reset lại currentYearlyIndex về 0
              currentWeeklyIndex = 0;
              PlayerPrefs.SetInt($"IndexYearly_{(int)eventData._isTypeYearEvent}", currentWeeklyIndex);  // Lưu lại giá trị đã reset vào PlayerPrefs
              Debug.Log("Reset currentMonthlyIndex về 0 vì vượt quá phạm vi.");
            }

            // Kiểm tra xem index hiện tại (currentWeeklyIndex) có nằm trong giới hạn danh sách `weeklyEventIds` hay không
            // Điều này để tránh lỗi khi index vượt quá phạm vi hoặc không hợp lệ
            // Nếu index hợp lệ, kiểm tra xem ID sự kiện (_idIntTypeEvents) có khớp với giá trị tại vị trí index trong `weeklyEventIds`
            if (eventData._idIntTypeEvents == weeklyEventIds[currentWeeklyIndex])
            {
              // Nếu điều kiện trên đúng, thêm sự kiện vào `eventList`
              eventList.Add(eventData._idEvent,
              new EventData(
                  eventData._idEvent,                 // ID sự kiện
                  eventData._strName,                 // Tên sự kiện
                  GetDayWeekly(eventData.weekDays),   // Ngày bắt đầu sự kiện (dựa vào thứ trong tuần)
                  eventData.FullDayEvent,             // Thời gian diễn ra sự kiện (số ngày)
                  eventData.weekDays                  // Thứ trong tuần mà sự kiện diễn ra
              ));
            }
          }
          break;

        case TimeEventData.TypeEvent.TypeEventMonth:

          DateTime firstEventMonth = GetDayMonth(eventData.StartDay, eventData.FullDayEvent);
          if (listIntTurnMonthly.ContainsKey((int)eventData._isTypeMonthEvent))
          {
            List<int> monthEventIds = listIntTurnMonthly[(int)eventData._isTypeMonthEvent];
            int currentMonthlyIndex = PlayerPrefs.GetInt($"IndexMonthly_{(int)eventData._isTypeMonthEvent}", 0);

            // currentMonthlyIndex = Mathf.Clamp(currentMonthlyIndex, 0, monthEventIds.Count - 1);
            if (currentMonthlyIndex >= monthEventIds.Count)
            {
              // Nếu vượt quá, reset lại currentYearlyIndex về 0
              currentMonthlyIndex = 0;
              PlayerPrefs.SetInt($"IndexYearly_{(int)eventData._isTypeYearEvent}", currentMonthlyIndex);  // Lưu lại giá trị đã reset vào PlayerPrefs
              Debug.Log("Reset currentMonthlyIndex về 0 vì vượt quá phạm vi.");
            }

            if (eventData._idIntTypeEvents == monthEventIds[currentMonthlyIndex])
            {
              // Debug.Log("Đã chuyền dữ liệu");

              eventList.Add(eventData._idEvent,
              new EventData(
                  eventData._idEvent,
                  eventData._strName,
                  firstEventMonth,
                  eventData.FullDayEvent,
                  null, // Không cần tuần cho sự kiện tháng
                  eventData.StartDay,
                  null
              ));
            }
          }
          break;

        case TimeEventData.TypeEvent.TypeEventYear:
          if (listIntTurnYearly.ContainsKey((int)eventData._isTypeYearEvent))
          {

            List<int> yearEventIds = listIntTurnYearly[(int)eventData._isTypeYearEvent];
            // int currentYearlyIndex = PlayerPrefs.GetInt($"IndexYearly_{(int)eventData._isTypeYearEvent}", 0);

            int currentYearlyIndex = PlayerPrefs.GetInt($"IndexYearly_{(int)eventData._isTypeYearEvent}", 0);

            if (currentYearlyIndex >= yearEventIds.Count)
            {
              // Nếu vượt quá, reset lại currentYearlyIndex về 0
              currentYearlyIndex = 0;
              PlayerPrefs.SetInt($"IndexYearly_{(int)eventData._isTypeYearEvent}", currentYearlyIndex);  // Lưu lại giá trị đã reset vào PlayerPrefs
              Debug.Log("Reset currentYearlyIndex về 0 vì vượt quá phạm vi.");
            }
            // Đảm bảo currentYearlyIndex nằm trong phạm vi hợp lệ của danh sách
            // currentYearlyIndex = Mathf.Clamp(currentYearlyIndex, 0, yearEventIds.Count - 1);


            if (eventData._idIntTypeEvents == yearEventIds[currentYearlyIndex])
            {
              eventList.Add(eventData._idEvent,
              new EventData(
                  eventData._idEvent,
                  eventData._strName,
                  new DateTime(currentYear, eventData.MonthStart, eventData.StartDay),
                  eventData.FullDayEvent,
                  null, // sự kiện năm không cần tuần
                  eventData.StartDay,
                  eventData.MonthStart
              ));
            }
          }
          break;
      }
    }
  }

  /// <summary>
  /// Kiểm tra tất cả sự kiện nào đang diễn ra
  /// </summary>
  void CheckEventStatus()
  {
    // currentTime = DateTime.Now; // Lấy thời gian hiện tại

    foreach (var eventItem in eventList)
    {
      EventData eventData = eventItem.Value;

      if (IsEventActive(eventData))
      {
        if (!triggeredEvents.Contains(eventData.EventName)) // Kiểm tra nếu chưa kích hoạt
        {
          TriggerEvent(eventData.EventName);
          triggeredEvents.Add(eventData.EventName);
          onTest?.Invoke(eventData);
          ArrayEvent.Add(eventData);
        }
      }
      else
      {
        triggeredEvents.Remove(eventData.EventName); // Reset trạng thái khi hết sự kiện
      }
    }
  }
  DateTime GetDayWeekly(DayOfWeek _targetDay)
  {
    DateTime currentTime = DateTime.Now; // Lấy thời gian hiện tại
                                         // DateTime currentTime = DateTime.UtcNow; // Lấy thời gian hiện tại

    // Lấy số thứ tự của ngày hiện tại (0: Sunday, 1: Monday, ..., 6: Saturday)
    int currentDayOfWeek = (int)currentTime.DayOfWeek;

    // Lấy số thứ tự của ngày mục tiêu
    int targetDayOfWeek = (int)_targetDay;

    // Tính khoảng cách giữa ngày hiện tại và ngày mục tiêu
    int difference = targetDayOfWeek - currentDayOfWeek;

    // Nếu difference > 0 -> targetDay là ngày sau currentDay -> Quay về targetDay của tuần trước
    if (difference > 0)
    {
      difference -= 7; // Lùi về tuần trước
    }

    // Tìm ngày trong tuần hiện tại
    DateTime resultDate = currentTime.AddDays(difference);

    return resultDate;
  }

  DateTime GetDayMonth(int _startDay, int fullDayEvent)
  {
    DateTime currentTime = DateTime.Now; // Lấy thời gian hiện tại
                                         // DateTime currentTime = DateTime.UtcNow; // Lấy thời gian hiện tại
    int currentMonth = currentTime.Month; // Lấy tháng hiện tại
    int currentYear = currentTime.Year; // Lấy năm hiện tại

    // Tính ngày bắt đầu
    DateTime resultDate = new DateTime(currentYear, currentMonth, _startDay);
    // Debug.Log("resultDate--" + resultDate.ToString(""));

    // Trả về ngày bắt đầu
    return resultDate;
  }

  /// <summary>
  /// Kiểm tra xem sự kiện có đang diễn ra hay không
  /// </summary>
  public bool IsEventActive(EventData eventData)
  {
    // Lấy thời gian hiện tại
    DateTime currentTime = DateTime.Now;
    //  DateTime currentTime = DateTime.UtcNow;

    // Kiểm tra nếu ngày hiện tại nằm ngoài phạm vi sự kiện
    if (currentTime < eventData.startDate || currentTime > eventData.endDate)
    {
      return false;
    }

    // Kiểm tra nếu sự kiện là sự kiện theo tuần
    if (eventData.weekDays.HasValue && eventData.startDate.DayOfWeek == eventData.weekDays)
    {
      // Kiểm tra nếu hôm nay là ngày đúng của sự kiện hoặc nằm trong phạm vi FullDayEvent
      if (currentTime >= eventData.startDate && currentTime < eventData.startDate.AddDays(eventData.FullDayEvent))
      {
        return true;
      }
    }

    // Kiểm tra nếu sự kiện là sự kiện theo tháng
    if (eventData.StartDay.HasValue && !eventData.StartMonth.HasValue)
    {
      // Kiểm tra nếu ngày hiện tại nằm trong phạm vi từ StartDay đến (StartDay + FullDayEvent - 1)
      if (currentTime.Day >= eventData.StartDay &&
          // currentTime.Day <= eventData.StartDay + eventData.FullDayEvent - 1)
          currentTime.Day <= eventData.StartDay + eventData.FullDayEvent)
      {
        return true;
      }

    }
    // Kiểm tra nếu sự kiện là sự kiện theo năm (một ngày cụ thể)
    if (eventData.StartDay.HasValue && eventData.StartMonth.HasValue)
    {
      // Tính ngày bắt đầu và ngày kết thúc của sự kiện dựa trên StartDay và FullDayEvent
      DateTime eventStartDate = new DateTime(currentTime.Year, eventData.StartMonth.Value, eventData.StartDay.Value);
      // DateTime eventEndDate = eventStartDate.AddDays(eventData.FullDayEvent - 1);
      DateTime eventEndDate = eventStartDate.AddDays(eventData.FullDayEvent);

      // Kiểm tra nếu ngày hiện tại nằm trong khoảng thời gian từ ngày bắt đầu đến ngày kết thúc
      if (currentTime >= eventStartDate && currentTime <= eventEndDate)
      {
        return true;
      }
    }

    // Nếu không khớp với bất kỳ loại sự kiện nào
    return false;
  }


  /// <summary>
  /// Hàm kích hoạt sự kiện
  /// </summary>
  public void TriggerEvent(string eventName)
  {
    // Debug.Log($"🎉 Sự kiện {eventName} đang diễn ra! 🎉");
    // TODO: Thêm code xử lý sự kiện tại đây (ví dụ: mở cửa hàng sự kiện, tặng quà, v.v.)
  }
  int ConvertTimeTodayToInt() // chuyển đổi time thành int (ngày hôm nay)
  {
    string STRToday = DateTime.Now.ToString("yyyyMMdd");
    // string STRToday = DateTime.UtcNow.ToString("yyyyMMdd");
    int IntToday = int.Parse(STRToday);
    return IntToday;
  }

  // check rest Tuần
  int GetNextResetTime() // khời tạo ngày tiếp theo reset dữ liệu (được gọi ở khi chưa khởi tạo dữ liệu và khi reset tuần)
  {
    DateTime now = DateTime.Now;
    // Tính số ngày còn lại đến Thứ Hai tiếp theo
    int daysUntilNextReset = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7; // tính xem bao nhiêu ngày nữa đến đầu tuần
    if (daysUntilNextReset == 0) // nếu nay là đầu tuần thì daysUntilNextReset = 7 để addDays (+ thêm 7 ngày nữa để tính t2 tuần sau)
    {
      daysUntilNextReset = 7;
    }
    // Đặt thời gian reset về đúng 00:00 (giữ nguyên ngày, reset giờ/phút/giây)
    DateTime nextResetDate = now.Date.AddDays(daysUntilNextReset); // tính ngày tuần sau là t2
    string nextResetString = nextResetDate.ToString("yyyyMMdd");
    int NextResetInt = int.Parse(nextResetString); // chuyển sang int để tính toán

    return NextResetInt;
  }

  void SaveLastUpdateWeekly(int _IntToday)//lưu lần cuối cùng updateWeekly
  {
    PlayerPrefs.SetInt("LastUpdateWeekly", _IntToday);
  }
  void SaveNextUpdateWeekly(int _IntNextUpdate) // lưu lần tiếp theo updateWeekly
  {
    PlayerPrefs.SetInt("NextUpdateWeekly", _IntNextUpdate);
  }
  // check rest Tháng
  int GetNextMonthlyResetTime()
  {
    DateTime now = DateTime.Now;
    // DateTime now = DateTime.UtcNow;
    DateTime nextMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1); // Lấy ngày đầu tiên của tháng sau
    string nextResetString = nextMonth.ToString("yyyyMMdd");
    return int.Parse(nextResetString); // Chuyển sang int để lưu trữ
  }

  void SaveLastUpdateMonthly(int _IntToday)//lưu lần cuối cùng UpdateMonthly
  {
    PlayerPrefs.SetInt("LastUpdateMonthly", _IntToday);
  }
  void SaveNextUpdateMonthly(int _IntUpdateMonthly) // lưu lần tiếp theo UpdateMonthly
  {
    PlayerPrefs.SetInt("NextUpdateMonthly", _IntUpdateMonthly);
  }
  // check rest Năm
  int GetNextYearlyResetTime()
  {
    DateTime now = DateTime.Now;
    // DateTime now = DateTime.UtcNow;
    DateTime nextYear = new DateTime(now.Year + 1, 1, 1); // Lấy ngày 1/1 của năm sau
    string nextResetString = nextYear.ToString("yyyyMMdd");
    return int.Parse(nextResetString); // Chuyển sang int để lưu trữ
  }
  void SaveLastUpdateYearly(int _IntToday)//lưu lần cuối cùng UpdateStrYearly
  {
    PlayerPrefs.SetInt("LastUpdateYearly", _IntToday);
  }
  void SaveNextUpdateYearly(int _IntUpdateStrYearly) // lưu lần tiếp theo UpdateStrYearly
  {
    PlayerPrefs.SetInt("NextUpdateYearly", _IntUpdateStrYearly);
  }



  // void GetEventTimeById(int eventId)
  // {
  //     // Kiểm tra xem eventList có chứa sự kiện với eventId không
  //     if (eventList.ContainsKey(eventId))
  //     {
  //         // Lấy sự kiện từ eventList bằng eventId
  //         EventData eventData = eventList[eventId];

  //         // Lấy thời gian bắt đầu và kết thúc của sự kiện
  //         DateTime startTime = eventData.startDate;
  //         DateTime endTime = eventData.endDate;

  //         // In ra thông tin thời gian
  //         Debug.Log($"Event ID: {eventId} - Start Time: {startTime} - End Time: {endTime}");
  //     }
  //     else
  //     {
  //         Debug.LogWarning($"Không tìm thấy sự kiện với ID: {eventId}");
  //     }
  // }

  public bool IsActiveEventEggHunt()
  {
    return IsEventActive(eventList[1]);
  }
}

/// <summary>
/// Lớp lưu trữ thông tin sự kiện
/// </summary>
public class EventData
{
  public int _idEvent; // ID của sự kiện
  public string EventName; // Tên sự kiện
  public DateTime startDate; // Ngày bắt đầu sự kiện
  public DateTime endDate; // Ngày bắt đầu sự kiện
  public DateTime CountdownEndDate; // Thời gian đếm ngược đến khi sự kiện kết thúc
  public int FullDayEvent; // Tổng số ngày diễn ra sự kiện ()
  public DayOfWeek? weekDays; // Ngày cụ thể trong tuần diễn ra sự kiện (nếu có)
  public int? StartDay; // Ngày bắt đầu của sự kiện (theo ngày tháng)
  public int? StartMonth; // Tháng bắt đầu của sự kiện (theo ngày tháng)
  public EventData(int _eventID, string name, DateTime start, int fullDayEvent, DayOfWeek? days = null, int? startDay = null, int? startMonth = null)
  {
    // thứ tự chuyền dữ liệu 1 : name , 2 ngày bắt đầu .3 tổng số ngày diễn ra sự kiện . 
    //4 ngày trong tuần nếu có . 5 ngày bắt đầu (dùng trong sự kiện tháng) . 6 chuyền vào tháng bắt đầu (dùng trong sự kiện năm) // dòng này k bắt buộc phải chuyền
    // sự kiện tuần bắt buộc phải chuyền vào thứ. sự kiện tháng bắt buộc phải đẻ tháng null . sự kiện năm bắt buộc phải chuyền vào cả ngày cả tháng
    // bắt buộc phải chuyền vào FullDayEvent
    DateTime currentTime = DateTime.Now; // Lấy thời gian hiện tại
                                         // DateTime currentTime = DateTime.UtcNow;
    _idEvent = _eventID; // giúp lấy sự kiện dễ dàng hơn
    EventName = name;
    startDate = start;
    FullDayEvent = fullDayEvent;
    endDate = startDate.AddDays(FullDayEvent);
    if (currentTime > startDate && currentTime < endDate)
    {
      CountdownEndDate = endDate;
    }
    weekDays = days;
    StartDay = startDay;
    StartMonth = startMonth;
  }


}