using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Granade : MonoBehaviourPunCallbacks
{
    [SerializeField] private ParticleSystem Explosion;
    [SerializeField] private float Radius,Force,Damage,Delay;
    [SerializeField] private AnimationCurve damageCurve,shakingCurve;
    public Player Owner;
    Transform firingPosition;
    float distance,CountDown;
    bool Exploded,StartCountDown;
    [SerializeField] private GameObject [] _particles;
    public AudioClip ExplosionSound;

    void Start()
    {
        CountDown = Delay;
        StartCountDown = true;
    }

    void Update()
    {
        if (StartCountDown)
        {
            CountDown -= Time.deltaTime;
            if (CountDown <= 0f && !Exploded)
            {
                Exploded = true;
                Explode();             
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
       PhotonNetwork.Instantiate(_particles[collision.gameObject.layer].name,transform.position,Quaternion.identity);
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 17) GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x / 2, GetComponent<Rigidbody>().velocity.y / 2, GetComponent<Rigidbody>().velocity.z / 2);
    }

    public void PlaySound ()
    {
       // GetComponent<AudioSource>().PlayOneShot(ExplosionSound);
    }

    public void Explode()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            //GetComponent<PhotonView>().RPC("PlaySound",RpcTarget.All);
            PhotonNetwork.Instantiate(Explosion.name, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);
            foreach (Collider nearbyObject in colliders)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(Force, transform.position, Radius);
                }                        
            }
            for (int i = 0;i<colliders.Length;i++)
            {
                if (colliders[i].GetComponent<HealthSystem>())
                {
                    HealthSystem hp = colliders[i].GetComponent<HealthSystem>();
                    distance = Vector3.Distance(colliders[i].transform.position, transform.position);
                    Damage = damageCurve.Evaluate(distance);
                    hp.TakeDamage(Damage);
                    if (hp.currHP <= 0)
                    {
                        FindObjectOfType<KillFedManager>().InstantPref(Owner.NickName.ToString(), hp.gameObject.GetComponent<PhotonView>().Owner.NickName.ToString(), 11);
                    }
                    else
                    {
                        PlayerControll player = colliders[i].GetComponent<PlayerControll>();
                        if (player != null)
                        {
                            player.weapon.StartCoroutine(player.weapon.Shaking(shakingCurve.Evaluate(distance)));
                        }
                    }
                }
            }
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
