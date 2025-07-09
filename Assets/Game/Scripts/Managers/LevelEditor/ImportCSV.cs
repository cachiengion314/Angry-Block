using System.Collections.Generic;
using UnityEngine;

public partial class LevelEditor : MonoBehaviour
{
  [Header("Import CSV file section")]
  [Tooltip("All CSV files should be placed at Resources folder")]
  [SerializeField] string csvFileName;
  [SerializeField] ReadLevelCSV readLevelCSV;

  // [NaughtyAttributes.Button]
  void ImportCSV()
  {
    Dictionary<int, MockUpLevelData> levelDesignDic
      = readLevelCSV.ReadDataFrom(csvFileName);

    var levelDesignDatas = new LevelDesignDatas
    {
      datas = new LevelDesignData[levelDesignObjs.Length]
    };

    for (int i = 0; i < levelDesignObjs.Length; ++i)
    {
      levelDesignDatas.datas[i] = new LevelDesignData()
      {
        Index = i,
        availableColors = levelDesignObjs[i].availableColors,
        powerUnlockItems = levelDesignObjs[i].powerUnlockItems,
        phases = levelDesignObjs[i].phases,
        emptyBoxes = levelDesignObjs[i].emptyBoxes,
        grasses = levelDesignObjs[i].grasses,
        iceBoxes = levelDesignObjs[i].iceBoxes,
        grillers = levelDesignObjs[i].grillers,
        giftBoxs = levelDesignObjs[i].giftBoxs,
        coffeeBoards = levelDesignObjs[i].coffeeBoards,
        plantPots = levelDesignObjs[i].plantPots,
        goldenTrays = levelDesignObjs[i].goldenTrays,
        coverLets = levelDesignObjs[i].coverLets,
        curtainLayerDatas = levelDesignObjs[i].curtainLayerDatas,
        MoneyBags = levelDesignObjs[i].MoneyBags,
        CupBoardDatas = levelDesignObjs[i].CupBoardDatas,
        MachineCreams = levelDesignObjs[i].MachineCreams,
        phaseTable = levelDesignObjs[i].phaseTable,
        GameDifficulty = levelDesignObjs[i].GameDifficulty,
      };
    }

    foreach (KeyValuePair<int, MockUpLevelData> kvp in levelDesignDic)
    {
      var i = kvp.Key - 1;
      if (i < 0 || i > levelDesignDatas.datas.Length - 1) continue;
      var mockData = kvp.Value;

      var Phases = new PhaseInLevel[mockData.Phases.Count];
      for (int j = 0; j < Phases.Length; ++j)
      {
        Phases[j] = new PhaseInLevel
        {
          shouldSpawnPairTray = mockData.Phases[j].shouldSpawnPairTray,
          availableColors = mockData.Phases[j].availableColors,
          quests = mockData.Phases[j].quests.ToArray()
        };
      }

      levelDesignDatas.datas[i] = new LevelDesignData()
      {
        Index = i,
        availableColors = mockData.AvailableColors,
        powerUnlockItems = mockData.PowerUnlockItems,
        phases = Phases,
        emptyBoxes = mockData.EmptyBoxes,
        grasses = mockData.Grasses,
        iceBoxes = mockData.IceBoxes,
        grillers = mockData.Grillers,
        giftBoxs = mockData.GiftBoxs,
        coffeeBoards = mockData.CoffeeBoards,
        plantPots = mockData.PlantPots,
        goldenTrays = mockData.GoldenTrays,
        coverLets = mockData.CoverLets,
        curtainLayerDatas = mockData.CurtainLayerDatas.ToArray(),
        MoneyBags = mockData.MoneyBags,
        CupBoardDatas = mockData.CupBoardDatas.ToArray(),
        MachineCreams = mockData.MachineCreams,
        phaseTable = mockData.PhaseTable,
        GameDifficulty = mockData.GameDifficulty,
      };
    }

    HoangNam.SaveSystem.Save(levelDesignDatas, KeyString.NAME_LEVEL_DESIGN_DATAS);
    Debug.Log("Import CSV file and save successfully");
  }
}