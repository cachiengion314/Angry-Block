using UnityEngine;

public enum PriceType
{
  Coin,
  Refresh,
  Rocket,
  Hammer,
  Swap,
  Ticket,
  TicketNoel,
  InfinityHeart
}

[CreateAssetMenu(fileName = "RewardData", menuName = "ScriptableObjects/RewardData", order = 1)]
public class RewardData : ScriptableObject
{
  public int[] Value;
  public Sprite Img;
  public PriceType Type;
}