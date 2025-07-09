using TMPro;
using UnityEngine;

public class GiftControl : MonoBehaviour
, ISpriteRenderer
, IDoTweenControl
, IFeedbackControl
{
  [Header("Dependencies")]
  [SerializeField] SpriteRenderer bodyRenderer;
  [SerializeField] TMP_Text valueText;
  [Header("Datas")]
  int _value;
  public int Value { get { return _value; } }
  [SerializeField] PriceType type;
  public PriceType Type { get { return type; } }

  public void SetValue(int value)
  {
    if (value == -1)
    {
      _value = value;
      valueText.gameObject.SetActive(false);
      return;
    }
    valueText.gameObject.SetActive(true);
    _value = value;
    if (type == PriceType.InfinityHeart)
      valueText.text = value.ToString() + " min";
    if (type == PriceType.Coin)
      valueText.text = value.ToString();
    if (type == PriceType.Refresh)
      valueText.text = value.ToString();
    if (type == PriceType.Hammer)
      valueText.text = value.ToString();
    if (type == PriceType.Rocket)
      valueText.text = value.ToString();
    if (type == PriceType.Swap)
      valueText.text = value.ToString();

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

  public void ChangeToUILayer()
  {
    bodyRenderer.sortingLayerName = "UI";
    valueText.GetComponent<MeshRenderer>().sortingLayerName = "UI";
  }
}