using UnityEngine;

public class PlayerMovementCC : MonoBehaviour
{
    public float speed = 5f;
    public float jump = 2f;
    public float gravity = -9.81f;

    private CharacterController cc;
    private Vector3 velocity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = cc.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jump * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * speed + velocity;

        cc.Move(finalMove * Time.deltaTime);
    }
}
