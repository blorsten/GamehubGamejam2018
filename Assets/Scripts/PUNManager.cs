using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;

public class PUNManager : PUNSingleton<PUNManager>
{
    public string Version = "1";
    public bool SearchingForMatch;
    public int playerCount;

    private void Awake()
    {
        PhotonNetwork.sendRate = 40;
        PhotonNetwork.sendRateOnSerialize = 40;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = true;
    }

    public void Connect(string name)
    {
        if (name == "")
            name = DateTime.Now.Millisecond.ToString();
        PhotonNetwork.player.NickName = name.Substring(0, Math.Min(name.Length, 15));
        PhotonNetwork.ConnectUsingSettings(Version);
        SearchingForMatch = true;
    }

    public void CancelSearch()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.Disconnect();
            SearchingForMatch = false;
        }
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        base.OnPhotonRandomJoinFailed(codeAndMsg);
        PhotonNetwork.CreateRoom("", new RoomOptions() { MaxPlayers = 4 }, TypedLobby.Default);
    }

    private void Update()
    {
        if (PhotonNetwork.inRoom)
        {
            bool start;
            if (PhotonNetwork.room.TryGetRoomProperty<bool>("START", out start) && start)
            {
                PhotonNetwork.LoadLevel("Map2");
            }

            if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
                StartGame();
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
        {
            Hashtable ht = new Hashtable();
            ht.Add("START", true);
            PhotonNetwork.room.SetCustomProperties(ht);
            PhotonNetwork.room.IsVisible = false;
            PhotonNetwork.room.IsOpen = false;
        }
    }
}
