using System;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
  Menu,
  Gameplay,
  GamepPause,
  Gameover,
  Gamewin,
  Tutorial,
}

public partial class GameManager : MonoBehaviour
{
  public event Action OnBooster1Change;
  public event Action OnBooster2Change;
  public event Action OnBooster3Change;
  public event Action OnSoundChange;
  public event Action OnMusicChange;
  public event Action OnHapticChange;
  public event Action OnCoinChange;


  public static GameManager Instance { get; private set; }

  [Header("Events")]
  public Action<GameState> onGameStateChanged;

  [Header("User Settings")]
  [Range(19, 20)]
  [SerializeField] int levelIndexCapacity;
  public int LevelIndexCapacity
  {
    get { return levelIndexCapacity; }
  }

  GameState _gameState;

  public int Booster1
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_1, 3);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_1, value);
      OnBooster1Change?.Invoke();
    }
  }

  public int Booster2
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_2, 3);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_2, value);
      OnBooster2Change?.Invoke();
    }
  }

  public int Booster3
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_3, 3);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_3, value);
      OnBooster3Change?.Invoke();
    }
  }

  public bool IsSoundOn
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_SOUND_ON, 1) == 1;
    set
    {
      if (!value) FirebaseAnalytics.LogEvent(KeyString.FIREBASE_SOUND_DISABLED);
      PlayerPrefs.SetInt(KeyString.KEY_IS_SOUND_ON, value ? 1 : 0);
      OnSoundChange?.Invoke();
    }
  }

  public bool IsMusicOn
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_MUSIC_ON, 1) == 1;
    set
    {
      if (!value) FirebaseAnalytics.LogEvent(KeyString.FIREBASE_MUSIC_DISABLED);
      PlayerPrefs.SetInt(KeyString.KEY_IS_MUSIC_ON, value ? 1 : 0);
      OnMusicChange?.Invoke();
    }
  }

  public bool IsHapticOn
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_HAPTIC_ON, 1) == 1;
    set
    {
      if(!value) FirebaseAnalytics.LogEvent(KeyString.FIREBASE_HAPTIC_DISABLED);
      PlayerPrefs.SetInt(KeyString.KEY_IS_HAPTIC_ON, value ? 1 : 0);
      OnHapticChange?.Invoke();
    }
  }

  public int CurrentCoin
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_COIN, 10000);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_COIN, value);
      OnCoinChange?.Invoke();
    }
  }

  public int CurrentLevelIndex
  {
    get
    {
      var currentLevelIdx = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_LEVEL_INDEX, 0);
      currentLevelIdx = currentLevelIdx > levelIndexCapacity ? levelIndexCapacity : currentLevelIdx;
      return currentLevelIdx;
    }
    set
    {
      var currentLevelIdx = value > levelIndexCapacity ? levelIndexCapacity : value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_LEVEL_INDEX, currentLevelIdx);
    }
  }
  /// <summary>
  /// Event section
  /// </summary>
  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
    DontDestroyOnLoad(gameObject);
  }

  /// <summary>
  /// Game State Section
  /// </summary>
  /// <returns></returns>
  public GameState GetGameState()
  {
    return _gameState;
  }

  public void SetGameState(GameState state)
  {
    _gameState = state;
    onGameStateChanged?.Invoke(state);
  }

  public void LoadSceneFrom(GameState state)
  {
    _gameState = state;
    onGameStateChanged?.Invoke(state);
  }

  public void LoadSceneFrom(string sceneName)
  {
    SceneManager.LoadScene(sceneName);
  }
}
