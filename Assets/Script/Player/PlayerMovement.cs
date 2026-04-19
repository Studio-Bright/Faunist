using UnityEngine;

public class PlayerMovementCC : MonoBehaviour
{
    public float normalSpeed = 5f;
    public float preBellSpeed = 2f;
    public float jump = 2f;
    public float gravity = -9.81f;

    public float currentSpeed;

    private CharacterController cc;
    private Vector3 velocity;

    public float ladderGrabDistance = 0.6f;
    public float ladderSpeed = 3f;

    private bool isOnLadder = false;
    private Ladder currentLadder;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (isOnLadder)
        {
            ClimbLadder();
            return;
        }

        bool grounded = cc.isGrounded;

        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * currentSpeed + velocity;

        cc.Move(finalMove * Time.deltaTime);

        TryDetectLadder();
    }

    void TryDetectLadder()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 inputDir = (transform.right * x + transform.forward * z).normalized;

        if (inputDir == Vector3.zero)
            inputDir = transform.forward;

        if (Physics.Raycast(transform.position, inputDir, out RaycastHit hit, ladderGrabDistance))
        {
            if (hit.transform.TryGetComponent(out Ladder ladder))
            {
                if (Input.GetAxis("Vertical") > 0.1f)
                {
                    EnterLadder(ladder);
                }
            }
        }
    }

    void EnterLadder(Ladder ladder)
    {
        isOnLadder = true;
        currentLadder = ladder;

        velocity = Vector3.zero;
    }
    void ClimbLadder()
    {
        float v = Input.GetAxis("Vertical");

        Vector3 nextPos = transform.position + Vector3.up * v * ladderSpeed * Time.deltaTime;

        // clamp between bottom and top
        float bottomY = currentLadder.bottomPoint.position.y;
        float topY = currentLadder.topPoint.position.y;

        nextPos.y = Mathf.Clamp(nextPos.y, bottomY, topY);

        if (nextPos.y >= topY - 0.05f && v > 0)
        {
            ExitLadder();
            return;
        }

        if (nextPos.y <= bottomY + 0.05f && v < 0)
        {
            ExitLadder();
            return;
        }
    

        cc.Move((nextPos - transform.position));
    }

    void ExitLadder()
    {
        isOnLadder = false;
        currentLadder = null;
    }

    public void SetPreBellState()
    {
        currentSpeed = preBellSpeed;
    }

    public void SetNormalState()
    {
        currentSpeed = normalSpeed;
    }
}