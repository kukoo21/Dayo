using UnityEngine;

public class ClickableDialogue : MonoBehaviour
{
    public Dialogue dialogue;

    private void OnMouseDown()
    {
        // Optional: check if player is close before allowing click
        if (CutsceneDialogueManager.Instance != null)
        {
            CutsceneDialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
