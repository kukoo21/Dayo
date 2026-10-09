using UnityEngine;

public class LocationVisitedTrigger : MonoBehaviour
{
    [Header("Location SO")]
    [SerializeField] private LocationSO locationVisited;
    [SerializeField] private bool destroyOnTouch = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LocationHistorytracker.Instance.RecordLocation(locationVisited);

            QuestManager.Instance.CompleteLocationObjective(locationVisited.locationID);

            if (destroyOnTouch)
                Destroy(gameObject);
        }
    }
}
