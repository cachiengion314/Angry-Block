using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// GiftItemSystem
/// </summary>
public partial class ItemSystem : MonoBehaviour
{
  [Header("Gift dependencies")]
  [SerializeField] GiftControl[] giftPrefs;
  [SerializeField] Transform giftParent;
  [SerializeField] Transform giftSpawnedPos;
  public float3 GiftSpawnedPos { get { return giftSpawnedPos.position; } }
  [SerializeField] Transform giftDestination;
  public float3 GiftDestination { get { return giftDestination.position; } }
  readonly float _giftHorizontalMargin = 4.2f;
  readonly float _giftVerticalMargin = 4.2f;

  float3 GetGiftPosAt(int row, int i, int amountOfItem)
  {
    if (amountOfItem == 0) return 0;

    var maxLength = math.length(rightBoundPos.position - leftBoundPos.position);

    var totalHorizontalLength
      = math.min(_giftHorizontalMargin * (amountOfItem - 1), maxLength);
    var currleftBoundPos
      = -1 * totalHorizontalLength / 2f + giftSpawnedPos.position.x;

    var x = _giftHorizontalMargin * i + currleftBoundPos;
    var y = giftSpawnedPos.position.y + -row * _giftVerticalMargin;
    return new float3(x, y, 0);
  }

  public float3 GetGiftPosAt(int i, int amountOfItem)
  {
    if (amountOfItem == 0) return 0;

    if (amountOfItem > 3)
    {
      var index = i % 3;
      var row = (int)math.floor(i / 3f);
      return GetGiftPosAt(row, index, 3);
    }
    return GetGiftPosAt(0, i, amountOfItem);
  }

  public GiftControl SpawnGiftBy(int index)
  {
    if (index < 0 || index > giftPrefs.Length - 1)
    {
      var _pref = giftPrefs[^1];
      var _clone = Instantiate(_pref, giftSpawnedPos.position, _pref.transform.rotation);
      _clone.transform.SetParent(giftParent);
      return _clone;
    }
    var pref = giftPrefs[index];
    var clone = Instantiate(pref, giftSpawnedPos.position, pref.transform.rotation);
    clone.transform.SetParent(giftParent);
    return clone;
  }

  public GiftControl SpawnGiftBy(PriceType type)
  {
    for (int i = 0; i < giftPrefs.Length; ++i)
    {
      if (type != giftPrefs[i].Type) continue;
      return SpawnGiftBy(i);
    }
    return null;
  }

  public GiftControl SpawnGiftBy(PriceType type, int value)
  {
    var gift = SpawnGiftBy(type);
    gift.SetValue(value);

    return gift;
  }
}