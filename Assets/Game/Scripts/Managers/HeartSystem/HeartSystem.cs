using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeartSystem : MonoBehaviour
{
  public static Action onBakedHeartSystem;
  public static Action onChangeHeart;

  public static HeartSystem Instance { get; private set; }

  //
  [Header("Settings")]
  [SerializeField] int minutePerHearts;
  public int MinutePerHearts
  {
    get
    {
      return minutePerHearts;
    }
  }

  double _useinfinityHeartTime; // Tinh theo giay (s)
  public double UseInfinityHeartTime
  {
    get
    {
      return _useinfinityHeartTime;
    }
    set
    {
      _useinfinityHeartTime = value;
      PlayerPrefs.SetInt(KeyString.KEY_USE_TIME_INFINITYHEART, (int)_useinfinityHeartTime);
      onChangeHeart?.Invoke();
    }
  }

  //
  private UnityEngine.GameObject[] _heartControls;

  //
  private DateTime _lastedTimeHeart;
  public DateTime LastedTimeHeart
  {
    get
    {
      return _lastedTimeHeart;
    }
    set
    {
      _lastedTimeHeart = value;
      PlayerPrefs.SetString(KeyString.KEY_LATEST_TIMEHEART, _lastedTimeHeart.ToString());
    }
  }

  private DateTime _lastedTimeInfinityHeart;
  public DateTime LastedTimeInfinityHeart
  {
    get
    {
      return _lastedTimeInfinityHeart;
    }
    set
    {
      _lastedTimeInfinityHeart = value;
      PlayerPrefs.SetString(KeyString.KEY_LATEST_TIME_INFINITYHEART, _lastedTimeInfinityHeart.ToString());
    }
  }

  private int _currentHeart;
  public int CurrentHeart
  {
    get
    {
      return _currentHeart;
    }
    set
    {
      _currentHeart = value;
      PlayerPrefs.SetInt(KeyString.KEY_CURRENT_HEART, _currentHeart);
      onChangeHeart?.Invoke();
    }
  }

  //
  private int _amountInvoke = 0;
  private bool _isBaked;
  private bool _isInfinityHeartOnRemote;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;

      SceneManager.activeSceneChanged += ChangedActiveScene;
      SceneManager.sceneLoaded += OnSceneLoaded;

      // InvokeOnce();
      DontDestroyOnLoad(Instance);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void OnDestroy()
  {
    SceneManager.activeSceneChanged -= ChangedActiveScene;
    SceneManager.sceneLoaded -= OnSceneLoaded;

    UseInfinityHeartTime = _useinfinityHeartTime;
    LastedTimeHeart = _lastedTimeHeart;
    LastedTimeInfinityHeart = _lastedTimeInfinityHeart;
    CurrentHeart = _currentHeart;
  }

  private void Start()
  {
    InitParameters();
  }

  private void FixedUpdate()
  {
    if (!_isBaked) return;

    CalculateHeart();
    CalculateInfinityHeart();
  }

  public void InvokeOnce()
  {
    if (_amountInvoke > 0) return;
    _amountInvoke++;

    FirebaseSetup.Instance.CheckVersion();
    InitParameters();
  }

  public void InitParameters()
  {
    var secondsPerHearts = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.config_game.energy_refill_time;
    minutePerHearts = secondsPerHearts / 60;

    var lastedTimeHeart = PlayerPrefs.GetString(KeyString.KEY_LATEST_TIMEHEART, "");
    var lastedTimeInfinityHeart = PlayerPrefs.GetString(KeyString.KEY_LATEST_TIME_INFINITYHEART, "");

    if (lastedTimeHeart == "")
    {
      lastedTimeHeart = DateTime.Now.ToString("O");
      PlayerPrefs.SetString(KeyString.KEY_LATEST_TIMEHEART, lastedTimeHeart);
    }

    if (lastedTimeInfinityHeart == "")
    {
      lastedTimeInfinityHeart = DateTime.Now.ToString("O");
      PlayerPrefs.SetString(KeyString.KEY_LATEST_TIME_INFINITYHEART, lastedTimeInfinityHeart);
    }

    _lastedTimeHeart = DateTime.Parse(lastedTimeHeart, null, DateTimeStyles.RoundtripKind);
    _lastedTimeInfinityHeart = DateTime.Parse(lastedTimeInfinityHeart, null, DateTimeStyles.RoundtripKind);

    if (PlayerPrefs.GetInt(KeyString.KEY_FIRST_TIME_HEART, 0) == 0)
    {
      PlayerPrefs.SetInt(KeyString.KEY_FIRST_TIME_HEART, 1);
      InitFirstTimeOpen();
      _isBaked = true;
      return;
    }

    BakingHeartSystem();
  }

  private void InitFirstTimeOpen()
  {
    _heartControls = (GameObject[])FindObjectsByType(typeof(HeartControl), FindObjectsSortMode.InstanceID);

    // TODO Config remote max energy to infinity
    var maxHearts = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.config_game.max_energy;
    if (maxHearts > 5)
    {
      _isInfinityHeartOnRemote = true;
      _currentHeart = 5;
    }

    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;

      var heartControl = _heartControls[i].GetComponent<HeartControl>();
      var heartMax = heartControl.GetHeartMax();

      CurrentHeart = heartMax;
    }

    onBakedHeartSystem?.Invoke();
  }

  public void BakingHeartSystem()
  {
    // TODO Config remote max energy to infinity
    var maxHearts = FirebaseSetup.Instance.FirebaseRemoteData.parameterGroups.Lobby.parameters.config_game.max_energy;
    if (maxHearts > 5)
    {
      _isInfinityHeartOnRemote = true;
      _currentHeart = 5;
    }

    _currentHeart = PlayerPrefs.GetInt(KeyString.KEY_CURRENT_HEART);
    _useinfinityHeartTime = PlayerPrefs.GetInt(KeyString.KEY_USE_TIME_INFINITYHEART);
    _heartControls = (GameObject[])FindObjectsByType(typeof(HeartControl), FindObjectsSortMode.InstanceID);

    if (_useinfinityHeartTime > 0)
    {
      TurnOnInfinityHeart();
    }
    onBakedHeartSystem?.Invoke();
    _isBaked = true;
  }

  private void ChangedActiveScene(Scene current, Scene next)
  {

  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    if (!_isBaked) return;
    UseInfinityHeartTime = _useinfinityHeartTime;
    LastedTimeHeart = _lastedTimeHeart;
    LastedTimeInfinityHeart = _lastedTimeInfinityHeart;
    CurrentHeart = _currentHeart;

    BakingHeartSystem();
  }

  #region Handle
  public bool TryDecreaseHeart(int amountDecrease)
  {
    if (_isInfinityHeartOnRemote)
    {
      return true;
    }

    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;

      if (_heartControls[i].GetComponent<HeartControl>().RemoveHeart(ref _currentHeart, amountDecrease))
      {
        CurrentHeart = _currentHeart;
        return true;
      }

      break;
    }

    return false;
  }

  public bool TryIncreaseHeart(int amountIncrease)
  {
    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;

      if (_heartControls[i].GetComponent<HeartControl>().IsFull(_currentHeart))
      {
        return false;
      }

      break;
    }

    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;

      _heartControls[i].GetComponent<HeartControl>().AddHeart(ref _currentHeart, amountIncrease);
      CurrentHeart = _currentHeart;

      break;
    }

    return true;
  }


  private void CalculateHeart()
  {
    var distanceTime = DateTime.Now - _lastedTimeHeart;
    var amountHeartHeal = distanceTime.TotalSeconds / (minutePerHearts * 60);

    if ((int)amountHeartHeal >= 1)
    {
      TryIncreaseHeart((int)amountHeartHeal);
    }

    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;

      if (_heartControls[i].GetComponent<HeartControl>().IsFull(_currentHeart))
      {
        _lastedTimeHeart = DateTime.Now;
        onChangeHeart?.Invoke();
        return;
      }

      break;
    }

    var newOpenTimeHeart = _lastedTimeHeart.AddSeconds((int)amountHeartHeal * minutePerHearts * 60);
    _lastedTimeHeart = newOpenTimeHeart;
    // CurrentHeart = _currentHeart;
    onChangeHeart?.Invoke();
  }

  // Infinity Heart
  private void CalculateInfinityHeart()
  {
    if (_useinfinityHeartTime <= 0)
    {
      _useinfinityHeartTime = 0;
      TurnOffInfinityHeart();
      _lastedTimeInfinityHeart = DateTime.Now;
      onChangeHeart?.Invoke();
      return;
    }

    var distanceTime = DateTime.Now - _lastedTimeInfinityHeart;
    _useinfinityHeartTime -= (int)distanceTime.TotalSeconds;
    _useinfinityHeartTime -= Time.deltaTime;
    _lastedTimeInfinityHeart = DateTime.Now;
    onChangeHeart?.Invoke();
  }

  public void TurnOnInfinityHeart()
  {
    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;
      _heartControls[i].GetComponent<HeartControl>().SetIsInfinityHeart(true);
    }

    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;

      CurrentHeart = _heartControls[i].GetComponent<HeartControl>().GetHeartMax();
      break;
    }
  }

  public void TurnOffInfinityHeart()
  {
    for (int i = 0; i < _heartControls.Length; i++)
    {
      if (_heartControls[i] == null) continue;
      _heartControls[i].GetComponent<HeartControl>().SetIsInfinityHeart(false);
    }

    onChangeHeart?.Invoke();
  }

  public void AddInfinityHeartTime(int minute)
  {
    var seconds = minute * 60;
    UseInfinityHeartTime += seconds;
    TurnOnInfinityHeart();
  }

  #endregion
}
