using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System;

public class DataManager : MonoBehaviour
{

    public static DataManager instance;
    public string Message;
    public DataInstance inst;
    public Launcher launcher;

   // DatabaseReference data;

    public MenuManager manager;
    public SettingsManager settings;
    public GameObject BackToMenuAfterVerification;

    public GameObject LoginButtonAfterVerified;

    [Header("Firebase")]
    public FirebaseAuth auth;
    public FirebaseUser user;
    [Space(5f)]

    [Header("Delete references")]
    [SerializeField]
    private InputField deletePassword;
    [SerializeField]
    private Text errorOutput;
    [Space(5f)]

    [Header("Login references")]
    [SerializeField]
    private InputField loginEmail, loginPassword, ResetPasswordInput;
    [SerializeField]
    private Text loginOutputText;
    [Space(5f)]

    [Header("Register references")]
    [SerializeField]
    private InputField usernameProfile, emailProfile, passwordProfile, confirmpasswordProfile;
    [SerializeField]
    private Text profileOutputText;
    [Space(5f)]


    [Header("Register references")]
    [SerializeField]
    private InputField registerUsername, registerEmail, registerPassword, registerConfirmPassword;
    [SerializeField]
    private Text registerOutputText, verifyEmailText;

    string ConfirmName, ActualPassword;

    public Uri PhotoURL { get; private set; }

    public string Username;
    public GameObject [] OnlineScreen;
    public bool isOnline;

    FirebaseDatabase data;
    public DataSnapshot snap;

    public GameObject[] WaitScreen;

    public void Awake()
    {
      
        data = FirebaseDatabase.DefaultInstance;

    }



    private void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            launcher = FindObjectOfType<Launcher>();
            manager = FindObjectOfType<MenuManager>();
        }
    }



    public void SaveNewPlayerData (string _username, string ID,bool online)
    {
        User user = new User(_username, ID, online);
        string json = JsonUtility.ToJson(user);
        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(ID).SetRawJsonValueAsync(json);
    }



    public void DeletePlayerData(string ID)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(ID).RemoveValueAsync();
    }

    /*public void TryAgain ()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(false);
        //StartCoroutine(GetOnlineStatus((bool online)=> {
            if (online == false)
            {
                FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").SetValueAsync(true);
                settings.online = true;

                manager.OpenWindow(3);
                FirebaseDatabase.DefaultInstance.GetReference("user").Child(auth.CurrentUser.UserId).Child("Online").ValueChanged += settings.IfOnlineValueChanged;

            }
            else
            {
                OnlineScreen[0].SetActive(true);
                OnlineScreen[1].SetActive(true);
                Debug.Log(auth.CurrentUser.DisplayName + ":" + " offline");
             //   FirebaseDatabase.DefaultInstance.GetReference("user").Child(auth.CurrentUser.UserId).Child("Online").ValueChanged += OnlineValueChanged;

            }

        }));
    }*/

  

    IEnumerator CheckName (Text _name,string ID, Text output,bool register)
    {
        bool Exists  = false;
        if (_name.text == "")
        {
            output.text = "Please enter new username!";
            usernameProfile.text = auth.CurrentUser.DisplayName;
        } else if (_name.text.Length < 4)
        {
            output.text = "Username is too short!";
            usernameProfile.text = auth.CurrentUser.DisplayName;
        }
        else       
        { var names = FirebaseDatabase.DefaultInstance.RootReference.Child("user").OrderByChild("Nickname").GetValueAsync();
            yield return new WaitUntil(predicate: () => names.IsCompleted);

            if (names.Exception != null)
            {
                Debug.LogError("Error: " + names.Exception);
            }
            else
            {
                DataSnapshot snapshot = names.Result;
               
          
                    if (!register)
                    {
                        foreach (DataSnapshot dataChildSnapshot in snapshot.Children)
                        {
                            Debug.Log("Before: " + _name + "Compare to: " + dataChildSnapshot.Child("Nickname").Value.ToString());
                            if (_name.text == dataChildSnapshot.Child("Nickname").Value.ToString() && dataChildSnapshot.Child("UserID").Value.ToString() != ID)
                            {
                                Exists = true;
                                output.text = "Username already exists!";
                                usernameProfile.text = auth.CurrentUser.DisplayName;
                                break;
                            }
                        }
                        

                        if (auth.CurrentUser.IsEmailVerified && !Exists)
                        {
                            output.text = "";
                            FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Nickname").SetValueAsync(_name.text);
                            UserProfile profile = new UserProfile
                            {
                                DisplayName = _name.text,

                                PhotoUrl = new System.Uri("https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg"),
                            };
                            auth.CurrentUser.UpdateUserProfileAsync(profile);
                        }
                    }
                    else
                    {
                        foreach (DataSnapshot dataChildSnapshot in snapshot.Children)
                        {
                        if (dataChildSnapshot != null && dataChildSnapshot.HasChild("Nickname"))
                        {
                            Debug.Log("Before: " + _name + "Compare to: " + dataChildSnapshot.Child("Nickname").Value.ToString());
                            if (_name.text == dataChildSnapshot.Child("Nickname").Value.ToString() && dataChildSnapshot.Child("UserID").Value.ToString() != ID)
                            {
                                Exists = true;
                                output.text = "Username already exists!";

                                break;
                            }
                        }
                        }

                        if (!Exists) StartCoroutine(RegisterLogic(registerUsername.text,registerEmail.text,registerPassword.text,registerConfirmPassword.text));
                    }
                
            }
        }

    }

    public void ClearOutput ()
    {
        profileOutputText.text = "";
    }



    private void Start()
    {   
        StartCoroutine(CheckAndFixDependacies());   
    }

    IEnumerator CheckAndFixDependacies()
    {
        var checkAndFixDependanciesTask = FirebaseApp.CheckAndFixDependenciesAsync();

        yield return new WaitUntil(predicate: () => checkAndFixDependanciesTask.IsCompleted);

        var dependancyResult = checkAndFixDependanciesTask.Result;

        if (dependancyResult == DependencyStatus.Available)
        {
            InitializeFirebase();
            Debug.Log("Database available");
        }
        else
        {
            Debug.Log($"Could not resolve all Firebase dependancies:{dependancyResult}");
        }

    }

    public void Verified ()
    {
        launcher.Loading(true);
        StartCoroutine(CheckAutoLogin());
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(CheckAutoLogin());
      //  auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        Debug.Log("Database initialized");
    }

    IEnumerator CheckAutoLogin()
    {
        yield return new WaitForEndOfFrame();
  
            if (user != null)
            {
                var reloadTask = user.ReloadAsync();

                yield return new WaitUntil(predicate: () => reloadTask.IsCompleted);
                user = auth.CurrentUser;
                // FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(false);
                Debug.Log("Auto-login");
                StartCoroutine(AutoLogin());

            }
            else
            {
                launcher.Loading(false);
                manager.OpenWindow(5);
            }
        
    }
    
    IEnumerator AutoLogin()
    {
       
        if (user != null)
        {
            usernameProfile.text = auth.CurrentUser.DisplayName;
            PhotonNetwork.NickName = auth.CurrentUser.DisplayName;
            if (user.IsEmailVerified)
            {
                var online = FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").GetValueAsync();
                yield return new WaitUntil(predicate: () => online.IsCompleted);

                if (online.Exception != null)
                {
                    Debug.LogError("Error: " + online.Exception);
                }
                else
                {
                    DataSnapshot snapshot = online.Result;
                    Debug.Log("online status");
                    Debug.Log(snapshot.Value);
                    if ((bool)snapshot.Value == false)
                    {
                   
                           FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").OnDisconnect().SetValue(false);
                        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").SetValueAsync(true);

                        Debug.Log("Ok");
                        manager.AwaitingScreenActive(1);
                        manager.OpenWindow(3);
                        launcher.Loading(false);
                        usernameProfile.text = auth.CurrentUser.DisplayName;
                        Debug.Log(auth.CurrentUser.DisplayName + ":" + " online");

                        //StartCoroutine(AddListener());
                        Debug.Log("Loggining...");
                    }
                    else
                    {
                        Debug.Log("Waiting...");
                        launcher.Loading(false);
                       
                       WaitScreen[0].SetActive(true);
                       WaitScreen[1].SetActive(true);
                        //  FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").ValueChanged += Wait;
                    }
                }
            }
            else
            {
            
                StartCoroutine(SendVerificationEmail());
            }
        }
        else
        {
            manager.AwaitingScreenActive(1);
            launcher.Loading(false);
            manager.OpenWindow(5);
        }
    }


    public void Wait (object sender, ValueChangedEventArgs args)
    {
         
        Debug.Log("Method addlistener called!");
    
          if (args.DatabaseError == null)
          {
                      if ((bool)args.Snapshot.Value == false)
                      {                       
                          if (auth.CurrentUser.IsEmailVerified)
                          {
                            StartCoroutine(AutoLogin());
                          }
                      }
          }


    }
  

    IEnumerator CheckVerification()
    {
        yield return new WaitUntil(() => auth.CurrentUser.IsEmailVerified == true);
        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(false);
        //  yield return new WaitForEndOfFrame();
       /* StartCoroutine(GetOnlineStatus((bool online) => {
            if (online == false)
            {
                FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").SetValueAsync(true);
                settings.online = true;
                launcher.Loading(false);
                manager.OpenWindow(3);
                FirebaseDatabase.DefaultInstance.GetReference("user").Child(auth.CurrentUser.UserId).Child("Online").ValueChanged += settings.IfOnlineValueChanged;

            }
            else
            {
                OnlineScreen[0].SetActive(true);
                OnlineScreen[1].SetActive(true);
                launcher.Loading(false);
                Debug.Log(auth.CurrentUser.DisplayName + ":" + " offline");
                FirebaseDatabase.DefaultInstance.GetReference("user").Child(auth.CurrentUser.UserId).Child("Online").ValueChanged += inst.OnlineValueChanged;

            }
            launcher.Loading(false);
        }));*/

        // manager.OpenWindow(3);
        Debug.Log("OK");
    }

   
   

    public void SignOut ()
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").SetValueAsync(false);
        auth.SignOut();
        FirebaseDatabase.DefaultInstance.GoOffline();
        settings.online = false;
        inst.online = false;
        //manager.OpenWindow(5);
    }   


    IEnumerator LoginBeforDelete(string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(auth.CurrentUser.Email, _password);

        var loginTask = auth.SignInWithCredentialAsync(credential);
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown error,please try again";

            switch (error)
            {
                case AuthError.MissingPassword:
                    output = "Please Enter Your Password";
                    break;
                case AuthError.WrongPassword:
                    output = "Incorrect Password";
                    break;
            }
            errorOutput.text = output;
        }
        else
        {
            manager.OpenWindow(5);
            manager.AwaitingScreenActive(1);
            DeletePlayerData(auth.CurrentUser.UserId);
            DeleteUser();
        }
    }

    public void DeleteUser()
    {
        //enter password before delete
      
        if (auth.CurrentUser != null)
        {
            auth.CurrentUser.DeleteAsync().ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }
                if (task.IsCompleted)
                {
                    Debug.Log("User deleted successfully.");
                }
            });
        }
    }
    public void DeleteUserButton ()
    {
        StartCoroutine(LoginBeforDelete(deletePassword.text));
    }


    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("Listener AuthStateChanged called!");
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed Out");
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log($"Signed In");
            }

        }
    }

    public void ClearOutputs()
    {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }

    public void LoginButton()
    {
        FirebaseDatabase.DefaultInstance.GoOnline();
         StartCoroutine(LoginLogic(loginEmail.text, loginPassword.text));
        launcher.Loading(true);
    }

    public void RegisterButton()
    {
        launcher.Loading(true);
        RegisterPreparations();
       // StartCoroutine(RegisterLogic(registerUsername.text, registerEmail.text, registerPassword.text, registerConfirmPassword.text));
    }

    private IEnumerator LoginLogic(string _email, string _password)
    {
        Credential credential = EmailAuthProvider.GetCredential(_email, _password);

        var loginTask = auth.SignInWithCredentialAsync(credential);
       
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = (FirebaseException)loginTask.Exception.GetBaseException();
            AuthError error = (AuthError)firebaseException.ErrorCode;
            string output = "Unknown error,please try again";

            switch (error)
            {
                case AuthError.None: output = AuthError.None.ToString(); break;
                case AuthError.Unimplemented: output = AuthError.Unimplemented.ToString(); break;
                case AuthError.Failure: output = AuthError.Failure.ToString(); break;
                case AuthError.InvalidCustomToken: output = AuthError.InvalidCustomToken.ToString(); break;
                case AuthError.UserDisabled: output = AuthError.UserDisabled.ToString(); break;
                case AuthError.AccountExistsWithDifferentCredentials: output = AuthError.AccountExistsWithDifferentCredentials.ToString(); break;
                case AuthError.OperationNotAllowed: output = AuthError.OperationNotAllowed.ToString(); break;
                case AuthError.EmailAlreadyInUse: output = AuthError.EmailAlreadyInUse.ToString(); break;
                case AuthError.RequiresRecentLogin: output = AuthError.RequiresRecentLogin.ToString(); break;
                case AuthError.CredentialAlreadyInUse: output = AuthError.CredentialAlreadyInUse.ToString(); break;
                case AuthError.InvalidEmail: output = AuthError.InvalidEmail.ToString(); break;
                case AuthError.WrongPassword: output = AuthError.WrongPassword.ToString(); break;
                case AuthError.TooManyRequests: output = AuthError.TooManyRequests.ToString(); break;
                case AuthError.UserNotFound: output = AuthError.UserNotFound.ToString(); break;
                case AuthError.ProviderAlreadyLinked: output = AuthError.ProviderAlreadyLinked.ToString(); break;
                case AuthError.NoSuchProvider: output = AuthError.NoSuchProvider.ToString(); break;
                case AuthError.InvalidUserToken: output = AuthError.InvalidUserToken.ToString(); break;
                case AuthError.UserTokenExpired: output = AuthError.UserTokenExpired.ToString(); break;
                case AuthError.NetworkRequestFailed: output = AuthError.NetworkRequestFailed.ToString(); break;
                case AuthError.InvalidApiKey: output = AuthError.InvalidApiKey.ToString(); break;
                case AuthError.AppNotAuthorized: output = AuthError.AppNotAuthorized.ToString(); break;
                case AuthError.UserMismatch: output = AuthError.UserMismatch.ToString(); break;
                case AuthError.WeakPassword: output = AuthError.WeakPassword.ToString(); break;
                case AuthError.NoSignedInUser: output = AuthError.NoSignedInUser.ToString(); break;
                case AuthError.ApiNotAvailable: output = AuthError.ApiNotAvailable.ToString(); break;
                case AuthError.ExpiredActionCode: output = AuthError.ExpiredActionCode.ToString(); break;
                case AuthError.InvalidActionCode: output = AuthError.InvalidActionCode.ToString(); break;
                case AuthError.InvalidMessagePayload: output = AuthError.InvalidMessagePayload.ToString(); break;
                case AuthError.InvalidPhoneNumber: output = AuthError.InvalidPhoneNumber.ToString(); break;
                case AuthError.MissingPhoneNumber: output = AuthError.MissingPhoneNumber.ToString(); break;
                case AuthError.InvalidRecipientEmail: output = AuthError.InvalidRecipientEmail.ToString(); break;
                case AuthError.InvalidSender: output = AuthError.InvalidSender.ToString(); break;
                case AuthError.InvalidVerificationCode: output = AuthError.InvalidVerificationCode.ToString(); break;
                case AuthError.InvalidVerificationId: output = AuthError.InvalidVerificationId.ToString(); break;
                case AuthError.MissingEmail: output = AuthError.MissingEmail.ToString(); break;
                case AuthError.MissingPassword: output = AuthError.MissingPassword.ToString(); break;
                case AuthError.QuotaExceeded: output = AuthError.QuotaExceeded.ToString(); break;
                case AuthError.RetryPhoneAuth: output = AuthError.RetryPhoneAuth.ToString(); break;
                case AuthError.SessionExpired: output = AuthError.SessionExpired.ToString(); break;
                case AuthError.AppNotVerified: output = AuthError.AppNotVerified.ToString(); break;
                case AuthError.AppVerificationFailed: output = AuthError.AppVerificationFailed.ToString(); break;
                case AuthError.CaptchaCheckFailed: output = AuthError.CaptchaCheckFailed.ToString(); break;
                case AuthError.InvalidAppCredential: output = AuthError.InvalidAppCredential.ToString(); break;
                case AuthError.MissingAppCredential: output = AuthError.MissingAppCredential.ToString(); break;
                case AuthError.InvalidClientId: output = AuthError.InvalidClientId.ToString(); break;
                case AuthError.InvalidContinueUri: output = AuthError.InvalidContinueUri.ToString(); break;
                case AuthError.MissingContinueUri: output = AuthError.MissingContinueUri.ToString(); break;
                case AuthError.KeychainError: output = AuthError.KeychainError.ToString(); break;
                case AuthError.MissingAppToken: output = AuthError.MissingAppToken.ToString(); break;
                case AuthError.MissingIosBundleId: output = AuthError.MissingIosBundleId.ToString(); break;
                case AuthError.NotificationNotForwarded: output = AuthError.NotificationNotForwarded.ToString(); break;
                case AuthError.WebContextCancelled: output = AuthError.WebContextCancelled.ToString(); break;
                case AuthError.DynamicLinkNotActivated: output = AuthError.DynamicLinkNotActivated.ToString(); break;
                case AuthError.Cancelled: output = AuthError.Cancelled.ToString(); break;
                case AuthError.InvalidProviderId: output = AuthError.InvalidProviderId.ToString(); break;
                case AuthError.WebInternalError: output = AuthError.WebInternalError.ToString(); break;
                case AuthError.WebStorateUnsupported: output = AuthError.WebStorateUnsupported.ToString(); break;
                case AuthError.TenantIdMismatch: output = AuthError.TenantIdMismatch.ToString(); break;
                case AuthError.UnsupportedTenantOperation: output = AuthError.UnsupportedTenantOperation.ToString(); break;
                case AuthError.InvalidLinkDomain: output = AuthError.InvalidLinkDomain.ToString(); break;
                case AuthError.RejectedCredential: output = AuthError.RejectedCredential.ToString(); break;
                case AuthError.PhoneNumberNotFound: output = AuthError.PhoneNumberNotFound.ToString(); break;
                case AuthError.InvalidTenantId: output = AuthError.InvalidTenantId.ToString(); break;
                case AuthError.MissingClientIdentifier: output = AuthError.MissingClientIdentifier.ToString(); break;
                case AuthError.MissingMultiFactorSession: output = AuthError.MissingMultiFactorSession.ToString(); break;
                case AuthError.MissingMultiFactorInfo: output = AuthError.MissingMultiFactorInfo.ToString(); break;
                case AuthError.MultiFactorInfoNotFound: output = AuthError.MultiFactorInfoNotFound.ToString(); break;
                case AuthError.AdminRestrictedOperation: output = AuthError.AdminRestrictedOperation.ToString(); break;
                case AuthError.UnverifiedEmail: output = AuthError.UnverifiedEmail.ToString(); break;
                case AuthError.SecondFactorAlreadyEnrolled: output = AuthError.SecondFactorAlreadyEnrolled.ToString(); break;
                case AuthError.MaximumSecondFactorCountExceeded: output = AuthError.MaximumSecondFactorCountExceeded.ToString(); break;
                case AuthError.UnsupportedFirstFactor: output = AuthError.UnsupportedFirstFactor.ToString(); break;
                case AuthError.EmailChangeNeedsVerification: output = AuthError.EmailChangeNeedsVerification.ToString(); break;
                default: break;
                    /*case AuthError.MissingEmail:
                        output = "Please Enter Your Email";
                        break;
                    case AuthError.MissingPassword:
                        output = "Please Enter Your Password";
                        break;
                    case AuthError.InvalidEmail:
                        output = "InvalidEmail";
                        break;
                    case AuthError.WrongPassword:
                        output = "Incorrect Password";
                        break;
                    case AuthError.UserNotFound:
                        output = "Account does not exist";
                        break;*/
            }
            launcher.Loading(false);
            loginOutputText.text = output;
        }
        else
        {
       
                FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(false);
                //    yield return new WaitForEndOfFrame();
               // StartCoroutine(Checking());
            
        }

    }

   

    private IEnumerator SendVerificationEmail()
    {
        if (user != null)
        {
            var emailTask = user.SendEmailVerificationAsync();
            manager.OpenWindow(7);
            manager.AwaitingScreenActive(1);
            manager.verifyEmailText.text = "Sending email, please wait!";
            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);
            if (emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown error,try again!";
                manager.verifyEmailText.text = "Unknown error,try again!";
                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Verification task was cancelled";
                        manager.verifyEmailText.text = "Verification task was cancelled";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        output = "Invalid email";
                        manager.verifyEmailText.text = "Invalid email";
                        break;
                    case AuthError.TooManyRequests:
                        output = "Too many requests";
                        manager.verifyEmailText.text = "Too many requests";
                        break;
                }
               // DataManager.instance.AwaitingVerification(false, user.Email, null);
            }
            else
            {
                manager.verifyEmailText.text = "Email sent, please submit!";
                Debug.Log("Email sent success");
                /*yield return new WaitUntil(()=>auth.CurrentUser.IsEmailVerified == true);
                manager.verifyEmailText.text = "Account verified!";
                FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(auth.CurrentUser.UserId).Child("Online").SetValueAsync(true);
                settings = FindObjectOfType<SettingsManager>();
                settings.online = true;
                manager.OpenWindow(3);
                manager.AwaitingScreenActive(1);
                // DataManager.instance.AwaitingVerification(true, user.Email, null);
                Debug.Log("User verified!");*/
            }
        }
    }

  /*  public void AwaitingVerification(bool _emailSent, string _email, string _output)
    {
        manager.OpenWindow(7);
        Debug.Log("AWAITING");
       
        if (_emailSent)
        {
    

        }
        else manager.verifyEmailText.text = $"Email Not Sent:{_output}\nPlease Verify {_email}";
    }*/

    private void RegisterPreparations ()
    {
        StartCoroutine(CheckName(registerUsername.textComponent,"0", registerOutputText,true));
    }

    private IEnumerator RegisterLogic(string _username, string _email,string _password, string _confirmPassword)
    {
      
        if (_username == "") registerOutputText.text = "Please enter a username"; 
        else if (_password != _confirmPassword) registerOutputText.text = "Passwords do not match!";
        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(_email,_password);

            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)registerTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown error,please try again";

                switch (error)
                {
                    case AuthError.InvalidEmail:
                        output = "Invalid Email";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        output = "Email Already In Use";
                        break;
                    case AuthError.WeakPassword:
                        output = "Weak password";
                        break;
                    case AuthError.MissingEmail:
                        output = "Please enter your email";
                        break;
                    case AuthError.MissingPassword:
                        output = "Please enter your password";
                        break;
                }
                registerOutputText.text = output;
                launcher.Loading(false);

            }
            else
            {
                UserProfile profile = new UserProfile
                {
                    DisplayName = _username,
                 
                PhotoUrl = new System.Uri("https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg"),
                };

                var defaultTask = user.UpdateUserProfileAsync(profile);

                yield return new WaitUntil(predicate: () => defaultTask.IsCompleted);
               
                    if (defaultTask.Exception != null)
                    {
                        user.DeleteAsync();
                        FirebaseException firebaseException = (FirebaseException)defaultTask.Exception.GetBaseException();
                        AuthError error = (AuthError)firebaseException.ErrorCode;
                        string output = "Unknown error,please try again";

                        switch (error)
                        {
                            case AuthError.Cancelled:
                                output = "Update user cancalled";
                                break;
                            case AuthError.SessionExpired:
                                output = "Session expired";
                                break;
                        }
                        registerOutputText.text = output;
                    launcher.Loading(false);
                }
                    else
                    {
                    launcher.Loading(false);
                    Debug.Log($"Firebase User Created Successfully: {user.DisplayName}({user.UserId})");
                        SaveNewPlayerData(_username, auth.CurrentUser.UserId,false);
                        StartCoroutine(SendVerificationEmail());
                    }
  
            }
        }
    }



    public void UpdateUserData ()
    {
        //      usernameProfile.text = auth.CurrentUser.DisplayName;
        // emailProfile.text = auth.CurrentUser.Email;

        //  passwordProfile.text = auth.CurrentUser.
    }

    public void NewUserName()
    {
        StartCoroutine(CheckName(usernameProfile.textComponent,auth.CurrentUser.UserId,profileOutputText,false));
    }
   
    public void ResetPassword()
    {
        auth.SendPasswordResetEmailAsync(ResetPasswordInput.text);
       // if (passwordProfile.text == confirmpasswordProfile.text) auth.CurrentUser.UpdatePasswordAsync(confirmpasswordProfile.text);
        Debug.Log("RESETED!!");
    }

    public void ResetEmail()
    {
       // auth.();
        // if (passwordProfile.text == confirmpasswordProfile.text) auth.CurrentUser.UpdatePasswordAsync(confirmpasswordProfile.text);
        Debug.Log("RESETED!!");
    }

    public void UpdateProfilePicture (string _newPfpURL)
    {
        StartCoroutine(UpdateProfilePictureLogic(_newPfpURL));
    }

    private IEnumerator UpdateProfilePictureLogic (string _newPfpURL)
    {
        if (user != null)
        {
            UserProfile profile = new UserProfile();

            try 
            {
                UserProfile _profile = new UserProfile
                {
                    PhotoUrl = new System.Uri(_newPfpURL),
                };
                _profile = profile;
            }
            catch
            {
                GetComponent<Launcher>().OnErrorWindow("Make sure your link is valid!");
                yield break;
            }

            var PfpTask = user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(predicate: () => PfpTask.IsCompleted);

            if (PfpTask.Exception != null)
            {
                Debug.LogError($"Updating Profile was unsuccessful: {PfpTask.Exception}");
            }
            else
            {
                GetComponent<LobbyManager>().LoadProfile();
                GetComponent<LobbyManager>().ChangePfpSucccess();
            }

                   

        }
    }

}
