using System;
using UnityEngine;

public class CountdownSystem : MonoBehaviour
{
  public static CountdownSystem Instance { get; private set; }
  public int maxTimeInSeconds = 1200;
  double _elapsedtimeSecounds;
  public double ElapsedtimeSecounds { get => _elapsedtimeSecounds; }
  string _timeStr;
  public Action<int> OnEndLoopTimeSpin;
  public Action<string> OnTimeUpdate;
  bool isPause = true;
  DateTime startTime;

  void Awake()
  {
    Instance = this;
  }

  void Start()
  {
    var IsEvented = GameManager.Instance.IsEvent;
    if (IsEvented) PlayCountdown();
  }

  public void PlayCountdown()
  {
    isPause = false;
    if (PlayerPrefs.HasKey(KeyString.KEY_EXIT_TIME))
    {
      long exitTime = long.Parse(PlayerPrefs.GetString(KeyString.KEY_EXIT_TIME));
      startTime = DateTime.FromBinary(exitTime);
    }
    else
    {
      startTime = DateTime.Now;
      PlayerPrefs.SetString(KeyString.KEY_EXIT_TIME, startTime.ToBinary().ToString());
    }
  }

  public void StopCountdown()
  {
    isPause = true;
  }

  void FixedUpdate()
  {
    LoopTime();
  }

  void LoopTime()
  {
    if (isPause) return;
    TimeSpan elapsedtime = DateTime.Now - startTime;
    _elapsedtimeSecounds = maxTimeInSeconds - elapsedtime.TotalSeconds;
    _timeStr = ConvertElapsedtimeSecondsToTimeStr(_elapsedtimeSecounds);
    OnTimeUpdate?.Invoke(_timeStr);

    if (elapsedtime.TotalSeconds > maxTimeInSeconds)
    {
      int amountSpin = (int)elapsedtime.TotalSeconds / maxTimeInSeconds;
      double timeRmaining = (double)elapsedtime.TotalSeconds % maxTimeInSeconds;
      startTime = DateTime.Now - TimeSpan.FromSeconds(timeRmaining);
      PlayerPrefs.SetString(KeyString.KEY_EXIT_TIME, startTime.ToBinary().ToString());

      ResetEvent();
      OnEndLoopTimeSpin?.Invoke(amountSpin);
    }
  }

  string ConvertElapsedtimeSecondsToTimeStr(double elapsedTime)
  {
    int totalSeconds = Mathf.FloorToInt((float)elapsedTime);
    int days = totalSeconds / 86400;
    int hours = (totalSeconds % 86400) / 3600;
    int minutes = (totalSeconds % 3600) / 60;
    int seconds = totalSeconds % 60;

    if (days > 0)
    {
      return $"{days}d {hours}h";
    }
    else if (hours > 0)
    {
      return $"{hours}h {minutes}m";
    }
    else if (minutes > 0)
    {
      return $"{minutes}m {seconds}s";
    }
    else
    {
      return $"{seconds}s";
    }
  }

  public void ResetEvent()
  {
    Debug.Log("reset event");
    GameManager.Instance.ResetEvent();
  }
}