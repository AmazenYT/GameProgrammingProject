using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Animator animator;
    float horizontalInput;
    float verticalInput;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float currentSpeed = 0f;
    public float maxSpeed = 12f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float jumpForce = 2f;
    public Vector3 jump;
    public bool isGrounded;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        jump = new Vector3(0f,2f,0f);
    }

    // Update is called once per frame
    void Update()
    {
       
        horizontalInput = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Vector3 inputDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        float movementSpeed = Mathf.Clamp01(inputDirection.magnitude);
        inputDirection.Normalize();
        Vector3 velocity= inputDirection * currentSpeed;
        rb.linearVelocity = velocity;

        if (movementSpeed > 0.1f)
        {
           currentSpeed += acceleration * Time.fixedDeltaTime; 
        }
        else
        {
            currentSpeed -= deceleration * Time.fixedDeltaTime;
        }
         currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }

    private void Jump()
    {
        rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        animator.SetTrigger("Jump");
        
    }

    private void UpdateAnimator()
    {
        float speedPercent = currentSpeed / maxSpeed;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), speedPercent, Time.deltaTime * 5f));
        animator.SetBool("Grounded", isGrounded);
    }

    private void OnCollisionStay()
    {
        {
            isGrounded = true;
        }
            
    }

    private void OnCollisionExit()
    {
        isGrounded = false;
    }
}
