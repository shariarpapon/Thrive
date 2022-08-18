using UnityEngine;

public class FPController : MonoBehaviour
{
    public static bool RestrictControls = false;

    public float speed = 7.0f;
    public float jumpHeight = 3;
    public float gravity = -9.81f;
    [Space]
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;

    [HideInInspector]
    public bool isGrounded;
    public bool canJump = true;

    private Vector3 velocity;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (RestrictControls == false)
        {
            MovementLogic();
            JumpLogic();
        }
            GravityLogic();
    }

    private void MovementLogic()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 dir = (transform.right * x) + (transform.forward * z);
        controller.Move(dir.normalized * speed * Time.deltaTime);
    }

    private void GravityLogic()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2.0f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void JumpLogic()
    {
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }

    public static bool Moving()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            return true;

        return false;
    }
}