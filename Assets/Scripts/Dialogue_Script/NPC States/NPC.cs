using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPCState { Default, Idle, Patrol, Wander, Talk }
    public NPCState currentState = NPCState.Patrol;
    private NPCState defaultState;

    public NPC_Patrol patrol;
    public NPC_Talk talk;
    public NPC_Wander wander;

    void Start()
    {
        defaultState = currentState;
        SwitchState(currentState);
    }

    public void SwitchState(NPCState newState)
    {
        currentState = newState;

        patrol.enabled = newState == NPCState.Patrol;
        talk.enabled = newState == NPCState.Talk;
        wander.enabled = newState == NPCState.Wander;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Talk);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(defaultState);
        }
    }
}
