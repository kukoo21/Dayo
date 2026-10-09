using UnityEngine;

public class NPCFacePlayer : MonoBehaviour
{
    public Transform player;  // Drag your Player here
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = player.position - transform.position;

        // Decide which axis dominates
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Horizontal
            if (direction.x > 0)
            {
                anim.SetInteger("Direction", 3); // Right
            }
            else
            {
                anim.SetInteger("Direction", 2); // Left
            }
        }
        else
        {
            // Vertical
            if (direction.y > 0)
            {
                anim.SetInteger("Direction", 1); // Up
            }
            else
            {
                anim.SetInteger("Direction", 0); // Down
            }
        }
    }
}
