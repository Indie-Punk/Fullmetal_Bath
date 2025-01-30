using System;
using DG.Tweening;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Events;

public class InteractionBtn : MonoBehaviour
{
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;

    [SerializeField] private BtnType btnType;
    [SerializeField] private Vector3 distance;
    [SerializeField] private float time;

    private bool pushed;
    private Sequence seq;
    private Vector3 defaultPos;

    enum BtnType
    {
        Click,
        Touch
    };

    public void Touch()
    {
        if (seq != null)
            return;
        switch (btnType)
        {
            case BtnType.Click:
                Click();
                break;
            case BtnType.Touch:
                if (!pushed)
                    PushOn();
                else
                    PushOff();
                break;
        }
    }

    void Click()
    {
        defaultPos = transform.localPosition;
        seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(defaultPos + distance, time));
        seq.Append(transform.DOLocalMove(defaultPos, time));
        seq.OnComplete(() =>
        {
            seq = null;
            OnActivate?.Invoke();
        });
    }

    public void PushOn()
    {
        defaultPos = transform.localPosition;
        seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(defaultPos + distance, time));
        seq.OnComplete(() =>
        {
            pushed = true;
            seq = null;
            OnActivate?.Invoke();
        });
    }

    public void PushOff()
    {
        seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMove(defaultPos, time));
        seq.OnComplete(() =>
        {
            pushed = false;
            seq = null;
            OnDeactivate?.Invoke();
        });
    }
}