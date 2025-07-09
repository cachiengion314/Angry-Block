using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountTimeSystem : MonoBehaviour
{
  public TextMeshProUGUI timerText; // Text UI để hiển thị thời gian

  private bool _hasCountTime;

  void Start()
  {
    if (GameManager.Instance.IsEvented())
    {
      _hasCountTime = true;
    }
  }

  void FixedUpdate()
  {
    if (!_hasCountTime) return;

    UpdateTime();
  }

  void UpdateTime()
  {
    var targetDate = new DateTime(2024, 12, 31);
    var remainingTime = (float)(targetDate - DateTime.Now).TotalSeconds;
    if (remainingTime > 0)
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
      int days = timeSpan.Days;
      int hours = timeSpan.Hours;
      int minutes = timeSpan.Minutes;
      int seconds = timeSpan.Seconds;

      // Tạo định dạng hiển thị
      string displayTime;

      if (days > 0)
      {
        // Hiển thị ngày và giờ
        displayTime = $"{days}d{hours}h";
      }
      else if (hours > 0)
      {
        // Hiển thị giờ và phút
        displayTime = $"{hours}h{minutes}m";
      }
      else if (minutes > 0)
      {
        // Hiển thị phút và giây
        displayTime = $"{minutes}m{seconds}s";
      }
      else
      {
        // Chỉ hiển thị giây nếu còn ít hơn 1 phút
        displayTime = $"{seconds}s";
      }
      timerText.text = displayTime;
    }
    else
    {
      _hasCountTime = false;
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
}
