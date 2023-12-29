using Firebase;
using Firebase.Auth;
using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseGoogleSignInManager : MonoBehaviour
{
    public Text infoText;
    public string webClientId = "<your client id here>";

    private GoogleSignInConfiguration configuration;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true, RequestProfile = true };
    }

    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.LogError("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    public void SignOutFromGoogle()
    {
        Debug.LogError("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        Debug.LogError("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
        UiManager.Instance.profileImg.gameObject.SetActive(false);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            AddToInformation("Email = " + task.Result.Email);
            AddToInformation("Google ID Token = " + task.Result.IdToken);
            AddToInformation("Email = " + task.Result.Email);
            AddToInformation("AvatarUrl = " + task.Result.ImageUrl);
            //SignInWithGoogleOnFirebase(task.Result.IdToken);
            Debug.LogError("Google Avtar Url:" + task.Result.ImageUrl);
            StartCoroutine(DownloadAvatar(task.Result.ImageUrl.ToString()));
            FirebaseManager.Instance.SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    //private void SignInWithGoogleOnFirebase(string idToken)
    //{
    //    Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

    //    auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
    //    {
    //        AggregateException ex = task.Exception;
    //        if (ex != null)
    //        {
    //            if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
    //                AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
    //        }
    //        else
    //        {
    //            AddToInformation("Sign In Successful.");
    //        }
    //    });
    //}

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void AddToInformation(string str) { Debug.LogError(str); }

    public IEnumerator DownloadAvatar(string avatarUrl)
    {
        Debug.LogError("google avtar url:" + avatarUrl);
        if (Uri.IsWellFormedUriString(avatarUrl, UriKind.Absolute))
        {
            WWW www = new WWW(avatarUrl);
            yield return www;
            Sprite profilePic = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f), 32);
            UiManager.Instance.profileImg.sprite = profilePic;
            UiManager.Instance.profileImg.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Invalid url of google profile pic");
        }
    }
}