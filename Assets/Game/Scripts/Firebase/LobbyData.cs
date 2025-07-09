using System;

[Serializable]
public struct LobbyData
{
  public ParameterLobbyDatas parameters;
}

[Serializable]
public struct ParameterLobbyDatas
{
  public AdmobData Admob;
  public UpdateConfigData update_config;
  public ConfigGameData config_game;
  public LuckyWheelDataRemote lucky_wheel;
  public NetworkRequiredData network_required;
  public FreeCoinData free_coin;
  public CheatData cheat_setting;
  public RefillHeartData refill_heart;
  public EggHuntLuckyOfferData egg_hunt_offer;
  public LuckyRewardRemoteData lucky_reward;
  public DailyTaskData daily_task;
  public DailyWeeklyData daily_weekly;
  public EggHuntLuckyEventData egg_hunt_event;
}

//
[Serializable]
public struct AdmobData
{
  public AdmobDeviceData ios;
  public AdmobDeviceData android;
}

[Serializable]
public struct AdmobDeviceData
{
  public AdmobTypeData admob_AOA;
}

[Serializable]
public struct AdmobTypeData
{
  public bool isShow;
  public string value;
}

//
[Serializable]
public struct ConfigGameData
{
  public int energy_refill_time;
  public int max_energy;
}

//
[Serializable]
public struct UpdateConfigData
{
  public UpdateConfigDeviceData ios;
  public UpdateConfigDeviceData android;
}

[Serializable]
public struct UpdateConfigDeviceData
{
  public string version;
  public string link_app;
  public int reward;
}

[Serializable]
public struct LuckyWheelDataRemote
{
  public LuckyWheelDeviceDataRemote ios;
  public LuckyWheelDeviceDataRemote android;
}

[Serializable]
public struct LuckyWheelDeviceDataRemote
{
  public float tier1;
  public float tier2;
  public float tier3;
  public float tier4;
  public int dailyLimit;
}

[Serializable]
public struct NetworkRequiredData
{
  public NetworkRequiredDeviceData ios;
  public NetworkRequiredDeviceData android;
}

[Serializable]
public struct NetworkRequiredDeviceData
{
  public bool network_required;
}

[Serializable]
public struct FreeCoinData
{
  public FreeCoinDeviceData ios;
  public FreeCoinDeviceData android;
}

[Serializable]
public struct FreeCoinDeviceData
{
  public int coin_received;
  public int limit_watch;
}

[Serializable]
public struct CheatData
{
  public CheatDeviceData ios;
  public CheatDeviceData android;
}

[Serializable]
public struct CheatDeviceData
{
  public bool isOn;
}

[Serializable]
public struct RefillHeartData
{
  public RefillHeartDeviceData ios;
  public RefillHeartDeviceData android;
}

[Serializable]
public struct RefillHeartDeviceData
{
  public int refill_amount;
}

[Serializable]
public struct DailyTaskData
{
  public DailyTaskManager.TaskDataJsonBase.TaskJson[] TasksTodayJson;
}
[Serializable]
public struct DailyWeeklyData
{
  public DailyWeeklyManager.WeeklyTaskDataJsonBase.WeeklyTask[] WeeklyTasksBaseJson;
}

[Serializable]
public struct EggHuntLuckyOfferData
{
  public EggHuntLuckyOfferDeviceData ios;
  public EggHuntLuckyOfferDeviceData android;
}

[Serializable]
public struct EggHuntLuckyOfferDeviceData
{
  public bool is_unlock_full_event;
}

[Serializable]
public struct LuckyRewardRemoteData
{
  public LuckyRewardRemoteDeviceData ios;
  public LuckyRewardRemoteDeviceData android;
}

[Serializable]
public struct LuckyRewardRemoteDeviceData
{
  public LuckyOfferData[] luckyOfferDatas;
}

[Serializable]
public struct EggHuntLuckyEventData
{
  public EggHuntLuckyEventDeviceData ios;
  public EggHuntLuckyEventDeviceData android;
}

[Serializable]
public struct EggHuntLuckyEventDeviceData
{
  public bool is_unlock_event;
  public string day_start_event;
  public string day_off_event;
}
