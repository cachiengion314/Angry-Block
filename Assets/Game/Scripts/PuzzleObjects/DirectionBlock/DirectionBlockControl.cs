using Unity.Mathematics;
using UnityEngine;

public class DirectionBlockControl : MonoBehaviour, IColorBlock
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] SpriteRenderer directionRenderer;
  [Header("Datas")]
  int _index;
  int _colorValue;
  Direction _direction;
  public Direction Direction { get { return _direction; } }

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

  public void SetDirection(Direction direction)
  {
    _direction = direction;
    var angle90 = 90 / 2f * math.PI / 180f;
    if (direction == Direction.Right)
    {
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(-angle90), math.cos(-angle90));
    }
    else if (direction == Direction.Up)
    {
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(0), math.cos(0));
    }
    else if (direction == Direction.Left)
    {
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(angle90), math.cos(angle90));
    }
    else if (direction == Direction.Down)
    {
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(2 * angle90), math.cos(2 * angle90));
    }
  }
}