using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    private FirebaseAuth auth;
    private FirebaseUser user;
    public string displayName;
    public string emailAddress;
    public string photoUrl;
    public string userId;

    public string loginStatusStr;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log($"Dependency Status: {dependencyStatus}");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        Debug.Log("InitializeFirebase......." + auth);
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("AuthStateChanged......." + auth);
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                emailAddress = user.Email ?? "";
                photoUrl = user.PhotoUrl.ToString();
                userId = user.UserId;
                Debug.Log("displayName :: " + user.DisplayName);
                Debug.Log("emailAddress :: " + user.Email);
                Debug.Log("photoUrl :: " + user.PhotoUrl.ToString());
            }
        }
        UiManager.Instance.usernameText.text = displayName;
        UiManager.Instance.emailAddressText.text = emailAddress;
        UiManager.Instance.photoUrlText.text = photoUrl;
    }

    public void SignUP(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                UiManager.Instance.loginStatusText.text = "CreateUserWithEmailAndPasswordAsync was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                UiManager.Instance.loginStatusText.text = "CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            loginStatusStr = "Firebase user created successfully \n name:" + newUser.DisplayName + " userid:" + newUser.UserId;
        });
         UiManager.Instance.loginStatusText.text = loginStatusStr;
    }

    public void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                UiManager.Instance.loginStatusText.text = "SignInWithEmailAndPasswordAsync was canceled.";
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                UiManager.Instance.loginStatusText.text = "SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception;
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            loginStatusStr = "User signed in successfully \n name:" + newUser.DisplayName + " \n userid:" + newUser.UserId + "\nemail:"+newUser.Email + "\nAvtarUrl:"+newUser.PhotoUrl; 
            UiManager.Instance.loginStatusText.text = loginStatusStr;
        });
        UiManager.Instance.loginStatusText.gameObject.SetActive(false);
        ShowActiveStatusTextCo();
    }

    Coroutine activestatusText;

    public void ShowActiveStatusTextCo()
    {
        if (activestatusText != null)
        {
            StopCoroutine(activestatusText);
        }
        activestatusText = StartCoroutine(ActiveStatusText());
    }
    IEnumerator ActiveStatusText()
    {
        yield return new WaitForSeconds(2);
        UiManager.Instance.loginStatusText.gameObject.SetActive(true);
    }

    public void SignOut()
    {
        auth.SignOut();
    }


    #region google signIn.......
    public void SignInWithGoogleOnFirebase(string googleIdToken)
    {
        Debug.Log("googleIdToken:" + googleIdToken);
        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            loginStatusStr = "User signed in successfully \n name:" + newUser.DisplayName + " \n userid:" + newUser.UserId + "\nemail:" + newUser.Email + "\nAvtarUrl:" + newUser.PhotoUrl;
            UiManager.Instance.loginStatusText.text = loginStatusStr;
        });
        UiManager.Instance.loginStatusText.gameObject.SetActive(false);
        ShowActiveStatusTextCo();
    }
    #endregion

    #region faceBook signIn.......
    public void FaceBookSingIn(string accessToken)
    {
        Debug.LogError("firebase FaceBookSingIn:" + accessToken);
        Firebase.Auth.Credential credential = Firebase.Auth.FacebookAuthProvider.GetCredential(accessToken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
            loginStatusStr = "User signed in successfully \n name:" + newUser.DisplayName + " \n userid:" + newUser.UserId + "\nemail:" + newUser.Email + "\nAvtarUrl:" + newUser.PhotoUrl;
            UiManager.Instance.loginStatusText.text = loginStatusStr;
        });
        UiManager.Instance.loginStatusText.gameObject.SetActive(false);
        ShowActiveStatusTextCo();
    }
    #endregion

    #region Apple SignIn.......

    public void AppleSignIn(string appleIdToken)
    {
        Firebase.Auth.Credential credential = Firebase.Auth.OAuthProvider.GetCredential("apple.com", appleIdToken, "acsdgfhrty", null);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",newUser.DisplayName, newUser.UserId);
            loginStatusStr = "User signed in successfully \n name:" + newUser.DisplayName + " \n userid:" + newUser.UserId + "\nemail:" + newUser.Email + "\nAvtarUrl:" + newUser.PhotoUrl;
            UiManager.Instance.loginStatusText.text = loginStatusStr;
        });
        UiManager.Instance.loginStatusText.gameObject.SetActive(false);
        ShowActiveStatusTextCo();
    }
    #endregion
}