using UnityEngine;

public class AppleLoginManager : MonoBehaviour
{
    public static AppleLoginManager Instance;
    private const string AppleUserIdKey = "AppleUserId";
    //public LoginMenuHandler LoginMenu;
    //public GameMenuHandler GameMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
       // _appleAuthManager
    }
}
