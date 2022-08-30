using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform container;
    [SerializeField] private GameObject Item, EndGameItem;
    public GameObject ScoreboardObject;
    PlayerControll control;
    Photon.Realtime.Player player;
    Hashtable _customProperties = new Hashtable();
    public GameObject[] item;
    public Dictionary<Player, ScoreboardListItem> scoreboardItems = new Dictionary<Player, ScoreboardListItem>();
    public ScoreboardListItem[] allChildren;

    private void Awake()
    {
        UpdateList();
    }

    public void UpdateListItems()
    {
        UpdateList();
    }

    public void EndList (Transform NewList)
    {
        allChildren = NewList.GetComponentsInChildren<ScoreboardListItem>();
        foreach (ScoreboardListItem child in allChildren)
        {
            Destroy(child.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
                ScoreboardListItem item = Instantiate(EndGameItem, NewList).GetComponent<ScoreboardListItem>();
                item.SetUp(player);
                scoreboardItems[player] = item;
        }
    }

    public void UpdateList()
    {
        allChildren = GetComponentsInChildren<ScoreboardListItem>();
        foreach (ScoreboardListItem child in allChildren)
        {
            Destroy(child.gameObject);
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            ScoreboardListItem item = Instantiate(Item, container).GetComponent<ScoreboardListItem>();
            item.SetUp(player);
            scoreboardItems[player] = item;
        }
        Debug.Log("Updated");
    }
    public void UpdateSetUp()
    {
        foreach (ScoreboardListItem child in allChildren)
        {

            //  child.SetUp();

        }
    }

    void Start()
    {
        //  UpdateList();
    }

    public void ScoreboardActive (bool active)
    {
       ScoreboardObject.SetActive(active);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
      if (control != null && !control.timer.GameStopped)  OnUpdateScoreboard(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (control != null && !control.timer.GameStopped) RemoveItemFromscoreBoard(otherPlayer);
    }

    public void RemoveItemFromscoreBoard(Player otherPlayer)
    {
        Destroy(scoreboardItems[otherPlayer].gameObject);
        scoreboardItems.Remove(otherPlayer);
    }
    [PunRPC]
    public void OnUpdateScoreboard(Player player)
    {

        ScoreboardListItem item = Instantiate(Item, container).GetComponent<ScoreboardListItem>();
        //        items = new GameObject[items.Length + 1];
        //      items[items.Length - 1] = item.gameObject;
        item.SetUp(player);
        scoreboardItems[player] = item;
    }
    public void DestroyAll()
    {

    }




    void Update()
    {

    }
}
