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
        // playerScoreTxt.text = $"Score: {score}";
        // username = "rod";
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
            StartCoroutine(
                ServerConnect.instance.SaveScoreToMySQL(SigninSampleScript.instance.user.DisplayName, Score));
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
