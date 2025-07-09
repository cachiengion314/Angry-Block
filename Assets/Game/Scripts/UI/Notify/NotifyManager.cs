using UnityEngine;
using DG.Tweening;
using TMPro;

public class NotifyManager : MonoBehaviour
{
    public static NotifyManager Instance { get; private set; }
    [SerializeField] GameObject _prefabNoti;
    public GameObject _parentShowNotif; // chuyền vào canva cần hiện thị(có thể thay đổi thành gameobject cha);
    public float _valuePosTranfrom;
    public float _valueScaleNoti;
    public string _textNoti; // string chuyền vào
    [SerializeField] StypeAnim stypeAnim;
    Vector2 PosTargetDesignated;
    float valueMoveDesignated;
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }

    public void CreatNotif()
    {
        if (_parentShowNotif != null)
        {
            GameObject NotiClone = Instantiate(_prefabNoti, _parentShowNotif.transform);
            NotiClone.transform.GetComponentInChildren<TMP_Text>().text = _textNoti;
            AnimNoti(NotiClone, stypeAnim);
        }

    }
    void AnimNoti(GameObject _gameobject, StypeAnim _stype)
    {
        RectTransform rectTransform = _gameobject.GetComponent<RectTransform>();

        Sequence sequence = DOTween.Sequence();

        switch (_stype)
        {
            case StypeAnim.MouseDown:
                // Lấy vị trí chuột trong Local Space của Canvas
                Vector3 mousePosition = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _parentShowNotif.GetComponent<RectTransform>(),
                    mousePosition,
                    Camera.main,
                    out Vector2 localPoint
                );

                // Đặt vị trí thông báo tại vị trí chuột
                rectTransform.anchoredPosition = localPoint;
                // Debug.Log("----" + rectTransform.anchoredPosition);

                // Tính vị trí mới (bay lên một đoạn nhỏ, ví dụ 50px)
                Vector2 targetPosition = new Vector2(localPoint.x, localPoint.y + 100f);

                // Tạo hiệu ứng di chuyển
                sequence.Append(rectTransform.DOAnchorPos(targetPosition, 0.5f).SetEase(Ease.OutBack))
                        .Join(rectTransform.DOScale(Vector3.one * _valueScaleNoti, 0.5f).SetEase(Ease.OutBack));
                break;

            case StypeAnim.TopCavan:
                // Hiệu ứng mặc định: bay từ trên xuống
                RectTransform canvasRect = _parentShowNotif.GetComponent<RectTransform>();
                float screenTop = canvasRect.rect.height / 2;
                // Đặt vị trí bắt đầu: Chính giữa cạnh trên
                rectTransform.anchoredPosition = new Vector2(0, screenTop);

                sequence.Join(rectTransform.DOAnchorPosY(_valuePosTranfrom, 0.5f).SetEase(Ease.OutBounce)); // Di chuyển từ trên xuống
                break;
            case StypeAnim.DesignatedLocation:

                rectTransform.anchoredPosition = PosTargetDesignated;

                // Tính vị trí mới (bay lên một đoạn nhỏ, ví dụ 50px)
                Vector2 targetPositionDesignated = new Vector2(PosTargetDesignated.x, PosTargetDesignated.y + valueMoveDesignated);

                // Tạo hiệu ứng di chuyển
                sequence.Append(rectTransform.DOAnchorPos(targetPositionDesignated, 0.6f).SetEase(Ease.OutBack))
                        .Join(rectTransform.DOScale(Vector3.one * _valueScaleNoti, 0.6f).SetEase(Ease.OutBack));

                break;
        }

    }
    public void SetupNotify(GameObject _parentShowNotifShow, string _testNotiShow, StypeAnim _stypeAnimShow)
    {
        _parentShowNotif = _parentShowNotifShow;
        _textNoti = _testNotiShow;
        stypeAnim = _stypeAnimShow;
    }

    public void SetupNotifyDesignatedPoS(GameObject _parentShowNotifShow, string _testNotiShow, 
    StypeAnim _stypeAnimShow, Vector2 _valuePosTranfrom , float _valueMove, float _valueScale)
    {
        _parentShowNotif = _parentShowNotifShow; // chuyền vào vị trí cha
        _textNoti = _testNotiShow; // notitext
        stypeAnim = _stypeAnimShow; // kiểu thực hiện anim
        PosTargetDesignated = _valuePosTranfrom; // vị trí suất hiện 
        valueMoveDesignated = _valueMove; // vị trí sau khi thực hiện xong anim 
        _valueScaleNoti =_valueScale ; // scale sau khi thực hiện anim
    }

}
public enum StypeAnim
{
    MouseDown,// vị trí trỏ chuột
    TopCavan, // từ trên xuống
    DesignatedLocation // vị trí chỉ định
}