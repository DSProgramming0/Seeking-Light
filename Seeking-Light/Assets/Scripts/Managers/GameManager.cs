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

   
}
