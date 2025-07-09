using UnityEngine;

interface IDayBlock
{
    public void ShowAlreadyActivatedDay();
    public void ShowToday();
    public void ShowTodayActivated();
    public void ShowTomorrow();
    public void ShowFollowingDay();
    public void InjectRewardImg(Sprite sprite);
}
