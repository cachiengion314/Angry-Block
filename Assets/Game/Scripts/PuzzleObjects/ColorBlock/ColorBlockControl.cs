using Unity.Mathematics;
using UnityEngine;

public class ColorBlockControl : MonoBehaviour
  , IColorBlock
  , IDamageable
  , IMoveable
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _index;
  int _initHealth;
  int _currentHealth;
  int _colorValue;
  DirectionBlockControl _whoLocked;
  DirectionBlockControl _whoPicked;

  public int GetColorValue()
  {
    return _colorValue;
  }

  public void SetColorValue(int colorValue)
  {
    _colorValue = colorValue;
    bodyRenderer.color = RendererSystem.Instance.GetColorBy(colorValue);
  }

  public void SetIndex(int index)
  {
    _index = index;
  }

  public int GetIndex()
  {
    return _index;
  }

  public void SetInitHealth(int health)
  {
    _initHealth = health;
    SetHealth(health);
  }

  public int GetHealth()
  {
    return _currentHealth;
  }

  public void SetHealth(int health)
  {
    _currentHealth = math.max(0, health);
  }

  public bool IsDead()
  {
    return _currentHealth <= 0;
  }

  public bool IsDamage()
  {
    return _currentHealth == _initHealth;
  }

  public DirectionBlockControl GetWhoLocked()
  {
    return _whoLocked;
  }

  public void SetWhoLocked(DirectionBlockControl block)
  {
    _whoLocked = block;
  }

  public DirectionBlockControl GetWhoPicked()
  {
    return _whoPicked;
  }

  public void SetWhoPicked(DirectionBlockControl block)
  {
    _whoPicked = block;
  }

  public void SetLockedPosition(float3 lockedPosition)
  {
    throw new System.NotImplementedException();
  }

  public float3 GetLockedPosition()
  {
    throw new System.NotImplementedException();
  }

  public Transform GetLockedTarget()
  {
    throw new System.NotImplementedException();
  }

  public void SetLockedTarget(Transform lockedTarget)
  {
    throw new System.NotImplementedException();
  }
}