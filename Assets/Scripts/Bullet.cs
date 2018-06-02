using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PhotonView _owner;

    // Use this for initialization
    void Start()
    {
        _owner = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.isMine)
            return;

        if (other.tag == "Player")
        {
            var otherPlayer = other.gameObject.transform.parent.GetComponent<PlayerController>();
            otherPlayer.pw.RPC("RPCKill", PhotonTargets.All);
            PhotonNetwork.Destroy(gameObject);
        }

        if (other.tag == "Ground")
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
