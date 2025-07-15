using Unity.Mathematics;

public interface IMoveable
{
  public void SetLockedPosition(float3 lockedPosition);
  public float3 GetLockedPosition();
}
