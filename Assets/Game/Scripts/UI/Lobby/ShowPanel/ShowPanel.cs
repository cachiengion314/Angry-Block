using System;
using DG.Tweening;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShowPanel : MonoBehaviour
{
  public static ShowPanel Instance { get; private set; }
  [SerializeField] TMP_Text globalCoinText;

  private void Start()
  {
    if (Instance == null)
    {
      Instance = this;
      InitUIBeginState();
    }
    else Destroy(gameObject);
  }

  void InitUIBeginState()
  {
    OnCoinChange();
    GameManager.Instance.OnCoinChange += OnCoinChange;
  }

  void OnDestroy()
  {
    GameManager.Instance.OnCoinChange -= OnCoinChange;
  }

  private void OnCoinChange()
  {
    globalCoinText.text = GameManager.Instance.CurrentCoin.ToString("#,##0");
  }
}
