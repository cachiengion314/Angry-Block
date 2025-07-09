using TMPro;
using UnityEngine;

public class UpdateVersionModal : MonoBehaviour
{
  [Header("Internal Denpendences")]
  [SerializeField] TextMeshProUGUI updateMessageTxt;

  #region Lifecycle Function
  private void OnEnable()
  {
    FirebaseSetup.onNeedUpdate += UpdateUI;
  }

  private void OnDisable()
  {
    FirebaseSetup.onNeedUpdate -= UpdateUI;
  }

  #endregion

  #region Button Function
  public void UpdateVersion()
  {
    var url = "";

#if UNITY_ANDROID
    url = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.update_config.android.link_app;
#elif UNITY_IOS
    url = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.update_config.ios.link_app;
#endif

    Application.OpenURL(url);
  }

  public void Exit()
  {
    Application.Quit();
  }

  public void Close()
  {
    LobbyPanel.Instance.CloseUpdateModal();
  }

  #endregion

  #region Event Function
  private void UpdateUI()
  {
    var version = "";

#if UNITY_ANDROID
    version = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.update_config.android.version;
#elif UNITY_IOS
    version = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.update_config.ios.version;
#endif

    var updateMessage = "New version " + version + " is available. Update now!";

    updateMessageTxt.text = updateMessage;
  }

  #endregion
}
