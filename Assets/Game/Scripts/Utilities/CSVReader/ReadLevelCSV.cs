using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class MockUpPhase
{
  public bool shouldSpawnPairTray;
  public int[] availableColors; // if this exist, it will overwrite the main availableColors
  public List<QuestInPhase> quests; // Max quest is 3
  public int currentPhase = 0;
}

[Serializable]
public class MockUpLevelData
{
  public int Level;
  public int[] AvailableColors;
  public bool[] PowerUnlockItems;
  public List<MockUpPhase> Phases;
  public int[] EmptyBoxes;
  public int[] Grasses;
  public int[] IceBoxes;
  public int[] Grillers;
  public int[] GiftBoxs;
  public int[] CoffeeBoards;
  public int[] PlantPots;
  public int[] GoldenTrays;
  public int[] CoverLets;
  public int[] CurtainLayers;
  public int[] MoneyBags;
  public int[] CupBoards;
  public int[] MachineCreams;
  public List<CurtainLayerData> CurtainLayerDatas;
  public List<CupBoardData> CupBoardDatas;
  public int PhaseTable;
  public int GameDifficulty;
}

public class ReadLevelCSV : MonoBehaviour
{
  public Dictionary<int, MockUpLevelData> levelDatabase = new();

  public Dictionary<int, MockUpLevelData> ReadDataFrom(string fileName)
  {
    List<Dictionary<string, object>> data = CSVReader.Read(fileName);

    for (int i = 0; i < data.Count; ++i)
    {
      string originalId = data[i]["Id"].ToString();
      var arr = originalId.Split("-");
      var currentLvl = ExtractValueFrom(arr[0]);

      var levelData = new MockUpLevelData();

      if (levelDatabase.ContainsKey(currentLvl))
        levelData = levelDatabase[currentLvl];

      levelData.Level = currentLvl;

      var isLvRow = arr.Length == 1;
      var isArrayRow = arr.Length == 2;
      var isQuestRow = arr.Length == 3;

      string AvailableColors = data[i]["AvailableColors"].ToString();
      string PowerUnlockItems = data[i]["PowerUnlockItems"].ToString();
      string EmptyBoxes = data[i]["EmptyBoxes"].ToString();
      string Grasses = data[i]["Grasses"].ToString();
      string IceBoxes = data[i]["IceBoxes"].ToString();
      string Grillers = data[i]["Grillers"].ToString();
      string GiftBoxs = data[i]["GiftBoxs"].ToString();
      string CoffeeBoards = data[i]["CoffeeBoards"].ToString();
      string PlantPots = data[i]["PlantPots"].ToString();
      string GoldenTrays = data[i]["GoldenTrays"].ToString();
      string CoverLets = data[i]["CoverLets"].ToString();
      string CurtainLayers = data[i]["CurtainLayers"].ToString();
      string MoneyBags = data[i]["MoneyBags"].ToString();
      string CupBoards = data[i]["CupBoards"].ToString();
      string MachineCreams = data[i]["MachineCreams"].ToString();
      string GameDifficulty = data[i]["GameDifficulty"].ToString();

      string ColorIndex = data[i]["ColorIndex"].ToString();
      string Amount = data[i]["Amount"].ToString();
      string ShouldSpawnPairTray = data[i]["ShouldSpawnPairTray"].ToString();
      string Size = data[i]["Size"].ToString();
      string PosCellInLayer = data[i]["PosCellInLayer"].ToString();
      string IndexToCalculateSortingLayer = data[i]["IndexToCalculateSortingLayer"].ToString();

      if (isLvRow)
      {
        levelData.AvailableColors = ConvertStringToIntArray(AvailableColors);
        levelData.PowerUnlockItems = ConvertToBoolArray(PowerUnlockItems);
        levelData.EmptyBoxes = ConvertStringToIntArray(EmptyBoxes);
        levelData.Grasses = ConvertStringToIntArray(Grasses);
        levelData.IceBoxes = ConvertStringToIntArray(IceBoxes);
        levelData.Grillers = ConvertStringToIntArray(Grillers);
        levelData.GiftBoxs = ConvertStringToIntArray(GiftBoxs);
        levelData.CoffeeBoards = ConvertStringToIntArray(CoffeeBoards);
        levelData.PlantPots = ConvertStringToIntArray(PlantPots);
        levelData.GoldenTrays = ConvertStringToIntArray(GoldenTrays);
        levelData.CoverLets = ConvertStringToIntArray(CoverLets);
        levelData.CurtainLayers = ConvertStringToIntArray(CurtainLayers);
        levelData.MoneyBags = ConvertStringToIntArray(MoneyBags);
        levelData.CupBoards = ConvertStringToIntArray(CupBoards);
        levelData.MachineCreams = ConvertStringToIntArray(MachineCreams);
        levelData.GameDifficulty = int.Parse(GameDifficulty);
      }
      if (isQuestRow)
      {
        var phase = ExtractValueFrom(arr[1]);
        if (levelData.Phases == null)
          levelData.Phases = new List<MockUpPhase>();

        var newQuest = new QuestInPhase()
        {
          cupAmount = int.Parse(Amount),
          cupColorIndex = int.Parse(ColorIndex)
        };
        if (phase > levelData.Phases.Count)
        {
          var newPhase = new MockUpPhase()
          {
            quests = new List<QuestInPhase>() { newQuest },
            shouldSpawnPairTray = int.Parse(ShouldSpawnPairTray) == 1,
            availableColors
              = !string.IsNullOrEmpty(AvailableColors) ? ConvertStringToIntArray(AvailableColors) : null
          };
          levelData.Phases.Add(newPhase);
        }
        else
        {
          levelData.Phases[phase - 1].quests.Add(newQuest);
          levelData.Phases[phase - 1].shouldSpawnPairTray = int.Parse(ShouldSpawnPairTray) == 1;
          levelData.Phases[phase - 1].availableColors
            = !string.IsNullOrEmpty(AvailableColors) ? ConvertStringToIntArray(AvailableColors) : null;
        }
      }
      if (isArrayRow)
      {
        var value = ExtractValueFrom(arr[1]);
        var name = ExtractWordFrom(arr[1]);

        if (name == "CurtainLayerDatas")
        {
          var sizeArr = ConvertStringToIntArray(Size);
          var newCurtainLayerData = new CurtainLayerData()
          {
            ColorIndex = int.Parse(ColorIndex),
            Amount = int.Parse(Amount),
            PosCellInLayer = ConvertStringToIntArray(PosCellInLayer),
            IndexToCalculateSortingLayer = int.Parse(IndexToCalculateSortingLayer),
            Size = new int2(sizeArr[0], sizeArr[1])
          };
          if (levelData.CurtainLayerDatas == null)
            levelData.CurtainLayerDatas = new List<CurtainLayerData>();

          if (value > levelData.CurtainLayerDatas.Count)
            levelData.CurtainLayerDatas.Add(newCurtainLayerData);
          else levelData.CurtainLayerDatas[value - 1] = newCurtainLayerData;
        }

        if (name == "CupBoardDatas")
        {
          var newCupBoardData = new CupBoardData()
          {
            PosCellInLayer = ConvertStringToIntArray(PosCellInLayer),
            IndexToCalculateSortingLayer = int.Parse(IndexToCalculateSortingLayer),
          };
          if (levelData.CupBoardDatas == null)
            levelData.CupBoardDatas = new List<CupBoardData>();

          if (value > levelData.CupBoardDatas.Count)
            levelData.CupBoardDatas.Add(newCupBoardData);
          else levelData.CupBoardDatas[value - 1] = newCupBoardData;
        }
      }

      if (levelDatabase.ContainsKey(currentLvl))
        levelDatabase[currentLvl] = levelData;
      else levelDatabase.Add(currentLvl, levelData);
    }

    return levelDatabase;
  }

  static bool[] ConvertToBoolArray(string input)
  {
    if (string.IsNullOrEmpty(input))
    {
      return Array.Empty<bool>();
    }
    return input.Split('-').Select(s => s == "1").ToArray();
  }

  static int[] ConvertStringToIntArray(string input)
  {
    return input.Split('-').Select(int.Parse).ToArray();
  }

  static int ExtractValueFrom(string strContent)
  {
    return int.Parse(Regex.Match(strContent, @"\d+").Value);
  }

  static string ExtractWordFrom(string input)
  {
    return Regex.Match(input, @"[A-Za-z]+").Value;
  }
}
