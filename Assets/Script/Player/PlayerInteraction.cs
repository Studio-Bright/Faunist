using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public Camera cam;
    public InventorySystem inventory;
    public PlayerMovementCC movement;
    public CameraContoller cameraController;
    public GameObject cursorCanvas;
    [Header("UI")]
    public GameObject mechanismPrompt;

    [HideInInspector] public Vector3 originalCamPosition;
    [HideInInspector] public Quaternion originalCamRotation;

    private bool inPuzzleMode = false;
    public MechanismInteraction currentMechanism;

    private List<EightSidedPuzzle> eightPuzzles = new List<EightSidedPuzzle>();
    private PhysicalStatePuzzle statePuzzle;
    private DominoPuzzle dominoPuzzle;

    private OutlineTarget currentOutline;

    public Transform fireVFXPosition;
    public GameObject fireVFX;
    public void FireVFX(Vector3 position)
    {
        GameObject vfx = Instantiate(fireVFX, position, Quaternion.identity);

    }
    void Start()
    {
        eightPuzzles.AddRange(Object.FindObjectsByType<EightSidedPuzzle>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        statePuzzle = FindFirstObjectByType<PhysicalStatePuzzle>();
        dominoPuzzle = FindFirstObjectByType<DominoPuzzle>();
    }

    void Update()
    {
        HandleOutline();

        if (inPuzzleMode)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                ExitPuzzle();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                FireVFX(fireVFXPosition.position);
            }
            HandleClick();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseSelectedItem();
        }

        HandleMechanismPrompt();
        HandleScroll();
        HandleInteraction();
        HandleClick();


    }

    void HandleScroll()
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

    void HandleInteraction()
    {
        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!inPuzzleMode)
                {
                    if (interactable != null)
                    {
                        interactable.Interact(this);
                    }
                }
                else
                {
                    return;
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
        if (currentOutline != null)
        {
            currentOutline.SetOutline(false);
            currentOutline = null;
        }
        inPuzzleMode = true;
        cursorCanvas.SetActive(false);
        currentMechanism = mechanism;

        // Activate every single octagonal puzzle registered in the scene
        foreach (var puzzle in eightPuzzles)
        {
            if (puzzle != null)
                puzzle.isInteractable = true;
        }

        if (statePuzzle != null) statePuzzle.isInteractable = true;
    }

    void ExitPuzzle()
    {
        if (currentMechanism != null)
        {
            cursorCanvas.SetActive(true);
            StartCoroutine(currentMechanism.ExitMechanism(this));

            // FIX: Replaced the single 'eightPuzzle' with a foreach loop over 'eightPuzzles'
            foreach (var puzzle in eightPuzzles)
            {
                if (puzzle != null)
                    puzzle.isInteractable = false;
            }

            if (statePuzzle != null) statePuzzle.isInteractable = false;
            //dominoPuzzle.isInteractable = false;
        }
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

                    DominoPuzzle dominoPuzzle = hit.collider.GetComponentInParent<DominoPuzzle>();
                    if (dominoPuzzle != null && dominoPuzzle.isInteractable)
                    {
                        dominoPuzzle.CheckPuzzle();
                        return;
                    }
                    RotateHandle rotateHandle =
    hit.collider.GetComponent<RotateHandle>();

                    if (rotateHandle != null)
                    {
                        rotateHandle.Rotate();

                        if (statePuzzle != null)
                        {
                            statePuzzle.CheckPuzzle();
                        }

                        return;
                    }

                }
                else
                {
                    CauldronInventory cauldron = hit.collider.GetComponent<CauldronInventory>();
                    if (cauldron != null)
                    {
                        cauldron.Interact(this);
                        return;
                    }
                }
            }
        }
    }

    void HandleOutline()
    {
        if (inPuzzleMode)
            return;

        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;
        OutlineTarget newOutline = null;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            newOutline = hit.collider.GetComponentInParent<OutlineTarget>();
        }

        if (currentOutline != newOutline)
        {
            if (currentOutline != null)
                currentOutline.SetOutline(false);

            currentOutline = newOutline;

            if (currentOutline != null)
                currentOutline.SetOutline(true);
        }
    }

    void UseSelectedItem()
    {
        PickupItem item = inventory.GetSelectedItem();

        if (item == null)
            return;

        item.Use(this);
    }

    void HandleMechanismPrompt()
    {
        if (inPuzzleMode)
        {
            mechanismPrompt.SetActive(false);
            return;
        }

        Ray ray = cam.ViewportPointToRay(Vector3.one * 0.5f);
        RaycastHit hit;

        bool lookingAtMechanism = false;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            MechanismInteraction mechanism =
                hit.collider.GetComponentInParent<MechanismInteraction>();

            if (mechanism != null)
            {
                lookingAtMechanism = true;
            }
        }

        mechanismPrompt.SetActive(lookingAtMechanism);
    }
}