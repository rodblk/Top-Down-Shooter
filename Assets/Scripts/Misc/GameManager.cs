using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int score;
    [SerializeField] private TextMeshProUGUI playerScoreTxt;

    private void OnEnable()
    {
        EnemyController.OnEnemyDestroyed += AddScore;
    }

    private void OnDisable()
    {
        EnemyController.OnEnemyDestroyed -= AddScore;
    }

    private void Start()
    {
        score = 0;
        playerScoreTxt.text = $"Score: {score}";
    }

    private void AddScore()
    {
        score++;
        playerScoreTxt.text = $"Score: {score}";
    }
}
