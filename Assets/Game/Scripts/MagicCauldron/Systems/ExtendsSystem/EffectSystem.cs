using Unity.Mathematics;
using UnityEngine;

public partial class EffectManager : MonoBehaviour
{
  [Header("Magic Cauldron Prefabs")]
  [SerializeField] GameObject decalPrefab;
  [SerializeField] ParticleSystem arcanSparkEfx;
  [SerializeField] ParticleSystem eggSplashPref;
  [SerializeField] ParticleSystem powerUpEfx;
  [SerializeField] ParticleSystem healBigEfx;

  public GameObject SpawnDecalAt(float3 pos)
  {
    var pref = Instantiate(decalPrefab, pos, decalPrefab.transform.rotation);
    return pref;
  }

  public ParticleSystem SpawnArcaneSparkAt(float3 pos)
  {
    var efx = Instantiate(arcanSparkEfx, pos, arcanSparkEfx.transform.rotation);
    return efx;
  }

  public ParticleSystem SpawnEggSplashAt(float3 pos)
  {
    ParticleSystem efx = Instantiate(eggSplashPref, pos, eggSplashPref.transform.rotation);
    return efx;
  }

  public ParticleSystem SpawnSparkUpEfxAt(float3 pos)
  {
    ParticleSystem efx = Instantiate(powerUpEfx, pos, powerUpEfx.transform.rotation);
    return efx;
  }

  public ParticleSystem SpawnHealBigEfxAt(float3 pos)
  {
    ParticleSystem efx = Instantiate(healBigEfx, pos, healBigEfx.transform.rotation);
    return efx;
  }
}