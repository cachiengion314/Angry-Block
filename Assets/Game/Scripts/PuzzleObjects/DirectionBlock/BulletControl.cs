using Unity.Mathematics;
using UnityEngine;

public class BulletControl : MonoBehaviour
  , IBullet
  , IMoveable
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _damage;
  float3 _velocity;
  float3 _lockedPosition;
  float _lifeDuration;
  Transform _lockedTarget;

  public void SetVelocity(float3 velocity)
  {
    _velocity = velocity;
  }

  public float3 GetVelocity()
  {
    return _velocity;
  }

  public int GetDamage()
  {
    return _damage;
  }

  public void SetDamage(int damage)
  {
    _damage = damage;
  }

  public void SetLockedPosition(float3 lockedPosition)
  {
    _lockedPosition = lockedPosition;
  }

  public float3 GetLockedPosition()
  {
    return _lockedPosition;
  }

  public float GetLifeDuration()
  {
    return _lifeDuration;
  }

  public void SetLifeDuration(float duration)
  {
    _lifeDuration = duration;
  }

  public Transform GetLockedTarget()
  {
    return _lockedTarget;
  }

  public void SetLockedTarget(Transform lockedTarget)
  {
    _lockedTarget = lockedTarget;
  }
}