using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviourPunCallbacks
{
    public AudioMixer mixer;
    public AudioClip button_pressed;
    public Launcher launcher;
    [SerializeField] private Dropdown ScreenMode;
    private static GameObject settings_manager;
    public float Sound;
    [SerializeField] private Slider music_volume,master_volume, sounds_volume;
    Resolution[] resolution;
    public ServerSettings settings;
    [SerializeField] private Dropdown resolutionDropDown,graphDropDown, serverDropDown;
    int currentResolutuion = 0;
    public InputField userName;
    public GameObject Check;
    public bool online;
    public Resolution maxResolution;

    void Awake()
    {

            settings_manager = this.gameObject;
           
        UpdateResolutions();
    }

    public void PlaySound ()
    {
        GetComponent<AudioSource>().PlayOneShot(button_pressed);
    }

    public void IfOnlineValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != null && args.Snapshot != null && sender != null)
        {
            if (args.DatabaseError != null) return;
            if ((bool)args.Snapshot.Value == false) FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(true);
            else return;
        }
    }


    private void OnApplicationQuit()
    {
        
        FirebaseDatabase.DefaultInstance.RootReference.Child("user").Child(FirebaseAuth.DefaultInstance.CurrentUser.UserId).Child("Online").SetValueAsync(false);
        FirebaseDatabase.DefaultInstance.GoOffline();
        Debug.Log("Quited");
    }

    public void ScreenModeChange(int mode)
    {
        //0 - fullscreen
        //1 - windowed
        //2 - borderless window
        if (mode == 0)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            PlayerPrefs.SetInt("ScreenMode", mode);
            PlayerPrefs.Save();
        }
        else if (mode == 1)
        {
            Screen.SetResolution(resolution[currentResolutuion].width, resolution[currentResolutuion].height,Screen.fullScreenMode = FullScreenMode.Windowed);
            PlayerPrefs.SetInt("ScreenMode", mode);
            PlayerPrefs.Save();
        }
        else if (mode == 2)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            PlayerPrefs.SetInt("ScreenMode", mode);
            PlayerPrefs.Save();
        }
    }
    
    public void Quit ()
    {
        Application.Quit();
    }

    public void UpdateScreenMode ()
    {
        //0 - fullscreen
        //1 - windowed
        //2 - borderless window
        ScreenMode.value = PlayerPrefs.GetInt("ScreenMode");

    }

    public void UpdateSettings ()
    {
        resolutionDropDown.value = PlayerPrefs.GetInt("Screen");
       // graphDropDown.value = 
       // UpdateScreenMode();
    }

    public void UpdateResolutions()
    {

        if (PlayerPrefs.HasKey("GraphicsQuality")) graphDropDown.value = PlayerPrefs.GetInt("GraphicsQuality");
        else graphDropDown.value = 0;
      
        resolution = Screen.resolutions;
        resolutionDropDown.ClearOptions();

            List<string> options = new List<string>();

        for (int i = 0; i < resolution.Length; i++)
        {
            string option = resolution[i].width + " X " + resolution[i].height + " " + resolution[i].refreshRate + "Hz";
            Debug.Log(resolution[i].width + " X " + resolution[i].height + " index: " + i);
            options.Add(option);
            if (resolution[i].width.Equals(Screen.width) && resolution[i].height.Equals(Screen.height) && resolution[i].refreshRate.Equals(Screen.dpi)) 
            {
                currentResolutuion = i;
                //Screen.SetResolution(resolution[i].width, resolution[i].height,Screen.fullScreenMode);
                Debug.Log("Current resolution:" + Screen.currentResolution);
            }
        }
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutuion;
        resolutionDropDown.RefreshShownValue();
        if (PlayerPrefs.HasKey("ScreenMode")) ScreenMode.value = PlayerPrefs.GetInt("ScreenMode");
        else ScreenMode.value = 0;
    }

    private void Start()
    {
       // Screen.SetResolution(1920, 1080, Screen.fullScreen);

      //  UpdateResolutions();
        if (PlayerPrefs.HasKey("Master"))
        {
            // mixer.SetFloat("volume", PlayerPrefs.GetFloat("volume"));
            Debug.Log("Master:" + PlayerPrefs.GetFloat("Master"));
            master_volume.value = PlayerPrefs.GetFloat("Master");
            mixer.SetFloat("Master", Mathf.Log10(PlayerPrefs.GetFloat("Master")) * 20);
        }
        if (PlayerPrefs.HasKey("Music"))
        {
            // mixer.SetFloat("volume", PlayerPrefs.GetFloat("volume"));
           
            music_volume.value = PlayerPrefs.GetFloat("Music");
            mixer.SetFloat("Music", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
        }
        if (PlayerPrefs.HasKey("Sounds"))
        {
            // mixer.SetFloat("volume", PlayerPrefs.GetFloat("volume"));
            sounds_volume.value = PlayerPrefs.GetFloat("Sounds");
            mixer.SetFloat("Sounds", Mathf.Log10(PlayerPrefs.GetFloat("Sounds")) * 20);
        }
        /*  if (PlayerPrefs.HasKey("GraphicsQuality")) graphDropDown.value = PlayerPrefs.GetInt("GraphicsQuality");
          else graphDropDown.value = 0;
          if (PlayerPrefs.HasKey("ScreenMode")) ScreenMode.value = PlayerPrefs.GetInt("ScreenMode");
          else ScreenMode.value = 0;*/



        //ExclusiveFullScreen - полный экран без границ
        //FullScreenWindow - полное окно
        //MaximizedWindow - полное окно без границ
        //Screen.fullScreen = false;


        ///fullScreen.isOn = false;
        //  Screen.fullScreenMode = FullScreenMode.Windowed;

        /* if (PlayerPrefs.HasKey("Screen"))
         {
             if (PlayerPrefs.GetInt("Screen") == 1)
             {
                 Screen.fullScreen = true;
                 fullScreen.isOn = true;
             }
             else
             {

               Screen.fullScreen = false;
                 fullScreen.isOn = false;
               Screen.fullScreenMode = FullScreenMode.Windowed;
           }
         }
         else
         {
             Screen.fullScreen = true;
             fullScreen.isOn = true;
         }*/
        /*  if (PlayerPrefs.HasKey("Server"))
          {
              settings.DevRegion = serverDropDown.options[PlayerPrefs.GetInt("Server")].text;
              serverDropDown.value = PlayerPrefs.GetInt("Server");
          }*/
    }

    public void Clear ()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SetResolution (int index)
    {
        Debug.Log(resolution[index].width + " X " + resolution[index].height);
        Screen.SetResolution(resolution[index].width, resolution[index].height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution",index);
        PlayerPrefs.Save();
    }

    public void SetUpServerRegion (int region)
    {
      /*  settings.DevRegion = serverDropDown.options[region].text;
        PlayerPrefs.SetInt("Server",region);
        PlayerPrefs.Save();
        Debug.Log("server region:" + settings.DevRegion);*/
    }

    public void SetGraphicSettings (int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        PlayerPrefs.Save();
    }

    private void Update()
    {
      //  Debug.Log(Screen.width + "X" + Screen.height + " " + Screen.fullScreenMode);
    }

    public void SetFullScreen(bool FullScreen)
    {


        if (FullScreen)
        {
            Screen.fullScreen = FullScreen;
            PlayerPrefs.SetInt("Screen", 1);
        }
        else
        {
            //  Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.ExclusiveFullScreen);
           
            Screen.fullScreen = FullScreen;
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            PlayerPrefs.SetInt("Screen", 0);
        }
       PlayerPrefs.Save();
    }

    public void SetVolume (float volume)
    {
        // mixer.SetFloat("volume",volume);
        mixer.SetFloat("Master", Mathf.Log10 (volume) * 20);
        PlayerPrefs.SetFloat("Master", volume);
        PlayerPrefs.Save();
    }
    public void SetMusicVolume(float volume)
    {
        // mixer.SetFloat("volume",volume);
        Debug.Log("Music:" + PlayerPrefs.GetFloat("Music"));
        mixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Music", volume);
        PlayerPrefs.Save();
    }
    public void SetSoundsVolume(float volume)
    {
        // mixer.SetFloat("volume",volume);
        mixer.SetFloat("Sounds", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("Sounds", volume);
        PlayerPrefs.Save();
    }

}
