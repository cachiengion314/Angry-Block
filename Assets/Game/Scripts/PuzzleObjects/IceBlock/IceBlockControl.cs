using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class IceBlockControl : MonoBehaviour, IColorBlock, ISpriteRend
{
    [SerializeField] SortingGroup sortingGroup;
    // [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SkeletonAnimation bodySkeleton;
    [SerializeField] TextMeshPro heartTxt;
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

    public void RemoveBlock()
    {
        if (blockParent.childCount <= 0) return;
        var block = blockParent.GetChild(0);
        if (!block.TryGetComponent(out Collider2D col)) return;
        col.enabled = true;
        block.SetParent(LevelManager.Instance.SpawnedParent);
        LevelManager.Instance.SetDirectionBlocks(Index, null);
        Destroy(gameObject);
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

        bodySkeleton.AnimationState.SetAnimation(0, "animation", false);
        heartTxt.gameObject.SetActive(false);
        Destroy(gameObject,1f);
    }

    void Visualze()
    {
        heartTxt.text = heart.ToString();
        // if (heart == 4) bodyRenderer.color = Color.blue;
        // if (heart == 3) bodyRenderer.color = Color.green;
        // if (heart == 2) bodyRenderer.color = Color.yellow;
        // if (heart == 1) bodyRenderer.color = Color.red;
    }

    public SpriteRenderer GetBodyRenderer()
    {
        return null;
    }

    public int GetSortingOrder()
    {
        return bodySkeleton.GetComponent<Renderer>().sortingOrder;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        sortingGroup.sortingOrder = sortingOrder;
        bodySkeleton.GetComponent<Renderer>().sortingOrder = sortingOrder +1;
        heartTxt.sortingOrder = sortingOrder +2;
        foreach (Transform child in blockParent)
        {
            if (!child.TryGetComponent(out ISpriteRend spriteRend)) continue;
            spriteRend.SetSortingOrder(sortingOrder);
        }
    }

    public void SetLayerName(string layerName)
    {
        sortingGroup.sortingLayerName = layerName;
    }
}
