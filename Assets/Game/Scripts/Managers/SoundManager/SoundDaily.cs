using UnityEngine;
using Lofelt.NiceVibrations;

/// <summary> Fix haptic issue when build in IOS
/// https://github.com/asmadsen/react-native-unity-view/issues/35
/// 
/// </summary>
/// 

/// SoundDaily
[RequireComponent(typeof(AudioSource))]
public partial class SoundManager : MonoBehaviour
{
    [Header("---Daily---")]
    [SerializeField] AudioClip suggesTaks;
    [SerializeField] AudioClip claimReward;
    public void PlaySuggesTaksDailySfx()
    {
        if (GameManager.Instance.IsHapticOn)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
        if (!GameManager.Instance.IsSoundOn) return;
        AudioSource.PlayClipAtPoint(suggesTaks, Vector3.forward * -5, 1);
    }
    public void PlayRewardDailySfx()
    {
        if (GameManager.Instance.IsHapticOn)
        {
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        }
        if (!GameManager.Instance.IsSoundOn) return;
        AudioSource.PlayClipAtPoint(claimReward, Vector3.forward * -5, 1);
    }

}