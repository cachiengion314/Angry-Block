using HoangNam;
using UnityEngine;

/// <summary>
/// Firebase Remote
/// </summary> <summary>
/// 
/// </summary>
public partial class FirebaseSetup : MonoBehaviour
{
  [Header("LevelDesignData")]
  [SerializeField] LevelDesignObj[] levelDesignObjs;

  [Header("CheckInternet Modal")]
  [SerializeField] RectTransform checkInternetModal;
  public RectTransform CheckInternetModal { get { return checkInternetModal; } }

  private void LoadOverwriteLevelDataFrom(LevelDesignDatas datas)
  {
        for (int i = 0; i < levelDesignObjs.Length; ++i)
    {
      var _levelDesignObj = levelDesignObjs[i];
      _levelDesignObj.availableColors = datas.datas[i].availableColors;
      _levelDesignObj.powerUnlockItems = datas.datas[i].powerUnlockItems;
      _levelDesignObj.phases = datas.datas[i].phases;
      _levelDesignObj.phaseTable = datas.datas[i].phaseTable;
      _levelDesignObj.GameDifficulty = datas.datas[i].GameDifficulty;

      _levelDesignObj.emptyBoxes = datas.datas[i].emptyBoxes;
      _levelDesignObj.grasses = datas.datas[i].grasses;
      _levelDesignObj.iceBoxes = datas.datas[i].iceBoxes;
      _levelDesignObj.grillers = datas.datas[i].grillers;
      _levelDesignObj.giftBoxs = datas.datas[i].giftBoxs;
      _levelDesignObj.coffeeBoards = datas.datas[i].coffeeBoards;
      _levelDesignObj.plantPots = datas.datas[i].plantPots;
      _levelDesignObj.goldenTrays = datas.datas[i].goldenTrays;
      _levelDesignObj.coverLets = datas.datas[i].coverLets;
      _levelDesignObj.curtainLayerDatas = datas.datas[i].curtainLayerDatas;
      _levelDesignObj.MoneyBags = datas.datas[i].MoneyBags;
      _levelDesignObj.CupBoardDatas = datas.datas[i].CupBoardDatas;
      _levelDesignObj.MachineCreams = datas.datas[i].MachineCreams;
      _levelDesignObj.NormalTrayDatas = datas.datas[i].NormalTrayDatas;
      _levelDesignObj.MagicNests = datas.datas[i].MagicNests;
      _levelDesignObj.FlowerPots = datas.datas[i].FlowerPots;
      _levelDesignObj.BeverageFridgeDatas = datas.datas[i].BeverageFridgeDatas;

      if (_levelDesignObj.grillers == null || _levelDesignObj.grillers.Length == 0) _levelDesignObj.grillers = new int[24];
      if (_levelDesignObj.giftBoxs == null || _levelDesignObj.giftBoxs.Length == 0) _levelDesignObj.giftBoxs = new int[24];
      if (_levelDesignObj.coffeeBoards == null || _levelDesignObj.coffeeBoards.Length == 0) _levelDesignObj.coffeeBoards = new int[24];
      if (_levelDesignObj.plantPots == null || _levelDesignObj.plantPots.Length == 0) _levelDesignObj.plantPots = new int[24];
      if (_levelDesignObj.goldenTrays == null || _levelDesignObj.goldenTrays.Length == 0) _levelDesignObj.goldenTrays = new int[24];
      if (_levelDesignObj.coverLets == null || _levelDesignObj.coverLets.Length == 0) _levelDesignObj.coverLets = new int[24];
      if (_levelDesignObj.curtainLayerDatas == null || _levelDesignObj.curtainLayerDatas.Length == 0) _levelDesignObj.curtainLayerDatas = new CurtainLayerData[0];
      if (_levelDesignObj.MoneyBags == null || _levelDesignObj.MoneyBags.Length == 0) _levelDesignObj.MoneyBags = new int[24];
      if (_levelDesignObj.CupBoardDatas == null || _levelDesignObj.CupBoardDatas.Length == 0) _levelDesignObj.CupBoardDatas = new CupBoardData[0];
      if (_levelDesignObj.MachineCreams == null || _levelDesignObj.MachineCreams.Length == 0) _levelDesignObj.MachineCreams = new int[24];
      if (_levelDesignObj.NormalTrayDatas == null || _levelDesignObj.NormalTrayDatas.Length == 0) _levelDesignObj.NormalTrayDatas = new NormalTrayData[24];
      if (_levelDesignObj.MagicNests == null || _levelDesignObj.MagicNests.Length == 0) _levelDesignObj.MagicNests = new int[24];
      if (_levelDesignObj.FlowerPots == null || _levelDesignObj.FlowerPots.Length == 0) _levelDesignObj.FlowerPots = new int[24];
      if (_levelDesignObj.BeverageFridgeDatas == null || _levelDesignObj.BeverageFridgeDatas.Length == 0) _levelDesignObj.BeverageFridgeDatas = new BeverageFridgeData[0];

      levelDesignObjs[i] = _levelDesignObj;
    }

    Debug.Log("LoadOverwriteLevelDataFrom successfully");
  }
}