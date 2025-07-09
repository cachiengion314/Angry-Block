using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeEventData", menuName = "ScriptableObjects/TimeEventData", order = 1)]
public class TimeEventData : ScriptableObject
{
    [Serializable]
    public class EventDataBase
    {
        public int _idEvent; // ID sự kiện
        public string _strName; // Tên sự kiện // note k dược để trùng tên gây ra lỗi(do unity nhận diện sai)
        public TypeEvent _isTypeEvent; // Loại sự kiện // nếu để type TypeEventMonth thì 1<=StartDay <=28. nếu để TypeEventWeeky2 thì 1<=StartDay <=28 và 1<=MonthStart<=12
        // public DataTimeWeekly _dataTimeWeekly;

        // Danh sách dữ liệu theo từng loại TypeEvent
        public DayOfWeek weekDays; // Dành cho TypeEventWeekly
        public int StartDay; // Ngày bắt đầu
        public int MonthStart; // Tháng bắt đầu 
        public int FullDayEvent; // Tổng số ngày diễn ra sự kiện
        public TypeWeekEvent _isTypeWeeklyEvent; // Loại sự kiện ngày //để cùng type + cùng weekDays sẽ thành hiệu ứng xoay tua
        public TypeMonthEvent _isTypeMonthEvent; // Loại sự kiện tháng //để cùng type + cùng StartDay sẽ thành hiệu ứng xoay tua
        public TypeYearEvent _isTypeYearEvent; // Loại sự kiện năm //để cùng type + cùng StartDay + MonthStart cùng  sẽ thành hiệu ứng xoay tua
        public int _idIntTypeEvents; // ID duy nhất cho loại sự kiện
    }

    public List<EventDataBase> AllEvents = new List<EventDataBase>();
    public Dictionary<string, EventDataBase> WeeklyListdata = new Dictionary<string, EventDataBase>(); // Danh sách sự kiện
    public Dictionary<string, int> eventTypeCounters = new Dictionary<string, int>();

    private void OnValidate()
    {
        if (AllEvents != null)
        {
            eventTypeCounters.Clear();
            WeeklyListdata.Clear(); //
            // Dictionary để theo dõi số lần xuất hiện của mỗi nhóm sự kiện
            // Dictionary<string, int> eventTypeCounters = new Dictionary<string, int>();

            for (int i = 0; i < AllEvents.Count; i++)
            {
                var currentEvent = AllEvents[i];
                currentEvent._idEvent = i; // Gán ID sự kiện theo thứ tự trong danh sách

                // Xác định key nhóm sự kiện
                string key = GenerateEventKey(currentEvent);
                // Debug.Log("" + key);
                // Kiểm tra nếu key đã tồn tại thì tăng giá trị, nếu chưa thì đặt về 0
                if (!eventTypeCounters.ContainsKey(key))
                {
                    eventTypeCounters[key] = 0;

                }
                else
                {
                    eventTypeCounters[key] += 1;
                }

                // Gán giá trị _idIntTypeEvents dựa trên số lần xuất hiện của key
                currentEvent._idIntTypeEvents = eventTypeCounters[key];


            }
        }
    }

    private string GenerateEventKey(EventDataBase eventData)
    {
        string key = "";

        switch (eventData._isTypeEvent)
        {
            case TypeEvent.TypeEventWeekly:
                key = $"{eventData._isTypeEvent}_{eventData._isTypeWeeklyEvent}";
                break;

            case TypeEvent.TypeEventMonth:
                key = $"{eventData._isTypeEvent}_{eventData._isTypeMonthEvent}";
                break;

            case TypeEvent.TypeEventYear:
                key = $"{eventData._isTypeEvent}_{eventData._isTypeYearEvent}";
                break;

            default:
                key = $"{eventData._isTypeEvent}"; // Nếu là TypeEventCustoms hoặc loại khác
                break;
        }

        // Debug.Log("Generated Key: " + key); // In key ra console
        return key;
    }


    public enum TypeEvent
    {
        TypeEventWeekly, // Ngày trong tuần
        TypeEventMonth,  // Ngày trong tháng
        TypeEventYear,   // Ngày trong năm
    }

    public enum TypeWeekEvent
    {
        TypeEventWeekyDaily,
        TypeEventWeeky1,
        TypeEventWeeky2,
        // TypeEventDay3
    }

    public enum TypeMonthEvent
    {
        TypeEventMonth1,
        TypeEventMonth2,
        TypeEventMonth3
    }

    public enum TypeYearEvent
    {
        TypeEventLuckEggsHunt,
        TypeEventYear2,
        TypeEventYear3
    }
}


