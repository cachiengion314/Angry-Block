using DG.Tweening;
using HoangNam;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Select Table
/// </summary> <summary>
/// 
/// </summary>
public partial class ToolEditorPanel : MonoBehaviour
{
  [Header("SelectTable Dependencies")]
  [Space(10)]
  [SerializeField] RectTransform contentTable;
  [SerializeField] AutoSizeScrollView autoSizeScrollView;
  [SerializeField] ScrollRect contentTableScrollRect;
  [SerializeField] Button prevTableBtn;
  [SerializeField] Button nextTableBtn;
  [SerializeField] Button selectTableBtn;
  [SerializeField] RectTransform cellTablesRect;
  [SerializeField] GridLayoutGroup cellTableLayoutGroup;

  private RectTransform[] _childContentTableRects;
  private float3[] _childContentTablePoses;

  private float _deltaDistance;

  private int _currentTableIndex = 0;

  private bool _isSelectingTable = true;

  private void Start()
  {
    Canvas.ForceUpdateCanvases();
    cellTableLayoutGroup.enabled = false;
    autoSizeScrollView.HideContentLayoutGroup();

    SetupChildContentTables();
    VisualizeContentTable();

    HideCellTables();
    UpdateBtnTableStatus();
  }

  private void SetupChildContentTables()
  {
    _childContentTableRects = new RectTransform[contentTable.childCount];
    _childContentTablePoses = new float3[contentTable.childCount];

    for (int i = 0; i < contentTable.childCount; i++)
    {
      _childContentTableRects[i] = contentTable.GetChild(i).GetComponent<RectTransform>();
      _childContentTablePoses[i] = _childContentTableRects[i].position;
    }

    _deltaDistance = _childContentTablePoses[1].x - _childContentTablePoses[0].x;
  }

  public void ClickNextTable()
  {
    if (_currentTableIndex >= contentTable.childCount - 1)
    {
      Utility.Print("Out of index");
      return;
    }

    SelectTableAt(_currentTableIndex + 1);
    VisualizeContentTable();
  }

  public void ClickPreviousTable()
  {
    if (_currentTableIndex <= 0)
    {
      Utility.Print("Out of index");
      return;
    }

    SelectTableAt(_currentTableIndex - 1);
    VisualizeContentTable();
  }

  public void ClickSelectTable()
  {
    _isSelectingTable = false;

    UpdateBtnTableStatus();
    ShowCellTables();
  }

  private void SelectTableAt(int index)
  {
    _currentTableIndex = index;
  }

  private void ShowCellTables()
  {
    for (int i = 0; i < cellTablesRect.childCount; i++)
    {
      var cell = cellTablesRect.GetChild(i);
      cell.gameObject.SetActive(true);
    }

    switch (_currentTableIndex)
    {
      case 1:
        HideCellTablesAt(new int[] { 3, 21 });
        break;
      case 2:
        HideCellTablesAt(new int[] { 9, 14 });
        break;
      case 3:
        HideCellTablesAt(new int[] { 5, 23 });
        break;
      case 4:
        HideCellTablesAt(new int[] { 0, 18 });
        break;
    }
  }

  private void HideCellTables()
  {
    for (int i = 0; i < cellTablesRect.childCount; i++)
    {
      var cell = cellTablesRect.GetChild(i);
      cell.gameObject.SetActive(false);
    }
  }

  private void HideCellTablesAt(int[] indexes)
  {
    for (int i = 0; i < indexes.Length; i++)
    {
      var cell = cellTablesRect.GetChild(indexes[i]);
      cell.gameObject.SetActive(false);
    }
  }

  private void UpdateBtnTableStatus()
  {
    nextTableBtn.gameObject.SetActive(_isSelectingTable);
    prevTableBtn.gameObject.SetActive(_isSelectingTable);
    selectTableBtn.gameObject.SetActive(_isSelectingTable);
    contentTableScrollRect.enabled = _isSelectingTable;
  }

  private void VisualizeContentTable()
  {
    DOTween.Kill(contentTable.transform);

    for (int i = 0; i < _childContentTableRects.Length; i++)
    {
      var targetPos = _childContentTablePoses[i].x - _deltaDistance * _currentTableIndex;
      _childContentTableRects[i].DOMoveX(targetPos, 0.3f);
    }
  }
}