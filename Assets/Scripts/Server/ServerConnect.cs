using System;
using System.Collections;
using System.Collections.Generic;
using SignInSample;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Server
{
    public class ServerConnect: MonoBehaviour
    {
        public static ServerConnect instance;

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

        public IEnumerator SaveScoreToMySQL(string username, int score)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("score", score);

            UnityWebRequest request = UnityWebRequest.Post("http://192.168.1.18/shootersql/register_score.php", form);
        
            yield return request.SendWebRequest();
        
            Debug.Log(request.result);
            Debug.Log(request.error);
            Debug.Log(request.downloadHandler.text);

            if (request.error == null)
                ScoreManager.serverResponse = request.downloadHandler.text.Split("*");
            else
                ScoreManager.serverResponse = null;

            request.Dispose();
        }

        public IEnumerator GetPlayerHighScore(TextMeshProUGUI playerBestScoreTxt)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", SigninSampleScript.instance.user.DisplayName);
            
            UnityWebRequest request = UnityWebRequest.Post("http://192.168.1.18/shootersql/get_player_info.php", form);

            yield return request.SendWebRequest();
            
            if (request.error == null)
            {
                // var result = request.downloadHandler.text;

                playerBestScoreTxt.text = $"Best score: {request.downloadHandler.text}";
            }
            else
                Debug.Log("Data retrieve failed");
            
            request.Dispose();
        }
    }
}
