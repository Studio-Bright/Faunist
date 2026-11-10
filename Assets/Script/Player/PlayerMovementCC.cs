using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public class PlayerMovementCC : MonoBehaviour
{

    public float normalSpeed = 5f;
    public float preBellSpeed = 2f;
    public float jump = 2f;
    public float gravity = -9.81f;
    public LightingStateManager LSM;
    public float currentSpeed;

    private CharacterController cc;
    private Vector3 velocity;

    public float ladderGrabDistance = 0.6f;
    public float ladderSpeed = 3f;

    private bool isOnLadder = false;
    private Ladder currentLadder;
    private Ladder ladderInFrontArea;
    private bool walkingSoundPlaying = false;
    public LevelLightmapData switcher;

    [Header("Knockback")]
    public float knockbackForce = 8f;
    public float knockbackDuration = 0.25f;
    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    void Awake()
    {
        LSM.SwitchToDay();
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();
        currentSpeed = normalSpeed;

         Debug.Log("Switcher = " + switcher.name);

        var scenariosField = typeof(LevelLightmapData)
            .GetField("lightingScenariosData",
                BindingFlags.NonPublic | BindingFlags.Instance);

        var list = scenariosField.GetValue(switcher) as System.Collections.IList;

        Debug.Log("Scenario count from PlayerMovement = " +
                  (list != null ? list.Count.ToString() : "NULL"));
    }

    void Update()
    {
        if (GameManager.IsPaused) return;

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

        // Normal movement
        Vector3 finalMove = move * currentSpeed + velocity;
        // WALKING AUDIO
        /* bool isMoving = move.magnitude > 0.1f && grounded;

         if (isMoving)
         {
             if (!walkingSoundPlaying)
             {
                 AudioManager.Instance.PlaySFX("Walking");
                 walkingSoundPlaying = true;
             }
         }
         else
         {
             if (walkingSoundPlaying)
             {
                 AudioManager.Instance.StopSFX("Walking");
                 walkingSoundPlaying = false;
             }
         }
        */
        // Add knockback
        if (knockbackTimer > 0f)
        {
            finalMove += knockbackVelocity;

            knockbackTimer -= Time.deltaTime;

            // Fade knockback over time
            knockbackVelocity = Vector3.Lerp(
                knockbackVelocity,
                Vector3.zero,
                10f * Time.deltaTime
            );
        }

        // ONE CharacterController.Move call
        cc.Move(finalMove * Time.deltaTime);

        TryDetectLadder();

        if (Input.GetKeyDown(KeyCode.V))
        {
            LSM.ToggleLighting();
        }
    }
   
    void TryDetectLadder()
    {
        if (Input.GetAxis("Vertical") <= 0.1f)
            return;

        if (ladderInFrontArea == null)
            return;

        Vector3 directionToLadder =
            ladderInFrontArea.transform.position - transform.position;

        directionToLadder.y = 0f;

        if (directionToLadder.sqrMagnitude < 0.001f)
            return;

        directionToLadder.Normalize();

        float facing = Vector3.Dot(
            transform.forward,
            directionToLadder
        );

        if (facing < 0.5f)
            return;

        EnterLadder(ladderInFrontArea);
    }
    public void SetLadderFrontArea(Ladder ladder, bool inside)
    {
        if (inside)
        {
            ladderInFrontArea = ladder;
        }
        else if (ladderInFrontArea == ladder)
        {
            ladderInFrontArea = null;
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
    public void ApplyKnockback(Vector3 explosionPosition)
    {
        Vector3 direction = transform.position - explosionPosition;

        // Only horizontal knockback
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        direction.Normalize();

        knockbackVelocity = direction * knockbackForce;
        knockbackTimer = knockbackDuration;

        Debug.Log(
            $"KNOCKBACK! Direction: {direction}, Force: {knockbackForce}"
        );
    }
}