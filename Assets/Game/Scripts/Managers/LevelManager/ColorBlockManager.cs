using UnityEngine;

public partial class LevelManager : MonoBehaviour
{
  ColorBlockControl[] _colorBlocks;
  public ColorBlockControl[] ColorBlocks { get { return _colorBlocks; } }

  ColorBlockControl FindColorBlockIn(Collider2D[] cols)
  {
    var colorBlock = FindObjIn<ColorBlockControl>(cols);
    return colorBlock;
  }
}