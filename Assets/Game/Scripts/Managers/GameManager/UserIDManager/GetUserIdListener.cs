using UnityEngine;

/// <summary>
/// GetUserIdListener
/// </summary> <summary>
/// </summary>
public partial class GameManager : MonoBehaviour
{
  string _userId;
  public string UserId { get { return _userId; } }
  string _userName;
  public string UserName { get { return _userName; } }
  int _gotUserIdResultCode = 0;
  public int GotUserIdResultCode { get { return _gotUserIdResultCode; } }

  public void OnGotUserIdSuccessfully(string userId, string userName)
  {
    _userId = userId;
    _userName = userName;
    _gotUserIdResultCode = 1;

    print("OnGotUserIdSuccessfully.userId " + userId);
    print("OnGotUserIdSuccessfully.userName " + userName);
  }

  public void OnGotUserIdFail()
  {
    _gotUserIdResultCode = 2;
    print("OnGotUserIdFail ");
  }
}