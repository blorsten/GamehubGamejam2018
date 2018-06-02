﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using System.Linq;

public class GameManager : PUNSingleton<GameManager>
{
    public static GameManager Instance { get; set; }
    public List<PlayerController> Players { get; set; }
    public List<SpawnPoint> spawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.connected)
            PhotonNetwork.Instantiate("Player", new Vector3(0, 0.6f, 0), Quaternion.identity, 0);

        Players = new List<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Players.Count != PhotonNetwork.room.PlayerCount)
        {
            Players.Clear();
            var all = FindObjectsOfType<PlayerController>().ToList();
            all.ForEach(x => Players.Add(x));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("Main");
        }
    }

    public void KillPlayer(PlayerController go)
    {
        if (go.playerMode == PlayerMode.Spectator)
            return;

        go.SetPlayerMode(PlayerMode.Spectator);

        int count = Players.Where(x => x.playerMode == PlayerMode.Normal).Count();

        if (count <= 1)
        {
            if (spawnPoints.Count >= Players.Count)
                foreach (var player in Players)
                {
                    player.Respawn();
                    player.SetPlayerMode(PlayerMode.Normal);
                }
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
