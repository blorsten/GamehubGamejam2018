using System;
using UnityEngine;
using Photon;

public class Mineral : PunBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    public BoxCollider collider;
    [SerializeField] private PhotonView _owner;

    private void Start()
    {
        GameManager.Instance.Respawn += OnRespawn;
    }

    public void Disable()
    {
        collider.enabled = false;
        _meshRenderer.enabled = false;
        _meshRenderer.UpdateGIMaterials();
    }

    private void OnRespawn()
    {
        collider.enabled = true;
        _meshRenderer.enabled = true;
        _meshRenderer.UpdateGIMaterials();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Disable();
            PhotonNetwork.Destroy(other.GetComponent<PhotonView>());
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Respawn -= OnRespawn;
    }
}
