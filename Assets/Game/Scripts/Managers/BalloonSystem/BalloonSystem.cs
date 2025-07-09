using UnityEngine;

public class BalloonSystem : MonoBehaviour
{
  public static BalloonSystem Instance { get; private set; }

  [Header("External Dependences")]
  [SerializeField] BalloonControl balloonControl;
  [SerializeField] LayerMask balloonLayer;

  private readonly float _timePerCheck = 10;
  private float _timeCurrent = 0;

  private void Awake()
  {
    Instance = this;
  }

  private void Update()
  {
    if (balloonControl.IsFlying) return;

    _timeCurrent += Time.deltaTime;
    if (_timeCurrent < _timePerCheck) return;

    TryMoveBalloon();
  }

  public void ShowBalloon()
  {
    balloonControl.gameObject.SetActive(true);
  }

  public void ShowBalloonWithDelay(float timeDelay = 0.5f)
  {
    LeanTween.delayedCall(gameObject, timeDelay, () =>
    {
      balloonControl.gameObject.SetActive(true);
    });
  }

  public void HideBalloon()
  {
    balloonControl.gameObject.SetActive(false);
  }

  public bool HasBalloonAtTouch()
  {
    Touch touch = Input.GetTouch(0);
    Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

    Collider2D _col = Physics2D.OverlapPoint(touchPos, balloonLayer);
    if (_col != null) return true;
    return false;
  }

  public bool IsFullProgress()
  {
    var progress = GameManager.Instance.ProgressBalloon;

    if (progress >= 200)
    {
      return true;
    }

    return false;
  }

  public void TryMoveBalloon()
  {
    if (balloonControl.IsFlying) return;
    if (!IsFullProgress()) return;

    balloonControl.gameObject.SetActive(true);
    balloonControl.Move();
    _timeCurrent = 0;
  }
}
