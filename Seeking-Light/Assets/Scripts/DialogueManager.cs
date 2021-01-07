using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private NPC thisNPC;
    [SerializeField] private bool isTalking = false;

    [SerializeField] private bool inRange = false;
    private int currentResponseTracker = 0;

    public GameObject player;
    public GameObject dialogueUI;

    public TextMeshProUGUI NPCName;
    public TextMeshProUGUI NPCDialogue;
    public TextMeshProUGUI playerResponse;

    void Start()
    {
        dialogueUI.SetActive(false);
    }

    void Update()
    {
        rangeCheck();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    void rangeCheck()
    {
        if (inRange)
        {
            if(Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                currentResponseTracker++;
                if(currentResponseTracker >= thisNPC.playerDialogue.Length - 1)
                {
                    currentResponseTracker = thisNPC.playerDialogue.Length - 1;
                }
            }
            else if(Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                currentResponseTracker--;
                if(currentResponseTracker < 0)
                {
                    currentResponseTracker = 0;
                }
            }

            Debug.Log("In range");
            if (Input.GetKeyDown(KeyCode.E) && isTalking == false)
            {
                startConversation();
            }
            else if(Input.GetKeyDown(KeyCode.E) && isTalking == true)
            {
                endConverstaion();
            }

            if(currentResponseTracker == 0 && thisNPC.playerDialogue.Length >= 0)
            {
                playerResponse.text = thisNPC.playerDialogue[0];
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    NPCDialogue.text = thisNPC.NPCDialogue[1];
                }
            }
            else if(currentResponseTracker == 1 && thisNPC.playerDialogue.Length >= 1)
            {
                playerResponse.text = thisNPC.playerDialogue[1];
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    NPCDialogue.text = thisNPC.NPCDialogue[2];
                }
            }
            else if (currentResponseTracker == 2 && thisNPC.playerDialogue.Length >= 2)
            {
                playerResponse.text = thisNPC.playerDialogue[2];
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    NPCDialogue.text = thisNPC.NPCDialogue[3];
                }
            }
        }
        else
        {
            Debug.Log("Not in range");
        }
    }

    private void startConversation()
    {
        isTalking = true;
        currentResponseTracker = 0;
        dialogueUI.SetActive(true);
        NPCName.text = thisNPC.name;
        NPCDialogue.text = thisNPC.NPCDialogue[0];
    }

    private void endConverstaion()
    {
        isTalking = false;
        dialogueUI.SetActive(false);

    }

    private IEnumerator chatOver()
    {
        yield return new WaitForSeconds(1.5f);
        endConverstaion();
    }
}
