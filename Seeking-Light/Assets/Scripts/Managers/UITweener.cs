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

[Header("Door Puzzle")]
    [SerializeField] private float doorLocalX;
    [SerializeField] private float doorScaleY;

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
        Debug.Log("Called");

        StartCoroutine(Blackout());
    }

    public void fadeOutBackground()
    {
        LeanTween.alphaCanvas(thisCanvasGroup, 0, 10 * Time.fixedDeltaTime);
    }

    private IEnumerator Blackout() //Fades screen to black
    {
        LeanTween.alphaCanvas(thisCanvasGroup, 1, 10 * Time.fixedDeltaTime);
        yield return new WaitForSeconds(duration);
        LeanTween.alphaCanvas(thisCanvasGroup, 0, 10 * Time.fixedDeltaTime);

        StopCoroutine(Blackout());
    }

    public void toggleDoor()
    {
        LeanTween.scaleY(this.gameObject, 0.5f, duration).setDelay(delay).setEase(inType);
        LeanTween.moveLocalX(this.gameObject, 0, duration);
    }   
}
