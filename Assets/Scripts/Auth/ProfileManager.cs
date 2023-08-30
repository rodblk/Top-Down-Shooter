using System.Collections;
using System.Collections.Generic;
using Server;
using SignInSample;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileManager : MonoBehaviour
{
    public TextMeshProUGUI username, bestScore;
    void Start()
    {
        // username.text = SigninSampleScript.instance.user.DisplayName;
        //
        // StartCoroutine(ServerConnect.instance.GetPlayerHighScore(bestScore));
    }

    public void Play()
    {
        SceneManager.LoadScene("Game 1");
    }
}
