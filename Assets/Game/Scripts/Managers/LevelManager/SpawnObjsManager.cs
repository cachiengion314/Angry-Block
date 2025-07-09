using Unity.Mathematics;
using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  [Header("Prefs")]
  [SerializeField] ColorBlockControl colorBlockPref;
  [SerializeField] DirectionBlockControl directionBlockPref;

  public ColorBlockControl SpawnColorBlockAt(float3 pos)
  {
    var obj = Instantiate(colorBlockPref, pos, colorBlockPref.transform.rotation);
    return obj;
  }

  public ColorBlockControl SpawnColorBlockAt(int index, Transform parent)
  {
    var pos = topGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(colorBlockPref, parent);
    obj.transform.position = pos;
    return obj;
  }

  public DirectionBlockControl SpawnDirectionBlockAt(int index, Transform parent)
  {
    var pos = bottomGrid.ConvertIndexToWorldPos(index);
    var obj = Instantiate(directionBlockPref, parent);
    obj.transform.position = pos;
    return obj;
  }
}
