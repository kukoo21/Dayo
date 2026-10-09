using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public Animator interactAnim;
    public Animator keyPress;
    public List<DialogueSO> conversations;
    public DialogueSO currentConversation;
    public Transform KeyPressObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // Makes NPC immovable when triggered
        anim.Play("Idle"); // Plays idle animation
        interactAnim.Play("Open"); // Plays open icon animation
        keyPress.Play("KeyPress"); // Plays key press icon animation

    }

    private void OnDisable()
    {
        anim.Play("Hammer"); // Plays hammer animation
        interactAnim.Play("Close"); // Plays close icon animation
        keyPress.Play("KeyPressClose"); // Plays key release icon animation
        rb.bodyType = RigidbodyType2D.Dynamic; // Makes NPC movable again
    }

    private void Update()
    {
        //============Interaction============

        if (Input.GetButtonDown("Interact"))
        {
            if (DialogueManager.Instance.isDialogueActive)
                DialogueManager.Instance.AdvanceDialogue();
            else
            {
                CheckForNewConversation();
                DialogueManager.Instance.StartDialogue(currentConversation);
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (DialogueManager.Instance.isDialogueActive)
        //        DialogueManager.Instance.AdvanceDialogue();
        //    else
        //    {
        //        CheckForNewConversation();
        //        DialogueManager.Instance.StartDialogue(currentConversation);
        //    }
        //}
    }

    private void CheckForNewConversation()
    {
        for (int i = 0; i < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo != null && convo.isConditionMet())
            {
                conversations.RemoveAt(i);
                currentConversation = convo;
            }
        }
    }
}