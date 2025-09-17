using UnityEngine;

public class PlayerDetectionZoneCheck : MonoBehaviour
{
    private GroundPatrolEnemyAI enemyParent;

    private void Awake()
    {
        enemyParent = GetComponentInParent<GroundPatrolEnemyAI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //If Player In Range, Disable DetectionZone, Activate HotZone And Tell Enemy Logic Script New Target Is Player And They Are In Range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            enemyParent.target = collision.transform;
            enemyParent.inRange = true;
            enemyParent.hotZone.SetActive(true);
        }
    }
}
