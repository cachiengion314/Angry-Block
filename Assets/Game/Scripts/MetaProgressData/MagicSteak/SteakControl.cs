using UnityEngine;

public class SteakControl : MonoBehaviour
{
    [SerializeField] GameObject unlockSteak;

    public void Lock()
    {
        unlockSteak.SetActive(false);
    }

    public void Unlock()
    {
        unlockSteak.SetActive(true);
    }
}
