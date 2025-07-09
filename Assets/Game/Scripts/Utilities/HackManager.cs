using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackManager : MonoBehaviour
{
  [SerializeField] TMP_InputField levelInput;
  [SerializeField] Button getDeviceIdBtn;
  [SerializeField] TMP_Text versionText;
  [SerializeField] TMP_Text deviceIdText;

  [Header("Hack")]
  [SerializeField] TMP_InputField ticketEggInput;
  [SerializeField] TMP_InputField coinInput;
  [SerializeField] TMP_InputField booster1Input;
  [SerializeField] TMP_InputField booster2Input;
  [SerializeField] TMP_InputField booster3Input;
  [SerializeField] TMP_InputField booster4Input;
  [SerializeField] TMP_InputField ticketInput;

  public void AddCoin(int amount)
  {
    GameManager.Instance.CurrentCoin += amount;
  }

  public void ChooseDayAt(int day)
  {
    DailyRewardModal.Instance.FakeToday = day;
  }

  public void GoToLevel()
  {
    var isInt = int.TryParse(levelInput.text, out int level);
    if (!isInt) return;
    GameManager.Instance.CurrentLevel = level - 1;

    GameManager.Instance.ConsiderTutorial();
    GameManager.Instance.LoadSceneFrom("Gameplay");
  }

  public void ShowAds()
  {
    GameManager.Instance.IsRemoveAds = false;
    LevelPlayAds.Instance.ShowBanner();
  }

  public void HideAds()
  {
    GameManager.Instance.IsRemoveAds = true;
    LevelPlayAds.Instance.HideBanner();
  }

  public void GetDeviceId()
  {
    var deviceID = SystemInfo.deviceUniqueIdentifier;
    deviceIdText.text = deviceID;
  }

  public void HackInfinityHearth()
  {
    HeartSystem.Instance.AddInfinityHeartTime(5);
  }

  public void HackTicket()
  {
    GameManager.Instance.CurrentTicket++;
    GameManager.Instance.CurrentTicketNoel++;
  }

  public void Increase1()
  {
    GameManager.Instance.ProgressBalloon += 2;
  }
  public void Increase20()
  {
    GameManager.Instance.ProgressBalloon += 40;
  }

  public void ClickHackTicketEgg()
  {
    var isInt = int.TryParse(ticketEggInput.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;
    GameManager.Instance.MoveEggTicketsAmount = amount;
  }

  public void ClickHackCoin()
  {
    var isInt = int.TryParse(coinInput.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;
    GameManager.Instance.CurrentCoin = amount;
  }

  public void ClickHackBooster1()
  {
    var isInt = int.TryParse(booster1Input.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;
    GameManager.Instance.CurrentRefresh = amount;
  }

  public void ClickHackBooster2()
  {
    var isInt = int.TryParse(booster2Input.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;
    GameManager.Instance.CurrentHammer = amount;
  }

  public void ClickHackBooster3()
  {
    var isInt = int.TryParse(booster3Input.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;
    GameManager.Instance.CurrentRocket = amount;
  }

  public void ClickHackBooster4()
  {
    var isInt = int.TryParse(booster4Input.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;
    GameManager.Instance.CurrentSwap = amount;
  }

  public void ClickHackTicket()
  {
    var isInt = int.TryParse(ticketInput.text, out int amount);
    if (!isInt) return;
    if (amount < 0) return;

    GameManager.Instance.CurrentTicket = amount;
    // GameManager.Instance.CurrentTicketNoel = amount;
  }
}
