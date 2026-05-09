using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{

    public float interactDistance = 3f;
    public Camera cam;
    public InventorySystem inventory;
    public PlayerMovementCC movement;
    public CameraContoller cameraController;
    public GameObject cursorCanvas;
    

    [HideInInspector] public Vector3 originalCamPosition;
    [HideInInspector] public Quaternion originalCamRotation;

    private bool inPuzzleMode = false;
    public MechanismInteraction currentMechanism;

    private EightSidedPuzzle eightPuzzle;
    private PhysicalStatePuzzle statePuzzle;
    private DominoPuzzle dominoPuzzle;

    private OutlineTarget currentOutline;

    void Start()
    {
        eightPuzzle = FindFirstObjectByType<EightSidedPuzzle>();
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

            HandleClick();   
            return;
        }

        HandleScroll();
        HandleInteraction();
        HandleClick();
    }

    void HandleScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0f)
        {
            inventory.Scroll(-1); // вверх
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
        eightPuzzle.isInteractable = true;
        statePuzzle.isInteractable = true;
        //dominoPuzzle.isInteractable = true;

    }

    void ExitPuzzle()
    {
        if (currentMechanism != null)
        {
            cursorCanvas.SetActive(true);
            StartCoroutine(currentMechanism.ExitMechanism(this));
            eightPuzzle.isInteractable = false;
            statePuzzle.isInteractable = false;
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
            newOutline =
                hit.collider.GetComponentInParent<OutlineTarget>();
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

}
