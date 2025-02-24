using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StringManipulator : MonoBehaviour
{
    public static StringManipulator instance = null;

    private List<string> separatedList;

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
        separatedList = new List<string>();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<string> GetSeparatedPrizeList(string stringToSeparate)
    {
        if (separatedList == null)
            separatedList = new List<string>();

        if (stringToSeparate.Length <= 0)
        {
            separatedList.Add("Empty String");
            return separatedList;
        }

        separatedList.Clear();

        string[] finalList = stringToSeparate.Split(new string[] { "," }, StringSplitOptions.None);
        separatedList.AddRange(finalList);

        return separatedList;
    }
}
