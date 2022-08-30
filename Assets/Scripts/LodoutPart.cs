using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LodoutPart : MonoBehaviour
{
    public LodoutItem item;
    public Image img;
    public Sprite[] sprite;
    public int MyCount;
    public int DefaultID;
  
    public GameObject[] Lists;
    public int[] MyList;
    public string[] Description;
    public Text _description;
    [SerializeField] private bool isRandom;
    public bool check;


    void Start()
    {
        if (!isRandom)
        {
            img.sprite = sprite[DefaultID];
            if (_description != null) _description.text = Description[DefaultID];
            item.ID_List[MyCount] = MyList[DefaultID];

        }
        else
        {
            int Rand = Random.Range(0, MyList.Length - 1);
            Debug.Log("Rand:" + Rand);
            img.sprite = sprite[MyList[Rand]];
            if (_description != null) _description.text = Description[Rand];
            item.ID_List[MyCount] = MyList[Rand];
            if (check)
            {
                for (int m = 0; m < 4; m++)
                {
                    if (item.ID_List[m] == MyList[Rand] && m != MyCount)
                    {
                        Debug.Log("CHANGE!!!");
                        return;
                    }

                }
            }
        }
         
    }


    public void OnChange(int dir)   
    {
        Debug.Log("Swaped");
        int Chosen = 0;
     for (int i = 0;i<sprite.Length;i++)
     {
            if (sprite[i] == img.sprite)
            {
                if (i + dir < sprite.Length && i + dir >= 0)
                {
                    if (sprite[sprite.Length - 1] != null)
                    {
                        img.sprite = sprite[i + dir];
                        if (_description != null) _description.text = Description[i + dir];
                        item.ID_List[MyCount] = MyList[i + dir];
                        Chosen = i + dir;
   
                    }
                }else if (i + dir > sprite.Length - 1)
                {
                    if (sprite[sprite.Length - 1] != null)
                    {
                        img.sprite = sprite[0];
                        if (_description != null) _description.text = Description[0];
                        item.ID_List[MyCount] = MyList[0];
                        Chosen = 0;
    
                    }
                }
                else if (i + dir < 0)
                {
                    if (sprite[sprite.Length - 1] != null)
                    {
                        img.sprite = sprite[sprite.Length - 1];
                        if (_description != null) _description.text = Description[sprite.Length - 1];
                        item.ID_List[MyCount] = MyList[sprite.Length - 1];
                        Chosen = sprite.Length - 1;
                        
                    }
                }
                if (check)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        if (item.ID_List[m] == MyList[Chosen] && m != MyCount)
                        {
                            OnChange(dir);
                            Debug.Log("CHANGE!!!");
                            break;
                        }

                    }
                }
                break;
            }
     }   
    }
    
    public void OpenList (int List)
    {
        for (int i = 0;i<Lists.Length;i++)
        {
            if (i == List) Lists[i].SetActive(true);
            else Lists[i].SetActive(false);
        }
    }

    public void Set (int ID)
    {
        img.sprite = sprite[ID];
        item.ID_List[MyCount] = MyList[ID];
    }
}
