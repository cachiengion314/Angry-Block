using UnityEngine;

/// <summary>
/// MoveTicketsSystem
/// </summary>
public partial class GameManager : MonoBehaviour
{
  public bool IsRandomData
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_RANDOM_DATA, 0) == 1;
    set => PlayerPrefs.SetInt(KeyString.KEY_IS_RANDOM_DATA, value ? 1 : 0);
  }
  int _currentEggGiftIndex;
  public int CurrentEggGiftIndex
  {
    get
    {
      return _currentEggGiftIndex;
    }
    set
    {
      _currentEggGiftIndex = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_EGG_GIFT_INDEX, value);
    }
  }

  int _moveEggTicketsAmount;
  public int MoveEggTicketsAmount
  {
    get
    {
      return _moveEggTicketsAmount;
    }
    set
    {
      _moveEggTicketsAmount = value;
      PlayerPrefs.SetInt(KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT, value);
    }
  }

  int _moveEggTicketsAmountNeedClaim;
  public int MoveEggTicketsAmountNeedClaim
  {
    get
    {
      return _moveEggTicketsAmountNeedClaim;
    }
    set
    {
      _moveEggTicketsAmountNeedClaim = value;
      PlayerPrefs.SetInt(KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT_NEEDCLAIM, value);
    }
  }

  int _currentNeedSolvingEggOrderIndex;
  public int CurrentNeedSolvingEggOrderIndex
  {
    get
    {
      return _currentNeedSolvingEggOrderIndex;
    }
    set
    {
      _currentNeedSolvingEggOrderIndex = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_NEED_SOLVING_EGG_ORDER_INDEX, value);
    }
  }
  public void SetCurrentNeedSolvingEggOrderIndex(int _add) // phep cong them
  {

    _moveEggTicketsAmount += _add;
    PlayerPrefs.SetInt(KeyString.KEY_MOVE_EGG_TICKETS_AMOUNT, _moveEggTicketsAmount);
  }
  public void SetCurrentNeedSolvingEggOrderIndexTutorialGameplay() // thang test
  {
    _moveEggTicketsAmount = KeyString.KEY_DEFAULT_MOVE_EGG_TICKETS_AMOUNT;
  }
}