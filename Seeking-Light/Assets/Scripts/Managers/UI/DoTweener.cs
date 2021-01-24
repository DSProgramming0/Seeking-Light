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

    [SerializeField] private float minRot;
    [SerializeField] private float maxRot;

    [SerializeField] private Ease _moveEase = Ease.Linear;

    [SerializeField] private DoTweenType _doTweenType = DoTweenType.MovementOneWay;
    private enum DoTweenType
    {
        MovementOneWay,
        MovementTwoWay,
        MovementOnEvent,
        MoveAndWait,
        Repeat,
        AlterSizeAndHide,
        AlterSize,
        RotateRepeat
    }

    void Start()
    {
        DOTween.SetTweensCapacity(100000, 100000);

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
        else if (_doTweenType == DoTweenType.MoveAndWait)
        {
            if (_targetLocation == Vector2.zero)
                _targetLocation = transform.position;
            StartCoroutine(moveAndWait());
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
        else if(_doTweenType == DoTweenType.RotateRepeat)
        {
            StartCoroutine(rotateRepeat());
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

    public void toggleCanMoveBack(bool _shouldShow)
    {
        if (_shouldShow)
        {
            canMoveBack = true;
        }
        else
        {
            canMoveBack = false;
        }
    }

    private IEnumerator moveAndWait()
    {
        RectTransform rectTrans = transform.GetComponent<RectTransform>();
        CanvasGroup thisCanvas = transform.GetComponentInParent<CanvasGroup>();
        if(canMoveBack == false)
        {
            rectTrans.DOAnchorPos(_targetLocation, _moveDuration, false).SetEase(_moveEase);
            thisCanvas.alpha += 0.2f * Time.deltaTime;
        }       

        yield return new WaitUntil(() => canMoveBack == true);

        rectTrans.DOAnchorPos(originalLocation, _moveDuration, false).SetEase(_moveEase);
        thisCanvas.alpha -= 0.4f * Time.deltaTime;


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


    private IEnumerator rotateRepeat()
    {
        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, Mathf.Lerp(transform.rotation.z, maxRot, Time.fixedDeltaTime * _moveDuration)));

        yield return new WaitForSeconds(_HoldPosDuration);

        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, Mathf.Lerp(transform.rotation.z, minRot, Time.fixedDeltaTime * _moveDuration)));

        StartCoroutine(rotateRepeat());
    }
}
