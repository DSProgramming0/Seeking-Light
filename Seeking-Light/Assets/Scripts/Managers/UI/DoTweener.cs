using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class DoTweener : MonoBehaviour
{
    [SerializeField] private Vector2 _targetLocation = Vector2.zero;
    [SerializeField] private Vector2 originalLocation = Vector2.zero;

    [SerializeField] private bool showOnStart;

    [Range(0f, 10.0f)]
    [SerializeField] private float _moveDuration = 1.0f;
    [Range(0f, 12.0f)]
    [SerializeField] public float _HoldPosDuration;
    [SerializeField] public bool canMoveBack;

    [SerializeField] private Vector2 originalSize;
    [SerializeField] private Vector2 targetSize;

    [SerializeField] private Ease _moveEase = Ease.Linear;

    [SerializeField] private DoTweenType _doTweenType = DoTweenType.MovementOneWay;
    private enum DoTweenType
    {
        MovementOneWay,
        MovementTwoWay,
        MovementOnEvent,
        Repeat,
        AlterSizeAndHide,
        AlterSize
    }

    void Start()
    {
        if (showOnStart)
        {
            InvokeTween(false);
        }
    }

    public void InvokeTween(bool shouldMoveOnEvent)
    {
        if (_doTweenType == DoTweenType.MovementOneWay)
        {
            if (_targetLocation == Vector2.zero)
                _targetLocation = transform.position;

            RectTransform rectTrans = transform.GetComponent<RectTransform>();
            rectTrans.DOAnchorPos(_targetLocation, _moveDuration).SetEase(_moveEase);
        }
        else if (_doTweenType == DoTweenType.MovementTwoWay)
        {
            if (_targetLocation == Vector2.zero)
                _targetLocation = transform.position;
            StartCoroutine(MoveBothWays());
        }
        else if (_doTweenType == DoTweenType.MovementOnEvent)
        {
            if (_targetLocation == Vector2.zero)
                _targetLocation = transform.position;
            MoveOnEvent(shouldMoveOnEvent);
        }
        else if (_doTweenType == DoTweenType.Repeat)
        {
            if (_targetLocation == Vector2.zero)
                _targetLocation = transform.position;
            StartCoroutine(Repeat());
        }
        else if (_doTweenType == DoTweenType.AlterSizeAndHide)
        {
            if (_targetLocation == Vector2.zero)
                _targetLocation = transform.position;

            StartCoroutine(alterSizeAndHide());
        }
        else if (_doTweenType == DoTweenType.AlterSize)
        {

            StartCoroutine(alterSize());
        }
    }

    private IEnumerator alterSizeAndHide()
    {
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        CanvasGroup thisCanvas = transform.GetComponent<CanvasGroup>();
        thisCanvas.alpha = 1;
        rectTrans.DOScale(targetSize, _moveDuration);
        yield return new WaitForSeconds(_HoldPosDuration);
        rectTrans.DOScale(originalSize, _moveDuration);
        yield return new WaitForSeconds(1.5f);
        thisCanvas.alpha = 0;
    }

    private IEnumerator alterSize()
    {
        Transform rectTrans = transform.GetComponent<Transform>();
        rectTrans.DOScale(targetSize, _moveDuration);
        yield return new WaitForSeconds(2f);
        rectTrans.DOScale(originalSize, _moveDuration);
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(alterSize());
    }

    private IEnumerator MoveBothWays()
    {
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        rectTrans.DOAnchorPos(_targetLocation, _moveDuration, false).SetEase(_moveEase);
        yield return new WaitForSeconds(_HoldPosDuration);
        rectTrans.DOAnchorPos(originalLocation, _moveDuration, false).SetEase(_moveEase);
    }

    private void MoveOnEvent(bool moveOnEvent)
    {
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        rectTrans.DOAnchorPos(_targetLocation, _moveDuration, false).SetEase(_moveEase);
        if (moveOnEvent)
        {
            rectTrans.DOAnchorPos(originalLocation, _moveDuration, false).SetEase(_moveEase);
        }
    }

    private IEnumerator Repeat()
    {
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        rectTrans.DOAnchorPos(_targetLocation, _moveDuration, false).SetEase(_moveEase);
        yield return new WaitForSeconds(0.5f);
        rectTrans.DOAnchorPos(originalLocation, _moveDuration, false).SetEase(_moveEase);
        yield return new WaitForSeconds(_moveDuration);

        StartCoroutine(Repeat());
    }
}
