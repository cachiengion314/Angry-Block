using Unity.Mathematics;
using UnityEngine;

public class BulletControl : MonoBehaviour, IBullet
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _damage;
  float3 _velocity;

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
}