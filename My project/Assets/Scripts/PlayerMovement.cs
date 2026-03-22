using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    //Player Movement Variables
    public Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private float currentSpeed = 0f;
    private bool grounded;
    public float maxSpeed = 12f;
    public float acceleration = 20f;
    public float deceleration = 25f;
    public float rotationSpeed = 10f;
    public float jumpForce;
    private bool isMoving = false; 
    private bool hasJumped = false;
    public Transform spawnPoint;
    //Slope Variables
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    //Cinemachine Camera
    public Transform cameraTarget;

   //Ground Check
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    public float groundDamping = 5f;

    //Animation Variable
    public Animator animator;
    
      // Sound Effects
    public AudioSource jumpSource;
    public AudioSource runSource;
    public AudioClip jumpSound;
    public AudioClip runningSound;
    public AudioSource spearSource;
    public AudioClip spearSound;
    public AudioSource ringSource;
    public AudioClip ringSound;
    
    //Chaos Spear Variables
    public GameObject chaosSpearPrefab;
    public Transform spearSpawnPoint;
    public Camera mainCamera;

    //Data Stuff
    public MB_GameManager gameManager;
    public RingManager ringManager;
    public PlayerLivesSO playerLives;
    public int startingLives;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        gameManager.gameStatus.playerName = "Frankie";
        gameManager.gameStatus.currentLevel++;
       if (playerLives.currentLives <= 0)
        {
            playerLives.currentLives = startingLives;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Player inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Raycast to check if grounded
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        grounded = Physics.Raycast(rayOrigin, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        // Resets jump if Shadow is on the ground
        if (grounded)
        {
            hasJumped = false;
        }

        // Slows Shadow down when on the ground and not moving to reach a halt
        rb.linearDamping = grounded ? groundDamping : 0f;

        // Jump will only activate when grounded and space bar is pressed
        if (grounded && Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        //Chaos Spear can be activated while moving, will fire a projectile at where mouse is pointing at
        if (Input.GetMouseButtonDown(0)) 
            {
                FireChaosSpear();
            }

        if (gameManager != null)
            {
                gameManager.gameStatus.playerPosition = transform.position;
            }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
       MovePlayer();
    }

    private void MovePlayer()
    {
        //Camera set up
        Vector3 camForward = cameraTarget.forward;
        Vector3 camRight = cameraTarget.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDir = camForward * verticalInput + camRight * horizontalInput;
        float movementSpeed = Mathf.Clamp01(inputDir.magnitude);

        // acceleration / deceleration
        if (movementSpeed > 0.1f)
            currentSpeed += acceleration * Time.fixedDeltaTime;
        else
            currentSpeed -= deceleration * Time.fixedDeltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // Player Movement 
        Vector3 moveVelocity;
        //Slope Physics 
        if (OnSlope() && grounded)
        {
            Vector3 slopeDir = Vector3.ProjectOnPlane(inputDir, slopeHit.normal).normalized;
            moveVelocity = slopeDir * currentSpeed;
        }
        else
        {
            moveVelocity = inputDir.normalized * currentSpeed;
        }

        // apply horizontal velocity while preserving vertical
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

        // clamp horizontal speed
        float maxSlopeSpeed = maxSpeed * 1.5f;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > maxSlopeSpeed)
        {
            Vector3 clamped = flatVel.normalized * maxSlopeSpeed;
            rb.linearVelocity = new Vector3(clamped.x, rb.linearVelocity.y, clamped.z);
        }
             // Sound Effects for running
        isMoving = movementSpeed > 0.1f;

        if (isMoving && grounded)
            {
                if (!runSource.isPlaying)
                    runSource.PlayOneShot(runningSound);
            }
        else
            {
                if (runSource.isPlaying)
                    runSource.Stop();
            }
    }

    private void Jump()
    {
       if (!grounded || hasJumped)
            return;

        hasJumped = true;

        Vector3 newVelocity = rb.linearVelocity;
        newVelocity.y = jumpForce;
        rb.linearVelocity = newVelocity;

        animator.SetTrigger("Jump");
        if (jumpSource) jumpSource.PlayOneShot(jumpSound);
    }

    private void FireChaosSpear()
    {
     Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
     RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            {
             targetPoint = hit.point;
            }
        else
            {
             targetPoint = ray.GetPoint(100f); 
            }

            Vector3 direction = (targetPoint - spearSpawnPoint.position).normalized;

            GameObject spear = Instantiate(chaosSpearPrefab, spearSpawnPoint.position, Quaternion.LookRotation(direction));
            if (spearSource) spearSource.PlayOneShot(spearSound);
}   

    private void UpdateAnimator()
    {
        float speedPercent = grounded ? currentSpeed / maxSpeed : 0f;
        animator.SetFloat("Speed", Mathf.Lerp(animator.GetFloat("Speed"), speedPercent, Time.deltaTime * 5f));
        animator.SetBool("Grounded", grounded);
    }

    //Slope Check
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle > 0f;
        }
        return false;
    }

    //Player Death
    public void Dead()
{
    if (playerLives == null) return;

    playerLives.currentLives--;

    Debug.Log("Player Died! Lives left: " + playerLives.currentLives);

    if (playerLives.currentLives <= 0)
    {
        GameOver();
    }
    else
    {
        Respawn();
    }
}

    private void Respawn()
    {
        transform.position = spawnPoint.position;
        rb.linearVelocity = Vector3.zero;
    }

    private void GameOver()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Kill"))
    {
        Dead();
    }
}

    void OnTriggerEnter(Collider other)
{
    if (other.gameObject.CompareTag("Ring"))
    {
        ringSource.PlayOneShot(ringSound);
        ringManager.ringCount++;

        
        if (gameManager != null)
        {
            gameManager.gameStatus.ringsCollected = ringManager.ringCount;
            gameManager.SaveGameStatus();  
        }

        Destroy(other.gameObject);
    }
}


    
}
