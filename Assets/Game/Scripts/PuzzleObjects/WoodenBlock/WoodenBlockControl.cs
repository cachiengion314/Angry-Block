using Unity.Mathematics;
using UnityEngine;

public class WoodenBlockControl : MonoBehaviour, ITrigger
{
    [Header("Dependencies")]
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer directionRenderer;
    [SerializeField] DirectionBlockControl directionBlock;
    public DirectionBlockControl DirectionBlock => directionBlock;

    public void OnTrigger<T>(T data)
    {
        if (data is int2 gridPosBlockAhead)
        {
            if (!TryGetComponent(out IDirectionBlock directionBlock)) return;
            if (!TryGetComponent(out IColorBlock colorBlock)) return;
            var gridPos = LevelManager.Instance.BottomGrid.ConvertIndexToGridPos(colorBlock.GetIndex());
            if (!gridPosBlockAhead.Equals(gridPos + directionBlock.GetDirection())) return;

            directionRenderer.gameObject.SetActive(true);
            Destroy(this);
        }
    }
}
