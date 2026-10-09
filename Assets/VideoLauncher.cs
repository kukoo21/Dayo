using UnityEngine;
using UnityEngine.Video;

public class VideoLauncher : MonoBehaviour
{
    public VideoPlayer videoPlayer;   // Assign in Inspector
    public GameObject videoBorder;    // Assign a UI border

    public void PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            if (videoBorder != null)
                videoBorder.SetActive(true);
        }
        else
        {
            Debug.LogError("VideoPlayer not assigned in Inspector!");
        }
    }

    public void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            if (videoBorder != null)
                videoBorder.SetActive(false);
        }
    }
}
