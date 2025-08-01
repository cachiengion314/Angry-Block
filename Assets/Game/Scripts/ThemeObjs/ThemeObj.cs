using UnityEngine;

[CreateAssetMenu(fileName = "ThemeObj", menuName = "ScriptableObjects/ThemeObj", order = 0)]
public class ThemeObj : ScriptableObject
{
  [Header("Elements")]
  public Color[] colorValues;
  public Sprite[] colorBlocks;
  public Sprite[] directionBlockSprites;
  public Sprite[] blastBlockSprites;
  public Sprite[] tunnelSprites;
}
