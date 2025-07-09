using System;
using UnityEngine;

public partial class LevelEditor : MonoBehaviour
{
  [Header("Level editor section")]
  public static LevelEditor Instance { get; private set; }

  [SerializeField] LevelComponentParent levelComponentsParent;
  [SerializeField] LevelDesignObj[] levelDesignObjs;

  public int[] availableColors;
  public CurtainLayerData[] curtainLayerDatas;
  public CupBoardData[] CupBoardDatas;
  public BeverageFridgeData[] BeverageFridgeDatas;
  public bool[] powerUnlockItems;
  public PhaseInLevel[] phases;

  [Range(0, 4)]
  public int phaseTable;
  public GameDifficulty gameDifficulty;
  [Range(1, 200)]
  public int levelSelected = 1;

  void Awake()
  {
    Instance = this;
  }

  public void SetCurrentLevelDesign(LevelDesignObj levelDesignObj)
  {
    levelDesignObjs[levelSelected - 1] = levelDesignObj;
  }

  public LevelDesignObj GetCurrentLevelDesign()
  {
    return levelDesignObjs[levelSelected - 1];
  }

  [NaughtyAttributes.Button]
  public void LoadLevelsFromDisk()
  {
    var _levelDesignsData = HoangNam.SaveSystem.Load<LevelDesignDatas>(
      KeyString.NAME_LEVEL_DESIGN_DATAS
    );

    for (int i = 0; i < levelDesignObjs.Length; ++i)
    {
      var _levelDesignObj = levelDesignObjs[i];
      _levelDesignObj.availableColors = _levelDesignsData.datas[i].availableColors;
      _levelDesignObj.powerUnlockItems = _levelDesignsData.datas[i].powerUnlockItems;
      _levelDesignObj.phases = _levelDesignsData.datas[i].phases;
      _levelDesignObj.phaseTable = _levelDesignsData.datas[i].phaseTable;
      _levelDesignObj.GameDifficulty = _levelDesignsData.datas[i].GameDifficulty;

      _levelDesignObj.emptyBoxes = _levelDesignsData.datas[i].emptyBoxes;
      _levelDesignObj.grasses = _levelDesignsData.datas[i].grasses;
      _levelDesignObj.iceBoxes = _levelDesignsData.datas[i].iceBoxes;
      _levelDesignObj.grillers = _levelDesignsData.datas[i].grillers;
      _levelDesignObj.giftBoxs = _levelDesignsData.datas[i].giftBoxs;
      _levelDesignObj.coffeeBoards = _levelDesignsData.datas[i].coffeeBoards;
      _levelDesignObj.plantPots = _levelDesignsData.datas[i].plantPots;
      _levelDesignObj.goldenTrays = _levelDesignsData.datas[i].goldenTrays;
      _levelDesignObj.coverLets = _levelDesignsData.datas[i].coverLets;
      _levelDesignObj.curtainLayerDatas = _levelDesignsData.datas[i].curtainLayerDatas;
      _levelDesignObj.MoneyBags = _levelDesignsData.datas[i].MoneyBags;
      _levelDesignObj.CupBoardDatas = _levelDesignsData.datas[i].CupBoardDatas;
      _levelDesignObj.MachineCreams = _levelDesignsData.datas[i].MachineCreams;
      _levelDesignObj.NormalTrayDatas = _levelDesignsData.datas[i].NormalTrayDatas;
      _levelDesignObj.MagicNests = _levelDesignsData.datas[i].MagicNests;
      _levelDesignObj.FlowerPots = _levelDesignsData.datas[i].FlowerPots;
      _levelDesignObj.BeverageFridgeDatas = _levelDesignsData.datas[i].BeverageFridgeDatas;

      if (_levelDesignObj.grillers == null || _levelDesignObj.grillers.Length == 0) _levelDesignObj.grillers = new int[24];
      if (_levelDesignObj.giftBoxs == null || _levelDesignObj.giftBoxs.Length == 0) _levelDesignObj.giftBoxs = new int[24];
      if (_levelDesignObj.coffeeBoards == null || _levelDesignObj.coffeeBoards.Length == 0) _levelDesignObj.coffeeBoards = new int[24];
      if (_levelDesignObj.plantPots == null || _levelDesignObj.plantPots.Length == 0) _levelDesignObj.plantPots = new int[24];
      if (_levelDesignObj.goldenTrays == null || _levelDesignObj.goldenTrays.Length == 0) _levelDesignObj.goldenTrays = new int[24];
      if (_levelDesignObj.coverLets == null || _levelDesignObj.coverLets.Length == 0) _levelDesignObj.coverLets = new int[24];
      if (_levelDesignObj.curtainLayerDatas == null || _levelDesignObj.curtainLayerDatas.Length == 0) _levelDesignObj.curtainLayerDatas = new CurtainLayerData[0];
      if (_levelDesignObj.MoneyBags == null || _levelDesignObj.MoneyBags.Length == 0) _levelDesignObj.MoneyBags = new int[24];
      if (_levelDesignObj.CupBoardDatas == null || _levelDesignObj.CupBoardDatas.Length == 0) _levelDesignObj.CupBoardDatas = new CupBoardData[0];
      if (_levelDesignObj.MachineCreams == null || _levelDesignObj.MachineCreams.Length == 0) _levelDesignObj.MachineCreams = new int[24];
      if (_levelDesignObj.NormalTrayDatas == null || _levelDesignObj.NormalTrayDatas.Length == 0) _levelDesignObj.NormalTrayDatas = new NormalTrayData[24];
      if (_levelDesignObj.MagicNests == null || _levelDesignObj.MagicNests.Length == 0) _levelDesignObj.MagicNests = new int[24];
      if (_levelDesignObj.FlowerPots == null || _levelDesignObj.FlowerPots.Length == 0) _levelDesignObj.FlowerPots = new int[24];
      if (_levelDesignObj.BeverageFridgeDatas == null || _levelDesignObj.BeverageFridgeDatas.Length == 0) _levelDesignObj.BeverageFridgeDatas = new BeverageFridgeData[0];

      levelDesignObjs[i] = _levelDesignObj;
    }

    Debug.Log("LoadLevelsFromDisk successfully");
  }

  [NaughtyAttributes.Button]
  public void Clear()
  {
    availableColors = new int[0];
    powerUnlockItems = new bool[4];
    phases = new PhaseInLevel[0];
    phaseTable = 0;
    curtainLayerDatas = new CurtainLayerData[0];
    CupBoardDatas = new CupBoardData[0];
    BeverageFridgeDatas = new BeverageFridgeData[0];
    gameDifficulty = GameDifficulty.VeryHard;
    levelComponentsParent.Clear();
  }

  [NaughtyAttributes.Button]
  void Load()
  {
    Clear();

    var levelDesignObj = GetCurrentLevelDesign();
    availableColors = levelDesignObj.availableColors;
    powerUnlockItems = levelDesignObj.powerUnlockItems;
    phases = levelDesignObj.phases;
    phaseTable = levelDesignObj.phaseTable;
    gameDifficulty = (GameDifficulty)levelDesignObj.GameDifficulty;
    curtainLayerDatas = levelDesignObj.curtainLayerDatas;
    CupBoardDatas = levelDesignObj.CupBoardDatas;
    BeverageFridgeDatas = levelDesignObj.BeverageFridgeDatas;

    LoadEmptyBoxesFrom(levelDesignObj);
    LoadGrassesFrom(levelDesignObj);
    LoadIceBoxesFrom(levelDesignObj);
    LoadGrillersFrom(levelDesignObj);
    LoadGiftBoxesFrom(levelDesignObj);
    LoadCoffeeBoardsFrom(levelDesignObj);
    LoadPlantPotsFrom(levelDesignObj);
    LoadGoldenTraysFrom(levelDesignObj);
    LoadCoverLetsFrom(levelDesignObj);
    LoadMoneyBagsFrom(levelDesignObj);
    LoadMachineCreamsFrom(levelDesignObj);
    LoadNormalTrayDatasFrom(levelDesignObj);
    LoadMagicNestsFrom(levelDesignObj);
    LoadFlowerPotsFrom(levelDesignObj);
  }

  [NaughtyAttributes.Button]
  public void Save()
  {
    var levelDesignObj = GetCurrentLevelDesign();

    levelDesignObj.availableColors = availableColors;
    levelDesignObj.powerUnlockItems = powerUnlockItems;
    levelDesignObj.phases = phases;
    levelDesignObj.phaseTable = phaseTable;
    levelDesignObj.GameDifficulty = (int)gameDifficulty;
    levelDesignObj.curtainLayerDatas = curtainLayerDatas;
    levelDesignObj.CupBoardDatas = CupBoardDatas;
    levelDesignObj.BeverageFridgeDatas = BeverageFridgeDatas;

    var components = levelComponentsParent.Components;
    for (int i = 0; i < components.Length; ++i)
    {
      SaveGrassesFrom(ref levelDesignObj, i);
      SaveEmptyBoxesFrom(ref levelDesignObj, i);
      SaveIceBoxesFrom(ref levelDesignObj, i);
      SaveGrillersFrom(ref levelDesignObj, i);
      SaveGiftBoxesFrom(ref levelDesignObj, i);
      SaveCoffeeBoardsFrom(ref levelDesignObj, i);
      SavePlantPotsFrom(ref levelDesignObj, i);
      SaveGoldenTraysFrom(ref levelDesignObj, i);
      SaveCoverLetsFrom(ref levelDesignObj, i);
      SaveMoneyBagsFrom(ref levelDesignObj, i);
      SaveMachineCreamsFrom(ref levelDesignObj, i);
      SaveNormalTrayDatasFrom(ref levelDesignObj, i);
      SaveMagicNestsFrom(ref levelDesignObj, i);
      SaveFlowerPotsFrom(ref levelDesignObj, i);
      SaveComponentUIsFrom(ref levelDesignObj, components, i);
    }

    SetCurrentLevelDesign(levelDesignObj);

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
        NormalTrayDatas = levelDesignObjs[i].NormalTrayDatas,
        MagicNests = levelDesignObjs[i].MagicNests,
        FlowerPots = levelDesignObjs[i].FlowerPots,
        BeverageFridgeDatas = levelDesignObjs[i].BeverageFridgeDatas,
        phaseTable = levelDesignObjs[i].phaseTable,
        GameDifficulty = levelDesignObjs[i].GameDifficulty,
      };
    }

    HoangNam.SaveSystem.Save(levelDesignDatas, KeyString.NAME_LEVEL_DESIGN_DATAS);
    Debug.Log("Save successfully");
  }
}
