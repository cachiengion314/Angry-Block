using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalCompletedNotify : MonoBehaviour
{
  [Header("Injected Dependencies")]
  [SerializeField] Sprite coinsSprite;
  [SerializeField] Sprite cupsSprite;
  [SerializeField] Sprite powerItemsSprite;
  [SerializeField] Image questImg;
  //
  [SerializeField] RectTransform goalBlock;
  [SerializeField] TMP_Text notifyContentText;
  //
  [SerializeField] RectTransform longTextBlock;
  [SerializeField] TMP_Text missionLongText;

  [SerializeField] RectTransform tickImg;
  //
  [SerializeField] TMP_Text progressText;
  [SerializeField] Slider progressBar;
  //
  [SerializeField] Sprite upSprite;
  [SerializeField] Sprite timeSprite;
  [SerializeField] Sprite gangSawSprite;

  [Tooltip("This value will be injected throughout Callback process, so its current value doesnt actually matter right now")]
  // public DailyGoalType type;

  void ShowProgressBarValueFrom(int currentAmount, int Amount)
  {
    var value = (float)currentAmount / Amount;
    progressBar.value = (float)value;
    progressText.text = currentAmount + "/" + Amount;
  }

  void ShowTextFrom(int Amount)
  {
    longTextBlock.gameObject.SetActive(true);
    // missionLongText.text = DailyGoalRule.GetTextFrom(Amount, type);
  }

  public void ShowQuestCompleted()
  {
    goalBlock.gameObject.SetActive(true);
    notifyContentText.gameObject.SetActive(false);

    // var currDailyGoal = DailyGoalManager.Instance.GetDailyGoalDataFrom(type);
    // var currLevel = currDailyGoal.CurrentLevelIndex;
    // var Amount = DailyGoalRule.GetAmountFrom(currLevel, type);
    // if (type == DailyGoalType.CollectCoin)
    // {
    //   questImg.sprite = coinsSprite;
    // }
    // else if (type == DailyGoalType.MergeCup)
    // {
    //   questImg.sprite = cupsSprite;
    // }
    // else if (type == DailyGoalType.UsePowerItem)
    // {
    //   questImg.sprite = powerItemsSprite;
    // }

    // ShowTextFrom(Amount);
    // ShowProgressBarValueFrom(Amount, Amount);

    tickImg.gameObject.SetActive(true);
  }

  public void ShowNotify(string notifyContent)
  {
    goalBlock.gameObject.SetActive(false);
    notifyContentText.gameObject.SetActive(true);
    notifyContentText.text = notifyContent;
  }
}
