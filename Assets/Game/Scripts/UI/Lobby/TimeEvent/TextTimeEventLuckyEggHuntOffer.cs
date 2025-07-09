using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TextTimeEventLuckyEggHuntOffer : MonoBehaviour
{
  [SerializeField] TMP_Text _textTimeTask;
  [SerializeField] Button buttonEventShowOffer;

  DateTime EndEvent;
  void Awake()
  {
    _textTimeTask = this.GetComponent<TMP_Text>();

    UpdateStatusTxtTime();
  }

  void OnEnable()
  {
    FirebaseSetup.onUpdateRemote += UpdateStatusTxtTime;

    // EndEvent = DateTime.Today.AddDays(IntAddDays());
    EndEvent = GetEndTimeForCurrentWeekWindow();

    var isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.is_unlock_event;
    var dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_off_event;

#if UNITY_ANDROID
    isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.is_unlock_event;
    dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_off_event;
#endif

    if (!isUnlockEvent) return;

    // Check timeEnd
    var coms = dayOffEvent.Split('/');
    if (coms.Length != 3) { return; }
    if (!int.TryParse(coms[0], out int dayOff)) { return; }
    if (!int.TryParse(coms[1], out int monthOff)) { return; }
    if (!int.TryParse(coms[2], out int yearOff)) { return; }

    var calendarOff = new DateTime(yearOff, monthOff, dayOff);
    TimeSpan remainingTime = calendarOff - DateTime.Now;

    if (remainingTime <= TimeSpan.Zero) return;

    var endEventEggTime = calendarOff;
    EndEvent = EndEvent > endEventEggTime ? endEventEggTime : EndEvent;
  }

  private void OnDisable()
  {
    FirebaseSetup.onUpdateRemote -= UpdateStatusTxtTime;
  }

  DateTime GetEndTimeForCurrentWeekWindow()
  {
    var today = DateTime.Today;
    switch (today.DayOfWeek)
    {
      case DayOfWeek.Tuesday:   // Thứ 3
      case DayOfWeek.Wednesday: // Thứ 4
        return today.DayOfWeek == DayOfWeek.Tuesday
            ? today.AddDays(1).AddHours(23).AddMinutes(59) // hết T4
            : today.AddHours(23).AddMinutes(59); // hết hôm nay
      case DayOfWeek.Friday:    // Thứ 6
      case DayOfWeek.Saturday:  // Thứ 7
        return today.DayOfWeek == DayOfWeek.Friday
            ? today.AddDays(1).AddHours(23).AddMinutes(59)
            : today.AddHours(23).AddMinutes(59);
      default:
        return today; // Các ngày khác: không hiển thị
    }
  }

  int IntAddDays()
  {
    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
    daysUntilMonday = daysUntilMonday == 0 ? 7 : daysUntilMonday; // Nếu hôm nay là thứ Hai, thì chọn thứ Hai tuần sau (7 ngày)
    // Debug.Log("daysUntilMonday--" + daysUntilMonday);
    return daysUntilMonday;
  }

  void Update()
  {
    var isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.ios.is_unlock_full_event;

#if UNITY_ANDROID
    isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.android.is_unlock_full_event;
#endif 

    if (isUnlockFullEvent)
    {
      _textTimeTask.text = "3 in 1";
      return;
    }

    TimeSpan timeRemaining = EndEvent - DateTime.Now;
    if (timeRemaining.TotalSeconds > 0)
    {
      // _textTimeTask.text = $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
      // _textTimeTask.text = $"{timeRemaining.Hours}h {timeRemaining.Minutes}m";
      _textTimeTask.text = $"{timeRemaining.Days}d {timeRemaining.Hours:D2}h {timeRemaining.Minutes:D2}m ";
    }
    else
    {
      _textTimeTask.text = "Event is over"; // Khi hết giờ, hiển thị 00:00
                                            // event is over
      if (buttonEventShowOffer != null)
      {
        buttonEventShowOffer.enabled = false;
      }
    }
  }

  private void UpdateStatusTxtTime()
  {
    var isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.ios.is_unlock_full_event;

#if UNITY_ANDROID
    isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.android.is_unlock_full_event;
#endif 

    if (isUnlockFullEvent)
    {
      if (_textTimeTask == null) return;

      // _textTimeTask.gameObject.SetActive(false);
      _textTimeTask.text = "3 in 1";
    }
  }
}
