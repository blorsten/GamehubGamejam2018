using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class GameManager : PunBehaviour
{
    public static GameManager Instance { get; set; }
    public List<PlayerController> Players { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        if (PhotonNetwork.connected)
            PhotonNetwork.Instantiate("Player", new Vector3(0, 0.6f, 0), Quaternion.identity, 0);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("Main");
        }
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
