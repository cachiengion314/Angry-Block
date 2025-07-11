using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class DirectionBlockControl : MonoBehaviour
  , IDirectionBlock
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] SpriteRenderer directionRenderer;
  [Header("Datas")]
  int _index;
  int _ammunition;
  int _colorValue;
  public int2 Direction { get; private set; }
  DirectionValue _directionValue;

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

  public void SetAmmunition(int ammunition)
  {
    _ammunition = math.max(0, ammunition);
  }

  public int GetAmmunition()
  {
    return _ammunition;
  }

  public void SetDirectionValue(DirectionValue directionValue)
  {
    _directionValue = directionValue;
    var angle90 = 90 / 2f * math.PI / 180f;
    if (directionValue == DirectionValue.Right)
    {
      Direction = new int2(1, 0);
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(-angle90), math.cos(-angle90));
    }
    else if (directionValue == DirectionValue.Up)
    {
      Direction = new int2(0, 1);
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(0), math.cos(0));
    }
    else if (directionValue == DirectionValue.Left)
    {
      Direction = new int2(-1, 0);
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(angle90), math.cos(angle90));
    }
    else if (directionValue == DirectionValue.Down)
    {
      Direction = new int2(0, -1);
      directionRenderer.transform.rotation
        = new Quaternion(0, 0, math.sin(2 * angle90), math.cos(2 * angle90));
    }
  }

  public DirectionValue GetDirectionValue()
  {
    return _directionValue;
  }

  public void InvokeFireAnimationAt(float3 direction, float _duration = .2f, int _loopTime = 2)
  {
    bodyRenderer.transform
      .DOScale(1.3f, _duration / _loopTime)
      .SetLoops(_loopTime, LoopType.Yoyo);
    directionRenderer.transform
      .DOScale(1.3f, _duration / _loopTime)
      .SetLoops(_loopTime, LoopType.Yoyo);
  }
}