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
        // [SerializeField] private GameObject rankingPanel;
        // [SerializeField] private TextMeshProUGUI playerBestScoreTxt;
        // [SerializeField] private List<GameObject> rankingRows; 
        
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

        public IEnumerator SaveScoreToMySQL(string username, int score, List<GameObject> rankingRows, TextMeshProUGUI playerBestScoreTxt, GameObject rankingPanel)
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
            {
                Debug.Log("Data insert success");
                var result = request.downloadHandler.text.Split("*");

                for (int i = 0; i < result.Length - 1; i++)
                {
                    if (result[i] != "")
                    {
                        var rowFormat = result[i].Split(",");
                        var texts = rankingRows[i].GetComponentsInChildren<TextMeshProUGUI>();
                        texts[0].text = rowFormat[0];
                        texts[1].text = rowFormat[1];
                        // Debug.Log($"Username: {rowFormat[0]} - Score: {rowFormat[1]}");
                    }
                }

                playerBestScoreTxt.text = $"Best score: {result[^1]}";
            }
            else
                Debug.Log("Data insert failed");
            
            request.Dispose();
            
            rankingPanel.SetActive(true);
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
