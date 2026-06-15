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
            public string Name;
            public int Score;
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
        
        // public IEnumerator SaveScoreToMySQL(string username, int score)
        // {
        //     WWWForm form = new WWWForm();
        //     form.AddField("username", username);
        //     form.AddField("score", score);
        //
        //     UnityWebRequest request = UnityWebRequest.Post("http://192.168.1.18/shootersql/register_score.php", form);
        //
        //     yield return request.SendWebRequest();
        //
        //     Debug.Log(request.result);
        //     Debug.Log(request.error);
        //     Debug.Log(request.downloadHandler.text);
        //
        //     if (request.error == null)
        //         ScoreManager.serverResponse = request.downloadHandler.text.Split("*");
        //     else
        //         ScoreManager.serverResponse = null;
        //
        //     request.Dispose();
        // }

        // public IEnumerator GetPlayerHighScore(TextMeshProUGUI playerBestScoreTxt)
        // {
        //     WWWForm form = new WWWForm();
        //     form.AddField("username", SigninSampleScript.instance.user.DisplayName);
        //     
        //     UnityWebRequest request = UnityWebRequest.Post("http://192.168.1.18/shootersql/get_player_info.php", form);
        //
        //     yield return request.SendWebRequest();
        //     
        //     if (request.error == null)
        //     {
        //         // var result = request.downloadHandler.text;
        //
        //         playerBestScoreTxt.text = $"Best score: {request.downloadHandler.text}";
        //     }
        //     else
        //         Debug.Log("Data retrieve failed");
        //     
        //     request.Dispose();
        // }
        
        public IEnumerator GetScores(int limit = 10)
        {
            using var req = UnityWebRequest.Get($"{BASE_URL}/scores?limit={limit}");
            req.SetRequestHeader("Content-Type", "application/json");
 
            yield return req.SendWebRequest();
 
            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<ScoresResponse>(req.downloadHandler.text);
                Debug.Log($"Scores recebidos: {res.scores.Length} entradas");
                foreach (var entry in res.scores)
                {
                    Debug.Log(entry.Name);
                    Debug.Log(entry.Score);
                }
            }
            else
            {
                Debug.LogError("Erro ao buscar scores: " + req.downloadHandler.text);
                // onComplete?.Invoke(null);
            }
        }
        
        public IEnumerator PostScore(string playerName, int score)
        {
            var bodyData = new PostScoreBody { name = playerName, score = score };
            string json = JsonUtility.ToJson(bodyData);
 
            using var req = new UnityWebRequest($"{BASE_URL}/scores", "POST");
            req.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
 
            yield return req.SendWebRequest();
 
            if (req.result == UnityWebRequest.Result.Success)
            {
                var res = JsonUtility.FromJson<PostScoreResponse>(req.downloadHandler.text);
                Debug.Log("Score enviado: " + res.message);
                // onComplete?.Invoke(res.message);
            }
            else
            {
                Debug.LogError("Erro ao enviar score: " + req.downloadHandler.text);
                // onComplete?.Invoke(null);
            }
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
