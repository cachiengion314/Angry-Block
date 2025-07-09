using System;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public class QuestInPhase
{
  // 1,2,3,4,5 for normal cup
  // 6 for special cup
  [Tooltip("11: grass, 12: bread(griller), 13: giftBox, 14: coffeePack, 15: goldenTray, 16: cupPack, 17: iceCream")]
  public int cupColorIndex; 
  public int cupAmount; // also include grassAmount
  public int cupCompleted = 0;
}

[Serializable]
public class PhaseInLevel
{
  public bool shouldSpawnPairTray;
  public int[] availableColors; // if this exist, it will overwrite the main availableColors
  public QuestInPhase[] quests; // Max quest is 3
  public int currentPhase = 0;
}

[Serializable]
public class CurtainLayerData
{
  public int ColorIndex;
  public int Amount;
  public int[] PosCellInLayer;
  public int IndexToCalculateSortingLayer;
  public int2 Size;
}

[Serializable]
public class CupBoardData
{
  public int[] PosCellInLayer;
  public int IndexToCalculateSortingLayer;
}

[Serializable]
public struct NormalTrayData
{
  public int[] ColorCups;
}

[Serializable]
public class BeverageFridgeData
{
  public int[] ColorCups;
  public int[] PosCellInLayer;
  public int IndexToCalculateSortingLayer;
}