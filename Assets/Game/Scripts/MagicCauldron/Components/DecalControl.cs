using UnityEngine;

public class DecalControl : MonoBehaviour
, ISpriteRenderer
, IDoTweenControl
, IFeedbackControl
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [Header("Datas")]
  int _colorValue;
  public int ColorValue { get { return _colorValue; } }

  public void SetColorValue(int colorValue)
  {
    _colorValue = colorValue;
  }

  public void SetDecalsSortingOrder(string sortingLayerName, int sortingOrder)
  {
    bodyRenderer.sortingLayerName = sortingLayerName;
    bodyRenderer.sortingOrder = sortingOrder;
  }

  public void ChangeTweeningTo(bool onOffValue)
  {
    throw new System.NotImplementedException();
  }

  public int GetSortingOrder()
  {
    throw new System.NotImplementedException();
  }

  public void InjectChannel(int channelId)
  {
    throw new System.NotImplementedException();
  }

  public bool IsTweening()
  {
    throw new System.NotImplementedException();
  }

  public void ResetSortingOrder()
  {
    throw new System.NotImplementedException();
  }

  public void SetInitSortingOrder(int sortingOrder)
  {
    throw new System.NotImplementedException();
  }

  public void SetSortingOrder(int sortingOrder)
  {
    throw new System.NotImplementedException();
  }

  public void SetSortinglayerTuTorial()
  {
    bodyRenderer.sortingLayerName = "Notif";
    bodyRenderer.sortingOrder = 10;
  }

  public void ResetSortinglayer()
  {
    bodyRenderer.sortingLayerName = "Item";
    bodyRenderer.sortingOrder = 10;
  }
}
