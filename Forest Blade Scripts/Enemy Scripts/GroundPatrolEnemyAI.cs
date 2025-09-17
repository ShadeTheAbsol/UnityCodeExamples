using UnityEngine;

[RequireComponent(typeof(Animator))]

public class GroundPatrolEnemyAI : MonoBehaviour
{

    #region EnemyStats
    [SerializeField]
    private float attackAggroDistance;
    [SerializeField]
    private float enemyMoveSpeed;
    [SerializeField]
    private float AttackCooldownTimerInitialValue;
    public float patrolDistance;
    public bool showPatrolPoints;
    #endregion

    #region Private And Semi Private Variables
    private Animator enemyAnimController;
    private float distanceToPlayer;
    private bool attackMode;

    private bool cooling;
    private bool playerDead;
    private float attackCooldownTimer;
    private Vector2 leftPatrolPosition;
    private Vector2 rightPatrolPosition;
    private Vector2 targetPos;
    #endregion

    #region Public Variables
    [HideInInspector] public Transform target;
    [HideInInspector] public bool inRange;
    public GameObject hotZone;
    public GameObject playerDetectionArea;
    #endregion

    private void OnEnable()
    {
        PlayerHealth.PlayerDeath += GameOver;
    }

    private void OnDisable()
    {
        PlayerHealth.PlayerDeath -= GameOver;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CalculatePatrolZone();
        SelectTarget();
        attackCooldownTimer = AttackCooldownTimerInitialValue;
        enemyAnimController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //If Not Attacking, Make Enemy Move
        if (!attackMode)
        {
            Move();
        }

        //If Enemy Outside Patrol Points And No Player In Range And Enemy Not In Attack Animation, Return To Patrol
        if (!InsideOfLimits() && !inRange && !enemyAnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            target = null;
            SelectTarget();
        }

        //If Player In Range, Chase Them And Attack
        if (inRange && target != null)
        {
            EnemyAttackPattern();
        }
    }

    private void EnemyAttackPattern()
    {
        distanceToPlayer = Vector2.Distance(transform.position, target.position);

        //If Player Outside Attack Aggro Range, Stop Attacking, Else If Player In Attack Aggro Range And Enemy Not In Cooldown Then Attack
        if (distanceToPlayer > attackAggroDistance)
        {
            StopAttack();
        }
        else if (attackAggroDistance >= distanceToPlayer && cooling == false)
        {
            Attack();
        }

        //If Enemy In Cooldown, Stop Attack Animation And Start Cooldown Timer
        if (cooling)
        {
            CoolDown();
            enemyAnimController.SetBool("Attack", false);
        }
    }

    private void Move()
    {
        enemyAnimController.SetBool("Walking", true);

        //If Enemy Is Not Currently In Attack Animation, Let Them Move Towards Player
        if (!enemyAnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(target != null)
            targetPos = target.position;

            targetPos.y = transform.position.y;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyMoveSpeed * Time.deltaTime);
        }
    }

    //Initiate Enemy Attack
    private void Attack()
    {
        attackCooldownTimer = AttackCooldownTimerInitialValue;
        attackMode = true;

        enemyAnimController.SetBool("Walking", false);
        enemyAnimController.SetBool("Attack", true);
    }

    //Stop Attacking And End Cooldown
    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        enemyAnimController.SetBool("Attack", false);
    }

    //Enemy Cooldown To Prevent Rapid Attacks
    private void CoolDown()
    {
        attackCooldownTimer -= Time.deltaTime;

        if (attackCooldownTimer <= 0 && cooling && attackMode)
        {
            cooling = false;
            attackCooldownTimer = AttackCooldownTimerInitialValue;
        }
    }

    //Cooldown Initiated By Animation Event At End Of Attack Animation
    private void TriggerCoolDown()
    {
        cooling = true;
    }

    //Called When Player Is Dead To End Attack Immediately And Return To Patrol
    private void GameOver()
    {
        playerDead = true;
        StopAttack();
        target = null;
        SelectTarget();
    }

    //Used For Debugging Purposes To Visually Show Patrol Points Based On Patrol Distance => Check ShowPatrolPoints On In Editor To See Visual Debugging
    private void OnDrawGizmos()
    {

        if (showPatrolPoints)
        {
            CalculatePatrolZone();
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(leftPatrolPosition.x, leftPatrolPosition.y, 0), Vector3.one * 0.4f);
            Gizmos.DrawCube(new Vector3(rightPatrolPosition.x, rightPatrolPosition.y, 0), Vector3.one * 0.4f);
        }
    }

    //Calculates Patrol Positions Based On Patrol Distance And Current Enemy Position At Start
    private void CalculatePatrolZone()
    {
        if (leftPatrolPosition != Vector2.zero && rightPatrolPosition != Vector2.zero)
            return;

        leftPatrolPosition = transform.position;
        leftPatrolPosition.x -= patrolDistance;

        rightPatrolPosition = transform.position;
        rightPatrolPosition.x += patrolDistance;
    }

    //Determines If Enemy Is Within Patrol Points
    private bool InsideOfLimits()
    {
        return transform.position.x > leftPatrolPosition.x && transform.position.x < rightPatrolPosition.x;
    }

    //Sets Patrol Point Target To Follow When Player Not Detected
    public void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, leftPatrolPosition);
        float distanceToRight = Vector2.Distance(transform.position, rightPatrolPosition);

        targetPos = distanceToLeft > distanceToRight ? leftPatrolPosition : rightPatrolPosition;
        Flip();
    }

    //Flips Enemy To 180 Degrees Or 0 Degrees On Y Axis To Face Patrol Point Or Player Based On Current Actions
    public void Flip()
    {
        Vector2 rotation = transform.eulerAngles;

        if (transform.position.x > targetPos.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }

        transform.eulerAngles = rotation;
    }

    //Enemy Is Killed By Player Weapon Upon Contact => No HP Logic Setup
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Weapon"))
        {
            gameObject.SetActive(false);
        }
    }
}
