using System;
using Unity.Mathematics;
using UnityEngine;
using HoangNam;
using Spine.Unity;
using DG.Tweening;

public enum GameDifficulty
{
  Noob,
  Beginner,
  Easy,
  Normal,
  Hard,
  VeryHard,
  ExtremelyDifficult,
}

public partial class ItemManager : MonoBehaviour
{
  public static ItemManager Instance { get; private set; }

  [Header("External Dependencies")]
  [SerializeField] GridWorld cupGrid;
  public GridWorld CupGrid { get { return cupGrid; } }
  [SerializeField] GridWorld trayGrid;
  public GridWorld TrayGrid { get { return trayGrid; } }

  [Header("Internal Dependencies")]
  [SerializeField] SkeletonAnimation boxPackedPref;
  [SerializeField] SkeletonAnimation boxPackedGoldenPref;
  [SerializeField] Sprite comboTextSpr;
  public Sprite ComboTextSpr { get { return comboTextSpr; } }
  [SerializeField] GameObject videoAdsPrefab;
  [SerializeField] ThemeObj[] themes;
  [SerializeField] Transform tableDepot;

  [Header("Colors")]
  [SerializeField] Color grassColor;
  [SerializeField] Color grillerColor;
  [SerializeField] Color giftBoxColor;
  [SerializeField] Color coffeeBoardColor;
  [SerializeField] Color goldenBoxColor;
  [SerializeField] Color cupBoardColor;
  [SerializeField] Color machineCreamColor;
  [SerializeField] Color magicNestColor;
  [SerializeField] Color leaveFlowerColor;

  public ThemeObj[] Themes { get { return themes; } }

  [Header("Settings")]
  public bool IsPacking;
  [Range(0, 1)]
  public float SlowFactor;
  public int CurrentThemeIndex;

  [Header("Difficulty Settings")]
  [Tooltip("more value for easy")]
  [Range(20f, 100f)]
  [SerializeField] float matchedQuestCupsRate;
  public float MatchedAvailableCupsRate { get { return matchedQuestCupsRate; } }
  [Tooltip("more value for easy")]
  [Range(2, 5)]
  [SerializeField] int maxSpawnCupsAmount;
  public int MaxSpawnCupsAmount { get { return maxSpawnCupsAmount; } }
  [Tooltip("less value for easy")]
  [Range(0f, 100f)]
  [SerializeField] float pairSpawnRate;
  public float PairSpawnRate { get { return pairSpawnRate; } }
  [Tooltip("less value for easy")]
  [Range(0f, 100f)]
  [SerializeField] float cupsMixedRate;
  public float CupsMixedRate { get { return cupsMixedRate; } }
  [Tooltip("less value for easy")]
  GameDifficulty _gameDifficultyIndex;

  void Start()
  {
    if (Instance == null)
    {
      InitPool();
      InitArray();

      Instance = this;
    }
    else Destroy(gameObject);

    GameManager.Instance.onGameStateChanged += GameManager_onGameStateChanged;
  }

  private void OnDestroy()
  {
    GameManager.Instance.onGameStateChanged -= GameManager_onGameStateChanged;
  }

  private void GameManager_onGameStateChanged(GameState state)
  {

  }

  void InitPool()
  {
    InitCupPool();
    InitWoodBoxPool();
    InitIceBoxPool();
    InitGrassPool();
    InitCoinPool();
    InitGrillerPool();
    InitGiftBoxesPool();
    InitCoffeeBoardsPool();
    InitPlantPotsPool();
    InitGoldenTraysPool();
    InitCoverLetPool();
    InitCurtainLayerPool();
    InitMoneyBagsPool();
    InitCupBoardsPool();
    InitMachineCreamsPool();
    InitMagicNestsPool();
    InitFlowerPotPool();
    InitLeavesFlowerPool();
    InitBeverageFridgesPool();
  }

  void InitArray()
  {
    Coins = new GameObject[trayGrid.GridSize.x * trayGrid.GridSize.y];
  }

  void OnTakeObjFromPool(GameObject obj)
  {
    obj.SetActive(true);
  }

  void OnReturnObjFromPool(GameObject obj)
  {
    obj.SetActive(false);
  }

  void OnDestroyPoolObj(GameObject obj)
  {
    Destroy(obj);
  }

  public void ClearItems()
  {
    ClearWoodBoxes();
    ClearGrasses();
    ClearCoins();
    ClearIceBoxes();
    ClearGrillers();
    ClearGiftBoxes();
    ClearCoffeeBoards();
    ClearPlantPots();
    ClearGoldenTrays();
    ClearCoverLets();
    ClearCurtainLayers();
    ClearMoneyBags();
    ClearCupBoards();
    ClearMachineCreams();
    ClearMagicNests();
    ClearFlowerPots();
    ClearLeavesFlowers();
    ClearBeverageFridges();
  }

  public void SetGameDifficulty(GameDifficulty gameDifficultyIndex)
  {
    _gameDifficultyIndex = gameDifficultyIndex;
    switch ((int)gameDifficultyIndex)
    {
      case 0: // a super easy mode which is made of chilling value just for noob
        matchedQuestCupsRate = 100;
        maxSpawnCupsAmount = 5;
        pairSpawnRate = 0;
        cupsMixedRate = 0;
        break;
      case 1:
        matchedQuestCupsRate = 85;
        maxSpawnCupsAmount = 4;
        pairSpawnRate = 0;
        cupsMixedRate = 25;
        break;
      case 2:
        matchedQuestCupsRate = 75;
        maxSpawnCupsAmount = 3;
        pairSpawnRate = 15;
        cupsMixedRate = 45;
        break;
      case 3:
        matchedQuestCupsRate = 72;
        maxSpawnCupsAmount = 3;
        pairSpawnRate = 30;
        cupsMixedRate = 55;
        break;
      case 4:
        matchedQuestCupsRate = 70;
        maxSpawnCupsAmount = 3;
        pairSpawnRate = 50;
        cupsMixedRate = 65;
        break;
      case 5:
        matchedQuestCupsRate = 65;
        maxSpawnCupsAmount = 3;
        pairSpawnRate = 70;
        cupsMixedRate = 75;
        break;
      case 6: // an extremely difficult mode which is the game is pretty much just end there if this thing enable
        matchedQuestCupsRate = 0;
        maxSpawnCupsAmount = 2;
        pairSpawnRate = 100;
        cupsMixedRate = 100;
        break;
      default:
        matchedQuestCupsRate = 75;
        maxSpawnCupsAmount = 3;
        pairSpawnRate = 11;
        cupsMixedRate = 25;
        break;
    }
  }

  public GameDifficulty GetGameDifficulty()
  {
    return _gameDifficultyIndex;
  }

  public bool IsFullfilQuestWith(
    float3 fromPos,
    int boxColorValue, int grassValue,
    int grillerValue, int giftBoxValue,
    int coffeeBoardValue, int goldenTrayValue,
    int cupBoardValue, int machineCreamValue,
    int magicNestValue, int leaveFlowerValue,
    int amountBread, int amountGiftBox,
    int amountCoffeeBoard, int amountCupBoard,
    int amountMachineCream, int amountMagicNest,
    Action<int, int> onCompletd = null,
    ColorIndex _colorIndex = ColorIndex.Blue)
  {
    return true;
  }

  private void PlayScaleIconQuestWith(Transform destPosQuest)
  {
    var leaveImg = destPosQuest.parent.GetChild(1);

    DOTween.Kill(leaveImg);
    leaveImg.DOScale(
      new float3(1, 1, 1) * 1.3f,
      0.3f
    )
    .SetLoops(2, LoopType.Yoyo)
    .SetTarget(leaveImg);
  }

  public void TryDropCoinAt(float3 pos, bool _isCombo = false)
  {
    var rand = UnityEngine.Random.Range(0, 1f);
    if (_isCombo == true) rand = 1;
    if (rand < .5f) return;
    var coin2 = ItemManager.Instance.SpawnCoinAt(pos);
    coin2.GetComponent<CoinControl>().CoinAmount = 1;
    LeanTween.move(coin2, pos + new float3(0, .5f, 0), .1f).setLoopPingPong(2);
  }

  public void ClearValueGrid()
  {
    for (int i = 0; i < trayGrid.Grid.Length; i++)
    {
      var worldPos = trayGrid.ConvertIndexToWorldPos(i);

      trayGrid.SetValueAt(worldPos, 0);
    }
  }
}
