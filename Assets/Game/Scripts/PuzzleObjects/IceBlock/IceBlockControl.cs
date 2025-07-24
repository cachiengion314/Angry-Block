using UnityEngine;

public class IceBlockControl : MonoBehaviour, IColorBlock, ISpriteRend
{
    [SerializeField] SpriteRenderer bodyRenderer;
    public Transform blockParent;
    int Index;
    int heart = 4;

    void Start()
    {
        LevelManager.Instance.OnDirectionBlockMove += OnTrigger;
    }
    void OnDestroy()
    {
        LevelManager.Instance.OnDirectionBlockMove -= OnTrigger;
    }

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

    public SpriteRenderer GetBodyRenderer()
    {
        return bodyRenderer;
    }

    public int GetSortingOrder()
    {
        return bodyRenderer.sortingOrder;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        bodyRenderer.sortingOrder = sortingOrder;
        foreach (Transform child in blockParent)
        {
            if (!child.TryGetComponent(out ISpriteRend spriteRend)) continue;
            spriteRend.SetSortingOrder(sortingOrder -2);
        }
    }
}
