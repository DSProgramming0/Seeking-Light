using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this.transform.parent.gameObject);
    }

    public static GameAssets instance
    {
        get
        {
            return _instance;
        }
    }

    [Header("AudioSource")]
    public AudioSource mainAudioSource;
    public Transform cryingChildAudioSource;

    [Header("GameObjects")]
    public GameObject interactKeyPrompt;

    [Header("TempEffect")]

    [Header("SoundClips")]
    public List<soundAudioClip> soundAudioClipArray;

    [Header("BackgroundMusic")]
    public List<soundAudioClip> backgroundMusicList;

    [System.Serializable]
    public class soundAudioClip
    {
        public SoundManager.Sound audioClipName;
        public AudioClip audioClip;
    }  
}
