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
        _meshRenderer.UpdateGIMaterials();
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            _meshRenderer.UpdateGIMaterials();
            gameObject.SetActive(false);
            PhotonNetwork.Destroy(other.GetComponent<PhotonView>());
        }
    }

    void OnDestroy()
    {
        GameManager.Instance.Respawn -= OnRespawn;
    }
}
