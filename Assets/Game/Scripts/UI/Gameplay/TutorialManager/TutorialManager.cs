using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
  private static TutorialManager _instance;
  public static TutorialManager Instance { get { return _instance; } }

  [Header("Internal")]
  [SerializeField] Sprite arrowSprite;
  [SerializeField] Sprite handSprite;

  [SerializeField] Image pointerImg;
  [SerializeField] Transform[] pathParents;
  [SerializeField] Transform[] pathNoelParents;

  [Header("TrayTutorials")]
  [SerializeField] int[] indexes;

  private void Awake()
  {
    _instance = this;
  }

  #region Booster
  public bool IsAlreadyPlayedLevel31AnimAt(int part)
  {
    return false;
  }

  public void PlayBooster4AnimAt(int part)
  {
    pointerImg.sprite = arrowSprite;
    if (part == 0)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 180);
      PlayAnimationAt(9);
    }
    else if (part == 1)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 0);
      PlayAnimationAt(10);
    }
    else if (part == 2)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 0);
      PlayAnimationAt(11);
    }
  }

  public bool IsAlreadyPlayedLevel10AnimAt(int part)
  {
    return false;
  }

  public void PlayBooster3AnimAt(int part)
  {
    pointerImg.sprite = arrowSprite;
    if (part == 0)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 180);
      PlayAnimationAt(7);
    }
    else if (part == 1)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, -30);
      PlayAnimationAt(8);
    }
  }

  public bool IsAlreadyPlayedLevel8AnimAt(int part)
  {
    return false;
  }

  public void PlayBooster2AnimAt(int part)
  {
    pointerImg.sprite = arrowSprite;
    if (part == 0)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 180);
      PlayAnimationAt(5);
    }
    else if (part == 1)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, -30);
      PlayAnimationAt(6);
    }
  }

  public bool IsAlreadyPlayedLevel5Anim()
  {
    return false;
  }

  public void PlayBooster1AnimAt(int part)
  {
    pointerImg.sprite = arrowSprite;

    if (part == 0)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 180);
      PlayAnimationAt(3);
    }
    else if (part == 1)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, -30);
      PlayAnimationAt(4);
    }
  }

  public bool IsAlreadyPlayedLevel1AnimAt(int part)
  {
    if (part == 0)
    {
      if (PlayerPrefs.GetInt("PlayLevel1AnimAt_0", 0) == 1)
        return true;
    }
    else if (part == 1)
    {
      if (PlayerPrefs.GetInt("PlayLevel1AnimAt_1", 0) == 1)
        return true;
    }
    else if (part == 2)
    {
      if (PlayerPrefs.GetInt("PlayLevel1AnimAt_2", 0) == 1)
        return true;
    }
    return false;
  }

  public void PlayLevel1AnimAt(int part)
  {
    pointerImg.sprite = handSprite;
    pointerImg.transform.eulerAngles = new float3(0, 0, 0);
    if (part == 0)
      PlayAnimationAt(0);
    else if (part == 1)
      PlayAnimationAt(1);
    else if (part == 2)
      PlayAnimationAt(2);
  }

  void PlayAnimationAt(int pathParentIndex)
  {
    LeanTween.cancel(pointerImg.gameObject);
    Vector3[] positions = new Vector3[pathParents[pathParentIndex].childCount];
    for (int i = 0; i < positions.Length; ++i)
    {
      positions[i] = pathParents[pathParentIndex].GetChild(i).position;
    }
    LeanTween.moveSpline(pointerImg.gameObject, positions, 1f)
        .setLoopClamp();
    LeanTween.value(pointerImg.gameObject, 1, 0, 1f)
        .setOnUpdate(UpdateTextAlpha)
        .setLoopClamp();
  }

  private void UpdateTextAlpha(float alpha)
  {
    Color color = pointerImg.color;
    color.a = alpha;
    pointerImg.color = color;
  }

  #endregion

  #region Noel Event
  public void PlayNoelAnimAt(int part)
  {
    pointerImg.sprite = arrowSprite;

    if (part == 0)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, 45);
      PlayNoelAnimationAt(0);
    }
    else if (part == 1)
    {
      pointerImg.transform.eulerAngles = new float3(0, 0, -90);
      PlayNoelAnimationAt(1);
    }
  }

  void PlayNoelAnimationAt(int pathParentIndex)
  {
    LeanTween.cancel(pointerImg.gameObject);
    Vector3[] positions = new Vector3[pathNoelParents[pathParentIndex].childCount];
    for (int i = 0; i < positions.Length; ++i)
    {
      positions[i] = pathNoelParents[pathParentIndex].GetChild(i).position;
      Debug.Log("positions[i] " + positions[i]);
    }
    Debug.Log(positions.Length);

    LeanTween.moveSpline(pointerImg.gameObject, positions, 1f)
        .setLoopClamp();
    LeanTween.value(pointerImg.gameObject, 1, 0, 1f)
        // .setOnUpdate(UpdateTextAlpha)
        .setLoopClamp();
  }

  #endregion
}
