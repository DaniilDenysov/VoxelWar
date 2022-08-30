using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lodoutchoice : MonoBehaviour
{

    public LodoutPart part;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetID (int ID)
    {
        part.Set(ID);
    }
}
