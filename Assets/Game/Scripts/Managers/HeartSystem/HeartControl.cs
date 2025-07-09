using System;
using UnityEngine;

public class HeartControl : MonoBehaviour
{
  [Header("Setting data")]
  [SerializeField] HeartData heartData;

  public void AddHeart(ref int currentHeart, int amount)
  {
    var newHeart = currentHeart + amount;

    if (IsFull(newHeart))
    {
      currentHeart = heartData.HeartMax;
      return;
    }

    currentHeart = newHeart;
  }

  public bool RemoveHeart(ref int currentHeart, int amount)
  {
    if (heartData.IsInfinityHeart) return true;
    if (currentHeart < amount) return false;

    currentHeart -= amount;
    return true;
  }

  public bool IsFull(int currentHeart)
  {
    return currentHeart >= heartData.HeartMax;
  }

  public bool IsEmpty(int currentHeart)
  {
    return currentHeart < 0;
  }

  public void SetHeartMax(int heartMax)
  {
    heartData.HeartMax = heartMax;
  }

  public void SetIsInfinityHeart(bool isInfinity)
  {
    heartData.IsInfinityHeart = isInfinity;
  }

  public int GetHeartMax()
  {
    return heartData.HeartMax;
  }

  public bool GetIsInfinityHeart()
  {
    return heartData.IsInfinityHeart;
  }
}

[Serializable]
public struct HeartData
{
  public int HeartMax;
  public bool IsInfinityHeart;
}

