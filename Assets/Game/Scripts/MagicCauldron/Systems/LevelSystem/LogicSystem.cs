using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public struct GiftData
{
  public int amount;
  public PriceType type;
  public int value;
}

/// <summary>
/// LogicSystem
/// </summary>
public partial class LevelSystem : MonoBehaviour
{
  [Header("Logic Datas")]
  readonly int[][] _initOrders = new int[][] {
    new int[3] {0, 1, 2}, // 1
    new int[3] {0, 1, 2}, // 2
    new int[3] {0, 1, 2}, // 3
    new int[4] {0, 1, 2, 3}, // 4
    new int[4] {0, 1, 2, 3}, // 5
    new int[4] {0, 1, 2, 3}, // 6
    new int[5] {0, 1, 2, 3, 4}, // 7
    new int[5] {0, 1, 2, 3, 4}, // 8
    new int[5] {0, 1, 2, 3, 4}, // 9
    new int[6] {0, 1, 2, 3, 4, 5}, // 10
    new int[6] {0, 1, 2, 3, 4, 5}, // 11
    new int[6] {0, 1, 2, 3, 4, 5}, // 12
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 13
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 14
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 15
  };
  readonly int[][] _destinationOrders = new int[][] {
    new int[3] {0, 1, 2}, // 1
    new int[3] {0, 1, 2}, // 2
    new int[3] {0, 1, 2}, // 3
    new int[4] {0, 1, 2, 3}, // 4
    new int[4] {0, 1, 2, 3}, // 5
    new int[4] {0, 1, 2, 3}, // 6
    new int[5] {0, 1, 2, 3, 4}, // 7
    new int[5] {0, 1, 2, 3, 4}, // 8
    new int[5] {0, 1, 2, 3, 4}, // 9
    new int[6] {0, 1, 2, 3, 4, 5}, // 10
    new int[6] {0, 1, 2, 3, 4, 5}, // 11
    new int[6] {0, 1, 2, 3, 4, 5}, // 12
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 13
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 14
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 15
  };
  [Header("Tutorial")]
  public bool IsNeedEggTutorial;
  readonly int[][] _tutorialInitOrders = new int[][] {
    new int[3] {0, 2, 1},
    new int[3] {0, 1, 2},
    new int[3] {0, 1, 2}, // 3
    new int[4] {0, 1, 2, 3}, // 4
    new int[4] {0, 1, 2, 3}, // 5
    new int[4] {0, 1, 2, 3}, // 6
    new int[5] {0, 1, 2, 3, 4}, // 7
    new int[5] {0, 1, 2, 3, 4}, // 8
    new int[5] {0, 1, 2, 3, 4}, // 9
    new int[6] {0, 1, 2, 3, 4, 5}, // 10
    new int[6] {0, 1, 2, 3, 4, 5}, // 11
    new int[6] {0, 1, 2, 3, 4, 5}, // 12
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 13
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 14
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 15
  };
  readonly int[][] _tutorialDestinationOrders = new int[][] {
    new int[3] {0, 1, 2},
    new int[3] {0, 1, 2},
    new int[3] {0, 1, 2}, // 3
    new int[4] {0, 1, 2, 3}, // 4
    new int[4] {0, 1, 2, 3}, // 5
    new int[4] {0, 1, 2, 3}, // 6
    new int[5] {0, 1, 2, 3, 4}, // 7
    new int[5] {0, 1, 2, 3, 4}, // 8
    new int[5] {0, 1, 2, 3, 4}, // 9
    new int[6] {0, 1, 2, 3, 4, 5}, // 10
    new int[6] {0, 1, 2, 3, 4, 5}, // 11
    new int[6] {0, 1, 2, 3, 4, 5}, // 12
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 13
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 14
    new int[7] {0, 1, 2, 3, 4, 5, 6}, // 15
  };
  int _moveEggTicketsAmount;
  public int MoveEggTicketsAmount { get { return _moveEggTicketsAmount; } }
  int _currentNeedSolvingOrderIndex;
  public int CurrentNeedSolvingEggOrderIndex { get { return _currentNeedSolvingOrderIndex; } }
  int _currentGiftIndex;
  public int CurrentGiftIndex { get { return _currentGiftIndex; } }
  readonly GiftData[][] _giftDatas = new GiftData[][] {
    new GiftData[1] {
      new() {type = PriceType.InfinityHeart, value = 15},
    }, // 1
    new GiftData[1] {
      new() {type = PriceType.Coin, value = 50}
    }, // 2
    new GiftData[1] {
      new() {type = PriceType.Refresh, value = 1}
    }, // 3
    new GiftData[1] {
      new() {type = PriceType.Coin, value = 50}
    }, // 4
    new GiftData[2] {
      new() {type = PriceType.Hammer, value = 1},
      new() {type = PriceType.InfinityHeart, value = 15}
    }, // 5
    new GiftData[1] {
      new() {type = PriceType.Coin, value = 100}
    }, // 6
    new GiftData[1] {
      new() {type = PriceType.Rocket, value = 1}
    }, // 7
    new GiftData[1] {
      new() {type = PriceType.InfinityHeart, value = 30}
    }, // 8
    new GiftData[1] {
      new() {type = PriceType.Coin, value = 100}
    }, // 9
    new GiftData[2] {
      new() {type = PriceType.Swap, value = 1},
      new() {type = PriceType.InfinityHeart, value = 15}
    }, // 10
    new GiftData[1] {
      new() {type = PriceType.Coin, value = 150}
    }, // 11
    new GiftData[1] {
      new() {type = PriceType.Refresh, value = 1}
    }, // 12
    new GiftData[1] {
      new() {type = PriceType.InfinityHeart, value = 15}
    }, // 13
    new GiftData[1] {
      new() {type = PriceType.Hammer, value = 1}
    }, // 14
    new GiftData[6] {
      new() {type = PriceType.Coin, value = 250},
      new() {type = PriceType.InfinityHeart, value = 60},
      new() {type = PriceType.Refresh, value = 1},
      new() {type = PriceType.Hammer, value = 1},
      new() {type = PriceType.Rocket, value = 1},
      new() {type = PriceType.Swap, value = 1},
    }, // 15
  };

  void FoundEmptySlotIndex(
    int nearestEmptyIndex,
    EggHolderControl currHolder,
    EggControl touchedEgg,
    float moveToHolderLength,
    float currentAnimDuration,
    ref Sequence seq
  )
  {
    OnBeginFoundEmptySlotIndex(touchedEgg);

    MoveEggToHolder(
      currHolder,
      nearestEmptyIndex,
      touchedEgg,
      out AnimateMovingData<EggControl> needMovingChildData
    );

    VisualizeMoveToHolder(
      needMovingChildData, moveToHolderLength, currentAnimDuration, ref seq
    );
  }

  void MatchingFail(
    EggControl[] rightOrderFromCurrHolder,
    EggControl[] wrongOrderFromCurrHolder,
    EggHolderControl currHolder,
    float removeWrongEggLength,
    float afterRemoveWrongEggDelayLength,
    float spawnedEggMoveToScreenDeltaLength,
    float currentAnimDuration,
    ref Sequence seq
  )
  {
    _isOnMatchingFailInvoke = true;

    OnBeginMatchFail(currHolder, rightOrderFromCurrHolder);

    var removeWrongEggPart1Length = 7 / 10f * removeWrongEggLength;
    var removeWrongEggPart2Length = 3 / 10f * removeWrongEggLength;

    var wrongCount = 0;
    for (int i = 0; i < rightOrderFromCurrHolder.Length; ++i)
      if (rightOrderFromCurrHolder[i] == null) wrongCount++;
    var deltaPart1Length = removeWrongEggPart1Length / rightOrderFromCurrHolder.Length;
    var deltaPart2Length = removeWrongEggPart2Length / wrongCount;

    var lastTimeHolder = itemSystem.GetSecondEggHolder();
    currHolder.SaveWrongOrder(wrongOrderFromCurrHolder);
    if (
      lastTimeHolder != null &&
      HasMatchedBetween(lastTimeHolder.WrongOrder, currHolder.WrongOrder)
    )
    {
      var _countWrongEgg = 0;
      for (int i = 0; i < rightOrderFromCurrHolder.Length; ++i)
      {
        var currOrder = rightOrderFromCurrHolder[i];
        var currOrderIndex = i;
        if (currOrder == null)
        {
          // return egg to spawned position event
          var childEgg = currHolder.ReleaseChildAt(currOrderIndex);
          if (childEgg == null) return;

          childEgg.transform.SetParent(itemSystem.EggParent);
          var dest = itemSystem.GetEggPosFrom(childEgg.SpawnedIndex, wrongCount);

          seq.InsertCallback(
            currentAnimDuration
              + deltaPart2Length * _countWrongEgg,
            () =>
            {
              childEgg.ResetSortingOrder();
            });

          seq.Insert(
            currentAnimDuration
              + deltaPart2Length * _countWrongEgg,
            childEgg.transform
              .DOMove(dest, deltaPart2Length)
              .SetEase(Ease.Linear)
              .OnComplete(() =>
              {
                _isOnMatchingFailInvoke = false;
                OnCompletedReturnEggToSpawnedPosition(childEgg);
              })
          );
          _countWrongEgg++;
        }
      }

      OnBeginDuplicateFailMatched(wrongCount);
      return;
    }

    var countWrongEgg = 0;
    var countRightEgg = 0;
    for (int i = 0; i < rightOrderFromCurrHolder.Length; ++i)
    {
      var currOrder = rightOrderFromCurrHolder[i];
      var currOrderIndex = i;

      // part 1 start
      if (currOrder != null)
      {
        seq.InsertCallback(
          currentAnimDuration
            + deltaPart1Length * currOrderIndex,
          () =>
          {
            // right egg change block color event
            var childEgg = currHolder.GetChildAt(currOrderIndex);
            if (childEgg == null) return;

            OnBeginRightEggShaking(childEgg);
            EffectManager.Instance.SpawnHealBigEfxAt(
              childEgg.transform.position + Vector3.down
            );
            EffectManager.Instance.SpawnSparkUpEfxAt(childEgg.transform.position);
            DOTween.To(
              () => childEgg.transform.position,
              (val) => { childEgg.transform.position = val; },
              childEgg.transform.position + Vector3.up,
              deltaPart1Length / 2f
            ).SetLoops(2, LoopType.Yoyo);
            currHolder.HighlightColorBlockAt(currOrderIndex);
          }
        );
        countRightEgg++;
      }

      if (currOrder == null)
      {
        seq.InsertCallback(
          currentAnimDuration
            + deltaPart1Length * currOrderIndex,
          () =>
          {
            // wrong egg shaking event
            var childEgg = currHolder.GetChildAt(currOrderIndex);
            if (childEgg == null) return;

            OnBeginWrongEggShaking(childEgg);
            EffectManager.Instance.SpawnSparkUpEfxAt(childEgg.transform.position);

            if (childEgg.TryGetComponent<IFeedbackControl>(out var component))
              component.InjectChannel(childEgg.GetInstanceID());
            FeedbackSystem.Instance.PlayRandomRotationShakesAt(
              childEgg.GetInstanceID(), 45, deltaPart1Length
            );
          }
        );

        // part 2 start
        seq.InsertCallback(
          currentAnimDuration
            + removeWrongEggPart1Length
            + deltaPart2Length * countWrongEgg,
          () =>
          {
            // remove wrong egg event
            var childEgg = currHolder.ReleaseChildAt(currOrderIndex);
            if (childEgg == null) return;

            OnBeginEggBreak(childEgg);

            var pos = childEgg.transform.position;
            var eggSplash = EffectManager.Instance.SpawnEggSplashAt(pos);

            var decal = EffectManager.Instance.SpawnDecalAt(pos);
            decal.GetComponent<DecalControl>().SetColorValue(childEgg.ColorValue);
            decal.transform.SetParent(currHolder.DecalParent);

            childEgg.gameObject.SetActive(false);
            itemSystem.RemoveNonPoolItem(childEgg.gameObject);
          }
        );
        countWrongEgg++;
      }
    }

    seq.InsertCallback(
      currentAnimDuration
        + removeWrongEggLength
        + afterRemoveWrongEggDelayLength,
      () =>
      {
        _isOnMatchingFailInvoke = false;

        var pos
          = currHolder.transform.position - new Vector3(0, itemSystem.VerticalMargin, 0);
        var newHolder = itemSystem.CloneEggHolderAt(pos, currHolder);
        newHolder.ClearDecals();

        _upperBound += itemSystem.VerticalMargin;
        var _upperBoundPosition = new float3(0, _upperBound, 0);
        var eggHolderParent = itemSystem.EggHolderParent;

        DOTween.To(
          () => eggHolderParent.transform.position,
          (val) => { eggHolderParent.transform.position = val; },
          _upperBoundPosition,
          .2f
        );

        SpawnAndMoveEggsToScreen(
          wrongOrderFromCurrHolder,
          spawnedEggMoveToScreenDeltaLength
        );

        OnCompleteAllEggsBreak();
      }
    );
  }

  void MatchingSuccess(
    float eggsMoveToDestinationLength,
    float giftPopOutLength,
    float giftMoveToDestinationLength,
    float spawnedEggMoveToScreenDeltaLength,
    EggHolderControl currHolder,
    float currentAnimDuration,
    ref Sequence seq
  )
  {
    _isOnMatchingSuccessInvoke = true;

    // eggs and gift animation
    seq.InsertCallback(
      currentAnimDuration,
      () =>
      {
        OnBeginMatchSuccess(currHolder);
        OnBeginAllEggMoveToDestination();
        // eggsMoveToDestinationLength event: eggs flying to their destination animation
        var eggs = currHolder.FindChilds();
        for (int i = 0; i < eggs.Count; ++i)
          eggs[i].transform.SetParent(itemSystem.EggParent);

        var _seq = DOTween.Sequence();
        var _currentAnimDuration = 0f;

        var _part0Length = 2f / 10f * eggsMoveToDestinationLength;
        var _part1Length = 5f / 10f * eggsMoveToDestinationLength;
        var _part2Length = 2f / 10f * eggsMoveToDestinationLength;
        var _part3Length = 1f / 10f * eggsMoveToDestinationLength;

        for (int i = 0; i < eggs.Count; ++i)
        {
          var egg = eggs[i];
          var currEggIndex = i;
          egg.SetSortingOrder(egg.InitSortingOrder + currEggIndex);
          egg.ChangeToUILayer();

          var destination1 = egg.transform.position + Vector3.up * 5f;
          var endScale1 = new Vector3(1.8f, 1.8f, 0f);
          var destination2 = (Vector3)itemSystem.GiftSpawnedPos + Vector3.up * 2.8f;
          var endScale2 = new Vector3(0.8f, 0.8f, 0f);
          var destination3 = (Vector3)itemSystem.GiftSpawnedPos;
          var endScale3 = new Vector3(0.6f, 0.6f, 0f);

          _seq.InsertCallback(_currentAnimDuration,
            () =>
            {
              EffectManager.Instance.SpawnHealBigEfxAt(
                egg.transform.position + Vector3.down * 1.1f
              );
              DOTween.To(
                () => egg.transform.position,
                (val) => { egg.transform.position = val; },
                egg.transform.position + Vector3.up,
                _part0Length / 2f
              ).SetLoops(2, LoopType.Yoyo);
              currHolder.HighlightColorBlockAt(currEggIndex);
            });

          _seq.Insert(
            _currentAnimDuration
              + _part0Length,
            egg.transform.DOScale(
              endScale1,
              _part0Length
            )
          );
          _seq.Insert(
            _currentAnimDuration
              + _part0Length,
            egg.transform.DOMove(
              destination1,
              _part1Length
            )
          );

          _seq.Insert(
            _currentAnimDuration
              + _part0Length
              + _part1Length,
            egg.transform.DOScale(
              endScale2,
              _part2Length
            )
          );
          _seq.Insert(
            _currentAnimDuration
              + _part0Length
              + _part1Length,
            egg.transform.DOMove(
              destination2,
              _part2Length
            )
          );

          _seq.Insert(
            _currentAnimDuration
              + _part0Length
              + _part1Length
              + _part2Length,
            egg.transform.DOScale(
              endScale3,
              _part3Length
            )
          );
          _seq.Insert(
            _currentAnimDuration
              + _part0Length
              + _part1Length
              + _part2Length,
            egg.transform.DOMove(
              destination3,
              _part3Length
            )
            .OnComplete(() =>
            {
              itemSystem.RemoveNonPoolItem(egg.gameObject);
              egg.gameObject.SetActive(false);
              OnCompleteEachEggMoveToDestination(egg);
            })
          );
        }

        _seq.InsertCallback(
          _currentAnimDuration
            + eggsMoveToDestinationLength,
          () =>
          {
            OnCompleteAllEggsMoveToDestination();
          }
        );

        // gift animation
        var giftData = GetCurrentGiftData();

        _currentGiftIndex++;
        GameManager.Instance.CurrentEggGiftIndex
          = math.min(_currentGiftIndex, _giftDatas.Length);

        var listOfGifts = new List<GiftControl>();
        for (int i = 0; i < giftData.Length; ++i)
        {
          var data = giftData[i];
          var gift = itemSystem.SpawnGiftBy(data.type, data.value);
          listOfGifts.Add(gift);
          gift.gameObject.SetActive(false);
          gift.transform.position = itemSystem.GetGiftPosAt(i, giftData.Length);
          gift.transform.localScale = new Vector3(.4f, .4f, 0);
          gift.ChangeToUILayer();

          // gift move to center screen animation
          var giftPopUpDestination = gift.transform.position + Vector3.down * 7.2f;
          var giftPopUpScaleDest = new Vector3(1.6f, 1.6f, 0);
          var giftPopOutMoveLength = 6f / 10f * giftPopOutLength;
          var giftPopOutShakeLength = 4f / 10f * giftPopOutLength;

          _seq.InsertCallback(
            _currentAnimDuration
              + eggsMoveToDestinationLength,
            () =>
            {
              gift.gameObject.SetActive(true);
            }
          );

          _seq.Insert(
            _currentAnimDuration
              + eggsMoveToDestinationLength,
            gift.transform.DOScale(
              giftPopUpScaleDest,
              giftPopOutMoveLength
            )
          );
          _seq.Insert(
            _currentAnimDuration
              + eggsMoveToDestinationLength,
            gift.transform.DOMove(
              giftPopUpDestination,
              giftPopOutMoveLength
            )
          );

          _seq.Insert(
            _currentAnimDuration
              + eggsMoveToDestinationLength
              + giftPopOutMoveLength,
            gift.transform.DOShakePosition(
              giftPopOutShakeLength,
              1, 10
            )
          );

          // gift move to book animation
          var giftDestination2 = itemSystem.GiftDestination;
          var giftScaleDest2 = new Vector3(.5f, .5f, 0);

          _seq.Insert(
            _currentAnimDuration
              + eggsMoveToDestinationLength
              + giftPopOutLength,
            gift.transform.DOScale(
              giftScaleDest2,
              giftMoveToDestinationLength
            )
          );
          _seq.Insert(
            _currentAnimDuration
              + eggsMoveToDestinationLength
              + giftPopOutLength,
            gift.transform.DOMove(
              giftDestination2,
              giftMoveToDestinationLength
            )
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
              OnCompletedGiftMoveToDestination(gift);
            })
          );
        }

        _seq.InsertCallback(
          _currentAnimDuration
            + eggsMoveToDestinationLength
            + giftPopOutLength
            + giftMoveToDestinationLength,
          () =>
          {
            OnCompletedAllGiftsMoveToDestination(listOfGifts);
          }
        );
      }
    );

    seq.InsertCallback(
      currentAnimDuration
        + eggsMoveToDestinationLength
        + giftPopOutLength
        + giftMoveToDestinationLength,
      () =>
      {
        itemSystem.RemoveEggHolders();
        itemSystem.EggHolderParent.transform.position = float3.zero;
        _upperBound = 0;
        _isOnMatchingSuccessInvoke = false;
      }
    );

    _currentNeedSolvingOrderIndex++;
    GameManager.Instance.CurrentNeedSolvingEggOrderIndex
      = math.min(_currentNeedSolvingOrderIndex, _initOrders.Length);
    if (_currentNeedSolvingOrderIndex > _initOrders.Length - 1)
    {
      seq.InsertCallback(
        currentAnimDuration
          + eggsMoveToDestinationLength
          + giftPopOutLength
          + giftMoveToDestinationLength,
        () =>
        {
          OnCompleteAll();
        }
      );
      return;
    }

    _isOnMatchingSuccessInvoke = true;

    seq.InsertCallback(
      currentAnimDuration
        + eggsMoveToDestinationLength
        + giftPopOutLength
        + giftMoveToDestinationLength,
      () =>
      {
        // spawn and move eggs screen event
        SpawnAndMoveEggsToScreen(
          GetCurrentInitOrder1(),
          spawnedEggMoveToScreenDeltaLength
        );
      }
    );

    seq.InsertCallback(
      currentAnimDuration
        + eggsMoveToDestinationLength
        + giftPopOutLength
        + giftMoveToDestinationLength
        + spawnedEggMoveToScreenDeltaLength,
      () =>
      {
        // end of spawn, move eggs event
        itemSystem.SpawnEggHolderAt(0, GetCurrentDefaultInitOrder());
        _isOnMatchingSuccessInvoke = false;

        itemSystem.ClearTempRemovedData();
      }
   );
  }

  List<EggControl> RestoreToOriginSpawnedOrder(EggControl[] someRandomOrder)
  {
    var listColorValues = new List<EggControl>();
    for (int i = 0; i < someRandomOrder.Length; ++i)
    {
      if (someRandomOrder[i] == null) continue;
      listColorValues.Add(someRandomOrder[i]);
    }
    listColorValues.Sort((a, b) => a.SpawnedIndex - b.SpawnedIndex);
    return listColorValues;
  }

  void SpawnAndMoveEggsToScreen(
    EggControl[] someRandomOrder,
    float spawnedEggMoveToScreenDeltaLength
  )
  {
    var listColorValues = RestoreToOriginSpawnedOrder(someRandomOrder);
    var colorValues = new int[listColorValues.Count];
    for (int i = 0; i < listColorValues.Count; ++i)
      colorValues[i] = listColorValues[i].ColorValue;

    SpawnAndMoveEggsToScreen(colorValues, spawnedEggMoveToScreenDeltaLength);
  }

  void SpawnAndMoveEggsToScreen(int[] colorValues, float spawnedEggMoveToScreenDeltaLength)
  {
    itemSystem.SpawnEggs(colorValues);

    var _seq = DOTween.Sequence();
    var _currentAnimDuration = 0f;
    MoveEggsToScreen(
      out List<AnimateMovingData<EggControl>> needMoveEggToScreenDatas
    );
    VisualizeMoveEggsToScreen(
      needMoveEggToScreenDatas,
      spawnedEggMoveToScreenDeltaLength,
      _currentAnimDuration,
      ref _seq
    );
  }

  void MoveEggsToScreen(out List<AnimateMovingData<EggControl>> datas)
  {
    datas = new List<AnimateMovingData<EggControl>>();

    var eggs = itemSystem.CollectSpawnedEggs();

    for (int i = 0; i < eggs.Count; ++i)
    {
      var egg = eggs[i];
      var desPos = itemSystem.GetEggPosFrom(i, eggs.Count);
      datas.Add(
        new AnimateMovingData<EggControl>()
        {
          needMovingObj = egg,
          fromPosition = egg.transform.position,
          destination = desPos
        });
    }
  }

  void MoveEggToHolder(
    EggHolderControl currHolder,
    int index,
    EggControl egg,
    out AnimateMovingData<EggControl> data
  )
  {
    currHolder.AdoptChildAt(index, egg);

    // moving animation
    data = new AnimateMovingData<EggControl>()
    {
      needMovingObj = egg,
      needMovingObjParent = currHolder.gameObject,
      fromPosition = egg.transform.position,
      destination
        = currHolder.transform.position + (Vector3)currHolder.GetEggLocalPosAt(index)
    };
  }

  int[] GetCurrentDefaultInitOrder()
  {
    var defaultOrder = GetCurrentInitOrder1();
    for (int i = 0; i < defaultOrder.Length; ++i) defaultOrder[i] = -1;
    return defaultOrder;
  }

  void ShuffleOrders()
  {
    for (int i = 0; i < _initOrders.Length; ++i)
    {
      var initOrder = _initOrders[i];
      var _initOrder = ShuffleValueFor(initOrder);
      _initOrders[i] = _initOrder;
    }
    for (int i = 0; i < _destinationOrders.Length; ++i)
    {
      var desOrder = _destinationOrders[i];
      var _desOrder = ShuffleValueFor(desOrder);
      _destinationOrders[i] = _desOrder;
    }
  }

  int[] GetCurrentInitOrder()
  {
    if (_currentNeedSolvingOrderIndex > _initOrders.Length - 1) return _initOrders[^1];
    if (IsNeedEggTutorial)
      return _tutorialInitOrders[_currentNeedSolvingOrderIndex];
    return _initOrders[_currentNeedSolvingOrderIndex];
  }

  int[] GetCurrentInitOrder1()
  {
    if (IsNeedEggTutorial) return _tutorialInitOrders[_currentNeedSolvingOrderIndex];
    var data = GetDataAt(_currentNeedSolvingOrderIndex);
    return data.InitOrders;
  }

  int[] GetCurrentDestinationOrder()
  {
    if (_currentNeedSolvingOrderIndex > _destinationOrders.Length - 1)
      return _destinationOrders[^1];
    if (IsNeedEggTutorial)
      return _tutorialDestinationOrders[_currentNeedSolvingOrderIndex];
    return _destinationOrders[_currentNeedSolvingOrderIndex];
  }

  int[] GetCurrentDestinationOrder1()
  {
    if (IsNeedEggTutorial) return _tutorialDestinationOrders[_currentNeedSolvingOrderIndex];
    var data = GetDataAt(_currentNeedSolvingOrderIndex);
    return data.DestinationOrders;
  }

  int[] GetCurrentEggOnScreen()
  {
    int[] currentEggHolder = GetCurrentEggHolderOrder();
    int[] initOrder = GetCurrentInitOrder1();

    int[] currentEggOnScreen = new int[initOrder.Length];
    for (int i = 0; i < currentEggOnScreen.Length; i++) currentEggOnScreen[i] = initOrder[i];

    for (int i = 0; i < currentEggHolder.Length; i++)
    {
      if (currentEggHolder[i] != -1)
      {
        for (int j = 0; j < currentEggOnScreen.Length; j++)
        {
          if (currentEggOnScreen[j] == currentEggHolder[i])
            currentEggOnScreen[j] = -1;
        }
      }
    }
    return currentEggOnScreen;
  }

  int[] GetCurrentEggHolderOrder()
  {
    if (IsNeedEggTutorial)
    {
      int[] currentEggHolder = new int[_tutorialInitOrders[_currentNeedSolvingOrderIndex].Length];
      for (int i = 0; i < currentEggHolder.Length; i++) currentEggHolder[i] = -1;
      return currentEggHolder;
    }
    var data = GetDataAt(_currentNeedSolvingOrderIndex);
    return data.CurrentEggHolder;
  }

  GiftData[] GetCurrentGiftData()
  {
    if (_currentGiftIndex > _giftDatas.Length - 1) return _giftDatas[^1];
    return _giftDatas[_currentGiftIndex];
  }

  int[] ShuffleValueFor(int[] originalArray)
  {
    var list = new List<int>(originalArray);
    for (int i = 0; i < originalArray.Length; ++i)
    {
      var idx = UnityEngine.Random.Range(0, list.Count);
      originalArray[i] = list[idx];
      list.RemoveAt(idx);
    }
    return originalArray;
  }

  bool HasMatchedBetween(
    in EggControl[] order1,
    in EggControl[] order2
  )
  {
    if (order1.Length != order2.Length) return false;
    for (int i = 0; i < order1.Length; ++i)
    {
      if (order1[i] == null && order2[i] != null) return false;
      if (order1[i] != null && order2[i] == null) return false;
      if (order1[i] == null && order2[i] == null) continue;
      if (order1[i].ColorValue != order2[i].ColorValue) return false;
    }
    return true;
  }

  bool HasMatchedBetween(
    in int[] destinationOrder,
    in EggControl[] selectedOrder,
    out EggControl[] rightOrder,
    out EggControl[] wrongOrder
  )
  {
    rightOrder = new EggControl[selectedOrder.Length];
    for (int i = 0; i < rightOrder.Length; ++i) rightOrder[i] = null;
    wrongOrder = new EggControl[selectedOrder.Length];
    for (int i = 0; i < wrongOrder.Length; ++i) wrongOrder[i] = null;

    var count = 0;
    for (int i = 0; i < selectedOrder.Length; ++i)
    {
      if (selectedOrder[i].ColorValue == destinationOrder[i])
      {
        rightOrder[i] = selectedOrder[i];
        count++;
      }
      else
      {
        wrongOrder[i] = selectedOrder[i];
      }
    }
    return count == selectedOrder.Length;
  }

  // test
  public void SetCurrentSolvingoder()
  {
    PlayerPrefs.SetInt(KeyString.KEY_CURRENT_NEED_SOLVING_EGG_ORDER_INDEX, 13);
    _currentNeedSolvingOrderIndex = 13;
  }
}