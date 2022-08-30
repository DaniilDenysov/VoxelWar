using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Flashing : MonoBehaviour
{
    public PostProcessVolume volume;
    Bloom bloom;
    bool down = true;

    void Start()
    {
        volume.profile.TryGetSettings(out bloom);
        Invoke("Up", 0.1f);
    }

 
    void Update()
    {





    
    }
    void Up ()
    {
        while (bloom.intensity < 35)
        { 
            bloom.intensity.value += 0.01f;
            Debug.Log("Up" + bloom.intensity.value);

        }
        if (bloom.intensity >= 35) Invoke("Down",0.1f);
    }
    void Down ()
    {
        while (bloom.intensity > 30)
        {
            bloom.intensity.value -= 0.01f;
            Debug.Log("Down" + bloom.intensity.value);

        }   
        if (bloom.intensity <= 30) Invoke("Up", 0.1f);
    }
}