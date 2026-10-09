using UnityEngine;
using Unity.Cinemachine;

public class LoadGameInitializer : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            Vector3 pos = new Vector3(
                PlayerPrefs.GetFloat("PlayerPosX"),
                PlayerPrefs.GetFloat("PlayerPosY"),
                PlayerPrefs.GetFloat("PlayerPosZ")
            );

            string mapName = PlayerPrefs.GetString("LastMap");

            var player = GameObject.FindGameObjectWithTag("Player");
            var confiner = FindFirstObjectByType<CinemachineConfiner2D>();

            if (player != null)
                player.transform.position = pos;

            if (confiner != null && !string.IsNullOrEmpty(mapName))
                confiner.BoundingShape2D = GameObject.Find(mapName).GetComponent<PolygonCollider2D>();
        }
    }
}
