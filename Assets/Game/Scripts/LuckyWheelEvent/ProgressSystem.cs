using UnityEngine;
using UnityEngine.UI;

public class ProgressSystem : MonoBehaviour
{
  public static ProgressSystem Instance { get; private set; }

  [SerializeField] Slider slider;
  [SerializeField] GiftCtrl[] giftCtrls;

  private void Awake()
  {
    Instance = this;
  }

  private void Start()
  {
    GameManager.onChangeAmountSpin += ChangeAmountSpin;
    GameManager.onChangeAmountSpin += UpdateSlider;
    ChangeAmountSpin();
    UpdateSlider();
  }

  private void OnDestroy()
  {
    GameManager.onChangeAmountSpin -= ChangeAmountSpin;
    GameManager.onChangeAmountSpin -= UpdateSlider;
  }

  void ChangeAmountSpin()
  {
    var amountSpin = GameManager.Instance.CurrentSpinNoel;
    foreach (var giftCtrl in giftCtrls)
    {
      giftCtrl.UpdateProgress(amountSpin);
      if (amountSpin < giftCtrl.spinMax)
      {
        giftCtrl.NotReceived();
      }
    }
    if (amountSpin >= giftCtrls[0].spinMax)
    {
      if (GameManager.Instance.IsReceivedGift1)
        giftCtrls[0].Received();
      else giftCtrls[0].ReachMilestone();
    }
    if (amountSpin >= giftCtrls[1].spinMax)
    {
      if (GameManager.Instance.IsReceivedGift2)
        giftCtrls[1].Received();
      else giftCtrls[1].ReachMilestone();
    }
    if (amountSpin >= giftCtrls[2].spinMax)
    {
      if (GameManager.Instance.IsReceivedGift3)
        giftCtrls[2].Received();
      else giftCtrls[2].ReachMilestone();
    }
  }

  public void ReceivedGift1()
  {
    giftCtrls[0].Receive();
    GameManager.Instance.IsReceivedGift1 = true;
  }
  public void ReceivedGift2()
  {
    giftCtrls[1].Receive();
    GameManager.Instance.IsReceivedGift2 = true;
  }
  public void ReceivedGift3()
  {
    giftCtrls[2].Receive();
    GameManager.Instance.IsReceivedGift3 = true;
  }

  void UpdateSlider()
  {
    int amountSpin = GameManager.Instance.CurrentSpinNoel;
    int maxAmountSpin = giftCtrls[2].spinMax;
    float value = (float)amountSpin / maxAmountSpin;
    if (value > 1) value = 1;
    slider.value = value;
  }
  public void Test()
  {
    GameManager.Instance.CurrentTicketNoel++;
    GameManager.Instance.CurrentSpinNoel++;
  }

  public GiftCtrl GetGiftControlAt(int index)
  {
    return giftCtrls[index];
  }
}