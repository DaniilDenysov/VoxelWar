using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorChanger : MonoBehaviour
{

    public Text Ammo;
    [SerializeField] private Texture2D cursorArrow;
    [SerializeField] private Texture2D cursorInventory;
    Vector3 _scope_rotation;
    //[SerializeField] private Transform scope;
    [SerializeField] private LayerMask layers;

    void Start()
    {
       
        //Cursor.SetCursor(cursorArrow,Vector2.zero,CursorMode.ForceSoftware);
    }


    void Update()
    {
        
        /* Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layers))
         {
             //Debug.DrawLine(ray.origin, hit.point,Color.red);          
             //_scope_rotation = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
             transform.position = hit.point;
         }*/
    }
}
