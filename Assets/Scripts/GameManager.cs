using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class GameManager : PUNSingleton<GameManager>
{
    private Mineral[] _minerals;
    private bool _finishedSetup;

    public PhotonView Owner { get; set; }
    public List<PlayerController> Players { get; set; }
    public PlayerController localPlayer { get; set; }
    private SpawnPoint[] spawnPoints;

    public Action Respawn;

    private void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        spawnPoints = FindObjectsOfType<SpawnPoint>().Where(point => point.gameObject.activeSelf).ToArray();
    }

    // Use this for initialization
    void Start()
    {
        _minerals = FindObjectsOfType<Mineral>();
        spawnPoints = FindObjectsOfType<SpawnPoint>().Where(point => point.gameObject.activeSelf).ToArray();

        Owner = GetComponent<PhotonView>();

        if (PhotonNetwork.connected)
            PhotonNetwork.Instantiate("Player", new Vector3(0, -30f, 0), Quaternion.identity, 0);

        Players = new List<PlayerController>();
        UpdatePlayerList();


    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.isMasterClient && _minerals.Count(mineral => mineral.IsAvailable) <= 2)
        {
            foreach (Mineral mineral in _minerals)
            {
                if (!mineral.IsAvailable)
                    mineral.Respawn = true;
            }
        }

        UpdatePlayerList();

        if (!_finishedSetup && PhotonNetwork.inRoom && Players.Count == PhotonNetwork.room.PlayerCount)
        {
            RespawnPlayers();
            _finishedSetup = true;
        }

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
            Transform transformPosition = spawnPoints[index].transform;
            playerController.pw.RPC("RPCRespawn", PhotonTargets.All, transformPosition.position, transformPosition.rotation.eulerAngles.y);
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
                var lastPlayer = Players.FirstOrDefault(x => x.playerMode == PlayerMode.Normal);
                ScoreBoard.Instance.AddScore(lastPlayer.pw.owner);
                Respawn.Invoke();
                if (spawnPoints.Length >= Players.Count)
                    RespawnPlayers();
            }
        }
    }

    private void OnDestroy()
    {

    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
