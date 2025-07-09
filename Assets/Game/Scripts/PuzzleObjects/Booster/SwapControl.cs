using UnityEngine;

public class SwapControl : MonoBehaviour
{
  [Header("Settings")]
  private BoxCollider2D _collider;

  void Update()
  {
    PressControl();
  }

  void PressControl()
  {
    if (Input.touchCount > 0)
    {
      Touch touch = Input.GetTouch(0);
      Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);

      switch (touch.phase)
      {
        case TouchPhase.Began:
          if (_collider == Physics2D.OverlapPoint(touchPos))
          {

            return;
          }
          // touching other items will not cause any effect
          break;
      }
    }
  }
}
