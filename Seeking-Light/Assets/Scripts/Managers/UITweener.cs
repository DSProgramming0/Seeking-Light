using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweener : MonoBehaviour
{
    public CanvasGroup thisCanvasGroup;
    public LeanTweenType inType;
    public LeanTweenType outType;
    public float duration;
    public float delay;

    public void showUI()
    {
        LeanTween.scale(gameObject, new Vector3(1, 1, 1), duration).setDelay(delay).setEase(inType);
    }

    public void hideUI()
    {
        LeanTween.scale(gameObject, new Vector3(0, 0, 0), duration).setDelay(delay).setEase(outType);
    }

    public void startFade()
    {
        StartCoroutine(Blackout());
    }

    public void fadeOutBackground()
    {
        LeanTween.alphaCanvas(thisCanvasGroup, 0, 10 * Time.fixedDeltaTime);
    }

    private IEnumerator Blackout()
    {
        LeanTween.alphaCanvas(thisCanvasGroup, 1, 10 * Time.fixedDeltaTime);
        yield return new WaitForSeconds(duration);
        LeanTween.alphaCanvas(thisCanvasGroup, 0, 10 * Time.fixedDeltaTime);

        StopCoroutine(Blackout());
    }
}
