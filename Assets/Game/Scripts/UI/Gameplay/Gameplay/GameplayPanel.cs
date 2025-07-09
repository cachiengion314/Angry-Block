using UnityEngine;

public partial class GameplayPanel : MonoBehaviour
{
  public static GameplayPanel Instance { get; private set; }

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
