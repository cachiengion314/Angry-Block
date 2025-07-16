using Unity.Mathematics;
using UnityEngine;

public class BlastBlockControl : MonoBehaviour
  , IColorBlock
  , IMoveable
  , IGun
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _index;
  int _colorValue;
  int _ammunition;

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

  public void SetAmmunition(int ammunition)
  {
    _ammunition = ammunition;
  }

  public int GetAmmunition()
  {
    return _ammunition;
  }
}