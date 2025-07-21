using UnityEngine;

public class BlockEditor : MonoBehaviour
{
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer arrowRenderer;
    [Header("Data")]
    public BlockType blockType;
    public DirectionBlockData directionBlockData;
    public TunnelData tunnelData;
    public void OnValidate()
    {
        if (arrowRenderer == null) return;
        
        if (directionBlockData.DirectionValue == DirectionValue.Right)
            arrowRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (directionBlockData.DirectionValue == DirectionValue.Left)
            arrowRenderer.transform.rotation = Quaternion.Euler(0, 0, 180);
        if (directionBlockData.DirectionValue == DirectionValue.Up)
            arrowRenderer.transform.rotation = Quaternion.Euler(0, 0, 90);
        if (directionBlockData.DirectionValue == DirectionValue.Down)
            arrowRenderer.transform.rotation = Quaternion.Euler(0, 0, -90);

        if (blockType == BlockType.None)
            bodyRenderer.color = Color.gray;
        if (blockType == BlockType.DirectionBlock)
            bodyRenderer.color = Color.yellow;
        if (blockType == BlockType.WoodenBlock)
            bodyRenderer.color = Color.brown;
        if (blockType == BlockType.Tunnel)
            bodyRenderer.color = Color.green;
        if (blockType == BlockType.IceBlock)
            bodyRenderer.color = Color.blue;
    }
}
