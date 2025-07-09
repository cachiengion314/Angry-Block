using HoangNam;
using UnityEngine;

public partial class LevelEditor : MonoBehaviour
{
  private void LoadEmptyBoxesFrom(LevelDesignObj levelDesignObj)
  {
    var emptyBoxes = levelDesignObj.emptyBoxes;

    if (emptyBoxes != null && emptyBoxes.Length > 0)
    {
      for (int i = 0; i < emptyBoxes.Length; ++i)
      {
        if (emptyBoxes[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.WoodBox;
        comp.WoodBoxHealth = emptyBoxes[i];

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadGrassesFrom(LevelDesignObj levelDesignObj)
  {
    var grasses = levelDesignObj.grasses;

    if (grasses != null && grasses.Length > 0)
    {
      for (int i = 0; i < grasses.Length; ++i)
      {
        if (grasses[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component1 = MapComponent.Grasses;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadIceBoxesFrom(LevelDesignObj levelDesignObj)
  {
    var iceBoxes = levelDesignObj.iceBoxes;

    if (iceBoxes != null && iceBoxes.Length > 0)
    {
      for (int i = 0; i < iceBoxes.Length; ++i)
      {
        if (iceBoxes[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.IceBox;
        comp.IceBoxHealth = iceBoxes[i];

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadGrillersFrom(LevelDesignObj levelDesignObj)
  {
    var grillers = levelDesignObj.grillers;
    if (grillers == null || grillers.Length == 0)
    {
      grillers = new int[24];
    }

    if (grillers != null && grillers.Length > 0)
    {
      for (int i = 0; i < grillers.Length; ++i)
      {
        if (grillers[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.Griller;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadGiftBoxesFrom(LevelDesignObj levelDesignObj)
  {
    var giftBoxs = levelDesignObj.giftBoxs;
    if (giftBoxs == null || giftBoxs.Length == 0)
    {
      giftBoxs = new int[24];
    }

    if (giftBoxs != null && giftBoxs.Length > 0)
    {
      for (int i = 0; i < giftBoxs.Length; ++i)
      {
        if (giftBoxs[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.GiftBox;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadCoffeeBoardsFrom(LevelDesignObj levelDesignObj)
  {
    var coffeeBoards = levelDesignObj.coffeeBoards;
    if (coffeeBoards == null || coffeeBoards.Length == 0)
    {
      coffeeBoards = new int[24];
    }

    if (coffeeBoards != null && coffeeBoards.Length > 0)
    {
      for (int i = 0; i < coffeeBoards.Length; ++i)
      {
        if (coffeeBoards[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.CoffeeBoard;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadPlantPotsFrom(LevelDesignObj levelDesignObj)
  {
    var plantPots = levelDesignObj.plantPots;
    if (plantPots == null || plantPots.Length == 0)
    {
      plantPots = new int[24];
    }

    if (plantPots != null && plantPots.Length > 0)
    {
      for (int i = 0; i < plantPots.Length; ++i)
      {
        if (plantPots[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.PlantPot;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadGoldenTraysFrom(LevelDesignObj levelDesignObj)
  {
    var goldenTrays = levelDesignObj.goldenTrays;
    if (goldenTrays == null || goldenTrays.Length == 0)
    {
      goldenTrays = new int[24];
    }

    if (goldenTrays != null && goldenTrays.Length > 0)
    {
      for (int i = 0; i < goldenTrays.Length; ++i)
      {
        if (goldenTrays[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component1 = MapComponent.GoldenTray;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadCoverLetsFrom(LevelDesignObj levelDesignObj)
  {
    var coverLets = levelDesignObj.coverLets;
    if (coverLets == null || coverLets.Length == 0)
    {
      coverLets = new int[24];
    }

    if (coverLets != null && coverLets.Length > 0)
    {
      for (int i = 0; i < coverLets.Length; ++i)
      {
        if (coverLets[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.Coverlet;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadMoneyBagsFrom(LevelDesignObj levelDesignObj)
  {
    var moneyBags = levelDesignObj.MoneyBags;
    if (moneyBags == null || moneyBags.Length == 0)
    {
      moneyBags = new int[24];
    }

    if (moneyBags != null && moneyBags.Length > 0)
    {
      for (int i = 0; i < moneyBags.Length; ++i)
      {
        if (moneyBags[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.MoneyBag;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadMachineCreamsFrom(LevelDesignObj levelDesignObj)
  {
    var machineCreams = levelDesignObj.MachineCreams;
    if (machineCreams == null || machineCreams.Length == 0)
    {
      machineCreams = new int[24];
    }

    if (machineCreams != null && machineCreams.Length > 0)
    {
      for (int i = 0; i < machineCreams.Length; ++i)
      {
        if (machineCreams[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.MachineCream;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadNormalTrayDatasFrom(LevelDesignObj levelDesignObj)
  {
    var normalTrayDatas = levelDesignObj.NormalTrayDatas;
    if (normalTrayDatas == null || normalTrayDatas.Length == 0)
    {
      Utility.Print("Load");
      normalTrayDatas = new NormalTrayData[24];
    }

    if (normalTrayDatas != null && normalTrayDatas.Length > 0)
    {
      for (int i = 0; i < normalTrayDatas.Length; ++i)
      {
        if (normalTrayDatas[i].ColorCups == null || normalTrayDatas[i].ColorCups.Length == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.NormalTray;

        levelComponentsParent.Components[i] = comp;
        levelComponentsParent.Components[i].NormalTrayData.ColorCups = new int[normalTrayDatas[i].ColorCups.Length];

        for (int j = 0; j < normalTrayDatas[i].ColorCups.Length; j++)
        {
          levelComponentsParent.Components[i].NormalTrayData.ColorCups[j] = normalTrayDatas[i].ColorCups[j];
        }
      }
    }
  }

  private void LoadMagicNestsFrom(LevelDesignObj levelDesignObj)
  {
    var magicNests = levelDesignObj.MagicNests;
    if (magicNests == null || magicNests.Length == 0)
    {
      magicNests = new int[24];
    }

    if (magicNests != null && magicNests.Length > 0)
    {
      for (int i = 0; i < magicNests.Length; ++i)
      {
        if (magicNests[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.MagicNest;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }

  private void LoadFlowerPotsFrom(LevelDesignObj levelDesignObj)
  {
    var flowerPots = levelDesignObj.FlowerPots;
    if (flowerPots == null || flowerPots.Length == 0)
    {
      flowerPots = new int[24];
    }

    if (flowerPots != null && flowerPots.Length > 0)
    {
      for (int i = 0; i < flowerPots.Length; ++i)
      {
        if (flowerPots[i] == 0) continue;
        var comp = levelComponentsParent.Components[i];
        comp.component2 = MapComponent.FlowerPot;

        levelComponentsParent.Components[i] = comp;
      }
    }
  }
}