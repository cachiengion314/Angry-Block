using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public partial class TextTimeWeeklyTask : MonoBehaviour
{
  private TMP_Text _TextID;
  [SerializeField] int _IdEventTest;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  [SerializeField] Button buttonEventShowPopupLuckyEggHunt;
  void Awake()
  {
    _TextID = this.GetComponent<TMP_Text>();
  }

  void Update()
  {
    UseTimeEventManager();
  }

  private void UseTimeEventManager()
  {
    if (TimeEventManager.Instance.eventList.ContainsKey(_IdEventTest))
    {
      EventData eventDataID1 = TimeEventManager.Instance.eventList[_IdEventTest];

      // Lấy thời gian 00:00 của ngày kết thúc
      DateTime endOfEventDay = eventDataID1.endDate.Date;

      // Tính thời gian còn lại
      TimeSpan remainingTime = endOfEventDay - DateTime.Now;

      // Kiểm tra nếu sự kiện đã kết thúc
      if (remainingTime <= TimeSpan.Zero)
      {
        _TextID.text = $"End Event:{eventDataID1.EventName}";
      }
      else
      {
        // Định dạng thời gian còn lại
        string formattedTime = $"{remainingTime.Days}d {remainingTime.Hours:D2}h {remainingTime.Minutes:D2}m ";

        // Hiển thị thời gian lên Text
        _TextID.text = formattedTime;
        // _TextID.text = $"{formattedTime}";
      }
    }
    else
    {
      _TextID.text = "event is over";
      if (buttonEventShowPopupLuckyEggHunt != null)
      {
        buttonEventShowPopupLuckyEggHunt.enabled = false;
      }
    }
  }
}
