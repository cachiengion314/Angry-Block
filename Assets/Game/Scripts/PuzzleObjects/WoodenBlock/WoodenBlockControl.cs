using Unity.Mathematics;
using UnityEngine;

public class WoodenBlockControl : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer directionRenderer;
    [Header("Datas")]
    [SerializeField] int2 gridPosBlockAhead;
    public bool IsHideHidden(int2 gridPosBlockAhead) => this.gridPosBlockAhead.Equals(gridPosBlockAhead);
    public void InitWoodenBlock()
    {
        bodyRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        directionRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        if (!TryGetComponent(out IDirectionBlock directionBlock)) return;
        if (!TryGetComponent(out IColorBlock colorBlock)) return;
        var gridPos = LevelManager.Instance.BottomGrid.ConvertIndexToGridPos(colorBlock.GetIndex());
        gridPosBlockAhead = gridPos + directionBlock.GetDirection();
    }
    public void ShowHidden()
    {
        directionRenderer.gameObject.SetActive(false);
    }

    public void HideHidden()
    {
        directionRenderer.gameObject.SetActive(true);
    }

}
