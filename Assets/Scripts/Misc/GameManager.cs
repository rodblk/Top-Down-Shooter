using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server;
using SignInSample;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // [SerializeField] public string username;
    [SerializeField] private int score;
    [SerializeField] private TextMeshProUGUI playerScoreTxt;
    
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private TextMeshProUGUI playerBestScoreTxt;
    [SerializeField] private List<GameObject> rankingRows;

    private float mainGameAreaWidth = 10f;
    private float mainGameAreaHeight = 10f;
    private float cameraSize = 5f;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            playerScoreTxt.text = $"Score: {score}";
        }
    }

    private void OnEnable()
    {
        EnemyController.OnEnemyDestroyed += AddScore;
        PlayerController.OnPlayerDie += SaveScore;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyDestroyed -= AddScore;
        PlayerController.OnPlayerDie -= SaveScore;
    }

    private void Start()
    {
        Score = 0;

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = mainGameAreaWidth / mainGameAreaHeight;

        if (screenRatio >= targetRatio)
        {
            Camera.main.orthographicSize = cameraSize;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.main.orthographicSize = cameraSize * differenceInSize;
        }
    }

    private void AddScore()
    {
        Score++;
    }

    public void GoToProfile()
    {
        SceneManager.LoadScene("Profile");
    }

    public void Retry()
    {
        Time.timeScale = 1;
        rankingPanel.SetActive(false);
        StageManager.instance.ResetStage();
    }

    public void SaveScore()
    {
        Debug.Log("E PRA SUBIR PONTUACAO");
        
        StartCoroutine(ServerConnect.instance.SaveScoreEverywhere(SigninSampleScript.instance.user.DisplayName, score, () =>
        {
            ScoreManager.instance.ShowHighScore();
        }));
    }
}
