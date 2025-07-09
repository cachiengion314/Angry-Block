using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Test : MonoBehaviour
{
  [SerializeField] TestData[] _testDatas;

  [SerializeField] Data[] _datas;

  private void Start()
  {
    StartCoroutine(IEStart());
  }

  IEnumerator IEStart()
  {
    _datas = new Data[_testDatas.Length];

    var datas = HoangNam.SaveSystem.LoadWith<TestDatas>("Test");

    for (int i = 0; i < datas.testData.Length; i++)
    {
      var testData = datas.testData[i];
      var _testData = _testDatas[i];

      _testData.Level = testData.Level;
      _testData.AvailableColor = testData.AvailableColor;
      _testData.Goals = testData.Goals;
      _testData.Boosters = testData.Boosters;
      _testData.Board = testData.Board;
      _testData.BlockMap = testData.BlockMap;
      _testData.RemoveTile = testData.RemoveTile;

      _testDatas[i] = _testData;
    }

    for (int i = 0; i < _testDatas.Length; i++)
    {
      var data = _datas[i];
      var testData = _testDatas[i];

      data.Level = ConvertLevelFrom(testData);
      data.AvailableColor = ConvertAvailableColorFrom(testData);
      data.Goals = ConvertGoalsFrom(testData);
      data.Boosters = ConvertBoosterFrom(testData);
      data.Board = ConvertBoardFrom(testData);
      data.BlockMap = ConvertBlockMapFrom(testData, data.Board);
      data.RemoveTile = ConvertRemoveTileFrom(testData, data.Board);

      _datas[i] = data;
    }

    yield break;
  }

  private int ConvertLevelFrom(TestData testData)
  {
    int level = ConvertIntFrom(testData.Level);

    return level;
  }

  private int[] ConvertAvailableColorFrom(TestData testData)
  {
    string[] strs = testData.AvailableColor.Split(",");
    int[] availableColors = new int[strs.Length];

    for (int i = 0; i < availableColors.Length; i++)
    {
      availableColors[i] = ConvertIntFrom(strs[i]);
    }

    return availableColors;
  }

  private Goal[] ConvertGoalsFrom(TestData testData)
  {
    var goals = testData.Goals.Split(";");
    Goal[] results = new Goal[goals.Length];

    for (int i = 0; i < goals.Length; i++)
    {
      string goal = goals[i];
      goal = goal.Replace("{", "");
      goal = goal.Replace("}", "");

      var str = goal.Split(",");
      var id = str[0].Split(":");
      var amount = str[1].Split(":");

      results[i].Index = ConvertIntFrom(id[1]);
      results[i].Amount = ConvertIntFrom(amount[1]);
    }

    return results;
  }

  private bool[] ConvertBoosterFrom(TestData testData)
  {
    var boosters = testData.Boosters.Split(",");
    bool[] results = new bool[boosters.Length];

    for (int i = 0; i < results.Length; i++)
    {
      results[i] = bool.Parse(boosters[i]);
    }

    return results;
  }

  private Board ConvertBoardFrom(TestData testData)
  {
    var board = testData.Board.Split(",");
    Board result = new()
    {
      Width = ConvertIntFrom(board[0]),
      Height = ConvertIntFrom(board[1])
    };

    return result;
  }

  private BlockMap[] ConvertBlockMapFrom(TestData testData, Board board)
  {
    var blockMaps = testData.BlockMap.Split(";");
    BlockMap[] results = new BlockMap[blockMaps.Length];

    for (int i = 0; i < results.Length; i++)
    {
      var blockMap = blockMaps[i];
      blockMap = blockMap.Replace("{", "");
      blockMap = blockMap.Replace("}", "");

      var poses = blockMap.Split(":")[0].Split(",");
      var valuesStr = blockMap.Split(":")[1].Split(",");

      var posX = ConvertIntFrom(poses[0]);
      var posY = ConvertIntFrom(poses[1]);

      results[i].PosIndex = posX * board.Height + posY;

      int[] values = new int[valuesStr.Length];
      for (int j = 0; j < values.Length; j++)
      {
        values[j] = ConvertIntFrom(valuesStr[j]);
      }

      results[i].Values = values;
    }

    return results;
  }

  private RemoveTile[] ConvertRemoveTileFrom(TestData testData, Board board)
  {
    var removeTiles = testData.RemoveTile.Split(";");
    RemoveTile[] results = new RemoveTile[removeTiles.Length];

    for (int i = 0; i < results.Length; i++)
    {
      var removeTile = removeTiles[i];
      removeTile = removeTile.Replace("{", "");
      removeTile = removeTile.Replace("}", "");

      var poses = removeTile.Split(":")[0].Split(",");
      var value = removeTile.Split(":")[1];

      var posX = ConvertIntFrom(poses[0]);
      var posY = ConvertIntFrom(poses[1]);
      results[i].PosIndex = posX * board.Height + posY;
      results[i].Values = ConvertIntFrom(value);
    }

    return results;
  }

  private int ConvertIntFrom(string str)
  {
    return int.Parse(str);
  }
}

[Serializable]
public struct TestDatas
{
  public TestData[] testData;
}

[Serializable]
public struct TestData
{
  public string Level;
  public string AvailableColor;
  public string Goals;
  public string Boosters;
  public string Board;
  public string BlockMap;
  public string RemoveTile;
}

[Serializable]
public struct Data
{
  public int Level;
  public int[] AvailableColor;
  public Goal[] Goals;
  public bool[] Boosters;
  public Board Board;
  public BlockMap[] BlockMap;
  public RemoveTile[] RemoveTile;
}

[Serializable]
public struct Goal
{
  public int Index;
  public int Amount;
}

[Serializable]
public struct Board
{
  public int Width;
  public int Height;
}

[Serializable]
public struct BlockMap
{
  public int PosIndex;
  public int[] Values;
}

[Serializable]
public struct RemoveTile
{
  public int PosIndex;
  public int Values;
}

