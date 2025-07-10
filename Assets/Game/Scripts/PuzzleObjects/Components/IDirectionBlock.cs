using Unity.Mathematics;

public interface IDirectionBlock : IColorBlock
{
  public int GetAmmunition();
  public void SetAmmunition(int ammunition);
  public void SetDirectionValue(DirectionValue direction);
  public DirectionValue GetDirectionValue();
  public void InvokeFireAnimationAt(float3 direction, float _duration = .1f, int _loopTime = 2);
}