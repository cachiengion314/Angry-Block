using Unity.Mathematics;

public interface IDirectionBlock : IColorBlock
{
  public void SetDirectionValue(DirectionValue direction);
  public DirectionValue GetDirectionValue();
}