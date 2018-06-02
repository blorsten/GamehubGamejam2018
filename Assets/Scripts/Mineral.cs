using UnityEngine;

public class Mineral : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private PhotonView _owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            PhotonNetwork.Destroy(_owner);
            PhotonNetwork.Destroy(other.GetComponent<PhotonView>());
        }
    }

    void OnDestroy()
    {
        _meshRenderer.UpdateGIMaterials();
    }
}
