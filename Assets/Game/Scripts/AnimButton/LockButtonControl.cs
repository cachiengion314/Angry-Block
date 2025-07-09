using UnityEngine;
using UnityEngine.UI;

public class LockButtonControl : MonoBehaviour
{
  [SerializeField] Button LockBtn;
  [SerializeField] GameObject LockObj;
  [SerializeField] GameObject UnlockObj;
  [SerializeField] int levelLimit;
  
  private void Start()
  {
    if (GameManager.Instance.CurrentLevel < levelLimit - 1)
    {
      LockObj.SetActive(true);
      UnlockObj.SetActive(false);
    }
    else
    {
      LockObj.SetActive(false);
      UnlockObj.SetActive(true);
    }
  }

  public void ClickLockBtn()
  {
    StartCoroutine(EffectManager.Instance.IEDelayShow(LockBtn, 1.5f));
    LobbyPanel.Instance.ShowNotifyWith("UNLOCK AT LEVEL " + levelLimit);
  }
}
