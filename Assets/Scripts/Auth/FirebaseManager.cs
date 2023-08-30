using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    public string GoogleWebAPI = "266029706545-1ef8qeap8kks6htjjrbh5oe0rd5q29gl.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    private Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;
    private Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseUser user;

    public TextMeshProUGUI UserNameTxt;
    public GameObject LoginScreen, ProfileScreen;

    public static FirebaseManager instance;

    public FirebaseUser User => user;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        
        // Configure webAPI key with google
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
    }

    private void Start()
    {
        InitFirebase();
    }

    void InitFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticateFinished);
    }

    void OnGoogleAuthenticateFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Fault");
            Debug.LogError(task.Result);
            Debug.LogError(task.Exception);
            Debug.LogError(task.Status);
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Login Cancel");
        }
        else
        {
            Firebase.Auth.Credential credential =
                Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithCredential was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInCredential encountered an error" + task.Exception);
                    return;
                }

                user = auth.CurrentUser;

                UserNameTxt.text = user.DisplayName;
                // UserEmailTxt.text = user.Email;
                //
                LoginScreen.SetActive(false);
                ProfileScreen.SetActive(true);

                Debug.Log(user.DisplayName);
                Debug.Log(user.Email);

                // StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
            });
        }
    }

    // private string CheckImageUrl(string url)
    // {
    //     if (!string.IsNullOrEmpty(url))
    //     {
    //         return url;
    //     }
    //
    //     return imageUrl;
    // }

    // IEnumerator LoadImage(string imageUrl)
    // {
    //     WWW www = new WWW(imageUrl);
    //     yield return www;
    //     
    //     UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, 
    //         www.texture.width, www.texture.height), new Vector2(0, 0));
    // }
}
