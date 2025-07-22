using Unity.Mathematics;
using UnityEngine;

public class ColorBlockControl : MonoBehaviour
  , IGameObj
  , IColorBlock
  , IDamageable
  , IMoveable
  , ISpriteRend
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _index;
  int _initHealth;
  int _currentHealth;
  int _colorValue;
  GameObject _whoLocked;
  GameObject _whoPicked;
  float3 _initPosition;
  float3 _lockedPosition;

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

  public GameObject GetWhoLocked()
  {
    return _whoLocked;
  }

  public void SetWhoLocked(GameObject block)
  {
    _whoLocked = block;
  }

  public GameObject GetWhoPicked()
  {
    return _whoPicked;
  }

  public void SetWhoPicked(GameObject block)
  {
    _whoPicked = block;
  }

  public void SetLockedPosition(float3 lockedPosition)
  {
    _lockedPosition = lockedPosition;
  }

  public float3 GetLockedPosition()
  {
    return _lockedPosition;
  }

  public Transform GetLockedTarget()
  {
    throw new System.NotImplementedException();
  }

  public void SetLockedTarget(Transform lockedTarget)
  {
    throw new System.NotImplementedException();
  }

  public GameObject GetGameObject()
  {
    return gameObject;
  }

  public void SetInitPostion(float3 pos)
  {
    _initPosition = pos;
  }

  public float3 GetInitPostion()
  {
    return _initPosition;
  }

  public void SetPath(float3[] path)
  {
    throw new System.NotImplementedException();
  }

  public float3[] GetPath()
  {
    throw new System.NotImplementedException();
  }

  public SpriteRenderer GetBodyRenderer()
  {
    return bodyRenderer;
  }

  public int GetSortingOrder()
  {
    return bodyRenderer.sortingOrder;
  }

  public void SetSortingOrder(int sortingOrder)
  {
    bodyRenderer.sortingOrder = sortingOrder;
  }
}