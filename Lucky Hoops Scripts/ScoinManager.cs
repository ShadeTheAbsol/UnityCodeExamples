using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;
using YourApp.Security.Cryptography;

public class ScoinManager : MonoBehaviour
{
    #region Variables
    public static ScoinManager instance = null;

    string scoinResponse;
    string prizesResponse;
    string costResponse;
    ScoinResults scoinResults;
    PrizeResults prizeResults;
    GameCostResults gameCostResults;
    int totalScoin;
    List<string> scoinPrizeList;

    //Encryption Settings
    RijndaelCrypt encryptMaster;
    string startGameResponse;
    SessionIDResponseBody sessionIDResults;
    long unixTime;

    //[Header("Game Data Settings")]
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
        StartCoroutine(SetupBoardServices());
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SetupBoardServices()
    {
        encryptMaster = new RijndaelCrypt("CHAINSAWKING");
        SetupStartData();

        MenuManager.instance.OpenLoadingScreen();
        scoinPrizeList = new List<string>();

        while (sessionIDResults == null || sessionIDResults.sessionId == "" || sessionIDResults.sessionId == null)
            yield return null;

        StartCoroutine(RetrieveTotalScoinOnly());

        while (totalScoin <= 0)
            yield return null;

        SetupGameCost();

        while (GetGameCost() <= 0)
            yield return null;

        SetupPrizes();

        while (scoinPrizeList.Count == 0)
            yield return null;

            MenuManager.instance.CloseLoadingScreen();
    }

    public void SetupStartData()
    {
        StartCoroutine(PassStartingData());
    }

    public class SessionIDResponseBody
    {
        public int responseCode;
        public string key;
        public string sessionId;
        public int gameId;
    }

    IEnumerator PassStartingData()
    {
        unixTime = new System.DateTimeOffset(System.DateTime.Now).ToUnixTimeSeconds();
        string dataToEncrypt = "gameId=13&sendTime=" + unixTime;
        string urlEncryptedData = encryptMaster.Encrypt(dataToEncrypt);
        string url = "/TestSmartClub/GameApiAction.do?action=startGame&encryptData=" + UnityWebRequest.EscapeURL(urlEncryptedData);
        //string url = "/SmartClub/GameApiAction.do?action=startGame&encryptData=" + UnityWebRequest.EscapeURL(urlEncryptedData);

        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
        www.SetRequestHeader("Accept", "application/json");

        www.downloadHandler = new DownloadHandlerBuffer();
        www.timeout = 30;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            if (www.responseCode == 403)
            {
                Debug.Log("USER NOT LOGGED IN");
                MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and submit scores.");
            }
            else
            {
                Debug.Log(www.error);
                Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
            }
        }
        else
        {
            Debug.Log(www.responseCode);
            if (www.responseCode != 200 && www.responseCode != 403)
            {
                Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
            }
            else if (www.responseCode == 403)
            {
                Debug.Log("USER NOT LOGGED IN AND NO ERROR");
                MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and submit scores.");
            }
        }

        if (www.isDone)
        {
            startGameResponse = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

            string decryptedSessionResponse = "";

            if (www.responseCode == 200)
            {
                decryptedSessionResponse = encryptMaster.Decrypt(startGameResponse);
            }
            else
            {
                decryptedSessionResponse = "";
            }

            sessionIDResults = JsonConvert.DeserializeObject<SessionIDResponseBody>(decryptedSessionResponse);

            if (sessionIDResults != null && sessionIDResults.responseCode == 200)
            {

            }
        }
    }

    public void AddScoins(int scoinEarnings)
    {
        StartCoroutine(UpdateScoins(scoinEarnings, "Prize Won"));
    }

    public void SubtractScoins(int scoinCosts)
    {
        StartCoroutine(UpdateScoins(-scoinCosts, "Scoins Spent"));
    }

    public class ScoinResults
    {
        public int responseCode;
        public int totalScoin;
        public string errorMessage;
    }

    IEnumerator UpdateScoins(int scoinAmount, string description)
    {
        if (sessionIDResults == null || sessionIDResults.sessionId == "" || sessionIDResults.sessionId == null)
        {
            Debug.Log("NO SESSION ID CURRENTLY AVAILABLE");
            SetupStartData();

            while (sessionIDResults == null || sessionIDResults.sessionId == "" || sessionIDResults.sessionId == null)
                yield return null;
        }

        unixTime = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

        string dataToEncrypt = "gameId=13&scoinAmount=" + scoinAmount + "&description=" + description + "&sessionId=" + sessionIDResults.sessionId + "&sendTime=" + unixTime;
        string urlEncryptedData = encryptMaster.Encrypt(dataToEncrypt);
        string url = "/TestSmartClub/GameApiAction.do?action=useScoin&encryptData=" + UnityWebRequest.EscapeURL(urlEncryptedData);
        //string url = "/SmartClub/GameApiAction.do?action=useScoin&encryptData=" + UnityWebRequest.EscapeURL(urlEncryptedData);

        WWWForm form = new WWWForm();

        string encryptedGoalsEarnedData = encryptMaster.Encrypt("");
        string encryptedWinningStatusData = encryptMaster.Encrypt("");
        string encryptedUserChosenData = encryptMaster.Encrypt("");

        if (GameManager.instance != null)
        {
            encryptedGoalsEarnedData = encryptMaster.Encrypt(GameManager.instance.GetGoalsEarnedAsString());
            encryptedWinningStatusData = encryptMaster.Encrypt(GameManager.instance.GetWonScoinStatusAsString());
            encryptedUserChosenData = encryptMaster.Encrypt("");
        }

        form.AddField("Value", encryptedGoalsEarnedData);
        form.AddField("Won", encryptedWinningStatusData);
        form.AddField("ChosenValue", encryptedUserChosenData);


        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
            www.SetRequestHeader("Accept", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                if (www.responseCode == 403)
                {
                    Debug.Log("USER NOT LOGGED IN");
                    MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and submit scores.");
                }
                else
                {
                    Debug.Log(www.error);
                    Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                    MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
                }
            }
            else
            {
                Debug.Log(www.responseCode);
                if (www.responseCode != 200 && www.responseCode != 403)
                {
                    Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                    MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
                }
                else if (www.responseCode == 403)
                {
                    Debug.Log("USER NOT LOGGED IN AND NO ERROR");
                    MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and submit scores.");
                }
            }

            if (www.isDone)
            {
                scoinResponse = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                string decryptedScoinResponse = encryptMaster.Decrypt(scoinResponse);
                scoinResults = JsonConvert.DeserializeObject<ScoinResults>(decryptedScoinResponse);

                if (scoinResults != null && scoinResults.responseCode == 200)
                {
                    totalScoin = scoinResults.totalScoin;
                }
            }
        }
    }

    public void SetupTotalScoin()
    {
            StartCoroutine(GetScoinResults());
    }

    public int GetTotalScoin ()
    {
        return totalScoin;
    }

    IEnumerator GetScoinResults()
    {
        string url = "/TestSmartClub/GameApiAction.do?action=getScoin";
        //string url = "/SmartClub/GameApiAction.do?action=getScoin";

        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
        www.SetRequestHeader("Accept", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.timeout = 30;
        
        yield return www.SendWebRequest();
        
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            if (www.responseCode == 403)
            {
                Debug.Log("USER NOT LOGGED IN");
                MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and earn SCOIN.");
            }
            else
            {
                Debug.Log(www.error);
                Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
            }
        }
        else
        {
            Debug.Log(www.responseCode);
            if (www.responseCode != 200 && www.responseCode != 403)
            {
                Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
            }
            else if (www.responseCode == 403)
            {
                Debug.Log("USER NOT LOGGED IN AND NO ERROR");
                MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and submit scores.");
            }
        }

        if (www.isDone)
        {
            scoinResponse = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);

            string decryptedScoinResponse = "";

            if (www.responseCode == 200)
            {
                decryptedScoinResponse = encryptMaster.Decrypt(scoinResponse);
            }
            else
            {
                decryptedScoinResponse = "";
            }

            scoinResults = JsonConvert.DeserializeObject<ScoinResults>(decryptedScoinResponse);

            if (scoinResults != null && scoinResults.responseCode == 200)
            {
                totalScoin = scoinResults.totalScoin;
                MenuManager.instance.OpenConfirmPopup();
            }
        }
    }


    IEnumerator RetrieveTotalScoinOnly()
    {
        string url = "/TestSmartClub/GameApiAction.do?action=getScoin";
        //string url = "/SmartClub/GameApiAction.do?action=getScoin";

        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
        www.SetRequestHeader("Accept", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.timeout = 30;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            if (www.responseCode == 403)
            {
                Debug.Log("USER NOT LOGGED IN");
                MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and earn SCOIN.");
            }
            else
            {
                Debug.Log(www.error);
                Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
            }
        }
        else
        {
            Debug.Log(www.responseCode);
            if (www.responseCode != 200 && www.responseCode != 403)
            {
                Debug.Log("Response code 403 not received, response code found is: " + www.responseCode);
                MenuManager.instance.OpenErrorPopup(-1, "Game currently not available. Please try again later.");
            }
            else if (www.responseCode == 403)
            {
                Debug.Log("USER NOT LOGGED IN AND NO ERROR");
                MenuManager.instance.OpenErrorPopup(403, "You need to be logged in to play the game and earn SCOIN.");
            }
        }

        if (www.isDone)
        {
            scoinResponse = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            string decryptedScoinResponse = "";

            if (www.responseCode == 200)
            {
                decryptedScoinResponse = encryptMaster.Decrypt(scoinResponse);
            }
            else
            {
                decryptedScoinResponse = "";
            }

            scoinResults = JsonConvert.DeserializeObject<ScoinResults>(decryptedScoinResponse);

            if (scoinResults != null && scoinResults.responseCode == 200)
            {
                Debug.Log("SCOIN GOT");
                totalScoin = scoinResults.totalScoin;
            }
        }
    }

    public class PrizeResults
    {
        public int responseCode;
        public string prizeList;
        public string errorMessage;
    }

    public void SetupPrizes()
    {
        StartCoroutine(GetPrizeList());
    }

    public int GetPrize(int prizeIndex)
    {
        if (scoinPrizeList != null && prizeIndex < scoinPrizeList.Count)
        {
            return int.Parse(scoinPrizeList[prizeIndex]);
        }
        else
        {
            //if (scoinPrizeList != null && scoinPrizeList.Count <= 0)
            //    Debug.Log("Empty Scoin Prize List Oh No");

            //Debug.Log("Scoin Prize List Is Null. Please Fix!");
            return -1;
        }
    }

    IEnumerator GetPrizeList()
    {

        string url = "/TestSmartClub/GameApiAction.do?action=getPrizeList&gameId=13";
        //string url = "/SmartClub/GameApiAction.do?action=getPrizeList&gameId=13";

        UnityWebRequest www = new UnityWebRequest(url, "GET");
        if (www != null)
        {
            www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
            www.SetRequestHeader("Accept", "application/json");
            www.downloadHandler = new DownloadHandlerBuffer();
            www.timeout = 30;

            yield return www.SendWebRequest();

            if (www != null)
            {
                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    Debug.Log(www.responseCode);
                }

                if (www.isDone)
                {
                    prizesResponse = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    if (prizesResponse != null)
                    {
                        string decryptedPrizeResponse = encryptMaster.Decrypt(prizesResponse);
                        prizeResults = JsonConvert.DeserializeObject<PrizeResults>(decryptedPrizeResponse);

                        if (prizeResults != null && prizeResults.responseCode == 200 && prizeResults.prizeList != null)
                        {
                            scoinPrizeList = StringManipulator.instance.GetSeparatedPrizeList(prizeResults.prizeList);
                        }
                    }
                    else
                    {
                        Debug.Log("PrizesResponse is empty or null.");
                    }
                }
            }
            else
            {
                Debug.Log("www on yield : is empty or null.");
            }
        }
        else    
        {
            Debug.Log("www on initialise : is empty or null.");
        }
    }

    public int GetScoinPrizeListLength()
    {
        if (scoinPrizeList == null)
        {
            return 0;
        }
        else
        {
            return scoinPrizeList.Count;
        }
    }

    public class GameCostResults
    {
        public int responseCode;
        public int costPrice;
        public string errorMessage;
    }

    public void SetupGameCost()
    {
        StartCoroutine(SetGameCostResults());
    }

    public int GetGameCost()
    {
        if (gameCostResults != null)
        {
            return gameCostResults.costPrice;
        }
        else
        {
            Debug.Log("Game Cost Results Are Null. Please Fix!");
            return -1;
        }
    }

    IEnumerator SetGameCostResults()
    {
        string url = "/TestSmartClub/GameApiAction.do?action=getStaticCostPrice&gameId=13";
        //string url = "/SmartClub/GameApiAction.do?action=getStaticCostPrice&gameId=13";

        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.SetRequestHeader("Content-Type", "application/json;charset=UTF-8");
        www.SetRequestHeader("Accept", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.timeout = 30;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.responseCode);
        }

        if (www.isDone)
        {
            costResponse = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            string decryptedCostResponse = encryptMaster.Decrypt(costResponse);
            gameCostResults = JsonConvert.DeserializeObject<GameCostResults>(decryptedCostResponse);

            if (gameCostResults != null && gameCostResults.responseCode == 200)
            {
                Debug.Log("Game Price: " + gameCostResults.costPrice);
            }
        }
    }
}
