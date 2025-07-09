using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialNoelSystem : MonoBehaviour
{
  [Header("External Dependences")]
  // Lobby
  [SerializeField] Canvas luckyWheelSke;
  [SerializeField] Canvas btnIconPass;
  [SerializeField] Image pointerImg;
  [SerializeField] GameObject tutorialImg;

  // BG
  [SerializeField] Image bgTutorialNoel;

  #region Lifecycle Function
  private void Awake()
  {
    StartCoroutine(IEAwake());
  }

  private void OnEnable()
  {
    LobbyPanel.onOpenLuckyWheel += OnOpenLuckyWheel;
    LobbyPanel.onOpenDailySweetPass += OnOpenDailySweetPass;
  }

  private void OnDisable()
  {
    LobbyPanel.onOpenLuckyWheel -= OnOpenLuckyWheel;
    LobbyPanel.onOpenDailySweetPass -= OnOpenDailySweetPass;
  }


  IEnumerator IEAwake()
  {
    yield return new WaitUntil(() => GameManager.Instance != null);
    tutorialImg.SetActive(false);
    bgTutorialNoel.gameObject.SetActive(false);

    if (!GameManager.Instance.IsEvented()) yield break;
    if (PlayerPrefs.GetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALNOEL, 0) == 1) yield break;

    StartCoroutine(IEStartTutorials());
  }
  #endregion

  #region Handle Function
  bool _isCompletedTutorial1;
  bool _isCompletedTutorial2;
  IEnumerator IEStartTutorials()
  {
    StartTutorialNoel1();
    yield return new WaitUntil(() => _isCompletedTutorial1);
    StartTutorialNoel2();
    yield return new WaitUntil(() => _isCompletedTutorial2);
    StartTutorialNoel3();
  }

  public void StartTutorialNoel1()
  {
    pointerImg.gameObject.SetActive(true);
    bgTutorialNoel.gameObject.SetActive(true);
    luckyWheelSke.overrideSorting = true;
    SetTransparentFrom(bgTutorialNoel, 0.7f);
    TutorialManager.Instance.PlayNoelAnimAt(0);
  }

  public void StartTutorialNoel2()
  {
    pointerImg.gameObject.SetActive(true);
    btnIconPass.overrideSorting = true;
    SetTransparentFrom(bgTutorialNoel, 0.7f);
    TutorialManager.Instance.PlayNoelAnimAt(1);
  }

  public void StartTutorialNoel3()
  {
    tutorialImg.SetActive(true);
    SetTransparentFrom(bgTutorialNoel, 0.7f);
    PlayerPrefs.SetInt(KeyString.KEY_BOOL_ISCOMPLETED_TUTORIALNOEL, 1);
  }

  #endregion

  #region Event Function
  private void OnOpenLuckyWheel()
  {
    luckyWheelSke.overrideSorting = false;
    SetTransparentFrom(bgTutorialNoel, 0f);
    pointerImg.gameObject.SetActive(false);

    LeanTween.delayedCall(gameObject, 0.7f, () =>
    {
      _isCompletedTutorial1 = true;
    });
  }

  private void OnOpenDailySweetPass()
  {
    luckyWheelSke.overrideSorting = false;
    btnIconPass.overrideSorting = false;
    SetTransparentFrom(bgTutorialNoel, 0f);
    pointerImg.gameObject.SetActive(false);

    LeanTween.delayedCall(gameObject, 0.7f, () =>
    {
      _isCompletedTutorial2 = true;
    });
  }

  #endregion

  #region Expand Function
  private void SetTransparentFrom(Image image, float transparent)
  {
    var color = image.color;
    color.a = transparent;
    image.color = color;
  }

  #endregion
}
