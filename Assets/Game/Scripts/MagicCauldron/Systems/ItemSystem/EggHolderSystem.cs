using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// BottleHolderSystem
/// </summary>
public partial class ItemSystem : MonoBehaviour
{
  [Header("EggHolder dependencies")]
  [SerializeField] EggHolderControl eggHolderPref;
  [SerializeField] Transform eggHolderParent;
  public Transform EggHolderParent { get { return eggHolderParent; } }
  [SerializeField] Transform eggHolderChildsParent;
  [Header("Datas")]
  readonly float startMargin = 2.7f;
  public float StartMargin { get { return startMargin; } }
  readonly float verticalMargin = 3.7f;
  public float VerticalMargin { get { return verticalMargin; } }

  public float3 GetEggHolderPosAt(int i)
  {
    var y = verticalMargin * i + startMargin + lowestCenterPos.position.y;
    return new float3(.0f, y, .0f);
  }

  public EggHolderControl GetSecondEggHolder()
  {
    if (eggHolderChildsParent.childCount < 2) return null;
    return eggHolderChildsParent.GetChild(1).GetComponent<EggHolderControl>();
  }

  public EggHolderControl GetFirstEggHolder()
  {
    if (eggHolderChildsParent.childCount < 1) return null;
    return eggHolderChildsParent.GetChild(0).GetComponent<EggHolderControl>();
  }

  public EggHolderControl SpawnEggHolderAt(int index, int[] colorValues)
  {
    var pos = GetEggHolderPosAt(index);
    return SpawnEggHolderAt(pos, colorValues);
  }

  public EggHolderControl SpawnEggHolderAt(float3 pos, int[] colorValues)
  {
    var eggHolder = Instantiate(eggHolderPref, eggHolderChildsParent);
    eggHolder.transform.SetAsFirstSibling();

    eggHolder.transform.position = pos;
    eggHolder.SetHolderCapacity(colorValues.Length);

    for (int i = 0; i < colorValues.Length; ++i)
    {
      if (colorValues[i] == -1) continue;
      var localPos = eggHolder.GetEggLocalPosAt(i);
      var egg = SpawnEggAt(pos + localPos, colorValues[i]);
      egg.SetPlacedIndex(i);
      eggHolder.HighlightColorBlockAt(i);
      eggHolder.AdoptChildAt(i, egg);
      egg.SetActitveShadowEgg(false);
      egg.SetSortingOrder(eggHolder.GetSortingOrder() + 1);
      eggHolder.LinkChilds();
    }

    return eggHolder;
  }

  public EggHolderControl CloneEggHolderAt(int index, EggHolderControl originalEggHolder)
  {
    var pos = GetEggHolderPosAt(index);
    return CloneEggHolderAt(pos, originalEggHolder);
  }

  public EggHolderControl CloneEggHolderAt(float3 pos, EggHolderControl originalEggHolder)
  {
    var originalSelectedOrder = originalEggHolder.GetCurrentPlacedOrder();
    var eggHolder = Instantiate(originalEggHolder, eggHolderChildsParent);
    eggHolder.transform.SetAsFirstSibling();
    eggHolder.ClonePlacedOrderFrom(originalSelectedOrder);

    eggHolder.transform.position = pos;

    return eggHolder;
  }

  public void RemoveEggHolders()
  {
    for (int i = eggHolderChildsParent.childCount - 1; i >= 0; i--)
    {
      var holder = eggHolderChildsParent.GetChild(i).GetComponent<EggHolderControl>();
      holder.LinkChilds();
      holder.gameObject.SetActive(false);
      RemoveNonPoolItem(holder.gameObject);
    }
  }
}