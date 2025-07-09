using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestNew : MonoBehaviour
{
  [SerializeField] TMP_InputField tMP_InputField;
  [SerializeField] LevelDesignObj levelDesignObj;

  public void Convert()
  {
    var jsonString = tMP_InputField.text;
    if (jsonString == "") return;

    var _levelDesignObj = JsonUtility.FromJson<LevelDesignData>(jsonString);

    levelDesignObj.availableColors = _levelDesignObj.availableColors;
    levelDesignObj.powerUnlockItems = _levelDesignObj.powerUnlockItems;
    levelDesignObj.phases = _levelDesignObj.phases;
    levelDesignObj.phaseTable = _levelDesignObj.phaseTable;
    levelDesignObj.GameDifficulty = _levelDesignObj.GameDifficulty;

    levelDesignObj.emptyBoxes = _levelDesignObj.emptyBoxes;
    levelDesignObj.grasses = _levelDesignObj.grasses;
    levelDesignObj.iceBoxes = _levelDesignObj.iceBoxes;
    levelDesignObj.grillers = _levelDesignObj.grillers;
    levelDesignObj.giftBoxs = _levelDesignObj.giftBoxs;
    levelDesignObj.coffeeBoards = _levelDesignObj.coffeeBoards;
    levelDesignObj.plantPots = _levelDesignObj.plantPots;
    levelDesignObj.goldenTrays = _levelDesignObj.goldenTrays;
    levelDesignObj.coverLets = _levelDesignObj.coverLets;
    levelDesignObj.curtainLayerDatas = _levelDesignObj.curtainLayerDatas;
    levelDesignObj.MoneyBags = _levelDesignObj.MoneyBags;
    levelDesignObj.CupBoardDatas = _levelDesignObj.CupBoardDatas;
    levelDesignObj.MachineCreams = _levelDesignObj.MachineCreams;

    if (levelDesignObj.grillers == null || levelDesignObj.grillers.Length == 0)
    {
      levelDesignObj.grillers = new int[24];
    }

    if (levelDesignObj.giftBoxs == null || levelDesignObj.giftBoxs.Length == 0)
    {
      levelDesignObj.giftBoxs = new int[24];
    }

    if (levelDesignObj.coffeeBoards == null || levelDesignObj.coffeeBoards.Length == 0)
    {
      levelDesignObj.coffeeBoards = new int[24];
    }

    if (levelDesignObj.plantPots == null || levelDesignObj.plantPots.Length == 0)
    {
      levelDesignObj.plantPots = new int[24];
    }

    if (levelDesignObj.goldenTrays == null || levelDesignObj.goldenTrays.Length == 0)
    {
      levelDesignObj.goldenTrays = new int[24];
    }

    if (levelDesignObj.coverLets == null || levelDesignObj.coverLets.Length == 0)
    {
      levelDesignObj.coverLets = new int[24];
    }

    if (levelDesignObj.curtainLayerDatas == null || levelDesignObj.curtainLayerDatas.Length == 0)
    {
      levelDesignObj.curtainLayerDatas = new CurtainLayerData[0];
    }

    if (levelDesignObj.MoneyBags == null || levelDesignObj.MoneyBags.Length == 0)
    {
      levelDesignObj.MoneyBags = new int[24];
    }

    if (levelDesignObj.CupBoardDatas == null || levelDesignObj.CupBoardDatas.Length == 0)
    {
      levelDesignObj.CupBoardDatas = new CupBoardData[0];
    }

    if (levelDesignObj.MachineCreams == null || levelDesignObj.MachineCreams.Length == 0)
    {
      levelDesignObj.MachineCreams = new int[24];
    }

    SceneManager.LoadScene("GameplayTool");
  }
}
