using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class MapTransition : MonoBehaviour
{
    [SerializeField] PolygonCollider2D mapBoundary;
    private CinemachineConfiner2D confiner;
    private CinemachineCamera vcam;

    [SerializeField] Direction direction;
    [SerializeField] Transform teleportTargetPos;

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 0.3f;     // fade in/out time
    [SerializeField] private float blackScreenPause = 1.5f; // how long to stay black

    private enum Direction { Up, Down, Left, Right, teleport }

    private void Awake()
    {
        confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        vcam = confiner.GetComponent<CinemachineCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DoTeleportWithFade(collision.gameObject));
        }
    }

    private IEnumerator DoTeleportWithFade(GameObject player)
    {
        // Fade to black
        yield return StartCoroutine(ScreenFader.FadeOut(fadeDuration));

        // Stay black for a short while
        yield return new WaitForSeconds(blackScreenPause);

        // Teleport logic
        confiner.BoundingShape2D = mapBoundary;
        UpdatePlayerPosition(player);
        confiner.InvalidateBoundingShapeCache();

        // Force Cinemachine to instantly snap to new position
        if (vcam != null)
        {
            vcam.ForceCameraPosition(player.transform.position, Quaternion.identity);
        }

        // Fade back in
        yield return StartCoroutine(ScreenFader.FadeIn(fadeDuration));
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        if (direction == Direction.teleport)
        {
            player.transform.position = teleportTargetPos.position;
            return;
        }

        Vector3 additivePos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                additivePos.y += 2;
                break;
            case Direction.Down:
                additivePos.y -= 2;
                break;
            case Direction.Left:
                additivePos.x -= 2;
                break;
            case Direction.Right:
                additivePos.x += 2;
                break;
        }
        player.transform.position = additivePos;
    }
}
