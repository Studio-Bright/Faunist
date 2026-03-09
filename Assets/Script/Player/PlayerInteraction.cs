using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera cam;
    public InventorySystem inventory;
    public PlayerMovementCC movement;
    public CameraContoller cameraController;

    [HideInInspector] public Vector3 originalCamPosition;
    [HideInInspector] public Quaternion originalCamRotation;

    private bool inPuzzleMode = false;
    private MechanismInteraction currentMechanism;

    private EightSidedPuzzle eightPuzzle;
    private PhysicalStatePuzzle statePuzzle;
    private DominoPuzzle dominoPuzzle;

    void Start()
    {
        eightPuzzle = FindFirstObjectByType<EightSidedPuzzle>();
        statePuzzle = FindFirstObjectByType<PhysicalStatePuzzle>();
        dominoPuzzle = FindFirstObjectByType<DominoPuzzle>();
    }

    void Update()
    {
        if (inPuzzleMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPuzzle();
            }

            HandleClick();   
            return;
        }

        HandleScroll();
        HandleInteraction();
        HandleClick();
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact(this);
                }
            }
        }
    }

    public void DisablePlayerControl()
    {
        originalCamPosition = cam.transform.position;
        originalCamRotation = cam.transform.rotation;

        movement.enabled = false;
        cameraController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnablePlayerControl()
    {
        movement.enabled = true;
        cameraController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        inPuzzleMode = false;
    }

    public void EnablePuzzleMode(MechanismInteraction mechanism)
    {
        inPuzzleMode = true;
        currentMechanism = mechanism;
        eightPuzzle.isInteractable = true;
        statePuzzle.isInteractable = true;
        dominoPuzzle.isInteractable = true;

    }

    void ExitPuzzle()
    {
        if (currentMechanism != null)
        {
            StartCoroutine(currentMechanism.ExitMechanism(this));
            eightPuzzle.isInteractable = false;
            statePuzzle.isInteractable = false;
            dominoPuzzle.isInteractable = false;

        }

    }
    void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        inventory.Scroll(scroll);
    }



    void HandleClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray;

            if (inPuzzleMode)
                ray = cam.ScreenPointToRay(Input.mousePosition);
            else
                ray = cam.ViewportPointToRay(Vector3.one * 0.5f);

            RaycastHit hit;

            float rayDistance = inPuzzleMode ? 100f : interactDistance;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (inPuzzleMode)
                {
                    RotatableItem rotatableItem = hit.collider.GetComponent<RotatableItem>();
                    if (rotatableItem != null)
                    {
                        rotatableItem.RotateSelf();
                        return;
                    }

                    PhysicalStatePuzzle statePuzzle = hit.collider.GetComponent<PhysicalStatePuzzle>();
                    if (statePuzzle != null)
                    {
                        statePuzzle.CheckPuzzle();
                        return;
                    }

                    DominoPuzzle dominoPuzzle = hit.collider.GetComponentInParent<DominoPuzzle>();
                    if (dominoPuzzle != null && dominoPuzzle.isInteractable)
                    {
                        dominoPuzzle.CheckPuzzle();
                        return;
                    }


                }
                else
                {
                    PickupItem pickupItem = hit.collider.GetComponent<PickupItem>();
                    if (pickupItem != null)
                    {
                        inventory.AddItem(pickupItem);
                        pickupItem.OnPickup();
                        return;
                    }

                    CauldronInventory cauldron = hit.collider.GetComponent<CauldronInventory>();
                    if (cauldron != null)
                    {
                        cauldron.Interact(this);
                        return;
                    }
                }
            }

            if (!inPuzzleMode)
                PlaceItem();
        }
    }


    void PlaceItem()
    {
        PickupItem selected = inventory.GetSelectedItem();
        if (selected == null) return;

        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;
        Vector3 dropPosition;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            dropPosition = hit.point + hit.normal * 0.05f;
        }
        else
        {
            dropPosition = cam.transform.position + cam.transform.forward * interactDistance;
        }

        selected.OnDrop(dropPosition);
        inventory.RemoveSelected();
    }
}
