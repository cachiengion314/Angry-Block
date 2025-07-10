using UnityEngine;

public class ColorBlockControl : MonoBehaviour
  , IColorBlock
  , IDamageable
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _index;
  int _initHealth;
  int _currentHealth;
  int _colorValue;

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
    _currentHealth = health;
  }

  public bool IsDead()
  {
    return _currentHealth <= 0;
  }

  public bool IsDamage()
  {
    return _currentHealth == _initHealth;
  }
}