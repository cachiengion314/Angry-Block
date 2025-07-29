using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [Header("Pos")]
    [SerializeField] Transform posParents;

    [Header("Object")]
    [SerializeField] Image obj;
    [SerializeField] Sprite handSpr;
    [SerializeField] Sprite arrowSpr;

    [Header("Drak Panel")]
    [SerializeField] GameObject darkPanel;
    [SerializeField] SpriteMask lightPanel;
    [SerializeField] Sprite squareSpr;
    [SerializeField] Sprite circleSpr;
    [SerializeField] Button exit1Btn;

    [Header("Tutorial Panel")]
    [SerializeField] GameObject panel;
    [SerializeField] TextMeshProUGUI textMesh;
    [SerializeField] Button exit2Btn;

    [Header("receive")]
    [SerializeField] GameObject receiveModal;
    [SerializeField] GameObject lightEfx;
    [SerializeField] TextMeshProUGUI nameBooster;
    [SerializeField] Image rewardImg;
    [SerializeField] Button receiveBtn;
    Action OnReceive;
    Action OnExit;
    Action OnTap;

    string keyTutorial = "";

    public bool IsTutorialAt(string keyTutorial)
    {
        return this.keyTutorial.Equals(keyTutorial);
    }

    public void DoHandMoveAt(string keyTutorial, Vector2 startValue, Vector2 endValue, float angle = 0, float duration = 1f)
    {
        this.keyTutorial = keyTutorial;

        obj.sprite = handSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    public void DoArrowMoveAt(string keyTutorial, Vector2 startValue, Vector2 endValue, float angle = 0, float duration = 1f)
    {
        this.keyTutorial = keyTutorial;

        obj.sprite = arrowSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    public void DoHandMoveAt(string keyTutorial, int index, float angle = 0, float duration = 1f)
    {
        var posParent = posParents.GetChild(index);
        Vector2 startValue = posParent.GetChild(0).position;
        Vector2 endValue = posParent.GetChild(1).position;

        this.keyTutorial = keyTutorial;

        obj.sprite = handSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    public void DoArrowMoveAt(string keyTutorial, int index, float angle = 0, float duration = 1f)
    {
        this.keyTutorial = keyTutorial;

        var posParent = posParents.GetChild(index);
        Vector2 startValue = posParent.GetChild(0).position;
        Vector2 endValue = posParent.GetChild(1).position;

        obj.sprite = arrowSpr;
        obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        obj.gameObject.SetActive(true);
        DoAnim(obj.transform, startValue, endValue, duration);
    }

    void DoAnim(Transform obj, Vector2 startValue, Vector2 endValue, float duration = 1f)
    {
        obj.DOKill();
        obj.position = startValue;
        obj.DOMove(endValue, duration)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }

    public void StopTutorial()
    {
        this.keyTutorial = "";
        HideObject();
        HideLight();
        HidePanel();
    }

    public void HideObject()
    {
        obj.transform.DOKill();
        obj.gameObject.SetActive(false);
    }

    public void HideLight()
    {
        darkPanel.SetActive(false);
        lightPanel.gameObject.SetActive(false);
    }

    public void HidePanel()
    {
        darkPanel.SetActive(false);
        panel.SetActive(false);
    }

    public void ShowSquareLightAt(string keyTutorial, int index)
    {
        var posParent = posParents.GetChild(index);
        Vector2 pos = posParents.GetChild(0).position;
        Vector2 size = posParent.GetChild(1).localScale;
        ShowSquareLightAt(keyTutorial, pos, size);
    }

    public void ShowCircleLightAt(string keyTutorial, int index)
    {
        var posParent = posParents.GetChild(index);
        Vector2 pos = posParents.GetChild(0).position;
        Vector2 size = posParent.GetChild(1).localScale;
        ShowCircleLightAt(keyTutorial, pos, size);
    }

    public void ShowSquareLightAt(string keyTutorial, Vector2 pos, Vector2 size)
    {
        this.keyTutorial = keyTutorial;

        darkPanel.SetActive(true);
        lightPanel.gameObject.SetActive(true);
        lightPanel.sprite = squareSpr;
        lightPanel.transform.position = pos;
        lightPanel.transform.localScale = size;
    }

    public void ShowCircleLightAt(string keyTutorial, Vector2 pos, Vector2 size)
    {
        this.keyTutorial = keyTutorial;

        darkPanel.SetActive(true);
        lightPanel.gameObject.SetActive(true);
        lightPanel.sprite = circleSpr;
        lightPanel.transform.position = pos;
        lightPanel.transform.localScale = size;
    }

    public void ShowTutorialPanelAt(string keyTutorial, string text, bool showBtn = false, Action OnExit = null)
    {
        this.keyTutorial = keyTutorial;
        this.OnExit = OnExit;

        panel.SetActive(true);
        exit2Btn.gameObject.SetActive(showBtn);
        textMesh.text = text;
    }

    public void Exit()
    {
        OnExit?.Invoke();
    }

    public void Tap()
    {
        OnTap?.Invoke();
    }

    public void ShowDarkPanel(Action OnTap = null)
    {
        darkPanel.SetActive(true);
        this.OnTap = OnTap;
    }

    public void ShowReceivePanel(string keyTutorial, Sprite rewardSprite, string name, Action OnReceive = null)
    {
        this.keyTutorial = keyTutorial;
        this.OnReceive = OnReceive;
        rewardImg.sprite = rewardSprite;
        receiveModal.SetActive(true);
        lightEfx.SetActive(false);
        nameBooster.gameObject.SetActive(false);
        rewardImg.transform.localScale = Vector2.zero;
        receiveBtn.transform.localScale = Vector2.zero;
        nameBooster.text = name;

        var atPosition = 0.3f;
        var duration = 0.3f;
        var spaceTime = 0.12f;
        Sequence seq = DOTween.Sequence();

        seq.InsertCallback(atPosition,()=> darkPanel.SetActive(true));

        seq.Insert(atPosition,
        rewardImg.transform.DOScale(Vector2.one, duration)
        .SetEase(Ease.OutBack));

        seq.InsertCallback(atPosition + duration,()=> nameBooster.gameObject.SetActive(true));

        seq.Insert(atPosition + duration,
        receiveBtn.transform.DOScale(Vector2.one, duration)
        .SetEase(Ease.OutBack));

        seq.InsertCallback(atPosition + spaceTime, () => lightEfx.SetActive(true));

        lightEfx.transform.DORotate(new Vector3(0, 0, 360f), 2f, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Restart);
    }

    public void HideReceivePanel()
    {
        var atPosition = 0f;
        var duration = 0.3f;
        var spaceTime = 0.12f;
        lightEfx.transform.DOKill();
        Sequence seq = DOTween.Sequence();

        seq.Insert(atPosition,
        rewardImg.transform.DOScale(Vector2.zero, duration)
        .SetEase(Ease.InBack));

        seq.Insert(atPosition + duration,
        receiveBtn.transform.DOScale(Vector2.zero, duration)
        .SetEase(Ease.InBack));

        seq.Insert(atPosition + spaceTime,
        lightEfx.transform.DOScale(Vector2.zero, duration)
        .SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            receiveModal.SetActive(false);
            darkPanel.SetActive(false);
            nameBooster.gameObject.SetActive(false);
            OnReceive?.Invoke();
        });
    }
}
