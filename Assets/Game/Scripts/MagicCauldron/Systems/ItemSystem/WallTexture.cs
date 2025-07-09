using UnityEngine;

public partial class ItemSystem : MonoBehaviour
{
  [Header("Wall texture")]
  [SerializeField] SpriteRenderer wallTexture;

  public void ChangeWallTextureOffset(float velocityY)
  {
    var currOffset = wallTexture.material.GetVector("_Offset");
    var newOffset = (Vector2)currOffset + -1 * new Vector2(0, velocityY);
    wallTexture.material.SetVector("_Offset", newOffset);
  }
}