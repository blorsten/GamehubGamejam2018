using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            var otherPlayer = other.gameObject.transform.parent.GetComponent<PlayerController>();
            GameManager.Instance.KillPlayer(otherPlayer);
        }

        if (other.tag == "Ground")
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
