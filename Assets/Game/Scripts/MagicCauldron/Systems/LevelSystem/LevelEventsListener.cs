using System;
using System.Collections.Generic;
using Firebase.Analytics;
using HoangNam;
using UnityEngine;

/// <summary>
/// LevelEventsListener
/// </summary>
public partial class LevelSystem : MonoBehaviour
{
  Action OnPickEggTutorialDone;
  public int IndexTutorialGamePlay = 0;

  void OnCompletedMoveToHolder(EggControl egg)
  {
    if (IsNeedEggTutorial == true)
    {
      UICanvaEventLuckEggHunt.Instance.PickEggsTutorial(IndexTutorialGamePlay);

    }
    egg.PlayAnimEgg();
  }

  void OnCompletedReturnEggToSpawnedPosition(EggControl egg)
  {
    egg.OffAnimEgg();
  }

  void OnCompleteEachEggMoveToDestination(EggControl egg)
  {
    SoundManager.Instance.PlayRewardEggSfx();
  }

  void OnCompletedEggMoveToScreen(EggControl egg)
  {
    SoundManager.Instance.PlayPickEggSfx();
  }

  void OnCompleteAllEggsBreak()
  {
    if (IsNeedEggTutorial == true)
    {
      UICanvaEventLuckEggHunt.Instance.StateTuTorial4();
      UICanvaEventLuckEggHunt.Instance.SetActiveButtonNextState();
      UICanvaEventLuckEggHunt.Instance.SetLayerEggHolder();
      PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_MOVE_TUTORIAL_GAMEPLAY_EGG, 3);
    }
  }

  void OnCompleteAllEggsMoveToDestination()
  {
    SoundManager.Instance.PlayRewardEggSfx();
  }

  void OnBeginWrongEggShaking(EggControl egg)
  {
    SoundManager.Instance.PlayPressBtnSfx();
  }

  void OnBeginRightEggShaking(EggControl egg)
  {
    SoundManager.Instance.PlayHittingSfx();
  }

  void OnBeginEggBreak(EggControl egg)
  {
    SoundManager.Instance.PlayBrokenEggsSfx();
    OnPickEggTutorialDone?.Invoke();
  }

  void OnBeginAllEggMoveToDestination()
  {
    SoundManager.Instance.PlaySuggessPosEggSfx();
    UICanvaEventLuckEggHunt.Instance.SetupStatusEggs(true);
  }

  void OnCannotMoveEgg(EggControl egg)
  { // k thể di chuyển (hết lượt)
    SoundManager.Instance.PlayPickEggSfx();
    UICanvaEventLuckEggHunt.Instance.ShowNotifNoMove();
  }

  void OnBeginFoundEmptySlotIndex(EggControl egg)
  {
    SoundManager.Instance.PlayPickEggSfx();
    egg.SetActitveShadowEgg(false);
    _moveEggTicketsAmount--;

    if (_moveEggTicketsAmount >= 0)
    {
      UICanvaEventLuckEggHunt.Instance.UpdateTurnMove(_moveEggTicketsAmount);
    }
    else
    {
      UICanvaEventLuckEggHunt.Instance.ShowNotifNoMove();
    }
  }

  void OnBeginDuplicateFailMatched(int wrongCount)
  {
    _moveEggTicketsAmount += wrongCount;
    GameManager.Instance.MoveEggTicketsAmount = _moveEggTicketsAmount;

    if (_moveEggTicketsAmount >= 0)
    {
      UICanvaEventLuckEggHunt.Instance.UpdateTurnMove(_moveEggTicketsAmount);
    }
    else
    {
      UICanvaEventLuckEggHunt.Instance.ShowNotifNoMove();
    }
  }

  void OnCompletedGiftMoveToDestination(GiftControl gift)
  {
    itemSystem.RemoveNonPoolItem(gift.gameObject);
    gift.gameObject.SetActive(false);
    EffectManager.Instance.SpawnArcaneSparkAt(gift.transform.position);
  }

  void OnCompletedAllGiftsMoveToDestination(List<GiftControl> gifts)
  {
    SoundManager.Instance.PlayCoinExplodeSfx();

    UICanvaEventLuckEggHunt.Instance.SetImageReward(_currentNeedSolvingOrderIndex);
    UICanvaEventLuckEggHunt.Instance.SetupStatusEggs(false);


    for (int i = 0; i < gifts.Count; ++i)
    {
      var gift = gifts[i];
      if (gift.Type == PriceType.InfinityHeart)
      {
        HeartSystem.Instance.AddInfinityHeartTime(gift.Value);
      }

      if (gift.Type == PriceType.Coin)
      {
        GameManager.Instance.CurrentCoin += gift.Value;
      }

      if (gift.Type == PriceType.Refresh)
      {
        GameManager.Instance.CurrentRefresh += gift.Value;
      }

      if (gift.Type == PriceType.Hammer)
      {
        GameManager.Instance.CurrentHammer += gift.Value;
      }

      if (gift.Type == PriceType.Rocket)
      {
        GameManager.Instance.CurrentRocket += gift.Value;
      }
      if (gift.Type == PriceType.Swap)
      {
        GameManager.Instance.CurrentSwap += gift.Value;
      }
    }
  }

  void OnBeginMatchFail(EggHolderControl eggHolder, EggControl[] rightOrder)
  {
    var placeOrder = rightOrder;
    int[] _eggHolderArr = new int[placeOrder.Length];
    for (int i = 0; i < placeOrder.Length; i++)
    {
      if (placeOrder[i] == null) { _eggHolderArr[i] = -1; continue; }
      _eggHolderArr[i] = placeOrder[i].ColorValue;
    }

    SaveEggData(_currentNeedSolvingOrderIndex, _eggHolderArr);
    GameManager.Instance.MoveEggTicketsAmount = _moveEggTicketsAmount;

    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(
        "lucky_egg_event_match",
        new Parameter[]{
          new ("ticket", GameManager.Instance.MoveEggTicketsAmount),
          new ("stage", _currentGiftIndex + 1),
          new ("result", "not completed")
        }
      );
    }

    Utility.Print("Ticket: " + GameManager.Instance.MoveEggTicketsAmount + "_Stage: " + (_currentGiftIndex + 1) + "not completed");
  }

  void OnBeginMatchSuccess(EggHolderControl eggHolder)
  {
    Debug.Log("OnBeginMatchSuccess--1");
    GameManager.Instance.MoveEggTicketsAmount = _moveEggTicketsAmount;
    Debug.Log("OnBeginMatchSuccess--2");
    if (IsNeedEggTutorial == true)
    {
      PlayerPrefs.SetInt(KeyString.KEY_DEFAULT_TUTORIAL_GAMEPLAY_EGG, 1);
      IsNeedEggTutorial = false;
    }
    Debug.Log("OnBeginMatchSuccess--3");
    if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
    {
      FirebaseAnalytics.LogEvent(
        "lucky_egg_event_match",
        new Parameter[]{
          new ("ticket", GameManager.Instance.MoveEggTicketsAmount),
          new ("stage", _currentGiftIndex + 1),
          new ("result", "completed")
        }
      );
    }

    Debug.Log("OnBeginMatchSuccess--4");
    Utility.Print("Ticket: " + GameManager.Instance.MoveEggTicketsAmount + "_Stage: " + (_currentGiftIndex + 1) + "completed");

    if (UICanvaEventLuckEggHunt.Instance != null)
    {
      UICanvaEventLuckEggHunt.Instance.UpdateProgress(_currentNeedSolvingOrderIndex);
    }
    Debug.Log("OnBeginMatchSuccess--5");
  }

  void OnCompleteAll()
  {
    UICanvaEventLuckEggHunt.Instance.notiWinAll.SetActive(true);
  }
}