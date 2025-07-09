using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Visualize LuckyOffer
/// </summary> <summary>
/// 
/// </summary>
public partial class GameManager : MonoBehaviour
{
  [Header("Settings Lucky Reward")]
  [SerializeField] int levelUnlockLuckyReward;
  public int LevelUnlockLuckyReward { get { return levelUnlockLuckyReward; } }

  private List<LuckyOfferRewardData> _luckyOfferRewardDatas;

  public void AddLuckyOfferData(LuckyOfferRewardData luckyOfferRewardData)
  {
    _luckyOfferRewardDatas ??= new();

    for (int i = 0; i < _luckyOfferRewardDatas.Count; i++)
    {
      var luckyOfferReward = _luckyOfferRewardDatas[i];

      if (luckyOfferReward.RewardType == luckyOfferRewardData.RewardType)
      {
        luckyOfferReward.Amount += luckyOfferRewardData.Amount;

        _luckyOfferRewardDatas[i] = luckyOfferReward;
        return;
      }
    }

    _luckyOfferRewardDatas.Add(luckyOfferRewardData);
  }

  public void VisualizeLuckyOffer()
  {
    _luckyOfferRewardDatas ??= new();
    if (_luckyOfferRewardDatas.Count == 0) return;

    Sequence seq = DOTween.Sequence();

    var currentTimeAnim = 0f;

    LobbyPanel.Instance.PanelFullScreen.gameObject.SetActive(true);
    for (int i = 0; i < _luckyOfferRewardDatas.Count; i++)
    {
      var luckyOfferReward = _luckyOfferRewardDatas[i];

      LobbyPanel.Instance.VisualizeOfferReward(
        ref seq,
        ref currentTimeAnim,
        luckyOfferReward
      );
    }

    _luckyOfferRewardDatas.Clear();

    seq.InsertCallback(
      currentTimeAnim,
      () =>
      {
        LobbyPanel.Instance.PanelFullScreen.gameObject.SetActive(false);
      }
    );
  }

  public bool IsUnlockLuckyReward()
  {
    if (CurrentLevel + 1 >= levelUnlockLuckyReward) return true;

    return false;
  }
}