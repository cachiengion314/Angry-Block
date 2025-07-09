using UnityEngine;
using DG.Tweening;

public class AnimLight : MonoBehaviour
{
    private Tween currentTween; // Lưu tween hiện tại để dừng nếu cần
    
    void OnEnable()
    {
        AnimLigth();
    }
    void OnDisable()
    {
        currentTween?.Kill();
    }

    void OnDestroy()
    {
        currentTween?.Kill();
    }
    void AnimLigth()
    {
        currentTween?.Kill();
        // Tạo Tween quay từ -45 đến 45 độ và lặp lại đều
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Tạo Tween quay liên tục
        currentTween = transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear) // Dùng Linear để quay đều
            .SetLoops(-1, LoopType.Restart); // Lặp lại vô hạn

    }
}
