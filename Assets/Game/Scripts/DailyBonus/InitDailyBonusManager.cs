using UnityEngine;

public partial class GameManager : MonoBehaviour
{
  [Header("DailyBonus manager")]
  [SerializeField] EventDataNguyen dailyBonusData;
  public EventDataNguyen DailyBonusData { get { return dailyBonusData; } }
  [SerializeField] RewardData[] data;
  public RewardData[] RewardDatas { get { return data; } }
}