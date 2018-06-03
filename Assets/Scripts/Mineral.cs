using System;
using UnityEngine;
using Photon;
using Random = UnityEngine.Random;

public class Mineral : PunBehaviour, IPunObservable
{
    private Vector3 _startScale;
    private float _timer;
    private float _spawnDuration;
    private Collider _collider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private PhotonView _owner;
    [SerializeField] private float _minSpawnDuration;
    [SerializeField] private float _maxSpawnDuration;
    [SerializeField] private AnimationCurve _animCurve;

    public PhotonView Owner
    {
        get { return _owner; }
    }

    public bool IsAvailable { get; set; }

    private void Start()
    {
        IsAvailable = true;
        _startScale = transform.localScale;
        _collider = GetComponent<Collider>();
        GameManager.Instance.Respawn += OnRespawn;

        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            _spawnDuration = Random.Range(_minSpawnDuration, _maxSpawnDuration);
    }

    void Update()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (!IsAvailable)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, _startScale, _animCurve.Evaluate(_timer));
            
            _timer += Time.deltaTime / _spawnDuration;
            
            if (_timer >= 1)
            {
                _timer = 0;
                _spawnDuration = Random.Range(_minSpawnDuration, _maxSpawnDuration);
                IsAvailable = true;
            }
        }
    }

    public void Toggle(bool toggler)
    {
        IsAvailable = false;

        //_collider.enabled = toggler;
        //_meshRenderer.enabled = toggler;
        //_meshRenderer.UpdateGIMaterials();
    }

    [PunRPC]
    public void RPCDisable(bool toggler)
    {
        Toggle(toggler);
    }

    private void OnRespawn()
    {
        Toggle(true);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.Respawn -= OnRespawn;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(IsAvailable);
        }
        else
        {
            IsAvailable = (bool) stream.ReceiveNext();
        }
    }
}
