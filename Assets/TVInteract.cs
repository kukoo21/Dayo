using UnityEngine;

public class TVInteract : MonoBehaviour
{
    public VideoLauncher videoLauncher; // assign in Inspector

    private void OnMouseDown()
    {
        Debug.Log("TV clicked!"); // check if this prints
        if (videoLauncher != null)
        {
            videoLauncher.PlayVideo();
        }
    }
}
