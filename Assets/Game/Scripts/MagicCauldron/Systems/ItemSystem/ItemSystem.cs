using UnityEngine;

public partial class ItemSystem : MonoBehaviour
{
  public static ItemSystem Instance { get; private set; }

  [Header("Item dependencies")]
  [SerializeField] Transform tempRemovedObjParent;
  public Transform TempRemovedObjParent { get { return tempRemovedObjParent; } }
  [SerializeField] Transform destroyedObjParent;

  void Start()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else Destroy(gameObject);
  }

  /// <summary>
  /// pooling callback, don't modified it!
  /// </summary>
  /// <param name="obj"></param>
  void OnTakeObjFromPool(GameObject obj)
  {
    obj.SetActive(true);
  }

  /// <summary>
  /// pooling callback, don't modified it!
  /// </summary>
  /// <param name="obj"></param>
  void OnReturnObjFromPool(GameObject obj)
  {
    obj.SetActive(false);
  }

  /// <summary>
  /// pooling callback, don't modified it!
  /// </summary>
  /// <param name="obj"></param>
  void OnDestroyPoolObj(GameObject obj)
  {
    Destroy(obj);
  }

  /// <summary>
  /// only use this function for non pooling items.
  /// Note that the obj didn't instantly disapear so 
  /// you have to manual setActive to false by yourself
  /// </summary>
  /// <param name="obj"></param>
  public void RemoveNonPoolItem(GameObject obj)
  {
    obj.transform.SetParent(tempRemovedObjParent);
  }

  /// <summary>
  /// only use this function when tempRemovedObjParent.childCount reach its capacit'y 
  /// or game logic itself have some conditions that need it to be removed.
  /// The capacity might be run from 200 to 1000 or more depend on the context.
  /// </summary>
  /// <param name="_delayDestroyDuration"></param>
  public void ClearTempRemovedData(float _delayDestroyDuration = 5)
  {
    for (int i = tempRemovedObjParent.childCount - 1; i >= 0; --i)
    {
      var obj = tempRemovedObjParent.GetChild(i).gameObject;
      obj.transform.SetParent(destroyedObjParent);
      Destroy(obj, _delayDestroyDuration);
    }
  }

  public void ScaleCameraOrthographicSize()
  {

  }
}