using Unity.Mathematics;
using UnityEngine;

public class WoodenBlockControl : MonoBehaviour, IInitialize, ITrigger
{
    [Header("Dependencies")]
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer directionRenderer;
    [Header("Datas")]
    [SerializeField] int2 gridPosBlockAhead;

    public void Initialize<T>(T data)
    {
        bodyRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        directionRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        if (!TryGetComponent(out IDirectionBlock directionBlock)) return;
        if (!TryGetComponent(out IColorBlock colorBlock)) return;
        var gridPos = LevelManager.Instance.BottomGrid.ConvertIndexToGridPos(colorBlock.GetIndex());
        gridPosBlockAhead = gridPos + directionBlock.GetDirection();
        directionRenderer.gameObject.SetActive(false);
    }

    public void OnTrigger<T>(T data)
    {
        if (data is int2 gridPosBlockAhead)
        {
            if (!this.gridPosBlockAhead.Equals(gridPosBlockAhead)) return;
            directionRenderer.gameObject.SetActive(true);
            Destroy(this);
        }
    }
}
