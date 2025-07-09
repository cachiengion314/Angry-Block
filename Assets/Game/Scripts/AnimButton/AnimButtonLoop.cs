using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class AnimButtonLoop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  private Button _btnPlay;
  [SerializeField] float _floatZoom;
  private Tween _currentTween;
  void Start()
  {
    _btnPlay = this.GetComponent<Button>();
    if (_btnPlay == null)
    {
      Debug.LogError("Button Play chưa được gán trong Inspector!");
      return;
    }
    UpdateButon();
  }
  void UpdateButon()
  {
    _currentTween?.Kill();
    _currentTween = _btnPlay.transform.DOScale(Vector3.one * _floatZoom, 0.8f)
       .SetEase(Ease.InOutSine)
       .SetLoops(-1, LoopType.Yoyo);
  }
  private void OnDestroy()
  {
    // Dừng tween khi đối tượng bị hủy
    _currentTween?.Kill();
  }
  public void OnPointerDown(PointerEventData eventData)
  {
    // Dừng animation khi bấm xuống
    _currentTween?.Pause();
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    // Tiếp tục animation khi thả ra
    _currentTween?.Play();
  }
}
