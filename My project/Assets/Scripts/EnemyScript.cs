using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour
{
    public MB_GameManager gameManager;
    //Enemy States
     public enum EnemyState
    {
        Patrol,
        Seek,
        ReturnToPath
    }

    //Path Script Variables
    [SerializeField] float waitTimeOnWayPoint = 1f;
    [SerializeField] PathScript path;

    //Navmesh and Animator Variables
    NavMeshAgent agent;
    Animator animator;

    //Player
    Transform player;

    //Player Detection Variables
    [SerializeField] float detectionRange = 8f;
    [SerializeField] float loseRange = 12f;
    [SerializeField] float fieldOfView = 120f;
    [SerializeField] LayerMask obstacleMask;
    //Enemy Speed Variables
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float chaseSpeed = 4f;
  
    EnemyState currentState = EnemyState.Patrol;
    float time = 0f;
    Vector3 returnPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

     void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.updatePosition = false;
        agent.updateRotation = true;
        agent.stoppingDistance = 0.1f;
        agent.speed = patrolSpeed;

        agent.destination = path.GetCurrentWayPoint();
    }

   void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();

                if (CanSeePlayer())
                {
                    currentState = EnemyState.Seek;
                    agent.speed = chaseSpeed;
                }
                break;

            case EnemyState.Seek:
                HandleSeek();

                if (!CanSeePlayer() &&
                    Vector3.Distance(transform.position, player.position) > loseRange)
                {
                    returnPosition = path.GetCurrentWayPoint();
                    agent.destination = returnPosition;
                    currentState = EnemyState.ReturnToPath;
                    agent.speed = patrolSpeed;
                }
                break;

            case EnemyState.ReturnToPath:
                if (!agent.pathPending &&
                    agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = EnemyState.Patrol;
                    agent.destination = path.GetNextWayPoint();
                }
                break;
        }

        UpdateAnimator();
    }

void HandlePatrol()
    {
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            time += Time.deltaTime;

            if (time >= waitTimeOnWayPoint)
            {
                agent.destination = path.GetNextWayPoint();
                time = 0f;
            }
        }
    }

    void HandleSeek()
    {
        agent.destination = player.position;
    }

    //Detects if the enemy can see the player
    bool CanSeePlayer()
    {
        if (player == null) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
            return false;

        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView / 2f)
            return false;

        // Line of sight check
        if (Physics.Raycast(transform.position + Vector3.up,
                            directionToPlayer,
                            distanceToPlayer,
                            obstacleMask))
            return false;

        return true;
    }

    void UpdateAnimator()
    {
        Vector3 worldVelocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);
        Vector3 normalizedVelocity = localVelocity / agent.speed;

        animator.SetFloat("Horizontal", normalizedVelocity.x, 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", normalizedVelocity.z, 0.1f, Time.deltaTime);
    }

    void OnAnimatorMove()
    {
        Vector3 rootPosition = animator.rootPosition;
        rootPosition.y = agent.nextPosition.y;
        transform.position = rootPosition;

        agent.nextPosition = transform.position;
    }
}