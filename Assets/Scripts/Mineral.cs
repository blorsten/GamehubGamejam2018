using System;
using UnityEngine;
using Photon;

public class Mineral : PunBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    public BoxCollider collider;
    [SerializeField] private PhotonView _owner;

    public PhotonView Owner
    {
        get { return _owner; }
    }

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

    [PunRPC]
    public void RPCDisable()
    {
        Disable();
    }

    private void OnRespawn()
    {
        collider.enabled = true;
        _meshRenderer.enabled = true;
        _meshRenderer.UpdateGIMaterials();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_owner.isMine)
            return;

        if (other.CompareTag("Projectile"))
        {
            _owner.RPC("RPCDisable", PhotonTargets.All);
            PhotonNetwork.Destroy(other.GetComponent<PhotonView>());
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.Respawn -= OnRespawn;
    }
}
