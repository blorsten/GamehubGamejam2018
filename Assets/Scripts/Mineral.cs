using System;
using UnityEngine;
using Photon;

public class Mineral : PunBehaviour
{
    private Collider _collider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private PhotonView _owner;

    public PhotonView Owner
    {
        get { return _owner; }
    }

    private void Start()
    {
        _collider = GetComponent<Collider>();
        GameManager.Instance.Respawn += OnRespawn;
    }

    public void Disable()
    {
        _collider.enabled = false;
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
        _collider.enabled = true;
        _meshRenderer.enabled = true;
        _meshRenderer.UpdateGIMaterials();
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.Respawn -= OnRespawn;
    }
}
