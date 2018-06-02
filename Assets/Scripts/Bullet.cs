using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public PlayerController controller;
    public PhotonView _owner;

    public bool IsDestroyed { get; set; }

    // Use this for initialization
    void Start()
    {
        _owner = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_owner.isMine && controller.playerMode == PlayerMode.Spectator)
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.isMine)
            return;

        if (other.tag == "Player")
        {
            var otherPlayer = other.gameObject.transform.parent.GetComponent<PlayerController>();
            if (otherPlayer != controller)
            {
                otherPlayer.pw.RPC("RPCKill", PhotonTargets.All);
            }
        }

        if (other.CompareTag("Mineral"))
        {
            if (!IsDestroyed)
            {
                var mineral = other.GetComponent<Mineral>();
                mineral.Owner.RPC("RPCDisable", PhotonTargets.All);
                IsDestroyed = true;
            }
        }

        PhotonNetwork.Destroy(_owner);
    }
}
