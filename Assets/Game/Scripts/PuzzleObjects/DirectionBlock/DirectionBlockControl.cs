using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class DirectionBlockControl : MonoBehaviour
  , IDirectionBlock
  , IColorBlock
  , IMoveable
  , IMergeable
  , IGameObj
  , ISpriteRend
{
  [Header("Dependencies")]
  [SerializeField] SortingGroup sortingGroup;
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] SpriteRenderer directionRenderer;
  [Header("Datas")]
  int _index;
  int _ammunition;
  int _colorValue;
  public int2 Direction { get; private set; }
  DirectionValue _directionValue;
  float3 _lockedPosition;
  float3 _initPosition;
  float3[] _path;
  Transform _lockedTarget;

  public int GetColorValue()
  {
    return _colorValue;
  }

  public void SetColorValue(int colorValue)
  {
    _colorValue = colorValue;
    bodyRenderer.sprite = RendererSystem.Instance.GetDirectionBlockAt(colorValue);
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
    return _lockedTarget;
  }

  public void SetLockedTarget(Transform lockedTarget)
  {
    _lockedTarget = lockedTarget;
  }

  public int2 GetDirection()
  {
    return Direction;
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
    _path = path;
  }

  public float3[] GetPath()
  {
    return _path;
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
    sortingGroup.sortingOrder = sortingOrder;
  }
}