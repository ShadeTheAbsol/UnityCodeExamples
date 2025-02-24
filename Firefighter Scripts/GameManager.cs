using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [Header("Popup Settings")]
    public GameObject screenOverlay;
    public GameObject countdownPopup;
    public GameObject scoinEarnedOverallPopup;
    public TextMeshProUGUI flamesHitCounter;
    public TextMeshProUGUI bombSelfDestructsCounter;
    public TextMeshProUGUI scoinEarnedOverallPopupAmount;

    [Header("UI Settings")]
    public TextMeshProUGUI scoinEarnedCounter;
    public TextMeshProUGUI scoinTotalCounter;

    [Header("Audio Settings")]
    public AudioMixer masterMixer;
    public AudioSource backgroundMusic;
    public AudioClip plinkSFX;
    public AudioClip fireSFX;
    public AudioClip bombSFX;

    AudioSource sfxPlayer;

    [Header("Game References")]
    public Sprite flameSprite;
    public Sprite smokeSprite;
    public Sprite bombSprite;
    public Sprite bombExplosionSprite;
    public Fire[] flames;
    public TextMeshProUGUI gameTimer;
    public TextMeshProUGUI countdownTimerText;

    [Header("Game Settings")]
    public int maxFlamesBurning;
    public float bombTimer;
    public float bombSelfDestructTimer;
    public float fireSpawnDelay;
    [Range(0, 9)] public int gameTimerMinutes;
    [Range(0, 59)] public int gameTimerSeconds;

    bool gameSettingsLoaded;
    bool runGameTimer;
    int gameTimerTotalSeconds;
    float timeRemaining;
    int countDownTimer;

    int currentflamesBurning;

    int flamesHit;
    int bombsHit;
    int bombSelfDestructs;
    int scoinEarned;
    bool wonScoinStatus;

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

        flamesHit = 0;
        scoinEarned = 0;
        scoinEarnedCounter.text = scoinEarned.ToString();
        countDownTimer = 4;
        gameTimerTotalSeconds = (gameTimerMinutes * 60) + gameTimerSeconds;
        DisplayGameTimer(gameTimerTotalSeconds);
        timeRemaining = gameTimerTotalSeconds;
        runGameTimer = false;
        gameSettingsLoaded = false;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        scoinTotalCounter.text = ScoinManager.instance.GetTotalScoin().ToString();

        if (gameSettingsLoaded)
        {
            gameSettingsLoaded = false;
            MenuManager.instance.OpenConfirmPopup();
        }

        if (runGameTimer)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                if (timeRemaining <= countDownTimer && countDownTimer > 0)
                {
                    countDownTimer -= 1;
                }

                DisplayGameTimer(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                runGameTimer = false;
                CancelInvoke("CheckFlames");
                HideAllFlames();
                EndRound();
            }
        }
    }

    public void StartCountDown()
    {
        StartCoroutine("CountDown");
    }

    IEnumerator CountDown()
    {
        countdownPopup.SetActive(true);
        screenOverlay.SetActive(true);
        int countdownValue = 3;

        for (int i = 0; i < 3; i++)
        {
            PlayCountDownPlinkSound();
            countdownTimerText.text = countdownValue.ToString();
            yield return new WaitForSeconds(1);
            countdownValue--;
        }

        countdownPopup.SetActive(false);
        screenOverlay.SetActive(false);
        StartGameRound();
    }

    void StartGameRound()
    {
        StartGameTimer();
        backgroundMusic.Play();
        InvokeRepeating("CheckFlames", 0.2f, 0.2f);
    }

    void DisplayGameTimer(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        if (timeToDisplay <= 0)
        {
            minutes = 0;
            seconds = 0;
        }

        gameTimer.text = string.Format("{0:0}:{1:00}", minutes,seconds);
    }

    void CheckFlames()
    {
            int flameIndex = Random.Range(0, flames.Length);
            //Debug.Log("Index: " + flameIndex);

            if (flames[flameIndex].CheckFireBurning() == false)
            {
                flames[flameIndex].ShowFire();
            }
    }

    void HideAllFlames()
    {
        for (int i = 0; i < flames.Length; i++)
        {
            flames[i].HideFlameInstantly();
        }
    }

    public void StartGameTimer()
    {
        runGameTimer = true;
    }

    public void IncreaseFiresBurning()
    {
        if(currentflamesBurning < maxFlamesBurning)
        currentflamesBurning++;
    }

    public void DecreaseActiveFlames()
    {
        if(currentflamesBurning > 0)
        currentflamesBurning--;
    }

    public void EndRound()
    {
        DOTween.Clear();
        screenOverlay.SetActive(true);

        if (flamesHit > 0)
            wonScoinStatus = true;

        flamesHitCounter.text = flamesHit.ToString() + " HITS";
        bombSelfDestructsCounter.text = bombSelfDestructs.ToString() + " MISSED";

        if (wonScoinStatus)
        {
            scoinEarned = GetFinalScore() * ScoinManager.instance.GetPrize(0);
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

    public void IncreaseFlamesHit()
    {
            flamesHit++;
    }

    public string GetFlamesHitAsString()
    {
        string flamesHitString = string.Join(",", flamesHit);

        return flamesHitString;
    }

    public void IncreaseBombsHit()
    {
        bombsHit++;
    }

    public void IncreaseBombSelfDestructs()
    {
        bombSelfDestructs++;
    }

    public int GetFinalScore()
    {
        int finalScore = flamesHit;
        int bombSelfDestructsPenaltyScore = bombSelfDestructs *= 2;
        finalScore -= bombSelfDestructsPenaltyScore;

        if (finalScore <= 0)
            finalScore = 0;

        return finalScore;
    }

    public string GetWonScoinStatusAsString()
    {
        return wonScoinStatus.ToString();
    }

    public bool CheckFireBurningLimitReached()
    {
        if (currentflamesBurning >= maxFlamesBurning)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetGameSettingsLoaded(bool settingsLoaded)
    {
        gameSettingsLoaded = settingsLoaded;
    }

    public bool CheckGameSettingsLoaded()
    {
        return gameSettingsLoaded;
    }

    public void PlayCountDownPlinkSound()
    {
        masterMixer.SetFloat("SFXVolume", 10);
        sfxPlayer.PlayOneShot(plinkSFX);
    }

    public void PlayFireExtinguishedSound()
    {
        masterMixer.SetFloat("SFXVolume", 20);
        sfxPlayer.PlayOneShot(fireSFX);
    }

    public void PlayBombHitSound()
    {
        masterMixer.SetFloat("SFXVolume", 20);
        sfxPlayer.PlayOneShot(bombSFX);
    }
}
