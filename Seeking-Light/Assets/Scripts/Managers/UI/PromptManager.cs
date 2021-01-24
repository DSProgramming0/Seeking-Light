using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptManager : MonoBehaviour
{
    public static PromptManager instance;

    private GameObject spawnedPrompt;
    private CanvasGroup thisCanvasGroup;

    public GameObject SpawnedPrompt
    {
        get { return spawnedPrompt; }
        set { spawnedPrompt = value; }
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;   
    }

   
    public void spawnPrompt(GameObject _objToSpawn, Vector2 _Pos, Vector2 _Offset)
    {
        if(spawnedPrompt == null)
        {
            spawnedPrompt = Instantiate(_objToSpawn, _Pos + _Offset, Quaternion.identity);

            thisCanvasGroup = spawnedPrompt.GetComponentInParent<CanvasGroup>();
        }

        thisCanvasGroup.alpha += Time.deltaTime;
    }

    public void destroyPrompt()
    {
        Destroy(spawnedPrompt);

        spawnedPrompt = null;
        thisCanvasGroup = null;
    }

    
}
