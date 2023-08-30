using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Server;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [SerializeField] public string username;
    [SerializeField] private int score;
    [SerializeField] private TextMeshProUGUI playerScoreTxt;

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
        score = 0;
        playerScoreTxt.text = $"Score: {score}";
        username = "rod";
    }

    private void AddScore()
    {
        score++;
        playerScoreTxt.text = $"Score: {score}";
    }

    public void SaveScore()
    {
        // StartCoroutine(ServerConnect.instance.SaveScoreToMySQL(username, score));
    }

    // IEnumerator SaveScore(string username, int score)
    // {
    //     WWWForm form = new WWWForm();
    //     form.AddField("username", username);
    //     form.AddField("score", score);
    //
    //     UnityWebRequest request = UnityWebRequest.Post("http://localhost/shootersql/register_score.php", form);
    //     
    //     yield return request.SendWebRequest();
    //     
    //     Debug.Log(request.result);
    //     Debug.Log(request.error);
    //     Debug.Log(request.downloadHandler.text);
    //     
    //     if (request.error == null)
    //         Debug.Log("Data insert success");
    //     else
    //         Debug.Log("Data insert failed");
    //
    // }
}
