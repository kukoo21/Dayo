using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MapGateController : MonoBehaviour
{
    [Header("Unlock Condition")]
    [Tooltip("Assign the NPC’s ActorSO whose dialogue must be completed to unlock this gate.")]
    public ActorSO requiredActor;

    private Collider2D gateCollider;
    private SpriteRenderer gateVisual;

    private void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
        gateVisual = GetComponent<SpriteRenderer>();

        if (gateCollider == null)
        {
            Debug.LogError("[MapGateController] Missing Collider2D on " + gameObject.name);
            enabled = false;
        }
    }

    private void Start()
    {
        UpdateGateState();
    }

    private void Update()
    {
        UpdateGateState();
    }

    private void UpdateGateState()
    {
        if (requiredActor == null || gateCollider == null)
            return;

        // ✅ Check only DialogueHistoryTracker (no PlayerPrefs)
        bool hasSpoken = false;

        if (DialogueHistoryTracker.Instance != null)
        {
            hasSpoken = DialogueHistoryTracker.Instance.HasSpokenWith(requiredActor);
        }
        else
        {
            Debug.LogWarning("[MapGateController] DialogueHistoryTracker instance not found in scene.");
        }

        // ✅ Apply gate state based on session data
        gateCollider.isTrigger = hasSpoken;

        if (gateVisual != null)
        {
            gateVisual.color = hasSpoken ? Color.gray : Color.white;
        }

        Debug.Log($"[MapGateController] Actor: {requiredActor.actorName}, hasSpoken: {hasSpoken}, isTrigger: {gateCollider.isTrigger}");
    }
}
