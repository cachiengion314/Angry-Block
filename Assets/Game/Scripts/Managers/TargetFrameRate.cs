using System.Collections;
using System.Threading;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
  [Header("Frame Settings")]
  public int MaxRate = 9999;
  public bool ShouldInvokeInEditor;
  public float TargetFrame = 60.0f;
  float currentFrameTime;

  void Awake()
  {
    if (!ShouldInvokeInEditor) return;

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
      currentFrameTime += 1.0f / TargetFrame;
      var t = Time.realtimeSinceStartup;
      var sleepTime = currentFrameTime - t - 0.01f;
      if (sleepTime > 0)
        Thread.Sleep((int)(sleepTime * 1000));
      while (t < currentFrameTime)
        t = Time.realtimeSinceStartup;
    }
  }
}
