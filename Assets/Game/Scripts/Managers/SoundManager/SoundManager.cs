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

  public void PlayMainThemeSfx()
  {
    audioSource.volume = 1f;
    audioSource.clip = mainTheme;
    audioSource.loop = true;
    audioSource.Play();
  }

  public void StopMainThemeSfx()
  {
    audioSource.Stop();
  }
}
