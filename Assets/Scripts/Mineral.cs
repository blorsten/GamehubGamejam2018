using System;
using UnityEngine;

public class Mineral : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private PhotonView _owner;

    private void Start()
    {
        GameManager.Instance.Respawn += OnRespawn;
    }

    private void OnRespawn()
    {
        gameObject.SetActive(true);
        _meshRenderer.UpdateGIMaterials();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            gameObject.SetActive(false);
            _meshRenderer.UpdateGIMaterials();
            PhotonNetwork.Destroy(other.GetComponent<PhotonView>());
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Respawn -= OnRespawn;
    }
}
