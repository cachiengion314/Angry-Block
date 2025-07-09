using UnityEngine;
using UnityEngine.EventSystems; // Import để sử dụng giao diện sự kiện
using DG.Tweening;
using UnityEngine.UI;
public class AnimButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  [SerializeField] private float _pressedScale = 0.8f; // Kích thước nhỏ hơn khi nhấn
  [SerializeField] private float _animationDuration = 0.2f; // Thời gian animation
  private Button buttonTarget;
  private Tween currentTween; // Lưu tween hiện tại để dừng nếu cần

  private Vector3 _originalScale; // Lưu lại kích thước gốc của Button
  void Start()
  {
    buttonTarget = GetComponent<Button>();
    if (buttonTarget == null)
    {
      Debug.LogError("Button component không tồn tại trên GameObject này!");
      return;
    }
    _originalScale = Vector3.one;
  }// Gọi khi nhấn vào nút
  public void OnPointerDown(PointerEventData eventData)
  {
    if (buttonTarget == null) return;

    // Dừng tween cũ nếu có
    currentTween?.Kill();
    // Thu nhỏ nút với DOTween
    currentTween = buttonTarget.transform.DOScale(_originalScale * _pressedScale, _animationDuration)
        .SetEase(Ease.OutQuad);
  }

  // Gọi khi thả tay khỏi nút
  public void OnPointerUp(PointerEventData eventData)
  {
    if (buttonTarget == null) return;

    // Dừng tween cũ nếu có
    currentTween?.Kill();
    // Trả về kích thước ban đầu với DOTween
    currentTween = buttonTarget.transform.DOScale(_originalScale, _animationDuration)
        .SetEase(Ease.OutQuad);
  }
  private void OnDestroy()
  {
    // Dừng tween khi đối tượng bị hủy
    currentTween?.Kill();
  }
}
