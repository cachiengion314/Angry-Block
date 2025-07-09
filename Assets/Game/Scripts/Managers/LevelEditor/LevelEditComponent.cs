using UnityEngine;

public enum MapComponent
{
  None = 0,
  WoodBox = 1,
  Grasses = 2,
  NormalTray = 3,
  VideoAd = 111,
  IceBox = 112,
  Griller = 113,
  GiftBox = 114,
  CoffeeBoard = 115,
  PlantPot = 116,
  GoldenTray = 117,
  CurtainLayer = 118,
  Coverlet = 119,
  MoneyBag = 120,
  MachineCream = 121,
  MagicNest = 122,
  FlowerPot = 123,
}

[ExecuteInEditMode]
public class LevelEditComponent : MonoBehaviour
{
  LevelEditor levelEditor;

  [Header("Box Setting")]
  public MapComponent component1;
  public MapComponent component2;

  [Range(1, 3)]
  public int WoodBoxHealth = 1;
  [Range(1, 3)]
  public int IceBoxHealth = 1;

  [Header("Use if have normalTray component")]
  public NormalTrayData NormalTrayData;

  [HideInInspector] public int GiftBoxHealth = 2;
  [HideInInspector] public int CoffeeBoardHealth = 4;
  [HideInInspector] public int PlantPotHealth = 3;
  [HideInInspector] public int CoverLetHealth = 1;
  [HideInInspector] public int MoneyBagHealth = 2;
  [HideInInspector] public int MachineCreamHealth = 2;
  [HideInInspector] public int FlowerPotHealth = 2;

  private Color _colorWoodBox = Color.red;
  private Color _colorGrass = Color.green;
  private Color _colorVideoAds = Color.blue;
  private Color _colorIceBox = Color.yellow;
  private Color _colorGriller = Color.magenta;
  private Color _colorGiftBox;
  private Color _colorCoffeeBoard;
  private Color _colorPlantPot = Color.cyan;
  private Color _colorGoldenTray;
  private Color _colorCurtainLayer;
  private Color _colorCoverlet;
  private Color _colorMoneyBag;
  private Color _colorMachineCream;
  private Color _colorNormalTray;
  private Color _colorMagicNest;
  private Color _colorFlowerPot;

  void Update()
  {
    _colorGiftBox = new Color(0.48f, 0f, 0.651f, 1f);
    _colorCoffeeBoard = new Color(0.53f, 0.2f, 0.1f, 1);
    _colorGoldenTray = new Color(0.67f, 0.67f, 0, 1);
    _colorCurtainLayer = new Color(0, 0.4f, 0, 1);
    _colorCoverlet = new Color(0.445f, 0, 1, 1);
    _colorMoneyBag = new(0.945f, 0.4f, 0.475f, 1);
    _colorMachineCream = new(0.25f, 0.32f, 0.77f, 1);
    _colorNormalTray = new(1, 1, 0, 1);
    _colorMagicNest = new(0, 0.15f, 1, 1);
    _colorFlowerPot = new(0.74f, 0.125f, 0, 1);

    TrySetColorMapComponent(component1);
    TrySetColorMapComponent(component2);

    if (component1 == MapComponent.None && component2 == MapComponent.None)
    {
      SetColor(Color.white);
    }
  }

  void SetColor(Color color)
  {
    if (GetComponent<SpriteRenderer>().color.Equals(color)) return;
    GetComponent<SpriteRenderer>().color = color;
  }

  public void InjextLevelEditor(LevelEditor levelEditor)
  {
    this.levelEditor = levelEditor;
  }

  private void TrySetColorMapComponent(MapComponent component)
  {
    if (component == MapComponent.WoodBox) SetColor(_colorWoodBox);
    if (component == MapComponent.Grasses) SetColor(_colorGrass);
    if (component == MapComponent.VideoAd) SetColor(_colorVideoAds);
    if (component == MapComponent.IceBox) SetColor(_colorIceBox);
    if (component == MapComponent.Griller) SetColor(_colorGriller);
    if (component == MapComponent.GiftBox) SetColor(_colorGiftBox);
    if (component == MapComponent.CoffeeBoard) SetColor(_colorCoffeeBoard);
    if (component == MapComponent.PlantPot) SetColor(_colorPlantPot);
    if (component == MapComponent.GoldenTray) SetColor(_colorGoldenTray);
    if (component == MapComponent.CurtainLayer) SetColor(_colorCurtainLayer);
    if (component == MapComponent.Coverlet) SetColor(_colorCoverlet);
    if (component == MapComponent.MoneyBag) SetColor(_colorMoneyBag);
    if (component == MapComponent.MachineCream) SetColor(_colorMachineCream);
    if (component == MapComponent.NormalTray) SetColor(_colorNormalTray);
    if (component == MapComponent.MagicNest) SetColor(_colorMagicNest);
    if (component == MapComponent.FlowerPot) SetColor(_colorFlowerPot);
  }
}
