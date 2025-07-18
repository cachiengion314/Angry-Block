using System.Collections;
using System.Threading;
using UnityEngine;

public class FrameRateSystem : MonoBehaviour
{
  [Header("Frame Settings")]
  public int MaxRate = 9999;
  public float TargetFrameRate = 60.0f;
  float currentFrameTime;
  [Tooltip("If enable this option will make sure editor running at TargetFrameRate")]
  public bool RunInEditor;

  void Awake()
  {
#if UNITY_EDITOR
    if (!RunInEditor) return;
#endif
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = MaxRate;
    currentFrameTime = Time.realtimeSinceStartup;
    StartCoroutine(WaitForNextFrame());
  }

  IEnumerator WaitForNextFrame()
  {
    while (true)
    {
      yield return new WaitForEndOfFrame();
      currentFrameTime += 1.0f / TargetFrameRate;
      var t = Time.realtimeSinceStartup;
      var sleepTime = currentFrameTime - t - 0.01f;
      if (sleepTime > 0)
        Thread.Sleep((int)(sleepTime * 1000));
      while (t < currentFrameTime)
        t = Time.realtimeSinceStartup;
    }
  }
}