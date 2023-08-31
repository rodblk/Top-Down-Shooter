using System.Collections;
using System.Collections.Generic;
using SignInSample;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    public void Login()
    {
        SigninSampleScript.instance.OnSignIn();
    }
}
