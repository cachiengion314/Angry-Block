using System;
using Firebase.Analytics;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Booster Manager
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
  public void TrySelfRemovedByHammerAt(float3 pos, IPoolItemControl obj)
  {
    // This function only work when IsTriggeredHammer is true
    if (!PowerItemPanel.Instance.IsTriggeredHammer || GameManager.Instance.CurrentHammer <= 0) return;
    if (trayGrid.IsPosOutsideAt(pos)) return;

    SoundManager.Instance.PlayHittingSfx();
    PowerItemPanel.Instance.IsTriggeredHammer = false;
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIAL_HAMMER, 1);

    OnSelfRemovedByHammer();

    var index = trayGrid.ConvertWorldPosToIndex(pos);
    if (index >= 0 && index <= Grasses.Length - 1)
    {
      var gObj = Grasses[index];
      if (gObj)
      {
        gObj.GetComponent<GrassControl>().RemoveFromTable();
      }
    }

    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_FIRST_USE_HAMMER) == 0)
      PlayerPrefs.SetInt(KeyString.KEY_BOOL_FIRST_USE_HAMMER, 1);
    else
      GameManager.Instance.CurrentHammer--;

    var dataWeekly = new MissonDataDailyWeekly(enumListeningDataDailyWeekly.UsePowerItem, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyWeekly.UsePowerItem, dataWeekly);

    var data = new MissonDataDailyTask(enumListeningDataDailyTask.Booster2, 1);
    EventActionManager.TriggerEvent(enumListeningDataDailyTask.Booster2, data);

    PowerItemPanel.Instance.UpdateAmountUI();
    PowerItemPanel.Instance.DeTrigger();
    EffectManager.Instance.SpawnHammerAt(pos, out float existingTime);
    obj.FullyRemoveFromTable();

    LeanTween.delayedCall(gameObject, existingTime, () =>
    {
      EffectManager.Instance.SpawnHammerExplosiveAt(pos);
      EffectManager.Instance.TurnOffDarkImg();
      AnimationManager.Instance.PlayShakesAt(1, .1f,
      () =>
      {
        // obj.FullyRemoveFromTable();
      });
    });

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(KeyString.FIREBASE_BOOSTER_SPEND,
         new Parameter[]
         {
            new ("level", (GameManager.Instance.CurrentLevel + 1).ToString()),
            new ("booster_name", "Hammer"),
            new ("placement", "Ingame")
         });
    }
  }

  private void OnSelfRemovedByHammer()
  {

  }
}