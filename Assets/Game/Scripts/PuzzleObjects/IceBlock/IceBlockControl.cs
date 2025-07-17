using UnityEngine;

public class IceBlockControl : MonoBehaviour, IColorBlock
{
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] Transform blockParent;
    int Index;
    int heart = 4;

    public int GetColorValue()
    {
        throw new System.NotImplementedException();
    }

    public int GetIndex()
    {
        return Index;
    }

    public void SetColorValue(int colorValue)
    {
        throw new System.NotImplementedException();
    }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public void Initialize(DirectionBlockData directionBlockData)
    {
        Visualze();
        LevelManager.Instance.OnDirectionBlockMove += OnTrigger;
        var directionBlock = LevelManager.Instance.SpawnDirectionBlockAt(Index, blockParent);
        directionBlock.SetIndex(Index);
        directionBlock.SetColorValue(directionBlockData.ColorValue);
        directionBlock.SetDirectionValue(directionBlockData.DirectionValue);
        directionBlock.SetAmmunition(directionBlockData.Ammunition);
        if (!directionBlock.TryGetComponent(out Collider2D col)) return;
        col.enabled = false;
    }

    void OnTrigger()
    {
        heart--;
        Visualze();
        if (heart > 0) return;
        LevelManager.Instance.OnDirectionBlockMove -= OnTrigger;
        if (blockParent.childCount <= 0) return;
        var block = blockParent.GetChild(0);
        if (!block.TryGetComponent(out Collider2D col)) return;
        col.enabled = true;
        LevelManager.Instance.SetDirectionBlocks(Index, block.gameObject);
        block.SetParent(LevelManager.Instance.SpawnedParent);
        Destroy(gameObject);
    }

    void Visualze()
    {
        if (heart == 4) bodyRenderer.color = Color.blue;
        if (heart == 3) bodyRenderer.color = Color.green;
        if (heart == 2) bodyRenderer.color = Color.yellow;
        if (heart == 1) bodyRenderer.color = Color.red;
    }
}
