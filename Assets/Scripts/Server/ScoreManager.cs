using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server;
using SignInSample;
using TMPro;
using UnityEngine;
using static Server.ServerConnect;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private List<GameObject> rankingRows;
    [SerializeField] private TextMeshProUGUI playerBestScoreTxt, currentDbText;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject noDataTxt;

    [HideInInspector] public static string[] serverResponse;

    private DBNames currentDB = DBNames.MySQL;
    private List<DBNames> dbList = new List<DBNames>();

    public static ScoreManager instance;

    private void Start()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);

        currentDB = DBNames.MySQL;
        dbList = Enum.GetValues(typeof(DBNames)).Cast<DBNames>().ToList();
    }

    public void ShowHighScore()
    {
        rankingPanel.SetActive(true);

        currentDbText.text = currentDB.ToString();
        
        StartCoroutine(ServerConnect.instance.GetScores(currentDB, 10, (scores) =>
        {
            foreach (var row in rankingRows)
            {
                var texts = row.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = "";
                texts[1].text = "";
            }
            
            if (scores == null)
            {
                foreach (var row in rankingRows)
                {
                    row.SetActive(false);
                }
                noDataTxt.SetActive(true);
            }
            else
            {
                for (int i = 0; i < scores.Length; i++)
                {
                    var texts = rankingRows[i].GetComponentsInChildren<TextMeshProUGUI>();
                    texts[0].text = i+1 + " - " + scores[i].name;
                    texts[1].text = scores[i].score.ToString();

                    Debug.Log($"INDEX {i}");
                    Debug.Log(rankingRows[i].name);
                    Debug.Log(scores[i].name);
                    Debug.Log(scores[i].score);
                }
                noDataTxt.SetActive(false);
            }
        }));

        // StartCoroutine(ServerConnect.instance.GetScoreByName(currentDB, SigninSampleScript.instance.user.DisplayName, playerBestScoreTxt));
        StartCoroutine(ServerConnect.instance.GetScoreByName(currentDB, "Rodrigo", playerBestScoreTxt));

    }

    public void NextDB()
    {
        var currentIndex = dbList.FindIndex(x => x == currentDB);

        if (currentIndex < dbList.Count-1)
            currentDB = dbList[currentIndex + 1];
        else
            currentDB = dbList[0];

        ShowHighScore();
    }
    public void PreviousDB()
    {
        var currentIndex = dbList.FindIndex(x => x == currentDB);

        if (currentIndex > 0)
            currentDB = dbList[currentIndex - 1];
        else
            currentDB = dbList[^1];

        ShowHighScore();
    }
}
