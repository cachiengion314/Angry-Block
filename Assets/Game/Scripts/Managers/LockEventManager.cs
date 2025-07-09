using UnityEngine;
using System;
using Spine.Unity;
using UnityEngine.SceneManagement;
using System.Collections;

public class LockEventManager : MonoBehaviour
{
  [Header("---LukcyEggHunt---")]
  [SerializeField] GameObject _buttonLukcyEggHunt; // sự kiện có id = 1
  [SerializeField] SkeletonGraphic skeLuckyEggHuntOffer;

  private void OnEnable()
  {
    FirebaseSetup.onUpdateRemote += OnUpdateRemote;
  }

  void Start()
  {
    // TimeEventManager.onNextYear += ClearDataNextYear;

    TimeEventManager.Instance.CheckNextYear();
    CheckLuckyEggHuntEvent();
    LobbyPanel.Instance.InitPosDefaultLiveOps();
  }

  private void OnDisable()
  {
    FirebaseSetup.onUpdateRemote -= OnUpdateRemote;
  }

  void CheckLuckyEggHuntEvent()
  {
    var isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.is_unlock_event;
    var dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_off_event;
    var dayStartEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_start_event;

#if UNITY_ANDROID
    isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.is_unlock_event;
    dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_off_event;
    dayStartEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_start_event;
#endif

    if (!isUnlockEvent)
    {
      HideEventLuckyEgg();
      return;
    }

    // Check timeEnd
    var coms = dayOffEvent.Split('/');
    if (coms.Length != 3) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms[0], out int dayOff)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms[1], out int monthOff)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms[2], out int yearOff)) { HideEventLuckyEgg(); return; }

    var calendarOff = new DateTime(yearOff, monthOff, dayOff);
    TimeSpan remainingTime = calendarOff - DateTime.Now;

    if (remainingTime <= TimeSpan.Zero)
    {
      HideEventLuckyEgg();
      GameManager.Instance.ClearDataEventLuckyEggHunt();
      return;
    }

    // Check timeStart
    var coms1 = dayStartEvent.Split('/');
    if (coms1.Length != 3) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms1[0], out int dayStart)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms1[1], out int monthStart)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms1[2], out int yearStart)) { HideEventLuckyEgg(); return; }

    var calendarStart = new DateTime(yearStart, monthStart, dayStart);
    TimeSpan remainingTime1 = DateTime.Now - calendarStart;

    if (remainingTime1 <= TimeSpan.Zero)
    {
      HideEventLuckyEgg();
      GameManager.Instance.ClearDataEventLuckyEggHunt();
      return;
    }

    _buttonLukcyEggHunt.SetActive(true); // Bật nút nếu sự kiện đang diễn ra
    CheckLuckyEggHuntOffer();

    if (SceneManager.GetActiveScene().name == "Lobby")
    {
      LobbyPanel.Instance.InitPosDefaultLiveOps();
    }
  }

  private void OnUpdateRemote()
  {
    var isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.is_unlock_event;
    var dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_off_event;
    var dayStartEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.ios.day_start_event;

#if UNITY_ANDROID
    isUnlockEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.is_unlock_event;
    dayOffEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_off_event;
    dayStartEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_event.android.day_start_event;
#endif

    if (!isUnlockEvent)
    {
      HideEventLuckyEgg();
      return;
    }

    // Check timeEnd
    var coms = dayOffEvent.Split('/');
    if (coms.Length != 3) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms[0], out int dayOff)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms[1], out int monthOff)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms[2], out int yearOff)) { HideEventLuckyEgg(); return; }

    var calendarOff = new DateTime(yearOff, monthOff, dayOff);
    TimeSpan remainingTime = calendarOff - DateTime.Now;

    if (remainingTime <= TimeSpan.Zero)
    {
      HideEventLuckyEgg();
      GameManager.Instance.ClearDataEventLuckyEggHunt();
      return;
    }

    // Check timeStart
    var coms1 = dayStartEvent.Split('/');
    if (coms1.Length != 3) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms1[0], out int dayStart)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms1[1], out int monthStart)) { HideEventLuckyEgg(); return; }
    if (!int.TryParse(coms1[2], out int yearStart)) { HideEventLuckyEgg(); return; }

    var calendarStart = new DateTime(yearStart, monthStart, dayStart);
    TimeSpan remainingTime1 = DateTime.Now - calendarStart;

    if (remainingTime1 <= TimeSpan.Zero)
    {
      HideEventLuckyEgg();
      GameManager.Instance.ClearDataEventLuckyEggHunt();
      return;
    }

    if (SceneManager.GetActiveScene().name == "Lobby")
    {
      StartCoroutine(ReloadPosDefaultLiveOps());
    }
  }

  IEnumerator ReloadPosDefaultLiveOps()
  {
    yield return new WaitUntil(() => !LobbyPanel.Instance.IsTweeningLiveOps);

    _buttonLukcyEggHunt.SetActive(true); // Bật nút nếu sự kiện đang diễn ra
    CheckLuckyEggHuntOffer();
    LobbyPanel.Instance.InitPosDefaultLiveOps();
  }

  void ClearDataNextYear()
  {
    GameManager.Instance.ClearDataEventLuckyEggHunt();
    // Debug.Log("ClearData");
  }

  void CheckLuckyEggHuntOffer()
  {
    DayOfWeek today = DateTime.Now.DayOfWeek;

    var isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.ios.is_unlock_full_event;

#if UNITY_ANDROID
    isUnlockFullEvent = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.egg_hunt_offer.android.is_unlock_full_event;
#endif 

    if (isUnlockFullEvent)
    {
      skeLuckyEggHuntOffer.gameObject.SetActive(true);
      return;
    }

    // // Nếu là thứ Bảy hoặc Chủ Nhật => bật offer
    // if (today == DayOfWeek.Saturday || today == DayOfWeek.Sunday)
    // {
    //   skeLuckyEggHuntOffer.gameObject.SetActive(true);
    // }
    // else
    // {
    //   skeLuckyEggHuntOffer.gameObject.SetActive(false);
    // }

    // ✅ Chỉ bật nếu hôm nay là Thứ 3, Thứ 4, Thứ 6, Thứ 7
    if (today == DayOfWeek.Tuesday || today == DayOfWeek.Wednesday ||
        today == DayOfWeek.Friday || today == DayOfWeek.Saturday)
    {
      skeLuckyEggHuntOffer.gameObject.SetActive(true);
    }
    else
    {
      skeLuckyEggHuntOffer.gameObject.SetActive(false);
    }
  }

  private void HideEventLuckyEgg()
  {
    _buttonLukcyEggHunt.SetActive(false);
    skeLuckyEggHuntOffer.gameObject.SetActive(false);
  }
}
