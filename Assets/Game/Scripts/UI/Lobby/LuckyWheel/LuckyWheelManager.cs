using UnityEngine;

public class LuckyWheelManager : MonoBehaviour
{
  public static LuckyWheelManager Instance { get; private set; }

  static int _maxAccumulation = 50;
  static public int MaxAccumulation
  {
    get { return _maxAccumulation; }
    set
    {

    }
  }

  static int _secondAccumulation = 20;
  static public int SecondAccumulation
  {
    get { return _secondAccumulation; }
    set
    {

    }
  }

  static int _firstAccumulation = 10;
  static public int FirstAccumulation
  {
    get { return _firstAccumulation; }
    set
    {
      
    }
  }

  public LuckyWheelData LuckyWheelData;


  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;

      InitBeginData();
    }
    else Destroy(gameObject);
  }

  void InitBeginData()
  {
    var _LuckyWheelData = HoangNam.SaveSystem.LoadWith<LuckyWheelData>(
        KeyString.NAME_LUCKY_WHEEL_DATA
    );
    if (_LuckyWheelData == null)
    {
      LuckyWheelData = new LuckyWheelData
      {
        AccumulationAmount = 0,
        CompletedCount = 0
      };
      return;
    }
    LuckyWheelData = new LuckyWheelData
    {
      AccumulationAmount = _LuckyWheelData.AccumulationAmount,
      CompletedCount = _LuckyWheelData.CompletedCount,
      ChestOpennedCount = _LuckyWheelData.ChestOpennedCount
    };
  }

  public void SaveLuckyWheelProgress()
  {
    HoangNam.SaveSystem.SaveWith(LuckyWheelData, KeyString.NAME_LUCKY_WHEEL_DATA);
  }
}
