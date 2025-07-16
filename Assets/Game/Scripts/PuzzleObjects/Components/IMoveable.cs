using Unity.Mathematics;
using UnityEngine;

public interface IMoveable
{
  public void SetLockedPosition(float3 lockedPosition);
  public float3 GetLockedPosition();
  public Transform GetLockedTarget();
  public void SetLockedTarget(Transform lockedTarget);
}
