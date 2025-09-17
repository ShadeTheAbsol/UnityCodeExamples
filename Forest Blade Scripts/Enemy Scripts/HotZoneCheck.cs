using UnityEngine;

public class HotZoneCheck : MonoBehaviour
{
    private GroundPatrolEnemyAI enemyParent;
    private bool inRange;
    private Animator enemyAnimController;

    private void Awake()
    {
        enemyParent = GetComponentInParent<GroundPatrolEnemyAI>();
        enemyAnimController = GetComponentInParent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //If Player In HotZone Range And Enemy Not In Attack Animation, Flip Enemy To Face Player
        if (inRange && !enemyAnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            enemyParent.Flip();
        }
    }

    //Detects If Player In HotZone Range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    //Disable HotZone When Player Leaves HotZone Range And Reenable Enemy Detection Range Hitbox And Have Enemy Return To Patrol
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
            gameObject.SetActive(false);
            enemyParent.playerDetectionArea.SetActive(true);
            enemyParent.inRange = false;
            enemyParent.SelectTarget();
        }
    }
}
