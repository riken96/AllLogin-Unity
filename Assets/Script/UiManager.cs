using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public string loginWith = "";

    [Header("LoginPanel Refernce")]
    public GameObject loginPanel;
    public InputField email;
    public InputField password;

    public Text usernameText;
    public Text emailAddressText;
    public Text photoUrlText;

    public Text loginStatusText;

    public Image profileImg;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void FirebaseRegisterBtnClick()
    {
        Debug.LogError("Firebase Sign In button click");
        loginWith = "FirebaseRegister";
        ShowLoginPanel();
    }

    public void FirebaseSignInBtnClick()
    {
        Debug.LogError("Firebase Sign In button click");
        loginWith = "FirebaseSignIn";
        ShowLoginPanel();
    }

    public void FirebaseSignOutBtnClick()
    {
        Debug.LogError("Firebase SignOut button click");
        FirebaseManager.Instance.SignOut();
    }

    public void FaceBookSignInBtnClick()
    {
        FaceBookManager.Instance.FacebookLogin();
        Debug.LogError("FaceBook Sign In button click");
    }

    public void FaceBookSignOutBtnClick()
    {
        FaceBookManager.Instance.FacebookLogout();
        UiManager.Instance.profileImg.gameObject.SetActive(false);
        Debug.LogError("FaceBook Sign Out button click");
    }

    public void AppleSignInBtnClick()
    {
        Debug.LogError("Apple SignIn button click");
    }

    public void AppleSignOutBtnClick()
    {
        Debug.LogError("Apple Sign out button click");
    }

    public void ShowLoginPanel()
    {
        email.text = "";
        password.text = "";
        loginPanel.SetActive(true);
    }

    public void LoginPanelLoginBtnClick()
    {
        switch (loginWith)
        {
            case "FirebaseRegister":
                FirebaseManager.Instance.SignUP(email.text,password.text);
                break;
            case "FirebaseSignIn":
                FirebaseManager.Instance.SignIn(email.text, password.text);
                break;
            default:
                break;
        }
        loginPanel.SetActive(false);
    }

    public void LoginPanelCancelBtnClick()
    {
        loginPanel.SetActive(false);
    }
}