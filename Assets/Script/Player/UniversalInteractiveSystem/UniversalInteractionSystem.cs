using UnityEngine;

public class UniversalInteractionSystem : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactionMask = ~0;

    [Header("Debug Visuals")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private Color rayColor = Color.red;

    [Header("Inventory")]
    [SerializeField] private InventorySystem inventory;

    [Header("Hands")]
    [SerializeField] private PlayerHands playerHands;

    [Header("UI")]
    [SerializeField] private GameObject mechanismPrompt;
    [SerializeField] private ThermometerUI thermometerUI;

    [Header("Outline")]
    [SerializeField] private bool enableOutlines = true;

    private PlayerInteractionContext context;

    private IUniversalInteractable currentTarget;
    private OutlineTarget currentOutline;

    private bool inputLocked;

    private void Awake()
    {
        context = new PlayerInteractionContext(
            this,
            inventory,
            playerHands
        );
    }

    private void Update()
    {
        DrawSceneRay();

        if (inputLocked)
            return;

        HandleInventoryScroll();
        UpdateTarget();
        HandleInput();
        UpdateThermometerUI();
    }

    // =========================================================
    // DEBUG RAY DRAWING
    // =========================================================

    private void DrawSceneRay()
    {
        if (!showDebugRay || playerCamera == null)
            return;

        Ray ray = CreateInteractionRay();
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, rayColor);
    }

    // =========================================================
    // TARGET DETECTION
    // =========================================================

    private void UpdateTarget()
    {
        IUniversalInteractable newTarget = FindTarget();

        if (newTarget == currentTarget)
            return;

        currentTarget = newTarget;

        UpdateOutline();
        UpdatePrompt();
    }

    private IUniversalInteractable FindTarget()
    {
        Ray ray = CreateInteractionRay();

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactDistance,
            interactionMask,
            QueryTriggerInteraction.Ignore))
        {
            return null;
        }

        Debug.DrawLine(ray.origin, hit.point, Color.green);

        return hit.collider.GetComponentInParent<IUniversalInteractable>();
    }

    // Helper to get full scene hierarchy path
    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }

    // =========================================================
    // INPUT
    // =========================================================

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Interact();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }

        if (Input.GetMouseButtonDown(1))
        {
            AlternateInteract();
        }
    }

    private void Interact()
    {
        if (currentTarget == null)
        {
            DropHeldItem();
            return;
        }

        currentTarget.Interact(context);
    }

    private void AlternateInteract()
    {
        if (currentTarget == null)
            return;

        currentTarget.AlternateInteract(context);
    }

    // =========================================================
    // INVENTORY
    // =========================================================

    private void HandleInventoryScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            inventory.Scroll(-1);
        }
        else if (scroll < 0f)
        {
            inventory.Scroll(1);
        }
    }

    // =========================================================
    // DROP
    // =========================================================

    public void DropHeldItem()
    {
        PickupItem selected = inventory.GetSelectedItem();

        if (selected == null)
            return;

        Ray ray = CreateInteractionRay();

        Vector3 position;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactionMask, QueryTriggerInteraction.Ignore))
        {
            // 1. Calculate a larger offset based on the item's actual collider size
            float itemRadius = 0.25f; // Safe default offset margin
            Collider itemCollider = selected.GetComponentInChildren<Collider>();
            if (itemCollider != null)
            {
                // Use half of the item's largest bounding box extent
                itemRadius = Mathf.Max(itemCollider.bounds.extents.x, itemCollider.bounds.extents.y, itemCollider.bounds.extents.z);
            }

            // 2. Push the item away from the surface by its radius + separation safety margin
            position = hit.point + (hit.normal * (itemRadius + 0.05f));

            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 2.0f);
            Debug.Log($"[Drop Point Raycast] Surface Hit: {hit.collider.name} | Normal: {hit.normal} | Position: {position}");
        }
        else
        {
            position = playerCamera.transform.position + playerCamera.transform.forward * (interactDistance * 0.75f);
            Debug.Log($"[Drop Point Raycast] No surface hit. Dropping in open air at {position}");
        }

        inventory.RemoveSelected();

        selected.OnDrop(position);

        PlayDropSound();
    }

    private void PlayDropSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Drop");
        }
    }

    // =========================================================
    // OUTLINE
    // =========================================================

    private void UpdateOutline()
    {
        if (!enableOutlines)
            return;

        OutlineTarget newOutline = null;

        if (currentTarget is Component component)
        {
            newOutline = component.GetComponentInParent<OutlineTarget>();
        }

        if (newOutline == currentOutline)
            return;

        if (currentOutline != null)
        {
            currentOutline.SetOutline(false);
        }

        currentOutline = newOutline;

        if (currentOutline != null)
        {
            currentOutline.SetOutline(true);
        }
    }

    // =========================================================
    // PROMPT
    // =========================================================

    private void UpdatePrompt()
    {
        if (mechanismPrompt == null)
            return;

        bool show =
            currentTarget != null &&
            currentTarget.ShowInteractionPrompt;

        mechanismPrompt.SetActive(show);
    }

    // =========================================================
    // PUBLIC API
    // =========================================================

    public Camera PlayerCamera => playerCamera;

    public float InteractionDistance => interactDistance;

    public void SetInputLocked(bool locked)
    {
        inputLocked = locked;

        if (locked)
        {
            currentTarget = null;

            if (currentOutline != null)
            {
                currentOutline.SetOutline(false);
                currentOutline = null;
            }

            if (mechanismPrompt != null)
                mechanismPrompt.SetActive(false);
        }
    }
    private void UpdateThermometerUI()
    {
        if (thermometerUI == null)
            return;

        if (currentTarget is Component component)
        {
            ThermometerInteractable thermometer =
                component.GetComponentInParent<ThermometerInteractable>();

            if (thermometer != null)
            {
                thermometerUI.ShowTemperature(
                    thermometer.TemperatureManager
                );

                return;
            }
        }

        thermometerUI.HideTemperature();
    }
    public Ray CreateInteractionRay()
    {
        return playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );
    }
}