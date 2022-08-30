using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageDisplayController : MonoBehaviour
{

    [SerializeField] private Text _Display;
    public float DamageDealt;
    [SerializeField] private float Speed, Lifetime;

    void Start()
    {
        _Display.text = "" + DamageDealt;
        Destroy(this.gameObject,Lifetime);
    }

    void FixedUpdate()
    {
        transform.Translate(transform.up * Speed * Time.deltaTime);
    }
}
