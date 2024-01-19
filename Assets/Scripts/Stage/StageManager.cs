using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StageManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> stages;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject finishSpacePrefab;
    // [SerializeField] private StageProgression progression;

    [SerializeField] private Image blackScreen;

    public static StageManager instance;

    private float positionX;
    private float positionZ;

    public LayerMask obstacleMask;

    private int stagesCleared;
    private int enemiesDestroyed;

    private GameObject player;
    private Vector3 startPosition;

    private GameObject finishSpace;
    private GameObject currentStage;

    private int enemiesQty;

    // private NavMeshSurface navmesh;

    private void OnEnable()
    {
        FinishSpace.OnFinishStage += ChangeStage;
        EnemyController.OnEnemyDestroyed += EnemyDestroyed;
    }

    private void OnDisable()
    {
        FinishSpace.OnFinishStage -= ChangeStage;
        EnemyController.OnEnemyDestroyed -= EnemyDestroyed;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
            
        stagesCleared = 0;
        enemiesDestroyed = 0;
        enemiesQty = 3;
        
        currentStage = Instantiate(stages[stagesCleared]);
        player = Instantiate(playerPrefab);
        
        startPosition = player.transform.position;
        currentStage.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
        
        SpawnEnemies();

    }

    public void EnemyDestroyed()
    {
        enemiesDestroyed++;
        if (enemiesDestroyed == enemiesQty)
            finishSpace = Instantiate(finishSpacePrefab);
    }

    public void SpawnEnemies()
    {
        var spawnArea = GameObject.Find("SpawnArea").GetComponent<Collider>();

        for (int i = 0; i < enemiesQty; i++)
        {
            bool allowSpawn = false;
            while (!allowSpawn)
            {
                positionX = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
                positionZ = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);
                Vector3 possibleSpawnPos = new Vector3(positionX, 0.5f, positionZ);
                var collided = Physics.OverlapSphere(possibleSpawnPos, 0.1f, obstacleMask);
                if (!(collided.Length > 0))
                    allowSpawn = true;
            }

            Vector3 spawnPosition = new Vector3(positionX, 0.5f, positionZ);

            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void ChangeStage()
    {
        StartCoroutine(HideGame(NextStage));
    }

    public void Retry()
    {
        StartCoroutine(HideGame(ResetStage));
    }

    IEnumerator HideGame(Action callback = null)
    {
        float lerpDuration = 0.5f; 
        float startValue = 0; 
        float endValue = 1;

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            var a = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, a);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (callback != null)
            callback();
    }
    
    IEnumerator ShowGame(Action callback = null)
    {
        float lerpDuration = 0.5f; 
        float startValue = 1; 
        float endValue = 0;

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {
            var a = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, a);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (callback != null)
            callback();
    }

    public void NextStage()
    {
        Destroy(currentStage);
        Destroy(finishSpace);
        
        stagesCleared++;
        if (stagesCleared == 3)
        {
            stagesCleared = 0;
            enemiesQty += 2;
        }
        
        enemiesDestroyed = 0;
        
        currentStage = Instantiate(stages[stagesCleared]);
        currentStage.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
        player.transform.position = startPosition;

        StartCoroutine(ShowGame(SpawnEnemies));
    }
    public void ResetStage()
    {
        Destroy(currentStage);
        Destroy(finishSpace);

        var enemies = FindObjectsOfType<EnemyController>();
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        
        var bullets = FindObjectsOfType<Bullet>();
        foreach (var bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }

        FindObjectOfType<GameManager>().Score = 0;
        stagesCleared = 0;
        enemiesDestroyed = 0;
        enemiesQty = 3;

        currentStage = Instantiate(stages[stagesCleared]);
        currentStage.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
        player = Instantiate(playerPrefab);
        startPosition = player.transform.position;

        StartCoroutine(ShowGame(SpawnEnemies));
    }
}
