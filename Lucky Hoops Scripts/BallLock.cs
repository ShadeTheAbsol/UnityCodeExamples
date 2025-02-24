using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallLock : MonoBehaviour
{
    Transform lockedInBall;

    Image hoopBack;
    Image hoopFront;
    Color hoopBackColour;
    Color hoopFrontColour;

    int hoopPrizeAmount;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.tag == "Hoop")
        {
            hoopBack = transform.parent.GetComponent<Image>();
            hoopFront = transform.parent.GetComponent<HoopMover>().hoopFrontNet.GetComponent<Image>();
            hoopBackColour = hoopBack.color;
            hoopFrontColour = hoopFront.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.tag == "Hoop")
        {
            SetLockedBall();
        }
        else if (transform.tag == "Floor")
        {
            Invoke("SetLostBall", 2f);
        }
    }

    void SetLockedBall()
    {
        GameManager.instance.SetBallScoreState(true);
        GameManager.instance.ball.gameObject.SetActive(false);
        lockedInBall = Instantiate(GameManager.instance.lockedBall, GameManager.instance.ball.position, Quaternion.identity, transform);
        lockedInBall.localPosition = new Vector3(1f, -4f, 0f);
        lockedInBall.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        lockedInBall.SetParent(transform.parent);
        SetLockedNet();
        GameManager.instance.SetScoinEarnedSoFar(hoopPrizeAmount);
        GameManager.instance.OpenGoalPopup();
    }

    void SetLostBall()
    {
        GameManager.instance.SetBallScoreState(false);
        GameManager.instance.ball.gameObject.SetActive(false);
        GameManager.instance.OpenGoalPopup();
    }

    void SetLockedNet()
    {
        hoopBackColour.a = 0.5f;
        hoopFrontColour.a = 0.5f;
        hoopBack.color = hoopBackColour;
        hoopFront.color = hoopFrontColour;
        gameObject.SetActive(false);
    }

    public void SetupHoopPrize(int prizeValue)
    {
        hoopPrizeAmount = prizeValue;
    }
}
