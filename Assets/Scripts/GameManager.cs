using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class GameManager : PUNSingleton<GameManager>
{
    public PhotonView Owner { get; set; }
    public List<PlayerController> Players { get; set; }
    public PlayerController localPlayer { get; set; }
    public SpawnPoint[] spawnPoints;

    public Action Respawn;

    // Use this for initialization
    void Start()
    {
        spawnPoints = FindObjectsOfType<SpawnPoint>();

        Owner = GetComponent<PhotonView>();

        if (PhotonNetwork.connected)
            PhotonNetwork.Instantiate("Player", new Vector3(0, -30f, 0), Quaternion.identity, 0);

        Players = new List<PlayerController>();
        UpdatePlayerList();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerList();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("Main");
        }
    }

    private void UpdatePlayerList()
    {
        if (PhotonNetwork.inRoom && Players.Count != PhotonNetwork.room.PlayerCount)
        {
            Players.Clear();
            var all = FindObjectsOfType<PlayerController>().ToList();

            if (all.Any())
                all.ForEach(x => Players.Add(x));
        }
    }

    public void RespawnPlayers()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        Shuffle(spawnPoints);

        for (var index = 0; index < Players.Count; index++)
        {
            PlayerController playerController = Players[index];
            playerController.pw.RPC("RPCRespawn", PhotonTargets.All, spawnPoints[index].transform.position);
        }
    }

    public void KillPlayer(PlayerController go)
    {
        if (go.playerMode == PlayerMode.Spectator)
            return;

        go.SetPlayerMode(PlayerMode.Spectator);

        int count = Players.Count(x => x.playerMode == PlayerMode.Normal);
        if (PhotonNetwork.room.PlayerCount > 1)
        {
            if (count <= 1)
            {
                Respawn.Invoke();
                if (spawnPoints.Length >= Players.Count)
                    RespawnPlayers();
            }
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Range(0, n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
