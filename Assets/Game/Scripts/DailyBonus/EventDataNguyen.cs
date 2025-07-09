using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EventDataNguyen", menuName = "EventDataNguyen", order = 0)]
public class EventDataNguyen : ScriptableObject
{
    [Header("Use")]
    public bool DayOfWeek;
    public bool Month;
    public bool Day;
    public bool Hour;
    public bool Minute;
    [Space(20)]
    [Header("Start")]
    public int startMonth;
    public int startDay;
    public int startHour;
    public int startMinute;
    public DayOfWeek startDayOfWeek;
    [Space(20)]
    [Header("End")]
    public int endMonth;
    public int endDay;
    public int endHour;
    public int endMinute;
    public DayOfWeek endDayOfWeek;
    [Space(20)]
    [Header("Length")]
    [Range(0,99)]
    public int lengthEvent;
}
