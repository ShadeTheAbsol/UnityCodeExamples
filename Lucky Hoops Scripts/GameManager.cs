using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Ball Crosshair Settings")]
    public Transform ballCrosshair;
    public float ballCrosshairSpeed;

    [Header("Ball Settings")]
    public Transform basketBallNetsContainer;
    public Transform ball;
    public Transform lockedBall;
    public Transform ballUpArrowIndicator;
    public float ballDragSpeed;
    public float ballThrowSpeed;
    Transform ballStartParent;
    Vector3 ballStartPos;
    Vector3 ballStartSize;
    Quaternion ballStartRot;
    Rigidbody2D ballRB;
    bool ballScoreState;

    [Header("Basketball Hoop Settings")]
    public List<Transform> hoops;
    List<BallLock> hoopLocks;
    List<TextMeshProUGUI> hoopPrizeTexts;

    [Header("Popup Settings")]
    public GameObject screenOverlay;
    public GameObject goalStatusPopup;
    public TextMeshProUGUI goalStatusText;
    public GameObject scoinEarnedOverallPopup;
    public TextMeshProUGUI goalsScoredCounter;
    public TextMeshProUGUI scoinEarnedOverallPopupAmount;

    [Header("UI Settings")]
    public GameObject[] ballsLeft;
    public TextMeshProUGUI scoinEarnedCounter;
    public TextMeshProUGUI scoinTotalCounter;
    public Button throwButton;

    [Header("Audio Settings")]
    public AudioClip scoreSFX;
    public AudioClip missSFX;

    AudioSource sfxPlayer;

    int goalsScored;
    int ballsRemaining;
    int scoinEarned;
    int scoinEarnedSoFar;
    bool wonScoinStatus;
    int ballsRemainingIndex;
    Vector2 ballCrosshairStartPos;

    #region Awake And Start Logic
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        sfxPlayer = GetComponent<AudioSource>();
        ballStartParent = ball.parent;
        ballStartPos = ball.localPosition;
        ballStartSize = ball.localScale;
        ballStartRot = ball.localRotation;
        ballCrosshairStartPos = ballCrosshair.localPosition;
        ballRB = ball.GetComponent<Rigidbody2D>();

        ballsRemaining = 5;
        ballsRemainingIndex = 4;
        goalsScored = 0;
        scoinEarned = 0;
        scoinEarnedSoFar = 0;
        scoinEarnedCounter.text = scoinEarned.ToString();

        hoopPrizeTexts = new List<TextMeshProUGUI>();
        hoopLocks = new List<BallLock>();

        for (int i = 0; i < hoops.Count; i++)
        {
            hoopPrizeTexts.Add(hoops[i].GetComponentInChildren<TextMeshProUGUI>());
            hoopLocks.Add(hoops[i].GetComponentInChildren<BallLock>());
        }

        StartCoroutine(SetupHoopPrizes());
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        scoinTotalCounter.text = ScoinManager.instance.GetTotalScoin().ToString();
    }

    public void FireBall()
    {
        throwButton.interactable = false;
        ballsRemaining--;
        ReduceBallCounter();
        ball.DOScale(0.97f, ballThrowSpeed);
        ball.DOLocalMove(ballCrosshair.localPosition, ballThrowSpeed).OnComplete(DropBall);
    }

    void DropBall()
    {
        ball.GetComponent<CircleCollider2D>().enabled = true;
        ballRB.bodyType = RigidbodyType2D.Dynamic;
        ball.SetParent(basketBallNetsContainer);
        ball.SetSiblingIndex(1);
    }

    public void RestartRound()
    {
        ballScoreState = false;
        throwButton.interactable = true;
        screenOverlay.SetActive(false);
        goalStatusPopup.SetActive(false);
        ResetBall();
    }

    public void ResetBall()
    {
        ball.gameObject.SetActive(true);
        ball.GetComponent<CircleCollider2D>().enabled = false;
        ballRB.bodyType = RigidbodyType2D.Kinematic;
        ballRB.velocity = Vector3.zero;
        ball.SetParent(ballStartParent);
        ball.localPosition = ballStartPos;
        ball.localScale = ballStartSize;
        ball.localRotation = ballStartRot;
        ballCrosshair.localPosition = ballCrosshairStartPos;
    }

    public void OpenGoalPopup()
    {
        screenOverlay.SetActive(true);

        if (ballsRemaining >= 0)
        {
            goalStatusPopup.SetActive(true);

            if (ballScoreState)
            {
                sfxPlayer.clip = scoreSFX;

                if (sfxPlayer.isPlaying == false)
                    sfxPlayer.Play();

                goalStatusText.text = "*HOOP*" + "\n" + "THERE IT IS";
                goalsScored++;
                scoinEarned = scoinEarnedSoFar;
                scoinEarnedCounter.text = scoinEarned.ToString();
            }
            else
            {
                sfxPlayer.clip = missSFX;

                if(sfxPlayer.isPlaying == false)
                    sfxPlayer.Play();

                goalStatusText.text = "MISSED!";
            }

            if (ballsRemaining > 0)
            {
                Invoke("RestartRound", 2f);
            }
            else
            {
                Invoke("EndRound", 2f);
            }
        }
    }

    public void EndRound()
    {
        DOTween.Clear();
        screenOverlay.SetActive(true);
        goalStatusPopup.SetActive(false);
        ResetBall();
        ballScoreState = false;

        if (goalsScored > 0)
            wonScoinStatus = true;

        goalsScoredCounter.text = "HOOPS " + goalsScored.ToString() + "/5";

        if (wonScoinStatus)
        {
            scoinEarned = scoinEarnedSoFar;
            scoinEarnedCounter.text = scoinEarned.ToString();
            scoinEarnedOverallPopup.SetActive(true);
            scoinEarnedOverallPopupAmount.text = scoinEarned.ToString();
            ScoinManager.instance.AddScoins(scoinEarned);
        }
        else
        {
            scoinEarned = 0;
            scoinEarnedCounter.text = scoinEarned.ToString();
            scoinEarnedOverallPopup.SetActive(true);
            scoinEarnedOverallPopupAmount.text = scoinEarned.ToString();
            ScoinManager.instance.AddScoins(scoinEarned);
        }
    }

    void ReduceBallCounter()
    {
        ballsLeft[ballsRemainingIndex].SetActive(false);
        ballsRemainingIndex--;
    }

    public string GetGoalsEarnedAsString()
    {
        string goalsEarnedString = string.Join(",", goalsScored);

        return goalsEarnedString;
    }

    public string GetWonScoinStatusAsString()
    {
        return wonScoinStatus.ToString();
    }

    IEnumerator SetupHoopPrizes()
    {
        MenuManager.instance.OpenLoadingScreen();

        while (ScoinManager.instance.GetScoinPrizeListLength() == 0)
            yield return null;

        MenuManager.instance.CloseLoadingScreen();

        for (int i = 0; i < hoops.Count; i++)
        {
            hoopPrizeTexts[i].text = ScoinManager.instance.GetPrize(i).ToString();
            hoopLocks[i].SetupHoopPrize(ScoinManager.instance.GetPrize(i));
        }
    }

    public void SetBallScoreState(bool state)
    {
        ballScoreState = state;
    }

    public void SetScoinEarnedSoFar(int scoinAmount)
    {
        scoinEarnedSoFar += scoinAmount;
    }
}
