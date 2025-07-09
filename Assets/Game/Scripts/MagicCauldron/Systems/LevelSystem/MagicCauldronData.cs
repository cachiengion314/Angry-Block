using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicCauldronData", menuName = "MagicCauldronData", order = 0)]
public class RandomDataObj : ScriptableObject
{
    public int[] InitOrders;
    public int[] CurrentEggHolder;
    public int[] DestinationOrders;
}
[Serializable]
public struct RandomDatas
{
    public RandomData[] randomDatas;
}

[Serializable]
public struct RandomData
{
    public int[] InitOrders;
    public int[] CurrentEggHolder;
    public int[] DestinationOrders;
}
