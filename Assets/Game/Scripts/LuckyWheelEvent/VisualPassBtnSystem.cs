using System;
using UnityEngine;

public class VisualPassBtnSystem : MonoBehaviour
{
    [SerializeField] GameObject exclamationMark;
    private void Start()
    {
        SetUp();
        GameManager.onReceivedReward += SetUp;
    }

    private void OnDestroy()
    {
        GameManager.onReceivedReward -= SetUp;
    }

    // int day
    // {
    //     get { return PlayerPrefs.GetInt("currentday", 1); }
    //     set { PlayerPrefs.SetInt("currentday", value); }
    // }
    void SetUp()
    {
        exclamationMark.SetActive(IsOnExclamationMark());
    }
    bool IsOnExclamationMark()
    {
        int day = DateTime.Today.Day;
        for (int i = 1; i <= day; i++)
        {
            if (GameManager.Instance.GetLoginDayAt(i) && !GameManager.Instance.GetRewardFreeDayAt(i)) return true;
            if (GameManager.Instance.IsUnlockFollowing && !GameManager.Instance.GetRewardPassDayAt(i)) return true;
        }
        return false;
    }
}
