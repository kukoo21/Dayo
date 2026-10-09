using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public static CustomCursor Instance;
    public Texture2D cursorTexture;   // Drag your PNG here in Inspector
    public Vector2 hotSpot = Vector2.zero; // The "click point" on the image (0,0 = top-left)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    
}
    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
}
    
