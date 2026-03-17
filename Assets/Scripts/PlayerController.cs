using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float playerSpeed = 0.5f;
    public float walkSpeed = 0.5f;
    public float runSpeed = 1.0f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;
    
    private Vector3 movement;
    private Vector3 velocity;
    private CharacterController controller;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = gameObject.GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        playerSpeed = walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //check if grounded
        bool grounded = controller.isGrounded;

        //get input
        Vector3 cameraRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
        Vector3 cameraForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        movement = Input.GetAxis("Horizontal") * cameraRight + Input.GetAxis("Vertical") * cameraForward;

        //check if jumping and add gravity to player velocity vector
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            velocity.y = jumpForce * -gravity;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        //set jump/falling animation if needed
        if (grounded)
        {
            animator.SetFloat("Vertical", 0);
        }
        else
        {
            animator.SetFloat("Vertical", velocity.y);
        }

        //Attack
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            animator.SetBool("Attacking", true);
        }
        else
        {
            animator.SetBool("Attacking", false);
        }

        //Dead
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            animator.SetBool("Dead", true);
            controller.height = 0.5f;
        }
        // Undo Dead shortcut for demo
        if (Input.GetKeyDown(KeyCode.RightAlt))
        {
            animator.SetBool("Dead", false);
            controller.height = 1.7f;
        }

        //rotate character in movement direction if input detected
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movement);
            animator.SetFloat("Forward", Mathf.Max(Mathf.Abs(Input.GetAxis("Vertical")), Mathf.Abs(Input.GetAxis("Horizontal"))));
            animator.SetBool("Walking", true);
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                animator.SetBool("Running", true);
                playerSpeed = runSpeed;
            }
            else
            {
                animator.SetBool("Running", false);
                playerSpeed = walkSpeed;
            }
        }
        else
        {
            animator.SetBool("Walking", false);
            animator.SetBool("Running", false);
            playerSpeed = walkSpeed;
        }
        controller.Move(new Vector3(movement.x * playerSpeed, velocity.y, movement.z * playerSpeed) * Time.deltaTime);
    }
}
