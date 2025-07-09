using UnityEngine;
using TMPro;
using System;

public class TimeDailyTask : MonoBehaviour
{
    [SerializeField] TMP_Text _textTimeTask;
    DateTime EndEvent;
    void Awake()
    {
        _textTimeTask = this.GetComponent<TMP_Text>();
    }
    void OnEnable()
    {
        EndEvent = DateTime.Today.AddDays(1);
    }
    void Update()
    {
        TimeSpan timeRemaining = EndEvent - DateTime.Now;
        if (timeRemaining.TotalSeconds > 0)
        {
            // _textTimeTask.text = $"{timeRemaining.Hours:D2}:{timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}";
            _textTimeTask.text = $"{timeRemaining.Hours}h {timeRemaining.Minutes}m";
        }
        else
        {
            _textTimeTask.text = "00:00"; // Khi hết giờ, hiển thị 00:00
        }

    }
}
