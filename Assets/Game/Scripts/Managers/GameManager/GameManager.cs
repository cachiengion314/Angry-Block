using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
  Menu,
  Gameplay,
  GamepPause,
  Gameover,
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
  [SerializeField] int maxLevelIndex;
  public int MaxLevelIndex
  {
    get { return maxLevelIndex; }
  }

  GameState _gameState;

  public int Booster1
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_1, 1);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_1, value);
      OnBooster1Change?.Invoke();
    }
  }

  public int Booster2
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_2, 1);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_BOOSTER_2, value);
      OnBooster2Change?.Invoke();
    }
  }

  public int Booster3
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_BOOSTER_3, 1);
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
      PlayerPrefs.SetInt(KeyString.KEY_IS_SOUND_ON, value ? 1 : 0);
      OnSoundChange?.Invoke();
    }
  }

  public bool IsMusicOn
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_MUSIC_ON, 1) == 1;
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_IS_MUSIC_ON, value ? 1 : 0);
      OnMusicChange?.Invoke();
    }
  }

  public bool IsHapticOn
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_IS_HAPTIC_ON, 1) == 1;
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_IS_HAPTIC_ON, value ? 1 : 0);
      OnHapticChange?.Invoke();
    }
  }

  public int CurrentCoin
  {
    get => PlayerPrefs.GetInt(KeyString.KEY_CURRENT_COIN, 0);
    set
    {
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_COIN, value);
      OnCoinChange?.Invoke();
    }
  }

  public int CurrentLevel
  {
    get
    {
      var currentLevel = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_LEVEL, 0);
      currentLevel = currentLevel > maxLevelIndex ? maxLevelIndex : currentLevel;
      return currentLevel;
    }
    set
    {
      // TODO
      var currentLevel = value > maxLevelIndex ? maxLevelIndex : value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_LEVEL, currentLevel);
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
