using UnityEngine;

public class Blade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject bladeTrailPrefab;
    [SerializeField] private ComboManager comboManager;
    [SerializeField] private LayerMask targetLayer;

    [Header("Settings")]
    [SerializeField] private Vector3 raycastOffset = new Vector3(0f, 0f, -5f);

    private GameObject bladeTrailInstance;
    private bool isCutting;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        #if UNITY_ANDROID || UNITY_IOS
                HandleTouchInput();
        #else
                HandleMouseInput();
        #endif
    }

    // ---------------- MOUSE (PC/Web/Editor) ----------------
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCutting();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopCutting();
        }

        if (isCutting)
        {
            transform.position = GetWorldPosition(Input.mousePosition);
            if (HasBladeMoved())
                PerformRaycast();
        }
    }

    // ---------------- TOUCH (MOBILE) ----------------
    private void HandleTouchInput()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartCutting();
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                StopCutting();
                break;
        }

        if (isCutting)
        {
            transform.position = GetWorldPosition(touch.position);
            if (touch.phase == TouchPhase.Moved)
                PerformRaycast();
        }
    }

    private void StartCutting()
    {
        isCutting = true;
        transform.position = GetWorldPosition(Input.mousePosition);
        if (bladeTrailPrefab)
            bladeTrailInstance = Instantiate(bladeTrailPrefab, transform);
    }

    private void StopCutting()
    {
        isCutting = false;
        if (bladeTrailInstance)
            Destroy(bladeTrailInstance);
    }

    private Vector3 GetWorldPosition(Vector3 screenPos)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;

        screenPos.z = 10f; 
        return cam.ScreenToWorldPoint(screenPos);
    }

    private bool HasBladeMoved()
    {
        return Input.GetMouseButton(0) &&
               (Mathf.Abs(Input.GetAxis("Mouse X")) > 0f ||
                Mathf.Abs(Input.GetAxis("Mouse Y")) > 0f);
    }

    private void PerformRaycast()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + raycastOffset;

        if (Physics.Raycast(rayOrigin, Vector3.forward, out hit, 100f, targetLayer))
        {
            Target slicedTarget = hit.transform.GetComponent<Target>();
            if (slicedTarget != null)
            {
                comboManager.OnTargetHit(slicedTarget, hit.point);
                slicedTarget.OnTargetHit(transform.position);
            }
        }
    }
}
