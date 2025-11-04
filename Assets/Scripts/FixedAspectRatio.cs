using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    [Tooltip("Desired aspect ratio, e.g., 16:9 = 1.7777")]
    public float targetAspect = 16f / 9f;

    private Camera cam;
    private float lastWidth;
    private float lastHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Start()
    {
        ApplyAspect();
    }

    void Update()
    {
        // Only reapply if screen size changed
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            ApplyAspect();
            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }

    void ApplyAspect()
    {
        if (cam == null) cam = GetComponent<Camera>();

        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Rect rect = cam.rect;

        if (scaleHeight < 1.0f)
        {
            // Add letterbox (top & bottom)
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            // Add pillarbox (sides)
            float scaleWidth = 1.0f / scaleHeight;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
        }

        cam.rect = rect;
    }
}
