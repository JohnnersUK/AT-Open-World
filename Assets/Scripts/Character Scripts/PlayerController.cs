using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Public
    [Header("Movment settings:")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float gravity = -12f;
    [SerializeField] private float baseGravity = -1f;
    [SerializeField] private float jumpHeight = 1f;
    [Range(0, 1)]
    [SerializeField] private float airControlPercent = 0.5f;

    [SerializeField] private float turnSmoothTime = 0.2f;
    [SerializeField] private float speedSmoothTime = 0.1f;

    // Private
    private float turnSmoothVelocity;
    private float speedSmoothVelocity;
    private float currentSpeed;
    private float yVel;

    private Animator anim;
    private Transform camT;
    private CharacterController cc;

    private bool dead = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        camT = Camera.main.transform;
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 rawInput;
        Vector2 inputDir;

        bool running;

        // Get inputs
        rawInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        inputDir = rawInput.normalized;
        running = Input.GetKey(KeyCode.LeftShift);

        // Move
        Move(inputDir, running);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Animaton controll
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
        anim.SetFloat("forwardVelocity", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        anim.SetFloat("sidewardVelocity", 0.0f);
    }

    // Default movment mode
    void Move(Vector2 inputDir, bool running)
    {
        float targetRotation;
        float targetSpeed;
        float localGravity = 0.0f;

        Vector3 velocity;

        // Rotation
        if (inputDir != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camT.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

        // Position
        targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        // Gravity fix
        if (cc.isGrounded)
        {
            localGravity = baseGravity;
        }
        else
        {
            localGravity = gravity;
        }

        yVel += Time.deltaTime * localGravity;
        velocity = transform.forward * currentSpeed + Vector3.up * yVel;

        // Apply the movment
        cc.Move(velocity * Time.deltaTime);

        currentSpeed = new Vector2(cc.velocity.x, cc.velocity.z).magnitude;

        if (cc.isGrounded)
        {
            anim.SetBool("Grounded", true);
        }
        else
        {
            anim.SetBool("Grounded", false);
        }

    }

    void Jump()
    {
        float jumpVelocity;

        if (cc.isGrounded)
        {
            anim.SetBool("Grounded", false);
            jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
            yVel = jumpVelocity;
        }
    }

    // Returns movment smoothtime adjusted for air travel
    float GetModifiedSmoothTime(float smoothTime)
    {
        if (cc.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "AI")
        {
            hit.gameObject.GetComponent<AiController>().Caught();
            Destroy(hit.gameObject);
        }
    }
}

