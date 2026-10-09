using UnityEngine;

public class EnemyHPTextNoFlip : MonoBehaviour
{
    void LateUpdate()
    {
        // Counter the parent's WORLD flip (not local)
        Vector3 worldScale = transform.lossyScale;

        transform.localScale = new Vector3(
            1f / Mathf.Abs(worldScale.x),
            1f / Mathf.Abs(worldScale.y),
            1f / Mathf.Abs(worldScale.z)
        );
    }
}
