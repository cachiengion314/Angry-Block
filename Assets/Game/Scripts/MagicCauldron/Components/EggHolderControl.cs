using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EggHolderControl : MonoBehaviour
, ISpriteRenderer
, IDoTweenControl
, IFeedbackControl
{
  [Header("Dependencies")]
  [SerializeField] Transform eggParent;
  [SerializeField] Transform decalParent;
  public Transform DecalParent { get { return decalParent; } }
  [SerializeField] Transform[] eggPosParents;
  [SerializeField] Transform[] eggPosRendererParents;

  [Header("Datas")]
  [SerializeField] Color normalBlockColor;
  [SerializeField] Color highlightBlockColor;
  int _currentPosIndex;
  EggControl[] _placedOrder;
  EggControl[] _wrongOrder;
  public EggControl[] WrongOrder { get { return _wrongOrder; } }

  public float3 GetEggLocalPosAt(int index)
  {
    var posParent = eggPosParents[_currentPosIndex];
    return posParent.transform.localPosition + posParent.GetChild(index).localPosition;
  }

  void SetBlockColorAt(int index, Color color)
  {
    var posRendererParent = eggPosRendererParents[_currentPosIndex];
    if (index < 0 || index > posRendererParent.childCount - 1) return;
    var posRend = posRendererParent.GetChild(index).GetComponent<SpriteRenderer>();
    posRend.material.SetColor("_OverwriteColor", color);
  }

  public void SetBlockSortingLayerNameAt(int index, string layerName, int sortingOrder)
  {
    var posRendererParent = eggPosRendererParents[_currentPosIndex];
    if (index < 0 || index > posRendererParent.childCount - 1) return;
    var posRend = posRendererParent.GetChild(index).GetComponent<SpriteRenderer>();
    posRend.sortingLayerName = layerName;
    posRend.sortingOrder = sortingOrder;
  }

  public void SetDecalSortingLayerNameAt(string layerName, int sortingOrder)
  {
    for (int i = 0; i < decalParent.childCount; i++)
    {
      decalParent.GetChild(i).GetComponent<DecalControl>().SetDecalsSortingOrder(layerName, sortingOrder);
    }
  }

  public void SetDecalSortingLayerTutorial()
  {
    for (int i = 0; i < decalParent.childCount; i++)
    {
      decalParent.GetChild(i).GetComponent<DecalControl>().SetSortinglayerTuTorial();
    }
  }

  public void ReSetDecalSortingLayerTutorial()
  {
    for (int i = 0; i < decalParent.childCount; i++)
    {
      decalParent.GetChild(i).GetComponent<DecalControl>().ResetSortinglayer();
    }
  }

  public void HighlightColorBlockAt(int index)
  {
    SetBlockColorAt(index, highlightBlockColor);
  }

  public void RestoreColorBlockAt(int index)
  {
    SetBlockColorAt(index, normalBlockColor);
  }

  public void SetHolderCapacity(int capacity)
  {
    if (capacity - 1 < 0 || capacity > eggPosParents.Length) return;
    _currentPosIndex = capacity - 1;

    for (int i = 0; i < eggPosParents.Length; ++i)
    {
      eggPosParents[i].gameObject.SetActive(false);
      if (i == _currentPosIndex) eggPosParents[i].gameObject.SetActive(true);
    }

    for (int i = 0; i < eggPosRendererParents.Length; ++i)
    {
      eggPosRendererParents[i].gameObject.SetActive(false);
      if (i == _currentPosIndex) eggPosRendererParents[i].gameObject.SetActive(true);
    }

    InitPlacedOrder(capacity);
  }

  public void InitPlacedOrder(int capacity)
  {
    _placedOrder = new EggControl[capacity];
    for (int i = 0; i < _placedOrder.Length; ++i)
      _placedOrder[i] = null;
  }

  public EggControl[] GetCurrentPlacedOrder()
  {
    return _placedOrder;
  }

  public void ClonePlacedOrderFrom(EggControl[] original)
  {
    _placedOrder = new EggControl[original.Length];
    LinkChilds();

    _currentPosIndex = _placedOrder.Length - 1;
    var count = 0;
    for (int i = 0; i < original.Length; ++i)
    {
      var oEgg = original[i];
      if (oEgg != null)
      {
        var egg = eggParent.GetChild(count).GetComponent<EggControl>();
        egg.SetPlacedIndex(oEgg.PlacedIndex);
        egg.SetColorValue(oEgg.ColorValue);
        egg.SetSkinByColor(oEgg.ColorValue);
        egg.BodyRenderer.enabled = true;
        egg.InjectInitSortingOrder(oEgg.InitSortingOrder);
        egg.transform.position = transform.position + (Vector3)GetEggLocalPosAt(i);

        _placedOrder[i] = egg;
        count++;
      }
    }
  }

  public void AdoptChildAt(int placedIndex, EggControl egg)
  {
    if (placedIndex < 0 || placedIndex > _placedOrder.Length - 1) return;
    egg.SetPlacedIndex(placedIndex);
    _placedOrder[placedIndex] = egg;
  }

  public void SaveWrongOrder(EggControl[] wrongOrder)
  {
    _wrongOrder = wrongOrder;
  }

  /// <summary>
  /// setParent for all childs so they will move along with this obj when it move
  /// </summary>
  public void LinkChilds()
  {
    for (int i = 0; i < _placedOrder.Length; ++i)
    {
      var child = _placedOrder[i];
      if (child == null) continue;
      child.transform.SetParent(eggParent);
    }
  }

  public EggControl GetChildAt(int index)
  {
    if (index < 0 || index > _placedOrder.Length - 1) return null;
    return _placedOrder[index];
  }

  public List<EggControl> FindChilds()
  {
    var list = new List<EggControl>();
    for (int i = 0; i < _placedOrder.Length; ++i)
    {
      var child = _placedOrder[i];
      if (child == null) continue;
      list.Add(child);
    }
    return list;
  }

  public EggControl ReleaseChildAt(int index)
  {
    if (index < 0 || index > _placedOrder.Length - 1) return null;
    if (_placedOrder[index] == null) return null;

    var obj = _placedOrder[index];
    _placedOrder[index] = null;
    obj.SetPlacedIndex(-1);
    return obj;
  }

  public int FindFirstEmptyIndex()
  {
    for (int i = 0; i < _placedOrder.Length; ++i)
    {
      if (_placedOrder[i] == null) return i;
    }
    return -1;
  }

  public int CountEmpty()
  {
    var count = 0;
    for (int i = 0; i < _placedOrder.Length; ++i)
    {
      if (_placedOrder[i] == null)
        count++;
    }
    return count;
  }

  public void ClearDecals()
  {
    for (int i = decalParent.childCount - 1; i >= 0; --i)
      Destroy(decalParent.GetChild(i).gameObject);
  }

  public void ChangeTweeningTo(bool onOffValue)
  {
    throw new System.NotImplementedException();
  }

  public int GetSortingOrder()
  {
    return 10;
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

  public void SetSortinglayerTutorial(int _index)
  {
    var posRendererParent = eggPosRendererParents[_currentPosIndex];
    if (_index < 0 || _index > posRendererParent.childCount - 1) return;
    var posRend = posRendererParent.GetChild(_index).GetComponent<SpriteRenderer>();
    posRend.sortingLayerName = "Notif";
    posRend.sortingOrder = 10;

  }

  public void ReSetSortinglayerTutorial(int _index)
  {
    var posRendererParent = eggPosRendererParents[_currentPosIndex];
    if (_index < 0 || _index > posRendererParent.childCount - 1) return;
    var posRend = posRendererParent.GetChild(_index).GetComponent<SpriteRenderer>();
    posRend.sortingLayerName = "Item";
    posRend.sortingOrder = 10;
  }
}