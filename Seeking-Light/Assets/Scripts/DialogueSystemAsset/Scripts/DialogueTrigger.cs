using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private AnimHook thisAnim;
    [SerializeField] private bool requiresInteractionPress;

    public DialogueSO dialogue;

    public DialogueManager dialogueManager;

    [SerializeField] private bool conversationExpended = false;

    public bool ConversationExpended
    {
        get { return conversationExpended; }
        set { conversationExpended = value; }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(ConversationExpended == false)
        {
            if (collision.CompareTag("Player"))
            {
                if (requiresInteractionPress)
                {
                    if (Input.GetButtonDown("Interact"))
                    {
                        StartDialogue();
                        ConversationExpended = true;
                    }
                }
                else
                {
                    StartDialogue();
                    ConversationExpended = true;
                }
               
            }
        }       
    }

    public void StartDialogue()
    {
        if(thisAnim != null)
        {
            dialogueManager.StartDialogue(dialogue, thisAnim);
        }
        else
        {
            dialogueManager.StartDialogue(dialogue, null);
        }
    }

}
