using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDesignObj", menuName = "ScriptableObjects/LevelDesignObj", order = 1)]
public class LevelDesignObj : ScriptableObject
{
  [Tooltip("Decide which color should be spawn in this level")]
  public int[] availableColors;

  [Tooltip("Set unlock or lock items in this specific level. True is unlock")]
  public bool[] powerUnlockItems;

  [Tooltip("Each level should have max amount is 3 phases")]
  public PhaseInLevel[] phases;

  [Tooltip("Positions of empty boxes in this level. 0 mean nothing and 1 mean have one, 111 for ads")]
  public int[] emptyBoxes;

  [Tooltip("Grasses in this level, 1 is grasses")]
  public int[] grasses;

  [Tooltip("Positions of ice boxes in this level. 0 mean nothing and 1 mean have one, 111 for ads")]
  public int[] iceBoxes;

  [Tooltip("Positions of griller in this level. 0 mean nothing and 1 mean have one")]
  public int[] grillers;

  public int[] giftBoxs;
  public int[] coffeeBoards;
  public int[] plantPots;
  public int[] goldenTrays;
  public int[] coverLets;
  public int[] MoneyBags;
  public int[] MachineCreams;
  public int[] MagicNests;
  public int[] FlowerPots;
  public NormalTrayData[] NormalTrayDatas;
  public CurtainLayerData[] curtainLayerDatas; 
  public CupBoardData[] CupBoardDatas;
  public BeverageFridgeData[] BeverageFridgeDatas;

  [Tooltip("Phase of table in game. 0 is default")]
  public int phaseTable;
  public int GameDifficulty;
}

[Serializable]
public class LevelDesignDatas
{
  public LevelDesignData[] datas;
}

[Serializable]
public class LevelDesignData
{
  public int Index;
  public int[] availableColors;
  public bool[] powerUnlockItems;
  public PhaseInLevel[] phases;
  public int[] emptyBoxes;
  public int[] grasses;
  public int[] iceBoxes;
  public int[] grillers;
  public int[] giftBoxs;
  public int[] coffeeBoards;
  public int[] plantPots;
  public int[] goldenTrays;
  public int[] coverLets;
  public int[] MoneyBags;
  public int[] MachineCreams;
  public int[] MagicNests;
  public int[] FlowerPots;
  public NormalTrayData[] NormalTrayDatas;
  public CurtainLayerData[] curtainLayerDatas; 
  public CupBoardData[] CupBoardDatas; 
  public BeverageFridgeData[] BeverageFridgeDatas;
  public int phaseTable;
  public int GameDifficulty;
}