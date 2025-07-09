using System;
using System.Collections.Generic;
using UnityEngine;

public class EventActionManager : MonoBehaviour
{
    private static Dictionary<string, Delegate> eventDictionary = new Dictionary<string, Delegate>();


    // Đăng ký listener với tham số linh hoạt
    public static void StartListening<TEnum, TData>(TEnum eventName, Action<TData> listener) where TEnum : Enum
    {
        string key = eventName.ToString();
        if (eventDictionary.TryGetValue(key, out var thisEvent))
        {
            eventDictionary[key] = Delegate.Combine(thisEvent, listener);
        }
        else
        {
            eventDictionary[key] = listener;
        }
    }

    public static void StopListening<TEnum, TData>(TEnum eventName, Action<TData> listener) where TEnum : Enum
    {
        string key = eventName.ToString();
        if (eventDictionary.TryGetValue(key, out var thisEvent))
        {
            eventDictionary[key] = Delegate.Remove(thisEvent, listener);
        }
    }

    public static void TriggerEvent<TEnum, TData>(TEnum eventName, TData data) where TEnum : Enum
    {
        string key = eventName.ToString();
        if (eventDictionary.TryGetValue(key, out var thisEvent))
        {
            if (thisEvent is Action<TData> callback)
            {
                callback.Invoke(data);
            }
        }
    }

}
public struct MissonDataDailyTask
{
    public string KeyType;
    public int UpProgess;


    public MissonDataDailyTask(enumListeningDataDailyTask inputKeyType, int inputUpprogess)
    {
        KeyType = inputKeyType.ToString();
        UpProgess = inputUpprogess;
    }
}

public struct MissonDataDailyWeekly
{
    public string KeyType;
    public int UpProgess;


    public MissonDataDailyWeekly(enumListeningDataDailyWeekly inputKeyType, int inputUpprogess)
    {
        KeyType = inputKeyType.ToString();
        UpProgess = inputUpprogess;
    }
}
public enum enumListeningDataDailyWeekly
{
    None,
    CollectCoinWeekly,
    Collect1000CupsWeekly,
    CompleteLevelWeekly,
    UnlockSlotsWeekly,
    UsePowerItem,
    ReviveWeekly
}

public enum enumListeningDataDailyTask
{
    None,

    Login,

    Booster1,
    Booster2,
    Booster3,
    Booster4,

    CompleteLevel,
    SpinLuckyWheel,
    Revive,
    CollectCoin,

    CompleteCupCappuccino,
    CompleteCupCherryJuice,
    CompleteCupGreentea,
    CompleteCupMocha,
    CompleteCupAppleJuice,
    CompleteCupLimeJuice,
    CompleteCupOrangeJuice,
    CompleteCupPinacolada,

}


