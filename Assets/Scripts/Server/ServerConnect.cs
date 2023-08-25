using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace Server
{
    public class ServerConnect: MonoBehaviour
    {
        [SerializeField] private GameObject rankingPanel;
        [SerializeField] private TextMeshProUGUI playerBestScoreTxt;
        [SerializeField] private List<GameObject> rankingRows; 
        
        public static ServerConnect instance;

        private void Start()
        {
            if (!instance)
                instance = this;
            else
                Destroy(this);
        }

        public IEnumerator SaveScoreToMySQL(string username, int score)
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("score", score);

            UnityWebRequest request = UnityWebRequest.Post("http://localhost/shootersql/register_score.php", form);
        
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
    }
}
