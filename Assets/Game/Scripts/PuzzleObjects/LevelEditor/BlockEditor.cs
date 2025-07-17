using UnityEngine;

public class BlockEditor : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public BlockType blockType;
    public void OnValidate()
    {
        if (blockType == BlockType.None)
            spriteRenderer.color = Color.gray;
        if (blockType == BlockType.DirectionBlock)
            spriteRenderer.color = Color.yellow;
        if (blockType == BlockType.WoodenBlock)
            spriteRenderer.color = Color.brown;
        if (blockType == BlockType.Tunnel)
            spriteRenderer.color = Color.green;
        if (blockType == BlockType.IceBlock)
            spriteRenderer.color = Color.blue;
    }
    [Header("Data")]

    public DirectionBlockData directionBlockData;
    public TunnelData tunnelData;
}
