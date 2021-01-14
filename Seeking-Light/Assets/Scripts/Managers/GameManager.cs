using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private List<GameObject> GameObjectsToReset;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        SoundManager.Initialize();
    }

    public void addGameObjectToList(GameObject _thisGameObject)
    {
        GameObjectsToReset.Add(_thisGameObject);
    }

    public void resetGameComponents()
    {
        foreach(GameObject GO in GameObjectsToReset)
        {
            if(GO.GetComponent<SpikeTrap>().SpikeTriggered == true)
            {
                SpikeTrap thisSpikeTrap = GO.GetComponent<SpikeTrap>();
                thisSpikeTrap.SpikeTriggered = false;
            }
        }

        GameObjectsToReset.Clear();
    }

    public void callFadeOutSound(AudioSource _audioSource, float _fadeTime)
    {
        StartCoroutine(FadeOutSounds(_audioSource, _fadeTime));
    }

    private IEnumerator FadeOutSounds(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        yield return new WaitForSeconds(1f);
        SoundManager.DestroySound(audioSource.gameObject);
    }


    public void disableSection(GameObject _sectionToDisable)
    {
        _sectionToDisable.SetActive(false);
    }

    public void enableSection(GameObject _sectionToEnable)
    {
        _sectionToEnable.SetActive(true);
    }
}
