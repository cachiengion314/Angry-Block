using Firebase.Analytics;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPlinko : MonoBehaviour
{
  public static SpawnPlinko Instance { get; private set; }
  [SerializeField] Transform _dotParent;
  public Transform dotParent => _dotParent;
  [SerializeField] Transform _rewardParent;
  public Transform rewardParent => _rewardParent;
  [SerializeField] Transform _ballParent;
  public Transform ballParent => _ballParent;
  [SerializeField] DotControl dotPref;
  [SerializeField] BallControl ballPref;
  [SerializeField] RewardControl rewardPref;
  [SerializeField] Button dropBtn;
  [SerializeField] Button closeBtn;
  [SerializeField] GameObject adsIcon;
  [SerializeField] TextMeshProUGUI ballAmount;
  public Transform coinPos;
  [SerializeField] int height = 8;
  [SerializeField] float spacing = 2f;
  [SerializeField] float3 centerPos = new Vector3(0, -5, 0);
  [SerializeField] Sprite[] rewardSprites;
  float[] rewardLever = new float[5] { 3f, 2f, 1f, 2f, 3f };
  BallControl currentBall;

  private int _amountDropPlinko;
  public int AmountDropPlinko
  {
    get
    {
      return _amountDropPlinko;
    }
    set
    {
      _amountDropPlinko = value;
      PlayerPrefs.SetInt("Amount_Drop_Plinko", _amountDropPlinko);
    }
  }

  private int _amountExtraPlinko;
  public int AmountExtraPlinko
  {
    get
    {
      return _amountExtraPlinko;
    }
    set
    {
      _amountExtraPlinko = value;
      PlayerPrefs.SetInt("Amount_Extra_Plinko", _amountExtraPlinko);
    }
  }

  void Start()
  {
    if (Instance == null) Instance = this;

    AmountDropPlinko = PlayerPrefs.GetInt("Amount_Drop_Plinko", 0);
    AmountExtraPlinko = PlayerPrefs.GetInt("Amount_Extra_Plinko", 0);
    SpawnDot();
    UpdateUI();
  }

  public void SpawnDot()
  {
    for (int i = 0; i < height; i++)
    {
      int dotCount = height - i;
      float startX = -spacing * (dotCount - 1) / 2f;
      float y = i * spacing;

      if (dotCount == 1)
      {
        var spawnBallPos = new float3(startX, y, 0) + centerPos;
        SpawnBall(spawnBallPos);
        continue;
      }

      for (int j = 0; j < dotCount; j++)
      {
        float x = startX + j * spacing;
        float3 pos = new float3(x, y, 0) + centerPos;
        if (i != 0)
        {
          var dot = Instantiate(dotPref, _dotParent);
          dot.transform.localPosition = pos;
        }
        if (i == 0 && j > 0 && j < dotCount - 1)
        {
          var reward = Instantiate(rewardPref, _rewardParent);
          reward.SetLever(rewardLever[j - 1], rewardSprites[j - 1]);
          reward.transform.localPosition = pos;
        }
      }
    }
  }

  public void Drop()
  {
    if (GameManager.Instance.PlinkoFreeTodayCount > 0)
    {
      GameManager.Instance.PlinkoFreeTodayCount--;
      currentBall?.DropBall();
      OffButton();

      LobbyPanel.Instance.TurnOffNoticeLuckySpin();
      AmountDropPlinko++;
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(
          "lucky_offer_reward",
            new Parameter[]{
              new("drop", AmountDropPlinko)
            }
          );
      }
    }
    else if (GameManager.Instance.PlinkoAdsTodayCount > 0)
    {
#if !UNITY_EDITOR
            LevelPlayAds.Instance.ShowRewardedAd(() => {      
                GameManager.Instance.PlinkoAdsTodayCount--;   
                currentBall?.DropBall();
                OffButton();
            }, "DropPlinko",
            () =>
            {
                StartCoroutine(EffectManager.Instance.IEDelayShow(dropBtn, 1.5f));
                LobbyPanel.Instance.ShowNotifyWith("ADS NOT READY");
            });
#else
      GameManager.Instance.PlinkoAdsTodayCount--;
      currentBall?.DropBall();
      OffButton();
#endif

      LobbyPanel.Instance.TurnOffNoticeLuckySpin();

      AmountExtraPlinko++;
      if (FirebaseSetup.Instance.FirebaseStatusCode == 1)
      {
        FirebaseAnalytics.LogEvent(
          "lucky_offer_reward",
            new Parameter[]{
              new("drop", AmountExtraPlinko)
            }
          );
      }
    }
    else
    {
      StartCoroutine(EffectManager.Instance.IEDelayShow(dropBtn, 1.5f));
      LobbyPanel.Instance.ShowNotifyWith("REACHED LIMIT TODAY");
    }
  }

  void SpawnBall(float3 pos)
  {
    currentBall = Instantiate(ballPref, _ballParent);
    currentBall.spawnBallPos = pos;
    currentBall.InitBall();
    currentBall.mass = 10;
  }

  public void OffButton()
  {
    dropBtn.interactable = false;
    closeBtn.interactable = false;
  }
  public void OnButton()
  {
    dropBtn.interactable = true;
    closeBtn.interactable = true;
    UpdateUI();
  }

  void UpdateUI()
  {
    ballAmount.text = $"Ball ({GameManager.Instance.PlinkoFreeTodayCount}/1)";
    if (GameManager.Instance.PlinkoFreeTodayCount > 0) adsIcon.SetActive(false);
    else adsIcon.SetActive(true);
  }
}
