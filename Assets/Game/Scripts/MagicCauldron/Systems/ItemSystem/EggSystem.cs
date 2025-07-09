using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// BottleSystem
/// </summary>
public partial class ItemSystem : MonoBehaviour
{
  [Header("Egg dependencies")]
  [SerializeField] EggControl eggPref;
  [SerializeField] Transform eggParent;
  public Transform EggParent { get { return eggParent; } }
  [SerializeField] Transform lowestCenterPos;
  [SerializeField] Transform leftBoundPos;
  [SerializeField] Transform rightBoundPos;
  [SerializeField] Transform eggSpawnPos;
  [SerializeField] int eggInitSortingOrder = 201;
  [Header("Datas")]
  readonly float horizontalMargin = 1.9f;

  public float3 GetEggPosFrom(int i, int amountOfItem)
  {
    if (amountOfItem == 0) return 0;

    var maxLength = math.length(rightBoundPos.position - leftBoundPos.position);

    var totalHorizontalLength = math.min(horizontalMargin * (amountOfItem - 1), maxLength);
    var currleftBoundPos
      = -1 * totalHorizontalLength / 2f + lowestCenterPos.position.x;

    var x = horizontalMargin * i + currleftBoundPos;
    var y = lowestCenterPos.position.y;
    return new float3(x, y, 0);
  }

  public void SpawnEggs(int[] colorValues)
  {
    var count = 0;
    for (int i = 0; i < colorValues.Length; ++i)
    {
      var colorValue = colorValues[i];
      if (colorValue == -1) continue;
      var egg = SpawnEggAt(eggSpawnPos.position, colorValue);

      egg.SetInitSortingOrder(eggInitSortingOrder);
      egg.SetPlacedIndex(-1);
      egg.SetSpawnedIndex(count);
      count++;
    }
  }

  public EggControl SpawnEggAt(float3 pos, int colorValue)
  {
    var egg = Instantiate(eggPref, eggParent);
    egg.transform.position = pos;
    egg.SetColorValue(colorValue);
    // egg.SetUpAnimEgg(colorValue); // setup anime
    egg.SetSkinByColor(colorValue);
    return egg;
  }

  public List<EggControl> CollectSpawnedEggs()
  {
    var list = new List<EggControl>();
    for (int i = 0; i < eggParent.childCount; ++i)
      list.Add(eggParent.GetChild(i).GetComponent<EggControl>());
    return list;
  }

  public EggControl FindEggFrom(Collider2D[] touchedColliders)
  {
    for (int i = 0; i < touchedColliders.Length; ++i)
    {
      if (touchedColliders[i].TryGetComponent<EggControl>(out var egg))
      {
        return egg;
      }
    }
    return null;
  }

  public EggControl GetEggAt(int index)
  {
    if (index <= eggParent.childCount - 1)
    {
      var egg = eggParent.GetChild(index).GetComponent<EggControl>();
      return egg;
    }
    return null;
  }
}
