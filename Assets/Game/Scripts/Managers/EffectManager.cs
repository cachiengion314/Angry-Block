using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using DG.Tweening;
using Spine.Unity;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class EffectManager : MonoBehaviour
{
  public static EffectManager Instance { get; private set; }

  [Header("Internal dependencies")]
  [SerializeField] GameObject lidPref;
  [SerializeField] Animator packingAnimator;
  [SerializeField] SkeletonAnimation boomEfx;
  [SerializeField] ParticleSystem woodSplashEfx;
  [SerializeField] ParticleSystem iceSplashEfx;
  [SerializeField] ParticleSystem goldSplashEfx;
  [SerializeField] ParticleSystem waterSplashEfx;
  [SerializeField] ParticleSystem coffeeSplashEfx;
  [SerializeField] ParticleSystem cupBreakSplashEfx;
  [SerializeField] CoinsParticle coinPsPrefab;
  [SerializeField] ParticleSystem hammerExplosiveEfx;
  [SerializeField] GameObject rectanglePref;
  public SpriteRenderer[] TrayShadows { get; private set; }
  [SerializeField] GameObject arrowPref;
  public GameObject[] Arrows { get; private set; }
  [SerializeField] SpriteRenderer darkImg;
  [SerializeField] ParticleSystem trailLightPref;
  [SerializeField] SkeletonAnimation hammerSkePref;
  [SerializeField] SkeletonAnimation rocketSkePref;
  [SerializeField] Transform tableDepotImg;
  [Header("Prefabs")]
  [SerializeField] GameObject coinsPrefab;
  [SerializeField] GameObject ticketPrefab;
  [SerializeField] GameObject unlockedReversePrefab;
  [SerializeField] GameObject unlockedHammerPrefab;
  [SerializeField] GameObject unlockedRocketPrefab;
  [SerializeField] GameObject unlockedSwapPrefab;
  [SerializeField] GameObject flashPrefab;
  [Header("Settings")]
  public bool IsDarkImgOn;
  [Range(0, 1)]
  [SerializeField] float shadowIntensitive;
  [Range(0, 2)]
  [SerializeField] float changeTime;
  [Header("Events")]
  Action<float3> onMoveTrailLightToTargetCompleted;
  [Header("Particles Image")]
  [SerializeField] ParticleImage particleCoins;
  [Header("Managers")]
  GameObject[] _rectangleImgs;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  IEnumerator Start()
  {
    yield return null;

    if (SceneManager.GetActiveScene().name == "Gameplay"
      || SceneManager.GetActiveScene().name == "LevelEditor"
      || SceneManager.GetActiveScene().name == "GameplayTool")
    {
      SpawnArrows();
      TurnOffDarkImg();
      SpawnRectangleShadows();
    }
  }

  void SpawnRectangleShadows()
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    _rectangleImgs = new GameObject[trayGrid.Grid.Length];

    for (int i = 0; i < trayGrid.Grid.Length; ++i)
    {
      var worldPos = trayGrid.ConvertIndexToWorldPos(i);
      var clone = Instantiate(rectanglePref, tableDepotImg);
      clone.transform.position = worldPos;

      var _y = -1 * (trayGrid.ConvertIndexToGridPos(i).y + 2) - 10;
      clone.GetComponentInChildren<SpriteRenderer>().sortingOrder = _y;

      _rectangleImgs[i] = clone;

      if (ItemManager.Instance.HasWoodBoxAt(worldPos))
      {
        if (ItemManager.Instance.WoodBoxesGrid[i] == 111)
          clone.GetComponentInChildren<SpriteRenderer>().color = new Color(0.85f, 0.8f, 0.8f, 1);
      }
    }
  }

  void SpawnArrows()
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    Arrows = new GameObject[trayGrid.GridSize.x];

    for (int x = 0; x < trayGrid.GridSize.x; ++x)
    {
      var worldPos = trayGrid.ConvertGridPosToWorldPos(new int2(x, 0)) + new float3(0, -1.0f, 0);
      var clone = Instantiate(arrowPref, tableDepotImg);
      var ren = clone.GetComponentInChildren<SpriteRenderer>();
      clone.transform.position = worldPos;
      clone.GetComponent<ArrowControl>().InjectIndex(x);

      Color color = ren.color;
      color.a = 0;
      ren.color = color;

      clone.SetActive(false);
      Arrows[x] = clone;
    }
  }

  public void TurnOnArrows()
  {
    for (int i = 0; i < Arrows.Length; ++i)
    {
      var clone = Arrows[i];
      var ren = clone.GetComponentInChildren<SpriteRenderer>();
      clone.SetActive(true);
      Color color = ren.color;
      if (color.a == 1) continue;
      AnimationManager.Instance.ValueWith(
          ren.gameObject, 1, 0f,
          .3f, (float val) =>
          {
            Color color = ren.color;
            color.a = val;
            ren.color = color;
          }
      ).setEaseOutBack();
    }
  }

  public void TurnOffArrows()
  {
    for (int i = 0; i < Arrows.Length; ++i)
    {
      var clone = Arrows[i];
      var ren = Arrows[i].GetComponentInChildren<SpriteRenderer>();
      if (!clone.activeSelf) continue;
      Color color = ren.color;

      AnimationManager.Instance.ValueWith(
        ren.gameObject, 0, 1,
        .3f, (float val) =>
        {
          Color color = ren.color;
          color.a = val;
          ren.color = color;
        }
      )
      .setEaseOutBack()
      .setOnComplete(() =>
      {
        clone.SetActive(false);
      });
    }
  }

  public void TurnOnDarkImg()
  {
    IsDarkImgOn = true;
    darkImg.gameObject.SetActive(true);
  }

  public void TurnOffDarkImg()
  {
    IsDarkImgOn = false;
    darkImg.gameObject.SetActive(false);
  }

  public void SpawnHammerExplosiveAt(float3 pos)
  {
    var _hammerExplosivePfx = Instantiate(hammerExplosiveEfx, pos, hammerExplosiveEfx.transform.rotation);
    _hammerExplosivePfx.Play();
  }

  public SkeletonAnimation SpawnExplosiveAt(float3 pos)
  {
    var _boomEfx = Instantiate(boomEfx, pos, boomEfx.transform.rotation);
    return _boomEfx;
  }

  public void MoveTrailLightTo(float3 desPos, float3 fromPos, Color color, Action onCompleted = null)
  {
    var clone = Instantiate(trailLightPref);
    if (clone.TryGetComponent(out SmokeTrail smoke))
    {
      smoke.ChangeColorTo(color);
    }
    clone.Play();
    clone.transform.position = fromPos;

    var randDestPos = HoangNam.Utility.GetRandomDir() * 2f;

    AnimationManager.Instance.MoveTo(new float3(randDestPos.x, randDestPos.y, 0), fromPos, clone.gameObject, .2f, () =>
    {
      AnimationManager.Instance.MoveTo(desPos, new float3(randDestPos.x, randDestPos.y, 0), clone.gameObject, .4f, () =>
        {
          onCompleted?.Invoke();
          onMoveTrailLightToTargetCompleted?.Invoke(desPos);
          var water = SpawnWaterSplashAt(desPos);
          water.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", color);
          Destroy(clone.gameObject);
        });
    });
  }

  public void MoveTrailLightWithoutSplashTo(float3 desPos, float3 fromPos, Color color, Action onCompleted = null)
  {
    var clone = Instantiate(trailLightPref);
    if (clone.TryGetComponent(out SmokeTrail smoke))
    {
      smoke.ChangeColorTo(color);
    }
    clone.Play();
    clone.transform.position = fromPos;

    var randDestPos = HoangNam.Utility.GetRandomDir() * 2f;

    AnimationManager.Instance.MoveTo(new float3(randDestPos.x, randDestPos.y, 0), fromPos, clone.gameObject, .35f, () =>
    {
      AnimationManager.Instance.MoveTo(desPos, new float3(randDestPos.x, randDestPos.y, 0), clone.gameObject, .75f, () =>
        {
          onCompleted?.Invoke();
          onMoveTrailLightToTargetCompleted?.Invoke(desPos);
          // var water = SpawnWaterSplashAt(desPos);
          // water.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", color);
          Destroy(clone.gameObject);
        });
    });
  }

  // Use if want light only move up from frompos, then move random to desPos
  public void MoveTrailLightObstacleTo(float3 desPos, float3 fromPos, Color color, Action onCompleted = null)
  {
    var clone = Instantiate(trailLightPref);
    if (clone.TryGetComponent(out SmokeTrail smoke))
    {
      smoke.ChangeColorTo(color);
    }
    clone.Play();
    clone.transform.position = fromPos;

    var randDestPos = new float2(fromPos.x, fromPos.y) + new float2(UnityEngine.Random.Range(0, 1f), 1) * 2;

    AnimationManager.Instance.MoveTo(new float3(randDestPos.x, randDestPos.y, 0), fromPos, clone.gameObject, .2f, () =>
    {
      AnimationManager.Instance.MoveTo(desPos, new float3(randDestPos.x, randDestPos.y, 0), clone.gameObject, .4f, () =>
        {
          onCompleted?.Invoke();
          onMoveTrailLightToTargetCompleted?.Invoke(desPos);
          var water = SpawnWaterSplashAt(desPos);
          water.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", color);
          Destroy(clone.gameObject);
        });
    });
  }

  public void MoveTrailLightFridgeCupTo(float3 desPos, float3 fromPos, float duration, Color color, Action onCompleted = null)
  {
    var clone = Instantiate(trailLightPref);
    if (clone.TryGetComponent(out SmokeTrail smoke))
    {
      smoke.ChangeColorTo(color);
    }
    clone.Play();
    clone.transform.position = fromPos;

    Sequence seq = DOTween.Sequence();
    var currentTimeAnim = 0f;
    var timeMoveTrail = duration;

    seq.Insert(
      currentTimeAnim,
      clone.transform.DOJump(
        desPos,
        -1,
        1,
        timeMoveTrail
      )
    );

    currentTimeAnim += timeMoveTrail;
    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        onCompleted?.Invoke();
        onMoveTrailLightToTargetCompleted?.Invoke(desPos);
        Destroy(clone.gameObject);
      }
    );
  }

  public void EmissingCoinsTo(Transform targetPos, int amount, Transform fromPos = null, Action onCompleted = null)
  {
    var coin = Instantiate(coinPsPrefab, fromPos.position, Quaternion.identity);
    coin.EmissingTo(targetPos, amount, fromPos, () =>
    {
      onCompleted?.Invoke();
      Destroy(coin.gameObject);
    });
  }

  public void EmissingCoinsWithParticleTo(Transform targetPos, int amount, Transform fromPos = null, Action onCompleted = null)
  {
    var parent = LobbyPanel.Instance.transform;

    if (SceneManager.GetActiveScene().name == "Gameplay")
    {
      parent = GameplayPanel.Instance.transform;
    }

    var particleCoin = Instantiate(particleCoins, fromPos.position, Quaternion.identity, parent);
    particleCoin.attractorTarget = targetPos;
    particleCoin.rateOverTime = amount;
    particleCoin.Play();
    particleCoin.duration = 0.5f;

    DOVirtual.DelayedCall(1.2f,
      () =>
      {
        SoundManager.Instance.PlayCoinReceiveSfx();
      }
    );

    DOVirtual.DelayedCall(2,
      () =>
      {
        onCompleted?.Invoke();
        Destroy(particleCoin.gameObject);
      }
    );
  }

  public void SpawnHammerAt(float3 pos, out float existingTime)
  {
    existingTime = .11f;
    var hammerSke = Instantiate(hammerSkePref, pos, Quaternion.identity);

    var timeDuration = hammerSke.Skeleton.Data.FindAnimation("Hammer_use").Duration;
    LeanTween.delayedCall(hammerSke.gameObject, timeDuration, () =>
    {
      Destroy(hammerSke);
    });
  }

  public void SpawnRocketAt(float3 fromPos, float3 toPos, out float existingTime)
  {
    var rocket = Instantiate(rocketSkePref, fromPos, Quaternion.identity);
    rocket.transform.position = fromPos + (float3)Vector3.down * 3.5f;

    var anim = rocket.Skeleton.Data.FindAnimation("Start");
    existingTime = anim.Duration;
    Destroy(rocket.gameObject, existingTime);
  }

  public ParticleSystem SpawnWaterSplashAt(float3 pos)
  {
    return Instantiate(waterSplashEfx, pos, Quaternion.identity);
  }

  public ParticleSystem SpawnCoffeeSplashAt(float3 pos)
  {
    return Instantiate(coffeeSplashEfx, pos, Quaternion.identity);
  }

  public ParticleSystem SpawnCupBreakSplashAt(float3 pos)
  {
    return Instantiate(cupBreakSplashEfx, pos, Quaternion.identity);
  }

  public void SpawnGoldSplashAt(float3 pos)
  {
    var splash = Instantiate(goldSplashEfx, pos, Quaternion.identity);
    splash.Play();
  }

  public void SpawnWoodSplashAt(float3 pos)
  {
    var splash = Instantiate(woodSplashEfx, pos, Quaternion.identity);
    splash.Play();
  }

  public void SpawnIceSplashAt(float3 pos)
  {
    var splash = Instantiate(iceSplashEfx, pos, Quaternion.identity);
    splash.Play();
  }

  public Animator SpawnPackingAnimAt(float3 pos)
  {
    var animator = Instantiate(packingAnimator, pos, Quaternion.identity);
    return animator;
  }

  public GameObject SpawnLidAt(float3 pos)
  {
    var lid = Instantiate(lidPref, pos, Quaternion.identity);
    return lid;
  }

  // PowerItem 
  public void SpawnEfxUnlockedPowerItemAt(float3 pos, int itemIndex, int _amount = 1)
  {
    SoundManager.Instance.PlayBalloonItemSfx();
    GameObject[] objs = new GameObject[_amount];
    for (int i = 0; i < _amount; ++i)
    {
      objs[i] = SpawnUnlockedPowerItemAt(pos, itemIndex);
    }

    for (int i = 0; i < _amount; ++i)
    {
      var obj = objs[i];
      var _time = .2f * (i + 1);
      LeanTween.scale(obj, new Vector3(1.1f, 1.2f, 0), _time)
        .setOnComplete(() =>
        {
          // SoundManager.Instance.PlayClaimNotCoinSfx();
          var img = PowerItemPanel.Instance.GetPowerItemImgFor(itemIndex);
          if (img == null) return;
          var seq = LeanTween.sequence();
          seq.insert(
            LeanTween.scale(obj, new Vector3(.8f, .8f, 0), .3f).setEaseInBounce()
          );
          seq.insert(
            LeanTween.move(obj, img.transform.position, .5f)
            .setEaseInSine()
            .setOnComplete(() =>
            {
              PowerItemPanel.Instance.ReceiveFreeItemFrom(itemIndex);
              Destroy(obj);
              SoundManager.Instance.PlayClaimItemSfx();
            })
          );
        });
    }
  }

  public GameObject SpawnUnlockedPowerItemAt(float3 pos, int itemIndex)
  {
    GameObject obj = null;
    if (itemIndex == 0)
      obj = Instantiate(unlockedReversePrefab, pos, Quaternion.identity);
    else if (itemIndex == 1)
      obj = Instantiate(unlockedHammerPrefab, pos, Quaternion.identity);
    else if (itemIndex == 2)
      obj = Instantiate(unlockedRocketPrefab, pos, Quaternion.identity);
    else if (itemIndex == 3)
      obj = Instantiate(unlockedSwapPrefab, pos, Quaternion.identity);
    return obj;
  }

  // Coin
  public void SpawnEfxCoinAt(float3 pos, int _amount = 1, Action _oncompleted = null, int _amountCoinDefault = -1, bool _isShowTxt = true)
  {
    SoundManager.Instance.PlayBalloonItemSfx();
    GameObject[] objs = new GameObject[_amount];
    for (int i = 0; i < _amount; ++i)
    {
      objs[i] = SpawnCoinAt(pos);
    }

    for (int i = 0; i < _amount; ++i)
    {
      var obj = objs[i];
      var _time = .2f * (i + 1);
      LeanTween.scale(obj, new Vector3(1.1f, 1.2f, 0), _time)
        .setOnComplete(() =>
        {
          var seq = LeanTween.sequence();
          seq.insert(
            LeanTween.scale(obj, new Vector3(.8f, .8f, 0), .3f).setEaseInBounce()
          );
          seq.insert(
            LeanTween.delayedCall(gameObject, 0, () =>
            {
              Destroy(obj);
              SoundManager.Instance.PlayCoinExplodeSfx();

              var _amountCoin = UnityEngine.Random.Range(20, 101);

              if (_amountCoinDefault != -1) _amountCoin = _amountCoinDefault;
              Instance.EmissingCoinsWithParticleTo(
                ShowPanel.Instance.GlobalCoinImg.transform, 10, transform, () =>
                {
                  SoundManager.Instance.PlayCoinReceiveSfx();

                  if (_isShowTxt)
                  {
                    GameManager.Instance.CurrentCoin += _amountCoin;
                  }
                }
              );

              if (_isShowTxt)
              {
                LeanTween.delayedCall(gameObject, 0.2f, () =>
                {
                  ShowPanel.Instance.ShowTextAt(
                    transform.position, "+" + _amountCoin.ToString(), Color.yellow, null, 1.1f
                  );
                  _oncompleted?.Invoke();
                });
              }
            })
          );
        });
    }
  }

  public GameObject SpawnCoinAt(float3 pos)
  {
    GameObject obj = Instantiate(coinsPrefab, pos, Quaternion.identity);
    return obj;
  }

  public void SpawnEfxTicketAt(float3 pos, int _amount = 1, Action _oncompleted = null)
  {
    SoundManager.Instance.PlayBalloonItemSfx();
    GameObject[] objs = new GameObject[_amount];
    for (int i = 0; i < _amount; ++i)
    {
      objs[i] = SpawnTicketAt(pos);
    }

    for (int i = 0; i < _amount; ++i)
    {
      var obj = objs[i];
      var _time = .2f * (i + 1);
      LeanTween.scale(obj, new Vector3(1.1f, 1.2f, 0), _time)
        .setOnComplete(() =>
        {
          var seq = LeanTween.sequence();
          seq.insert(
            LeanTween.scale(obj, new Vector3(.8f, .8f, 0), .5f)
              .setEaseInBounce()
              .setOnComplete(() =>
              {
                Destroy(obj);
              })
          );
        });
    }
  }

  public GameObject SpawnTicketAt(float3 pos)
  {
    GameObject obj = Instantiate(ticketPrefab, pos, Quaternion.identity);
    return obj;
  }

  public GameObject SpawnFlashStarAt(float3 pos)
  {
    var obj = Instantiate(flashPrefab, pos, Quaternion.identity);
    return obj;
  }

  public IEnumerator IEDelayShow(Button button, float timeDelay)
  {
    button.interactable = false;
    yield return new WaitForSeconds(timeDelay);
    button.interactable = true;

    yield break;
  }

  public void TurnOnRectangleShadowAt(float3 pos)
  {
    var trayGrid = ItemManager.Instance.TrayGrid;
    if (trayGrid.IsPosOutsideAt(pos)) return;

    var index = trayGrid.ConvertWorldPosToIndex(pos);
    if (index < 0 || index > _rectangleImgs.Length - 1) return;

    var rect = _rectangleImgs[index];
    if (rect == null) return;
    var rectRenderer = rect.GetComponentInChildren<SpriteRenderer>();
    if (rectRenderer == null) return;

    if (ItemManager.Instance.TrayGrid.GetValueAt(pos) == 999) return;
    if (ItemManager.Instance.TrayGrid.GetValueAt(pos) == 1000) return;
    if (ItemManager.Instance.HasCoffeeBoardAt(pos)) return;
    if (ItemManager.Instance.HasCoverLetAt(pos)) return;
    if (ItemManager.Instance.HasCupBoardAt(pos)) return;
    if (ItemManager.Instance.HasCurtainLayerAt(pos)) return;
    if (ItemManager.Instance.HasGiftBoxAt(pos)) return;
    if (ItemManager.Instance.HasGoldenTrayAt(pos)) return;
    if (ItemManager.Instance.HasGrassAt(pos)) return;
    if (ItemManager.Instance.HasGrillerAt(pos)) return;
    if (ItemManager.Instance.HasMachineCreamAt(pos)) return;
    if (ItemManager.Instance.HasMoneyBagAt(pos)) return;
    if (ItemManager.Instance.HasPlantPotAt(pos)) return;
    if (ItemManager.Instance.HasWoodBoxAt(pos)) return;
    if (ItemManager.Instance.HasVideoAdsAt(pos)) return;
    if (ItemManager.Instance.HasMagicNestAt(pos)) return;
    if (ItemManager.Instance.HasFlowerPotAt(pos)) return;
    if (ItemManager.Instance.HasLeavesFlowerAt(pos)) return;
    if (ItemManager.Instance.HasBeverageFridgeAt(pos)) return;

    DOTween.Kill(rectRenderer);
    rectRenderer.DOColor(Color.green, .3f);

    var child = rect.transform.GetChild(0);
    DOTween.Kill(child);
    child.DOLocalMoveY(.25f, .3f);
  }

  public void TurnOffAllRectangleShadows(int excludeIndex = -1)
  {
    for (int i = 0; i < _rectangleImgs.Length; ++i)
    {
      if (i == excludeIndex) continue;

      var rect = _rectangleImgs[i];
      if (rect == null) continue;
      var rectRenderer = rect.GetComponentInChildren<SpriteRenderer>();
      if (rectRenderer == null) continue;
      if (rectRenderer.color == Color.white) continue;
      if (rectRenderer.color == new Color(0.85f, 0.8f, 0.8f, 1)) continue;

      DOTween.Kill(rectRenderer);
      rectRenderer.DOColor(Color.white, .3f);

      var child = rect.transform.GetChild(0);
      DOTween.Kill(child);
      child.DOLocalMoveY(0f, .3f);
    }
  }

  public GameObject GetRectangleShadowAt(float3 pos)
  {
    var index = ItemManager.Instance.TrayGrid.ConvertWorldPosToIndex(pos);
    if (index < 0 || index > ItemManager.Instance.TrayGrid.Grid.Length - 1) return null;

    if (_rectangleImgs != null && _rectangleImgs.Length > 0)
    {
      return _rectangleImgs[index];
    }

    return null;
  }
}
