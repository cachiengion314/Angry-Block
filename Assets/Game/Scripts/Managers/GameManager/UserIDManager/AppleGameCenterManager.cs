using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;
#if UNITY_IOS
using Apple.GameKit;
#endif

public class AppleGameCenterManager : MonoBehaviour
{
  // Start is called before the first frame update
#if UNITY_IOS
  async void Start()
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) // prevent invoke in unity editor
      await LoginAsync();
  }

  /// <summary>
  /// Convert from "A:_abfbbb0591da61d12644b64b1fb9d8f5" to _abfbbb0591da61d12644b64b1fb9d8f5
  /// </summary>
  /// <param name="input"></param>
  /// <returns></returns>
  string ExtractGameIdFrom(string input)
  {
    string pattern = @":([^\s]+)"; // Matches non-whitespace characters only
    Match match = Regex.Match(input, pattern);
    if (match.Success)
      return match.Groups[1].Value;
    return null;
  }

  public async Task LoginAsync()
  {
    if (GKLocalPlayer.Local.IsAuthenticated)
    {
      Debug.Log("GameKit: player already authenticated.");
      return;
    }

    try
    {
      // Perform the authentication
      var player = await GKLocalPlayer.Authenticate();
      Debug.Log($"GameKit Authentication: player {player.DisplayName}");
      Debug.Log($"GameKit Authentication: playerId {player.GamePlayerId}");

      // Retrieve identity data
      var localPlayer = GKLocalPlayer.Local;
      var playerId = ExtractGameIdFrom(localPlayer.GamePlayerId);

      GameManager.Instance.OnGotUserIdSuccessfully(
        playerId,
        localPlayer.DisplayName
      );
    }
    catch (Exception ex)
    {
      Debug.LogError($"GameKit Authentication failed: {ex}");
      // Optional: notify user of failure, retry logic, etc.
      GameManager.Instance.OnGotUserIdFail();
    }
  }
#endif
}