using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private Transform[] positions;
    [SerializeField] private Text Name;
    [SerializeField] private GameObject EnemyUI;
    [SerializeField] private Image barFast, barSlow;
    private PlayerControll player;
    public HealthSystem HP;
    public float Damage, LimbsLifetime, nextTimeToAtack,Rate, Distance; //maxHP = 100,currHP,currHPSlow;
    //public Slider HP,AP;
    public Transform actualPosition,nowPosition;
    NavMeshAgent agent;
    [SerializeField] private Transform LookPoint;
    [SerializeField] private Transform[] LimbsPosition;
    [SerializeField] private GameObject[] LimbsPrefs;
    [SerializeField] private int LevelOfAgression; //0 - none,1 - atack,3 - chasing
    public bool inCombat,isDying;
    Vector3 home;
    [SerializeField] private LayerMask layers;

    void Start()
    {
        home = transform.position;
      //  currHP = maxHP;
       // currHPSlow = maxHP;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerControll>();
       // actualPosition = positions[0];
        //agent.SetDestination(actualPosition.position);
    }
    public void FollowPlayer ()
    {
        if (player)
           if (!isDying) agent.SetDestination(player.gameObject.transform.position);
            else if (!isDying) agent.SetDestination(home);
    }
    
    public void FindPlayer ()
    {

    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(LookPoint.transform.position,transform.forward,out hit, Distance,layers))
        {
            if (hit.collider.GetComponent<PlayerControll>() && hit.collider.GetComponent<HealthSystem>().currHP > 0 && Time.time >= nextTimeToAtack)
            {
                hit.collider.GetComponent<HealthSystem>().DoDamage(Damage);
                nextTimeToAtack = Time.time + 1f / Rate;
            }
        }
       /* if (currHPSlow != currHP)
        {
            currHPSlow = Mathf.Lerp(currHPSlow,currHP,t);
            t += 0.05f * Time.deltaTime;
        }
        barFast.fillAmount = currHP / maxHP;
        barSlow.fillAmount = currHPSlow / maxHP;*/
        FollowPlayer();

        //Debug.Log(nowPosition.position);
        /*if (inCombat == false && HP.value > 50)
        {
            if (actualPosition.position.x == nowPosition.position.x && actualPosition.position.z == nowPosition.position.z) //проверка на совпадение координат
            {
               // Debug.Log("Reached");
                for (int i = 0; i <= positions.Length - 1; i++) //запуск перебора координат, чтобы понять сле
                {
                    if (positions[i].position.x == actualPosition.position.x && positions[i].position.z == actualPosition.position.z)
                    {
                        if (i + 1 <= positions.Length - 1) //проверка след точки на существование
                        {
                           // Debug.Log("i:" + i);
                            actualPosition = positions[i + 1];
                            agent.SetDestination(actualPosition.position);

                        }
                        else if (i + 1 > positions.Length - 1)  //если её нету, то возврат в начальную точку
                        {
                           // Debug.Log("i" + i);
                            actualPosition = positions[0];
                            agent.SetDestination(actualPosition.position);

                        }
                        break;
                    }

                }
            }
        }
        else if (HP.value > 0 && HP.value <= 50) //chasing
        {
            agent.SetDestination(player.gameObject.transform.position);
            inCombat = true;
        }
        else if (HP.value == 0) 
        {
            onDeath();
        }*/

    }
    public void OnHealthDecrease()
    {
    }
   
    public void onDeath ()
    {
        isDying = true;
        agent.Stop();
        for (int i = 0; i < LimbsPrefs.Length; i++)
        {
           GameObject limb = Instantiate(LimbsPrefs[i], LimbsPosition[i].transform.position, LimbsPosition[i].transform.rotation);
            Destroy(limb,LimbsLifetime);
         //   limb.GetComponent<Rigidbody>().v
        }
        Destroy(this.gameObject);
    }
    public void Atack ()
    {
        if (HP.currHP > 0)
        {
            player.gameObject.GetComponent<HealthSystem>().DoDamage(Damage);
            /*  player.HP.value -= Damage * LevelOfAgression;
              player.AP.value += Damage * 2;
              AP.value -=  Damage * LevelOfAgression;
              Debug.Log("Agr:" + LevelOfAgression);*/
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            agent.isStopped = true;
        }
            /*  if (other.gameObject.tag == "Player" && LevelOfAgression != 2)
              {
                  LevelOfAgression = 1;
                  player = other.gameObject.GetComponent<PlayerControll>();
                  EnemyUI.SetActive(true);
                  inCombat = true;           
                  agent.isStopped = true;
              }else if (other.gameObject.tag == "Player" && LevelOfAgression == 2)
              {
                  player.AP.value += player.Damage * 3;
                  Atack();
                  EnemyUI.SetActive(true);
                  inCombat = true;
                  agent.isStopped = true;
              }*/
        }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Atack();
        }
        /*      if (other.gameObject.tag == "Player")
              {
                  inCombat = true;
                  transform.LookAt(other.transform);
                  agent.isStopped = true;
              }*/
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            agent.isStopped = false;
        }
        /*if (other.gameObject.tag == "Player")
        {
            if (LevelOfAgression != 2) EnemyUI.SetActive(false);
            inCombat = false;
            agent.isStopped = false;
            agent.SetDestination(actualPosition.position);
        }*/
    }
}
