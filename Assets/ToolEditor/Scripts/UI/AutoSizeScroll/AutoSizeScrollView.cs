using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AutoSizeScrollView : MonoBehaviour
{
  [Header("Internal Dependencies")]
  [SerializeField] RectTransform toolCanvasRect;
  [SerializeField] GridLayoutGroup contentLayoutGroup;

  private void Awake()
  {
    AutoContentSize();
  }

  private void AutoContentSize()
  {
    var width = toolCanvasRect.rect.width;
    var height = toolCanvasRect.rect.height;

    contentLayoutGroup.cellSize = new float2(width, height);
  }

  public void HideContentLayoutGroup()
  {
    contentLayoutGroup.enabled = false;
  }
}
