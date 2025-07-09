using UnityEngine;

public class AdsManager : MonoBehaviour
{
  public static AdsManager Instance { get; private set; }

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

    DontDestroyOnLoad(gameObject);
  }

  public void ShowRewardAds()
  {

  }

  public void ShowInterstitialAds()
  {

  }
}
