using UnityEngine;

public class SlotControl : MonoBehaviour
{
    [SerializeField] BoxCollider2D col2D;
    [SerializeField] GameObject unlockSlot;

    public void LockSlot()
    {
        unlockSlot.SetActive(true);
        col2D.enabled = true;
    }

    public void UnlockSlot()
    {
        unlockSlot.SetActive(false);
        col2D.enabled = false;
    }
}
