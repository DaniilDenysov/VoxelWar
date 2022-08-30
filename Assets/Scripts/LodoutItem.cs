using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LodoutItem : MonoBehaviour
{
    public int[] ID_List;
    public Image[] ImageList;
    public Sprite[] sprite;
    public bool Chosen;
    public SpawnPlayers spawn;
    public LodoutPart manager;

    private void Start()
    {
        for (int i = 0;i<4;i++)
        {
            ImageList[i].sprite = sprite[ID_List[i]];
        }
    }
    public void Choose()
    {
       Chosen = true;
        spawn.Spawn();       
    }
}

