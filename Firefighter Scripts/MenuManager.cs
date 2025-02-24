using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    #region Variables
    public static MenuManager instance = null;

    int giftRoundCost;

    [Header("Confirm Popup UI Settings")]
    public GameObject screenOverlay;
    public GameObject loadingScreenPopup;
    public GameObject confirmPopup;
    public GameObject errorPopup;
    public TextMeshProUGUI errorCodeNumberText;
    public TextMeshProUGUI errorMessageText;
    public GameObject yesButton;
    public GameObject buyButton;
    public TextMeshProUGUI totalScoinCounter;
    public TextMeshProUGUI roundCostText;
    #endregion

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

    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    } 
    
    public void NewGame()
    {
        SceneManager.LoadScene("FireFighter Board Blue");
    }

    public void PurchaseRound()
    {
        ScoinManager.instance.SubtractScoins(giftRoundCost);
        GameManager.instance.StartCountDown();
    }

    public void LaunchScoinStore()
    {
        Application.OpenURL("/TestSmartClub/purchase.do");
        //Application.OpenURL("/SmartClub/purchase.do");
    }

    public void OpenConfirmPopup()
    {
            if (ScoinManager.instance.GetTotalScoin() <= 0 || ScoinManager.instance.GetGameCost() <= 0)
            {
                OpenLoadingScreen();
            }
            else
            {
                CloseLoadingScreen();
            }

        LoadConfirmPopupDetails();
        confirmPopup.SetActive(true);
    }

    public void LoadConfirmPopupDetails()
    {
        int totalScoin = ScoinManager.instance.GetTotalScoin();
        totalScoinCounter.text = totalScoin.ToString();
        giftRoundCost = ScoinManager.instance.GetGameCost();
        roundCostText.text = giftRoundCost.ToString();

        if (giftRoundCost > totalScoin)
        {
            yesButton.SetActive(false);
            buyButton.SetActive(true);
        }
        else
        {
            yesButton.SetActive(true);
            buyButton.SetActive(false);
        }
    }

    public void OpenErrorPopup(int responseCode, string errorText)
    {
        screenOverlay.SetActive(true);
        errorPopup.SetActive(true);
        errorCodeNumberText.text = responseCode.ToString();
        errorMessageText.text = errorText;
    }

    public void OpenLoadingScreen()
    {
        if (loadingScreenPopup.activeSelf == true)
            return;

        screenOverlay.SetActive(true);
        loadingScreenPopup.SetActive(true);
    }

    public void CloseLoadingScreen()
    {
        if (loadingScreenPopup.activeSelf == false)
            return;

        screenOverlay.SetActive(false);
        loadingScreenPopup.SetActive(false);
    }

    public void QuitGame()
    {
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
            Application.Quit();
#elif (UNITY_ANDROID)
            Application.Quit();
#elif (UNITY_IOS)
            Application.Quit();
#elif (UNITY_WEBGL)
            Application.OpenURL("/TestSmartClub/GamesScoin.do");
#endif
    }
}
