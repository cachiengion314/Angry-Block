using UnityEngine;

[ExecuteInEditMode]
public class LevelComponentParent : MonoBehaviour
{
  [SerializeField] LevelEditor levelEditor;
  [SerializeField] LevelEditComponent[] components;
  public LevelEditComponent[] Components { get { return components; } }

  private void Awake()
  {
    for (int i = 0; i < components.Length; ++i)
    {
      components[i].InjextLevelEditor(levelEditor);
    }
    Debug.Log("Inject successfully!");
  }

  public void Clear()
  {
    for (int i = 0; i < components.Length; ++i)
    {
      components[i].component1 = MapComponent.None;
      components[i].component2 = MapComponent.None;
      components[i].WoodBoxHealth = 1;
      components[i].IceBoxHealth = 1;
      components[i].GiftBoxHealth = 2;
      components[i].CoffeeBoardHealth = 4;
      components[i].PlantPotHealth = 3;
      components[i].CoverLetHealth = 1;
      components[i].MoneyBagHealth = 2;
      components[i].MachineCreamHealth = 1;
      components[i].FlowerPotHealth = 2;
      components[i].NormalTrayData = new();
    }
  }
}
