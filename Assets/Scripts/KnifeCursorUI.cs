using UnityEngine;
using UnityEngine.UI;

public class KnifeCursorUI : MonoBehaviour
{
    [Header("Assign the RectTransform of the UI Image that represents the knife")]
    public RectTransform knifeRect; // drag KnifeUI's RectTransform here

    [Header("Canvas the knife lives on (optional)")]
    public Canvas canvas; // if left empty, will attempt to find parent canvas

    [Header("Optional offset in screen pixels")]
    public Vector2 offset = Vector2.zero;

    [Header("Smoothing (0 = instant)")]
    [Tooltip("0 for snap, >0 for lerp smoothing. Typical small values: 10-30")]
    public float smoothing = 0f;

    void Awake()
    {
        if (knifeRect == null)
        {
            Debug.LogError("KnifeCursorUI: knifeRect is not assigned.");
            enabled = false;
            return;
        }

        if (canvas == null)
            canvas = knifeRect.GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("KnifeCursorUI: no Canvas found for knifeRect.");
            enabled = false;
            return;
        }

        // Hide system cursor on platforms that show it
        #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        Cursor.visible = false;
        #endif
    }

    void OnDisable()
    {
        // restore system cursor if disabled
        #if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
        Cursor.visible = true;
        #endif
    }

    void Update()
    {
        Vector2 screenPos;
    
        #if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            screenPos = Input.GetTouch(0).position + offset;
        else
            return; // no touch
        #else
        screenPos = (Vector2)Input.mousePosition + offset;
        #endif
    
        Vector2 anchored;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out anchored);
    
        if (smoothing > 0f)
        {
            Vector2 cur = knifeRect.anchoredPosition;
            knifeRect.anchoredPosition = Vector2.Lerp(cur, anchored, 1f - Mathf.Exp(-smoothing * Time.deltaTime));
        }
        else
        {
            knifeRect.anchoredPosition = anchored;
        }
    }

}
