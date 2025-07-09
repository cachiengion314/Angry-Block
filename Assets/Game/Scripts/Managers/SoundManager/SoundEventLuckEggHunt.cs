using UnityEngine;
using Lofelt.NiceVibrations;

/// <summary> Fix haptic issue when build in IOS
/// https://github.com/asmadsen/react-native-unity-view/issues/35
/// 
/// </summary>
/// 

/// SoundEventLuckEggHunt
[RequireComponent(typeof(AudioSource))]
public partial class SoundManager : MonoBehaviour
{
    [Header("---EventLuckyEggHunt---")]
    [SerializeField] AudioClip pickEggs;
    [SerializeField] AudioClip brokenEggs;
    [SerializeField] AudioClip SuggessPosEgg;
    [SerializeField] AudioClip rewardEgg;

    public void PlayPickEggSfx()
    {
        if (GameManager.Instance.IsHapticOn)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }
        if (!GameManager.Instance.IsSoundOn) return;
        AudioSource.PlayClipAtPoint(pickEggs, Vector3.forward * -5, 1);
    }
    public void PlayBrokenEggsSfx()
    {
        if (GameManager.Instance.IsHapticOn)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.Success);
        }
        if (!GameManager.Instance.IsSoundOn) return;
        AudioSource.PlayClipAtPoint(brokenEggs, Vector3.forward * -5, 1);
    }
    public void PlaySuggessPosEggSfx()
    {
        if (GameManager.Instance.IsHapticOn)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
        if (!GameManager.Instance.IsSoundOn) return;
        AudioSource.PlayClipAtPoint(SuggessPosEgg, Vector3.forward * -5, 1);
    }
    public void PlayRewardEggSfx()
    {
        if (GameManager.Instance.IsHapticOn)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
        if (!GameManager.Instance.IsSoundOn) return;
        AudioSource.PlayClipAtPoint(rewardEgg, Vector3.forward * -5, 1);
    }
}