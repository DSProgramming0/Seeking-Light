using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueSO dialogue;

    public DialogueManager dialogueManager;

    [SerializeField] private bool conversationExpended = false;

    void OnTriggerStay2D(Collider2D collision)
    {
        if(conversationExpended == false)
        {
            if (collision.CompareTag("Player") && Input.GetButtonDown("Interact"))
            {
                StartDialogue();
                conversationExpended = true;
            }
        }       
    }

    public void StartDialogue()
    {
        dialogueManager.StartDialogue(dialogue);
    }

}
