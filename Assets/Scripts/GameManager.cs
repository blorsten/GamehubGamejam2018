using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : PUNSingleton<GameManager>
{
    public PhotonView Owner { get; set; }
    public List<PlayerController> Players { get; set; }
    public List<SpawnPoint> spawnPoints;

    public Action Respawn;

    // Use this for initialization
    void Start()
    {
        Owner = GetComponent<PhotonView>();

        if (PhotonNetwork.connected)
            PhotonNetwork.Instantiate("Player", new Vector3(0, 0.6f, 0), Quaternion.identity, 0);

        Players = new List<PlayerController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.inRoom && Players.Count != PhotonNetwork.room.PlayerCount)
        {
            Players.Clear();
            var all = FindObjectsOfType<PlayerController>().ToList();

            if (!all.Any())
                return;

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

        int count = Players.Count(x => x.playerMode == PlayerMode.Normal);
        if (PhotonNetwork.room.PlayerCount > 1)
        {
            if (count <= 1)
            {
                Respawn.Invoke();
                if (spawnPoints.Count >= Players.Count)
                    foreach (var player in Players)
                    {
                        player.Respawn();
                        player.SetPlayerMode(PlayerMode.Normal);
                    }
            }
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
