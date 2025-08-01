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
  [SerializeField] AudioClip winLevelSfx;
  [SerializeField] AudioClip loseLevelSfx;
  [SerializeField] AudioClip clickBlockSfx;
  [SerializeField] AudioClip blockMoveSfx;
  [SerializeField] AudioClip mergeBlockSfx;
  [SerializeField] AudioClip shootingSfx;
  [SerializeField] AudioClip destoyColorBlockSfx;
  [SerializeField] AudioClip destoyShootingBlockSfx;

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

  public void PlayPressBtnSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(pressBtnSfx, Vector3.forward * -5, 1f);
  }

  public void PlayClickBlockSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(clickBlockSfx, Vector3.forward * -5, 1f);
  }

  public void PlayBlockMoveSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(blockMoveSfx, Vector3.forward * -5, 1f);
  }

  public void PlayMergeBlockSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(mergeBlockSfx, Vector3.forward * -5, 1f);
  }

  public void PlayShootingSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(shootingSfx, Vector3.forward * -5, 1f);
  }

  public void PlayDestoyColorBlockSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(destoyColorBlockSfx, Vector3.forward * -5, 1f);
  }

  public void PlayDestoyShootingBlockSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(destoyShootingBlockSfx, Vector3.forward * -5, 1f);
  }

  public void PlayWinLevelSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(winLevelSfx, Vector3.forward * -5, 1f);
  }

  public void PlayLoseLevelSfx()
  {
    if (GameManager.Instance.IsHapticOn)
    {
      HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
    if (!GameManager.Instance.IsSoundOn) return;
    AudioSource.PlayClipAtPoint(loseLevelSfx, Vector3.forward * -5, 1f);
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
