using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SignInSample;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Server
{
    public class ServerConnect: MonoBehaviour
    {
        public static ServerConnect instance;
        
        private const string BASE_URL = "https://zgzqdjffx0.execute-api.us-east-2.amazonaws.com/prod";

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
                Destroy(this);
        }
        
        [System.Serializable]
        public class ScoreEntry
        {
            public string name;
            public int score;
        }
 
        [System.Serializable]
        public class ScoresResponse
        {
            public ScoreEntry[] scores;
        }
        
        [System.Serializable]
        private class PostScoreBody
        {
            public string name;
            public int score;
        }
 
        [System.Serializable]
        private class PostScoreResponse
        {
            public string message;
        }

        public enum DBNames
        {
            MySQL,
            Postgres,
            Mongo,
            Dynamo
        }

        public IEnumerator GetScores(DBNames dbname, int limit = 10, System.Action<ScoreEntry[]> onComplete = null)
        {
            string path;
            switch (dbname)
            {
                case DBNames.MySQL:
                    path = "scores";
                    break;
                case DBNames.Postgres:
                    path = "scores-pg";
                    break;
                case DBNames.Mongo:
                    path = "scores-mongo";
                    break;
                case DBNames.Dynamo:
                    path = "scores-dynamo";
                    break;
                default:
                    path = "scores";
                    break;
            }
            
            using var req = UnityWebRequest.Get($"{BASE_URL}/{path}?limit={limit}");
            req.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"{BASE_URL}/{path}?limit={limit}");
 
            yield return req.SendWebRequest();
 
            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<ScoresResponse>(req.downloadHandler.text);
                Debug.Log($"Scores recebidos: {res.scores.Length} entradas");
                onComplete?.Invoke(res.scores);
            }
            else
            {
                Debug.LogError("Erro ao buscar scores: " + req.downloadHandler.text);
                onComplete?.Invoke(null);
            }
        }
        
        public IEnumerator GetScoreByName(DBNames dbname, string playerName, TextMeshProUGUI text)
        {
            string path;
            switch (dbname)
            {
                case DBNames.MySQL:
                    path = "scores";
                    break;
                case DBNames.Postgres:
                    path = "scores-pg";
                    break;
                case DBNames.Mongo:
                    path = "scores-mongo";
                    break;
                case DBNames.Dynamo:
                    path = "scores-dynamo";
                    break;
                default:
                    path = "scores";
                    break;
            }
            
            using var req = UnityWebRequest.Get($"{BASE_URL}/{path}?name={UnityWebRequest.EscapeURL(playerName)}");
            req.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"{BASE_URL}/{path}?name={UnityWebRequest.EscapeURL(playerName)}");
            
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<ScoresResponse>(req.downloadHandler.text);
                Debug.Log(res);
                if (res != null)
                    text.text = "Best score: " + res.scores[0].score.ToString();
                else
                    text.text = "Best score: " + "0";
            }
            else
            {
                Debug.LogError("Erro ao buscar score: " + req.downloadHandler.text);
                text.text = "Best score: " + "0";
            }
        }
        
        public IEnumerator PostScore(DBNames dbname, string playerName, int score)
        {
            Debug.Log($"CHEGOU NO POST DO {dbname}");
            string path;
            switch (dbname)
            {
                case DBNames.MySQL:
                    path = "scores";
                    break;
                case DBNames.Postgres:
                    path = "scores-pg";
                    break;
                case DBNames.Mongo:
                    path = "scores-mongo";
                    break;
                case DBNames.Dynamo:
                    path = "scores-dynamo";
                    break;
                default:
                    path = "scores";
                    break;
            }
            
            var bodyData = new PostScoreBody { name = playerName, score = score };
            string json = JsonUtility.ToJson(bodyData);

            Debug.Log($"{BASE_URL}/{path}");
 
            using var req = new UnityWebRequest($"{BASE_URL}/{path}", "POST");
            req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();
 
            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<PostScoreResponse>(req.downloadHandler.text);
                Debug.Log("Score enviado: " + res.message);
            }
            else
            {
                Debug.LogError("Erro ao enviar score: " + req.downloadHandler.text);
            }
        }
        
        public IEnumerator SaveScoreEverywhere(string playerName, int score, System.Action onAllDone = null)
        {
            Debug.Log("ENTROU NO SAVE TUDO");
            // Inicia as 4 requisições ao mesmo tempo
            var mysqlReq    = StartCoroutine(PostScore(DBNames.MySQL,    playerName, score));
            var postgresReq = StartCoroutine(PostScore(DBNames.Postgres, playerName, score));
            var mongoReq    = StartCoroutine(PostScore(DBNames.Mongo,    playerName, score));
            var dynamoReq   = StartCoroutine(PostScore(DBNames.Dynamo,   playerName, score));
 
            // Espera todas terminarem
            yield return mysqlReq;
            yield return postgresReq;
            yield return mongoReq;
            yield return dynamoReq;
 
            Debug.Log("Pontuação enviada para todos os bancos.");
            onAllDone?.Invoke();
        }



        
    //     public IEnumerator GetPlayerHighScoreMongo()
    //     {
    //         using (UnityWebRequest request = UnityWebRequest.Get("https://sa-east-1.aws.data.mongodb-api.com/app/data-uyyqs/endpoint/data/v1/highscore"))
    //         {
    //             yield return request.SendWebRequest();
    //
    //             if (request.isNetworkError || request.isHttpError)
    //             {
    //                 Debug.Log(request.error);
    //                 // if (callback != null)
    //                 // {
    //                 //     // callback.Invoke(null);
    //                 // }
    //             }
    //             else
    //             {
    //                 Debug.Log(request.downloadHandler.text);
    //                 // if (callback != null)
    //                 // {
    //                 //     // callback.Invoke(PlayerData.Parse(request.downloadHandler.text));
    //                 // }
    //             }
    //
    //             Debug.Log(request);
    //         }
    //     }
    }
}
