using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private CanvasGroup blackoutPanel;

    void Awake()
    {
        instance = this;
    }

    public void fadeout()
    {
        blackoutPanel.GetComponent<UITweener>().startFade();
    }
}
