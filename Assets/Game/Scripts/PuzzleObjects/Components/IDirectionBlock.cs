using Unity.Mathematics;
using UnityEngine;

public interface IDirectionBlock
{
  public void SetDirectionValue(DirectionValue direction);
  public DirectionValue GetDirectionValue();
  public int2 GetDirection();
}