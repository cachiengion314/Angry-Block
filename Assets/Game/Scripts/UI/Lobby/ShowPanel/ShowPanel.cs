using System;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowPanel : MonoBehaviour
{
  public static ShowPanel Instance { get; private set; }

  [Header("Internal Dependencies")]
  [SerializeField] TMP_Text showText; // for show up some text
  [SerializeField] Image describeImg; // for image describe by text
  [SerializeField] RectTransform imageBlock;
  [SerializeField] Image darkBg;
  [SerializeField] Image lightImg;
  [SerializeField] Image showImg;
  [SerializeField] TMP_Text topText;
  [SerializeField] TMP_Text bottomText;
  [SerializeField] Button claimBtn;
  [SerializeField] Button claimX2Btn;
  [SerializeField] TMP_Text globalCoinText;
  public TMP_Text GlobalCoinText { get { return globalCoinText; } }
  [SerializeField] Image globalCoinImg;
  public Image GlobalCoinImg { get { return globalCoinImg; } }
  [SerializeField] RectTransform coinBlock;
  public RectTransform CoinBlock { get { return coinBlock; } }

  // v1.4
  [SerializeField] RectTransform contents;
  [SerializeField] RectTransform reward;
  [SerializeField] Button homeBtn;

  // v1.5
  [SerializeField] GameObject[] textTutorials;

  [Header("Settings")]
  Transform coinDesPos;
  RewardData currentPriceData;
  Action<RewardData> claimCallback;
  Action<RewardData> claimX2Callback;

  [Header("Coffee Break")]
  [SerializeField] RectTransform coffeeBreakObj;
  [SerializeField] RectTransform coffeeBreakIcon;
  [SerializeField] TMP_Text rewardTxt;
  [SerializeField] TMP_Text amountCoinTxt;
  [SerializeField] RectTransform coinIconImg;

  private void Start()
  {
    if (Instance == null)
    {
      Instance = this;
      // First time ever event
      InitUIBeginState();
    }
    else Destroy(gameObject);

    HideAllDemo();
  }

  void InitUIBeginState()
  {
    showText.transform.localScale = Vector3.zero;
    imageBlock.gameObject.SetActive(false);
    lightImg.gameObject.SetActive(false);
    showImg.rectTransform.localScale = Vector3.zero;

    globalCoinText.text = GameManager.Instance.CurrentCoin.ToString("#,##0");
  }

  public ShowPanel InjectPriceData(RewardData priceData, Transform coinDesPos, Action<RewardData> claimCallback = null, Action<RewardData> claimX2Callback = null)
  {
    currentPriceData = priceData;
    this.claimCallback = claimCallback;
    this.claimX2Callback = claimX2Callback;
    this.coinDesPos = coinDesPos;
    return this;
  }

  public void Claim()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    LeanTween.delayedCall(gameObject, .2f, () =>
    {
      CloseImg();
      if (currentPriceData && currentPriceData.Type == PriceType.Coin)
      {
        EffectManager.Instance.EmissingCoinsWithParticleTo(
            coinDesPos,
            10, showImg.transform,
            () =>
            {
              claimCallback?.Invoke(currentPriceData);
            }
        );
        return;
      }
      claimCallback?.Invoke(currentPriceData);
    });
  }

  public void ClaimX2()
  {
    SoundManager.Instance.PlayPressBtnSfx();
    claimX2Btn.interactable = false;

    LevelPlayAds.Instance.ShowRewardedAd(() =>
    {
      claimX2Btn.interactable = true;
      CloseAdEarnedReward();
    },
    "Claimx2",
    () =>
    {
      ShowNotify("ADS NOT READY");
      StartCoroutine(EffectManager.Instance.IEDelayShow(claimX2Btn, 1.5f));
    }
    );
  }

  public void ShowImgWith(Sprite sprite, string topDescription, string bottomDescription, Color textColor, bool isNeedClaim = true)
  {
    SoundManager.Instance.PlayClaimDailyRewardSfx();
    showImg.sprite = sprite;

    LeanTween
        .scale(showImg.gameObject, Vector3.one, .2f)
        .setEaseInCirc();
    imageBlock?.gameObject.SetActive(true);

    topText.text = topDescription;
    topText.color = textColor;
    bottomText.color = textColor;
    bottomText.text = bottomDescription;
    lightImg.gameObject.SetActive(true);
    LeanTween.rotateAround(lightImg.gameObject, -Vector3.forward, 360, 8.8f).setLoopClamp();

    if (!isNeedClaim)
    {
      claimBtn.gameObject.SetActive(false);
      claimX2Btn.gameObject.SetActive(false);
      LeanTween.delayedCall(gameObject, 1.3f, Claim);
    }
    else
    {
      claimBtn.gameObject.SetActive(true);
      claimX2Btn.gameObject.SetActive(true);
    }
  }

  public void ShowRewardOfferWith(Sprite sprite, string topDescription, string bottomDescription, Color textColor, float timeDuration, PriceType priceType, int amount)
  {
    SoundManager.Instance.PlayClaimDailyRewardSfx();
    showImg.sprite = sprite;

    // LeanTween
    //     .scale(showImg.gameObject, Vector3.one * 1.5f, .2f)
    //     .setEaseInCirc();
    showImg.transform.localScale = Vector3.one * 1.5f;
    showImg.GetComponent<CanvasGroup>().alpha = 0;
    showImg.GetComponent<CanvasGroup>().DOFade(1, timeDuration);
    imageBlock?.gameObject.SetActive(true);
    darkBg.gameObject.SetActive(false);

    topText.text = topDescription;
    topText.color = textColor;
    bottomText.color = textColor;
    bottomText.text = bottomDescription;
    lightImg.gameObject.SetActive(false);
    LeanTween.rotateAround(lightImg.gameObject, -Vector3.forward, 360, 8.8f).setLoopClamp();

    claimBtn.gameObject.SetActive(false);
    claimX2Btn.gameObject.SetActive(false);

    DOVirtual.DelayedCall(timeDuration,
      () =>
      {
        CloseLuckyRewardImg(amount, priceType);
      }
    );
  }

  public void ShowImgsWith(Sprite[] sprite, string[] topDescription, string[] bottomDescription, Color[] textColor, bool isNeedClaim = true)
  {
    SoundManager.Instance.PlayClaimDailyRewardSfx();
    for (int i = 0; i < sprite.Length - 1; i++)
    {
      GameObject newReward = Instantiate(reward.gameObject, contents);
    }

    for (int i = 0; i < sprite.Length; i++)
    {
      var reward = contents.GetChild(i);
      var rewardImg = reward.GetComponent<Image>();
      var rewardTopText = reward.GetChild(0).GetComponent<TextMeshProUGUI>();
      var rewardBottomText = reward.GetChild(1).GetComponent<TextMeshProUGUI>();

      rewardImg.sprite = sprite[i];
      LeanTween
          .scale(reward.gameObject, Vector3.one, .2f)
          .setEaseInCirc();
      rewardTopText.text = topDescription[i];
      rewardTopText.color = textColor[i];
      rewardBottomText.color = textColor[i];
      rewardBottomText.text = bottomDescription[i];
    }

    imageBlock.gameObject.SetActive(true);
    lightImg.gameObject.SetActive(true);
    LeanTween.rotateAround(lightImg.gameObject, -Vector3.forward, 360, 8.8f).setLoopClamp();

    if (!isNeedClaim)
    {
      claimBtn.gameObject.SetActive(false);
      claimX2Btn.gameObject.SetActive(false);
      LeanTween.delayedCall(gameObject, 1.3f, Claim);
    }
    else
    {
      claimBtn.gameObject.SetActive(true);
      claimX2Btn.gameObject.SetActive(true);
    }
  }

  public void ShowCoffeeBreakWith(int amountCoin, Action onCompleted = null)
  {
    amountCoinTxt.text = "+" + amountCoin.ToString();
    coffeeBreakIcon.localScale = Vector3.zero;
    amountCoinTxt.transform.localScale = Vector3.zero;
    rewardTxt.transform.localScale = Vector3.zero;
    coinIconImg.localScale = Vector3.zero;

    Sequence seq = DOTween.Sequence();

    var currentTimeAnim = 0f;
    coffeeBreakObj.gameObject.SetActive(true);

    var targetScale = Vector3.one * 0.35f;
    var timeScale = 0.5f;
    var timeDelayAds = 2f;

    seq.Insert(
      currentTimeAnim,
      coffeeBreakIcon.DOScale(targetScale, timeScale)
    );

    currentTimeAnim += timeScale;
    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        amountCoinTxt.transform.localScale = Vector3.one;
        rewardTxt.transform.localScale = Vector3.one;
        coinIconImg.localScale = Vector3.one;
      }
    );

    currentTimeAnim += timeDelayAds;
    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        coffeeBreakObj.gameObject.SetActive(false);
        onCompleted?.Invoke();
      }
    );
  }

  void CloseImg(Action endCallback = null)
  {
    LeanTween.cancel(lightImg.gameObject);
    LeanTween
      .scale(contents.GetChild(0).gameObject, Vector3.zero, .15f)
      .setEaseInCirc()
      .setOnComplete(() =>
      {
        imageBlock.gameObject.SetActive(false);
        InitUIBeginState();
        endCallback?.Invoke();
      });

    for (int i = 1; i < contents.childCount; i++)
    {
      LeanTween
     .scale(contents.GetChild(i).gameObject, Vector3.zero, .15f)
     .setEaseInCirc()
     .setOnComplete(() =>
     {
       for (int i = contents.childCount - 1; i > 0; i--)
       {
         Destroy(contents.GetChild(i).gameObject);
       }
     });
    }
  }

  void CloseLuckyRewardImg(int amount, PriceType priceType)
  {
    LeanTween.cancel(lightImg.gameObject);

    var newLuckyReward = Instantiate(contents.GetChild(0).gameObject, contents);
    newLuckyReward.transform.GetChild(0).gameObject.SetActive(false);
    newLuckyReward.transform.GetChild(1).gameObject.SetActive(false);

    contents.GetChild(0).gameObject.SetActive(false);
    showText.transform.localScale = Vector3.zero;
    darkBg.gameObject.SetActive(false);
    lightImg.gameObject.SetActive(false);
    showImg.rectTransform.localScale = Vector3.zero;

    globalCoinText.text = GameManager.Instance.CurrentCoin.ToString("#,##0");

    Sequence seq = DOTween.Sequence();
    var timeFly = 0.4f;

    seq.InsertCallback(0,
      () =>
      {
        if (priceType == PriceType.Coin)
        {
          newLuckyReward.SetActive(false);
          EffectManager.Instance.SpawnEfxCoinAt(
            newLuckyReward.transform.position,
            1,
            null,
            amount,
            false
          );
        }
        else
        {
          newLuckyReward.transform.DOMove(
            LobbyPanel.Instance.SkePlayBtn.transform.position,
            timeFly
          );
        }
      }
    );

    seq.InsertCallback(
      timeFly + 0.05f,
      () =>
      {
        imageBlock.gameObject.SetActive(false);
        contents.GetChild(0).gameObject.SetActive(true);
        darkBg.gameObject.SetActive(true);
        Destroy(newLuckyReward);
      }
    );
  }

  public void ShowTextAt(Vector2 screenPoint, string textValue, Color color, Sprite sprite = null, float _scale = 1f, float _slowFactor = 1f, float _spriteScale = 1)
  {
    var _showText = Instantiate(showText, transform);

    _showText.transform.position = screenPoint;
    _showText.color = color;
    _showText.rectTransform.position = screenPoint;
    _showText.text = textValue;

    var targetScale = Vector3.one * _scale;
    var targetMove = screenPoint + Vector2.up * 1.5f;

    LeanTween.scale(_showText.gameObject, targetScale, _slowFactor).setEaseOutBack();
    LeanTween.move(_showText.gameObject, targetMove, _slowFactor * 1.1f).setEaseOutBack()
      .setOnComplete(() =>
      {
        Destroy(_showText.gameObject);
      });

    if (sprite)
    {
      var _describeImg = Instantiate(describeImg, transform);
      var imgPos = screenPoint + Vector2.right * .8f;
      var imgTargetMove = imgPos + Vector2.up * 1.5f;
      _describeImg.sprite = sprite;
      _describeImg.transform.position = imgPos;
      LeanTween.scale(_describeImg.gameObject, targetScale * _spriteScale, _slowFactor).setEaseOutBack();
      LeanTween.move(_describeImg.gameObject, imgTargetMove, _slowFactor * 1.1f).setEaseOutBack()
      .setOnComplete(() =>
      {
        Destroy(_describeImg.gameObject);
      });
    }
  }

  private void UpdateTextAlpha(float alpha)
  {
    Color color = showText.color;
    color.a = alpha;
    showText.color = color;
  }

  public void CloseAdEarnedReward()
  {
    LeanTween.delayedCall(gameObject, .2f, () =>
    {
      CloseImg();
      if (currentPriceData.Type == PriceType.Coin)
      {
        EffectManager.Instance.EmissingCoinsWithParticleTo(
            coinDesPos,
            10, showImg.transform,
            () =>
            {
              claimX2Callback?.Invoke(currentPriceData);
            }
        );
        return;
      }
      claimX2Callback?.Invoke(currentPriceData);
    });
  }

  public void ToggleHomeBtn()
  {
    if (homeBtn.gameObject.activeSelf)
    {
      homeBtn.gameObject.SetActive(false);
      return;
    }

    homeBtn.gameObject.SetActive(true);
  }

  public void ShowHomeBtn()
  {
    homeBtn.gameObject.SetActive(true);
  }

  public void HideHomeBtn()
  {
    homeBtn.gameObject.SetActive(false);
  }

  void ShowNotify(string message)
  {
    if (SceneManager.GetActiveScene().name == KeyString.NAME_SCENE_LOBBY)
      LobbyPanel.Instance.ShowNotifyWith(message);
  }

  // v1.5
  public void ShowDemoBoosterAt(int index)
  {
    textTutorials[index].SetActive(true);
  }

  public void HideDemoBoosterAt(int index)
  {
    textTutorials[index].SetActive(false);
  }

  public void HideAllDemo()
  {
  }
}
