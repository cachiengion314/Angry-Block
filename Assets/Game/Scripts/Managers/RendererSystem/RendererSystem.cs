using UnityEngine;

public class RendererSystem : MonoBehaviour
{
  public static RendererSystem Instance { get; private set; }

  [Header("Dependences")]
  [SerializeField] ThemeObj[] themes;
  int _currentThemeIndex = 0;

  void Start()
  {
    if (Instance == null)
      Instance = this;
    else Destroy(gameObject);
  }

  public ThemeObj GetCurrentTheme()
  {
    if (_currentThemeIndex >= 0 && _currentThemeIndex <= themes.Length - 1)
      return themes[_currentThemeIndex];
    return themes[0];
  }

  public Color GetColorBy(int colorValue)
  {
    var theme = GetCurrentTheme();
    if (colorValue > theme.colorValues.Length - 1) return theme.colorValues[^1];
    return theme.colorValues[colorValue];
  }

  public Sprite GetColorBlockAt(int colorValue)
  {
    var theme = GetCurrentTheme();
    if (colorValue > theme.colorBlocks.Length - 1) return theme.colorBlocks[^1];
    return theme.colorBlocks[colorValue];
  }

  public Sprite GetDirectionBlockAt(int colorValue)
  {
    var theme = GetCurrentTheme();
    if (colorValue > theme.directionBlockSprites.Length - 1) return theme.directionBlockSprites[^1];
    return theme.directionBlockSprites[colorValue];
  }

  public Sprite GetBlastBlockAt(int colorValue)
  {
    var theme = GetCurrentTheme();
    if (colorValue > theme.blastBlockSprites.Length - 1) return theme.blastBlockSprites[^1];
    return theme.blastBlockSprites[colorValue];
  }

  public Sprite GetTunnelSprite(int index)
  {
    var theme = GetCurrentTheme();
    if (index > theme.tunnelSprites.Length - 1) return theme.tunnelSprites[^1];
    return theme.tunnelSprites[index];
  }
}
