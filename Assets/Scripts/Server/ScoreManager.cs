using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using SignInSample;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private List<GameObject> rankingRows;
    [SerializeField] private TextMeshProUGUI playerBestScoreTxt;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject noDataTxt;

    [HideInInspector] public static string[] serverResponse;

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
    }

    public void ShowHighScore()
    {
        rankingPanel.SetActive(true);
        
        StartCoroutine(ServerConnect.instance.GetScores(ServerConnect.DBNames.MySQL, 10, (scores) =>
        {
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
                    texts[0].text = i+1 + " - " + scores[i].Name;
                    texts[1].text = scores[i].Score.ToString();
                }
                noDataTxt.SetActive(false);
            }
        }));

        StartCoroutine(ServerConnect.instance.GetScoreByName(ServerConnect.DBNames.MySQL, SigninSampleScript.instance.user.DisplayName, playerBestScoreTxt));
        
            
        
        // if (serverResponse != null)
        // {
        //     for (int i = 0; i < serverResponse.Length - 1; i++)
        //     {
        //         if (serverResponse[i] != "")
        //         {
        //             var rowFormat = serverResponse[i].Split(",");
        //             var texts = rankingRows[i].GetComponentsInChildren<TextMeshProUGUI>();
        //             texts[0].text = rowFormat[0];
        //             texts[1].text = rowFormat[1];
        //         }
        //     }
        //
        //     playerBestScoreTxt.text = $"Best score: {serverResponse[^1]}";
        //     noDataTxt.SetActive(false);
        // }
        // else
        // {
        //     foreach (var row in rankingRows)
        //     {
        //         row.SetActive(false);
        //     }
        //     noDataTxt.SetActive(true);
        // }

    }
}
