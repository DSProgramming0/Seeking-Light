using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("GameEvents")]
    [SerializeField] private AnimHook currentAnim;
    [SerializeField] private DoTweener sidePrompt;
    public GameEvent ConversationEnded;
    public GameEvent ConversationStarted;
    [Space]
    public StringVariable playerName;
    [Tooltip("For the typing animation. Determine how long it takes for each character to appear")]
    public float timeBetweenChars = 0.05f;
    [Header("UI")]
    [SerializeField] private bool isTyping = false;
    [SerializeField] private Button continueButton;
    [SerializeField] private TextMeshProUGUI continueButtonText;
    public TextMeshProUGUI speakerNameTxtUI;
    public TextMeshProUGUI speakerTextUI;

    [Tooltip("The part of UI that display the UI")]
    public GameObject DialogueUI;
    [Tooltip("The text UIs that display options")]
    public TextMeshProUGUI[] optionsUI;
    [SerializeField] private int currentOptionIndex = 0;
    [SerializeField] private GameObject currentOptionHighlight;

    DialogueSO dialogue;
    Sentence currentSentence;  

    public void StartDialogue(DialogueSO dialogueSO, AnimHook _currentAnim)
    {
        if (!dialogueSO.isAvailable)
        {
            return;
        }
        if (ConversationStarted!=null)
        {
            ConversationStarted.Raise();
        }
        //animator.SetTrigger("InDialogue");

        if(_currentAnim != null)
        {
            currentAnim = _currentAnim;
            currentAnim.startNPCconverstation();
        }
        
        PlayerStates.instance.currentConverstaionState = PlayerConverstaionStates.IN_CONVERSATION;
        speakerTextUI.text = null;
        speakerTextUI.text = null;
        HideOptions();
        DialogueUI.SetActive(false);
        
        dialogue = dialogueSO;
        if (speakerNameTxtUI!=null)
        {
            speakerNameTxtUI.text = playerName.Value + ":";
        }
        currentSentence = dialogue.startingSentence;

        DisplayDialogue();
    }

    public void GoToNextSentence()
    {
        if(currentSentence == null)
        {
            EndDialogue();
        }
        currentSentence = currentSentence.nextSentence;
        DisplayDialogue();
    }

    public void DisplayDialogue()
    {
        if (currentSentence == null)
        {
            EndDialogue();
            return;
        }

        if (!currentSentence.HasOptions())
        {
            DialogueUI.SetActive(true);
            HideOptions();
            // sentence with no options
            // can either be from player or npc
            TextMeshProUGUI dialogueText;
            if (currentSentence.from.Value == playerName.Value)
            {
                // from player, set the textbox
                if (speakerNameTxtUI != null)
                {
                    speakerNameTxtUI.text = playerName.Value + ":";
                }
                dialogueText = speakerTextUI;
            }
            else
            {
                // from npc
                if (speakerNameTxtUI != null)
                {
                    speakerNameTxtUI.text = currentSentence.from.name + ":";

                }
                dialogueText = speakerTextUI;
            }

            // display the text
           StopAllCoroutines();
           StartCoroutine(Typeout(currentSentence.text, dialogueText));
        }
        else
        {
            // with options. can only be from player
            DisplayOptions();
        }
    }

    void Update()
    {    
        checkMenuInput();
    }

    IEnumerator Typeout(string sentence, TextMeshProUGUI textbox)
    {
        textbox.text = "";
        foreach (var letter in sentence.ToCharArray())
        {
            isTyping = true;
            textbox.text += letter;
            SoundManager.Play2DSound(SoundManager.Sound.Blip1, 1f, .025f);
            yield return new WaitForSeconds(timeBetweenChars);
        }

        if (textbox.text == sentence)
        {
            Debug.Log("Sentece complete");
            isTyping = false;
        }
    }

    private void checkMenuInput()
    {
        if (PlayerStates.instance.currentConverstaionState == PlayerConverstaionStates.IN_CONVERSATION)
        {
            if (isTyping == false)
            {
                currentOptionHighlight.SetActive(true);
                continueButton.interactable = true;
                continueButtonText.enabled = true;

                if (Input.GetButtonDown("MenuPositive"))
                {
                    currentOptionIndex += 1;
                    if (currentOptionIndex > currentSentence.options.Count - 1)
                    {
                        currentOptionIndex = 0;
                    }
                }

                if (Input.GetButtonDown("MenuNegative"))
                {
                    currentOptionIndex -= 1;
                    if (currentOptionIndex < 0)
                    {
                        currentOptionIndex = currentSentence.options.Count - 1;
                    }
                }

                highlightOption(currentOptionIndex);

            }
            else
            {
                currentOptionHighlight.SetActive(false);
                continueButton.interactable = false;
                continueButtonText.enabled = false;
            }

            if (currentSentence.options.Count == 0)
            {
                Debug.Log("No options available");
                currentOptionHighlight.SetActive(false);

                if (Input.GetButtonDown("Submit") && isTyping == false)
                {
                    GoToNextSentence();
                }
            }
        }
        else
        {
            currentOptionHighlight.SetActive(false);
            continueButton.interactable = false;
            continueButtonText.enabled = false;
        }
    }

    public void highlightOption(int index)
    {
        currentOptionHighlight.transform.position = optionsUI[index].transform.position;

        if (Input.GetButtonDown("Submit") && isTyping == false && currentSentence.options.Count > 0)
        {
            OptionsOnClick(index);
        }
    }

    public void OptionsOnClick(int index)
    {
        Choice option = currentSentence.options[index];
        if (option.consequence!=null)
        {
            Debug.Log("Raise Events");
            option.consequence.Raise();
        }
        currentSentence = option.nextSentence;
        sidePrompt.InvokeTween(false);
        DisplayDialogue();
    }

    public void DisplayOptions()
    {
        //Debug.Log(currentSentence.options.Count);
        DialogueUI.SetActive(false);
        //OptionsUI.SetActive(true);
        if (currentSentence.from.Value == playerName.Value)
        {
            // from player, set the textbox
            if (speakerNameTxtUI != null)
            {
                speakerNameTxtUI.text = playerName.Value + ":";
            }
        }

        if (currentSentence.options.Count <= optionsUI.Length)
        {
            for (int i = 0; i < currentSentence.options.Count; i++)
            {
                Debug.Log(currentSentence.options[i].text);
                optionsUI[i].text = currentSentence.options[i].text;
                optionsUI[i].gameObject.SetActive(true);
            }
        }
    }

    public void HideOptions()
    {
        foreach (TextMeshProUGUI option in optionsUI)
        {
            option.gameObject.SetActive(false);
        }
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue ended");
        //animator.SetTrigger("OutDialogue");

        if (ConversationEnded!=null)
        {
            if (currentAnim != null)
            {
                currentAnim.endNPConverstaion();
            }

            ConversationEnded.Raise();
            PlayerStates.instance.currentConverstaionState = PlayerConverstaionStates.NOT_IN_CONVERSATION;

            currentAnim = null;

        }

    }
}
