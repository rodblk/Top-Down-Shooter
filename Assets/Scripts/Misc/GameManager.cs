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
        // playerScoreTxt.text = $"Score: {score}";
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
        try
        {
            StartCoroutine(ServerConnect.instance.PostScore(ServerConnect.DBNames.MySQL, SigninSampleScript.instance.user.DisplayName, Score));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Debug.Log("Could not access database");
            // throw;
        }
        finally
        {
            // Chama pra mostrar highscore
            ScoreManager.instance.ShowHighScore();
        }
        
    }
}
