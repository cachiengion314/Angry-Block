using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CoinsParticle : MonoBehaviour
{
  [Header("Element")]
  [SerializeField] ParticleSystem coinPS;

  [Header("Setting")]
  [SerializeField] ParticleSystem hitMiscEfx;
  [SerializeField] private float moveSpeed;
  public int CoinDefaultAmount;
  private int coinAmount;

  public bool IsPlaying()
  {
    if (coinPS.isPlaying) return true;
    return false;
  }

  public void EmissingTo(Transform targetPos, int amount, Transform fromPos = null, Action onCompleted = null)
  {
    if (coinPS.isPlaying) return;

    if (fromPos)
    {
      transform.position = fromPos.transform.position;
    }

    ParticleSystem.Burst burst = coinPS.emission.GetBurst(0);
    burst.count = amount;
    coinPS.emission.SetBurst(0, burst);
    coinPS.Play();
    coinAmount = amount;

    ParticleSystem.MainModule main = coinPS.main;

    StartCoroutine(PlayCoinParticlesCoroutine(targetPos, onCompleted));
  }

  IEnumerator PlayCoinParticlesCoroutine(Transform targetPos, Action onCompleted = null)
  {
    yield return new WaitForSeconds(.58f);
    ParticleSystem.MainModule main = coinPS.main;
    main.gravityModifier = 0;

    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[coinAmount];

    while (coinPS.isPlaying)
    {
      coinPS.GetParticles(particles);

      for (int i = 0; i < particles.Length; ++i)
      {
        if ((targetPos.position - particles[i].position).sqrMagnitude < .09f)
        {
          // Instantiate(
          //     hitMiscEfx, particles[i].position + -Vector3.forward, Quaternion.identity
          // ).Play();
          continue;
        }
        var _speedFactor = (i + 1) % 12;
        if (_speedFactor < 3) _speedFactor = 3;
        var _moveSpeed = _speedFactor * moveSpeed;
        MoveEachFrameTo(ref particles[i], targetPos.position, _moveSpeed);
      }

      coinPS.SetParticles(particles);
      yield return null; // one frame have passed
    }
    onCompleted?.Invoke();
  }

  void MoveEachFrameTo(ref ParticleSystem.Particle particle, in Vector3 targetPos, float speed)
  {
    particle.position = Vector3.MoveTowards(
        particle.position, targetPos, speed * Time.deltaTime
    );
  }
}
