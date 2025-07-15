using UnityEngine;
using Lofelt.NiceVibrations;

/// <summary> Fix haptic issue when build in IOS
/// https://github.com/asmadsen/react-native-unity-view/issues/35
/// </summary>
[RequireComponent(typeof(AudioSource))]
public partial class SoundManager : MonoBehaviour
{
  public static SoundManager Instance { get; private set; }

  [Header("Injected Dependencies")]
  [SerializeField] AudioClip mainTheme;
  [SerializeField] AudioClip pressBtnSfx;
  [SerializeField] AudioClip hittingSfx;
  [SerializeField] AudioClip trayTranferSfx;
  [SerializeField] AudioClip boomSfx;
  // new sound
  [SerializeField] AudioClip comboSfx;
  [SerializeField] AudioClip coinDropSfx;
  [SerializeField] AudioClip handOnCoinSfx;
  [SerializeField] AudioClip pickedTraySfx;
  [SerializeField] AudioClip placedTraySfx;
  [SerializeField] AudioClip packedSfx;
  [SerializeField] AudioClip nextStageSfx;
  [SerializeField] AudioClip nextLevelSfx;
  [SerializeField] AudioClip levelFailSfx;
  // ver 1.3
  [SerializeField] AudioClip iceDestroySfx;
  [SerializeField] AudioClip swapSfx;
  [SerializeField] AudioClip claimItemSfx;
  [SerializeField] AudioClip balloonItemSfx;
  [SerializeField] AudioClip coinReceiveSfx;
  [SerializeField] AudioClip coinExplodeSfx;
  [SerializeField] AudioClip claimDailyRewardSfx;
  [SerializeField] AudioClip claimBreadSfx;
  //ver 1.5.4
  [SerializeField] AudioClip giftBoxSfx;
  [SerializeField] AudioClip coffeeBoardSfx;
  [SerializeField] AudioClip plantPotSfx;
  [SerializeField] AudioClip coverLetSfx;

  // ver 1.5.5
  [SerializeField] AudioClip moneyBagSfx;
  [SerializeField] AudioClip cupBoardSfx;
  [SerializeField] AudioClip machineCreamSfx;
  [SerializeField] AudioClip dinhdinhSfx;
  [SerializeField] AudioClip getRewardSfx;
  [SerializeField] AudioClip magicNestSfx;

  // ver 1.5.16
  [SerializeField] AudioClip flowerPotBloomingSfx;
  [SerializeField] AudioClip leavesSpreadOutSfx;


  [Header("Components")]
  [SerializeField] AudioSource audioSource;

  private void Awake()
  {
    if (Instance == null)
      Instance = this;
    else Destroy(gameObject);
    DontDestroyOnLoad(gameObject);
  }

  private void Start()
  {
    OnMusicChange();
    GameManager.Instance.OnMusicChange += OnMusicChange;
  }

  void OnDestroy()
  {
    GameManager.Instance.OnMusicChange -= OnMusicChange;
  }

  void OnMusicChange()
  {
    if (GameManager.Instance.IsMusicOn)
    {
      PlayMainThemeSfx();
    }
    else
    {
      StopMainThemeSfx();
    }
  }

  public void PlayBoomSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(boomSfx, Vector3.forward * -5, 1);
  }

  public void PlayTrayTransferSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(trayTranferSfx, Vector3.forward * -5, 1);
  }

  public void PlayCoinDropSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(coinDropSfx, Vector3.forward * -5, 1);
  }

  public void PlayDinhDinhSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(dinhdinhSfx, Vector3.forward * -5, 1);
  }

  public void PlayGetRewardSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(getRewardSfx, Vector3.forward * -5, 1);
  }

  public void PlayHapticWith(HapticPatterns.PresetType presetType)
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(presetType);
    }
  }

  public void PlayPressBtnSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(pressBtnSfx, Vector3.forward * -5, 1);
  }

  public void PlayHittingSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(hittingSfx, Vector3.forward * -5, 1);
  }

  public void PlayHandOnCoinSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(handOnCoinSfx, Vector3.forward * -5, 1);
  }

  public void PlayPickedTraySfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(pickedTraySfx, Vector3.forward * -5, 1);
  }

  public void PlayPlacedTraySfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(placedTraySfx, Vector3.forward * -5, 1);
  }

  public void PlayNextLevelSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(nextLevelSfx, Vector3.forward * -5, 1);
  }

  public void PlayLevelFailSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(levelFailSfx, Vector3.forward * -5, 1);
  }

  public void PlayPackedSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(packedSfx, Vector3.forward * -5, 1);
  }

  public void PlayNextStageSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(nextStageSfx, Vector3.forward * -5, 1);
  }

  public void PlayIceDestroySfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(iceDestroySfx, Vector3.forward * -5, 1);
  }

  public void PlaySwapSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(swapSfx, Vector3.forward * -5, 1);
  }

  public void PlayClaimItemSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(claimItemSfx, Vector3.forward * -5, 1);
  }

  public void PlayBalloonItemSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(balloonItemSfx, Vector3.forward * -5, 1);
  }

  public void PlayCoinReceiveSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(coinReceiveSfx, Vector3.forward * -5, 1);
  }

  public void PlayCoinExplodeSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(coinExplodeSfx, Vector3.forward * -5, 1);
  }

  public void PlayClaimDailyRewardSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(claimDailyRewardSfx, Vector3.forward * -5, 1);
  }

  public void PlayClaimBreadSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(claimBreadSfx, Vector3.forward * -5, 1);
  }

  public void PlayGiftBoxSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(giftBoxSfx, Vector3.forward * -5, 1);
  }

  public void PlayCoffeeBoardSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(coffeeBoardSfx, Vector3.forward * -5, 1);
  }

  public void PlayPlantPotSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(plantPotSfx, Vector3.forward * -5, 1);
  }

  public void PlayCoverLetSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(coverLetSfx, Vector3.forward * -5, 1);
  }

  public void PlayMoneyBagSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(moneyBagSfx, Vector3.forward * -5, 1);
  }

  public void PlayCupBoardSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(cupBoardSfx, Vector3.forward * -5, 1);
  }

  public void PlayMachineCreamSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(machineCreamSfx, Vector3.forward * -5, 1);
  }

  public void PlayMagicNestSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(magicNestSfx, Vector3.forward * -5, 1);
  }

  public void PlayFlowerPotBloomingSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(flowerPotBloomingSfx, Vector3.forward * -5, 1);
  }

  public void PlayLeavesSpreadOutSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(leavesSpreadOutSfx, Vector3.forward * -5, 1);
  }

  public void PlayMainThemeSfx()
  {
    audioSource.volume = .35f;
    audioSource.clip = mainTheme;
    audioSource.loop = true;
    audioSource.Play();
  }

  public void StopMainThemeSfx()
  {
    audioSource.Stop();
  }
}
