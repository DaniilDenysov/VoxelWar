using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Firebase.Database;
using Firebase.Auth;

public class DataInstance : MonoBehaviour
{

    private static GameObject instance;
    public bool online;
    public string Message;
    public bool Called = true;

    void Start()
    {
     

    }

    private void OnLevelWasLoaded(int level)
    {
        /*if (instance == null)
        {
            instance = gameObject;
            online = FindObjectOfType<DataInstance>().online;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/
        //  if (instance == null) GameObject.FindGameObjectWithTag("DataBase").GetComponent<DataManager>().inst = this;
    }

    private void Awake()
    {
       
    }

   

   /* IEnumerator ChangeOnlineStatus()
    {
        Debug.Log("Works!!!");
        FirebaseDatabase.DefaultInstance.GoOnline();
        yield return new WaitForSeconds(3);
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            var online = FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").GetValueAsync();
            yield return new WaitUntil(predicate: () => online.IsCompleted);

            if (online.Exception != null)
            {
                Debug.LogError("Error: " + online.Exception);

            }
            else
            {

                DataSnapshot snapshot = online.Result;

                Debug.Log("online status");

                Debug.Log(FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online") + "Result:" + (bool)snapshot.Value);

                if ((bool)snapshot.Value == false)
                {
                    FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(true);
                }
            }
        }
        else
        {
            FindObjectOfType<Launcher>().Loading(false);
            // loginOutputText.text = "Error, please try again";
        }
    }
    */
}
