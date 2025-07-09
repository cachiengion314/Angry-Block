using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TapScreen : MonoBehaviour, IPointerDownHandler
{
  [Header("External Dependences")]
  [SerializeField] Image bgTutorialNoel;

  public void OnPointerDown(PointerEventData eventData)
  {
    bgTutorialNoel.gameObject.SetActive(false);
    gameObject.SetActive(false);
  }
}
