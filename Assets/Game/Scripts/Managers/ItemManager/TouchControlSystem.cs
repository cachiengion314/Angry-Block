using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// TouchControl System
/// </summary> <summary>
/// 
/// </summary>
public partial class ItemManager : MonoBehaviour
{
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
      if (BalloonSystem.Instance.HasBalloonAtTouch()) return;

      switch (touch.phase)
      {
        case TouchPhase.Began:
          OnTouchBegan(touchPos);
          break;
      }
    }
  }

  private void OnTouchBegan(float2 touchPos)
  {
    var pos = new float3(touchPos.x, touchPos.y, 0);

    if (HasCurtainLayerAt(pos))
    {
      OnTouchCurtainLayerAt(pos);
      return;
    }

    if (HasGrillerAt(pos))
    {
      OnTouchGrillerAt(pos);
      return;
    }

    if (HasGiftBoxAt(pos))
    {
      OnTouchGiftBoxAt(pos);
      return;
    }

    if (HasCoffeeBoardAt(pos))
    {
      OnTouchCoffeeBoardAt(pos);
    }

    if (HasPlantPotAt(pos))
    {
      OnTouchPlantPotAt(pos);
    }

    if (HasCoverLetAt(pos))
    {
      OnTouchCoverLetAt(pos);
    }

    if (HasMoneyBagAt(pos))
    {
      OnTouchMoneyBagAt(pos);
    }

    if (HasCupBoardAt(pos))
    {
      OnTouchCupBoardAt(pos);
    }

    if (HasMachineCreamAt(pos))
    {
      OnTouchMachineCreamAt(pos);
    }

    if (HasMagicNestAt(pos))
    {
      OnTouchMagicNestAt(pos);
    }

    if (HasFlowerPotAt(pos))
    {
      OnTouchFlowerPotAt(pos);
    }

    if (HasBeverageFridgeAt(pos))
    {
      OnTouchBeverageFridgeAt(pos);
    }
    
    if (HasLeavesFlowerAt(pos))
    {
      OnTouchLeavesFlowerAt(pos);
    }
  }
}