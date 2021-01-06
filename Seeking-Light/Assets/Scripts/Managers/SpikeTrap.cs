using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private PlayerDeath _playerDeath;

    private bool spikeTriggered = false;
    
    public bool SpikeTriggered //Getter and setter which is reset when the reset method on the GameManager script is called
    {
        get { return spikeTriggered; }
        set { spikeTriggered = value; }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(SpikeTriggered == false)
        {
            if (collision.CompareTag("Player"))
            {
                if (_playerDeath != null)
                {
                    _playerDeath.Death();
                }
                else
                {
                    _playerDeath = collision.GetComponent<PlayerDeath>();
                    _playerDeath.Death();
                }

                StartCoroutine(playImpalementSounds());

                SpikeTriggered = true;
                GameManager.instance.addGameObjectToList(this.gameObject); //Adds itself to the list as it needs to be reset so it can be re used when the player respawns
            }
        }        
    }

    private IEnumerator playImpalementSounds()
    {
        yield return new WaitForSeconds(.1f);
        SoundManager.Play2DSound(SoundManager.Sound.SpikeImpalement1, 2f, .5f);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        SoundManager.Play2DSound(SoundManager.Sound.SpikeImpalement1, 2f, .5f);
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        SoundManager.Play2DSound(SoundManager.Sound.SpikeImpalement1, 2f, .5f);

        StopCoroutine(playImpalementSounds());
    }

}
