using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;
using System;

// This class is intended to be used the the AppsFlyerObject.prefab

public class AppsFlyerObjectScript : MonoBehaviour, IAppsFlyerConversionData
{
  public static AppsFlyerObjectScript Instance { get; private set; }

  // These fields are set from the editor so do not modify!
  //******************************//
  public string devKey;
  public string appID;
  public string UWPAppID;
  public string macOSAppID;
  public bool isDebug;
  public bool getConversionData;
  //******************************//

  private string _uniqueUserID;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    DontDestroyOnLoad(gameObject);
  }

  void Start()
  {
    // These fields are set from the editor so do not modify!
    //******************************//
    AppsFlyer.setIsDebug(isDebug);
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        AppsFlyer.initSDK(devKey, UWPAppID, getConversionData ? this : null);
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
    AppsFlyer.initSDK(devKey, macOSAppID, getConversionData ? this : null);
#else
    AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
#endif
    //******************************/

    TryCreateUniqueUserId();
    AppsFlyer.setCustomerUserId(_uniqueUserID);

#if UNITY_IOS && !UNITY_EDITOR
  AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif

    AppsFlyer.startSDK();
  }


  void Update()
  {

  }

  // Mark AppsFlyer CallBacks
  public void onConversionDataSuccess(string conversionData)
  {
    AppsFlyer.AFLog("didReceiveConversionData", conversionData);
    Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
    // add deferred deeplink logic here
  }

  public void onConversionDataFail(string error)
  {
    AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
  }

  public void onAppOpenAttribution(string attributionData)
  {
    AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
    Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
    // add direct deeplink logic here
  }

  public void onAppOpenAttributionFailure(string error)
  {
    AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
  }

  private void TryCreateUniqueUserId()
  {
    if (PlayerPrefs.GetString("UniqueUserID", "") == "")
    {
      _uniqueUserID = DateTime.Now.ToString();
      PlayerPrefs.SetString("UniqueUserID", _uniqueUserID);

      return;
    }

    _uniqueUserID = PlayerPrefs.GetString("UniqueUserID");
  }
}
