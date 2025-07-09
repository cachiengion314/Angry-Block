using HoangNam;
using UnityEngine;

public partial class LevelEditor : MonoBehaviour
{
  private void SaveGrassesFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.grasses != null && levelDesignObj.grasses.Length > 0)
    {
      levelDesignObj.grasses[index] = 0;
    }
  }

  private void SaveEmptyBoxesFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.emptyBoxes != null && levelDesignObj.emptyBoxes.Length > 0)
    {
      levelDesignObj.emptyBoxes[index] = 0;
    }
  }

  private void SaveIceBoxesFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.iceBoxes != null && levelDesignObj.iceBoxes.Length > 0)
    {
      levelDesignObj.iceBoxes[index] = 0;
    }
  }

  private void SaveGrillersFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.grillers == null || levelDesignObj.grillers.Length == 0)
    {
      levelDesignObj.grillers = new int[24];
    }

    if (levelDesignObj.grillers != null && levelDesignObj.grillers.Length > 0)
    {
      levelDesignObj.grillers[index] = 0;
    }
  }

  private void SaveGiftBoxesFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.giftBoxs == null || levelDesignObj.giftBoxs.Length == 0)
    {
      levelDesignObj.giftBoxs = new int[24];
    }

    if (levelDesignObj.giftBoxs != null && levelDesignObj.giftBoxs.Length > 0)
    {
      levelDesignObj.giftBoxs[index] = 0;
    }
  }

  private void SaveCoffeeBoardsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.coffeeBoards == null || levelDesignObj.coffeeBoards.Length == 0)
    {
      levelDesignObj.coffeeBoards = new int[24];
    }

    if (levelDesignObj.coffeeBoards != null && levelDesignObj.coffeeBoards.Length > 0)
    {
      levelDesignObj.coffeeBoards[index] = 0;
    }
  }

  private void SavePlantPotsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.plantPots == null || levelDesignObj.plantPots.Length == 0)
    {
      levelDesignObj.plantPots = new int[24];
    }

    if (levelDesignObj.plantPots != null && levelDesignObj.plantPots.Length > 0)
    {
      levelDesignObj.plantPots[index] = 0;
    }
  }

  private void SaveGoldenTraysFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.goldenTrays == null || levelDesignObj.goldenTrays.Length == 0)
    {
      levelDesignObj.goldenTrays = new int[24];
    }

    if (levelDesignObj.goldenTrays != null && levelDesignObj.goldenTrays.Length > 0)
    {
      levelDesignObj.goldenTrays[index] = 0;
    }
  }

  private void SaveCoverLetsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.coverLets == null || levelDesignObj.coverLets.Length == 0)
    {
      levelDesignObj.coverLets = new int[24];
    }

    if (levelDesignObj.coverLets != null && levelDesignObj.coverLets.Length > 0)
    {
      levelDesignObj.coverLets[index] = 0;
    }
  }

  private void SaveMoneyBagsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.MoneyBags == null || levelDesignObj.MoneyBags.Length == 0)
    {
      levelDesignObj.MoneyBags = new int[24];
    }

    if (levelDesignObj.MoneyBags != null && levelDesignObj.MoneyBags.Length > 0)
    {
      levelDesignObj.MoneyBags[index] = 0;
    }
  }

  private void SaveMachineCreamsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.MachineCreams == null || levelDesignObj.MachineCreams.Length == 0)
    {
      levelDesignObj.MachineCreams = new int[24];
    }

    if (levelDesignObj.MachineCreams != null && levelDesignObj.MachineCreams.Length > 0)
    {
      levelDesignObj.MachineCreams[index] = 0;
    }
  }

  private void SaveNormalTrayDatasFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.NormalTrayDatas == null || levelDesignObj.NormalTrayDatas.Length == 0)
    {
      Utility.Print("Save");
      levelDesignObj.NormalTrayDatas = new NormalTrayData[24];
    }

    if (levelDesignObj.NormalTrayDatas != null && levelDesignObj.NormalTrayDatas.Length > 0)
    {
      if (levelComponentsParent.Components[index].NormalTrayData.ColorCups == null
          || levelComponentsParent.Components[index].NormalTrayData.ColorCups.Length == 0)
      {
        levelDesignObj.NormalTrayDatas[index] = new NormalTrayData();
        return;
      }

      var normalTrayData = new NormalTrayData
      {
        ColorCups = new int[levelComponentsParent.Components[index].NormalTrayData.ColorCups.Length]
      };

      for (int i = 0; i < normalTrayData.ColorCups.Length; i++)
      {
        normalTrayData.ColorCups[i] = levelComponentsParent.Components[index].NormalTrayData.ColorCups[i];
      }

      levelDesignObj.NormalTrayDatas[index] = normalTrayData;
    }
  }

  private void SaveMagicNestsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.MagicNests == null || levelDesignObj.MagicNests.Length == 0)
    {
      levelDesignObj.MagicNests = new int[24];
    }

    if (levelDesignObj.MagicNests != null && levelDesignObj.MagicNests.Length > 0)
    {
      levelDesignObj.MagicNests[index] = 0;
    }
  }

  private void SaveFlowerPotsFrom(ref LevelDesignObj levelDesignObj, int index)
  {
    if (levelDesignObj.FlowerPots == null || levelDesignObj.FlowerPots.Length == 0)
    {
      levelDesignObj.FlowerPots = new int[24];
    }

    if (levelDesignObj.FlowerPots != null && levelDesignObj.FlowerPots.Length > 0)
    {
      levelDesignObj.FlowerPots[index] = 0;
    }
  }

  private void SaveComponentUIsFrom(ref LevelDesignObj levelDesignObj, LevelEditComponent[] components, int index)
  {
    var com1 = components[index].component1;
    var com2 = components[index].component2;

    SaveComponentUIFrom(ref levelDesignObj, components, com1, index);
    SaveComponentUIFrom(ref levelDesignObj, components, com2, index);
  }

  private void SaveComponentUIFrom(ref LevelDesignObj levelDesignObj, LevelEditComponent[] components, MapComponent component, int index)
  {
    var woodBoxHealth = components[index].WoodBoxHealth;
    var iceBoxHealth = components[index].IceBoxHealth;
    var giftBoxHealth = components[index].GiftBoxHealth;
    var coffeeBoardHealth = components[index].CoffeeBoardHealth;
    var plantPotHealth = components[index].PlantPotHealth;
    var coverLetHealth = components[index].CoverLetHealth;
    var moneyBagHealth = components[index].MoneyBagHealth;
    var machineCreamHealth = components[index].MachineCreamHealth;
    var flowerPotHealth = components[index].FlowerPotHealth;

    if (component == MapComponent.Grasses) levelDesignObj.grasses[index] = 1;
    else if (component == MapComponent.WoodBox) levelDesignObj.emptyBoxes[index] = woodBoxHealth;
    else if (component == MapComponent.VideoAd) levelDesignObj.emptyBoxes[index] = 111;
    else if (component == MapComponent.IceBox) levelDesignObj.iceBoxes[index] = iceBoxHealth;
    else if (component == MapComponent.Griller) levelDesignObj.grillers[index] = 1;
    else if (component == MapComponent.GiftBox) levelDesignObj.giftBoxs[index] = giftBoxHealth;
    else if (component == MapComponent.CoffeeBoard) levelDesignObj.coffeeBoards[index] = coffeeBoardHealth;
    else if (component == MapComponent.PlantPot) levelDesignObj.plantPots[index] = plantPotHealth;
    else if (component == MapComponent.Coverlet) levelDesignObj.coverLets[index] = coverLetHealth;
    else if (component == MapComponent.GoldenTray) levelDesignObj.goldenTrays[index] = 1;
    else if (component == MapComponent.MoneyBag) levelDesignObj.MoneyBags[index] = moneyBagHealth;
    else if (component == MapComponent.MachineCream) levelDesignObj.MachineCreams[index] = machineCreamHealth;
    else if (component == MapComponent.MagicNest) levelDesignObj.MagicNests[index] = 1;
    else if (component == MapComponent.FlowerPot) levelDesignObj.FlowerPots[index] = flowerPotHealth;
  }
}