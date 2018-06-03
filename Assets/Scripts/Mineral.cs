using System;
using UnityEngine;
using Photon;
using Random = UnityEngine.Random;

[SelectionBase]
public class Mineral : PunBehaviour, IPunObservable
{
    public bool _isBeingGathered;
    public InAudioEvent suck;
    public InAudioEvent breaksound;
    private float timer;

    private float _retractTimer;
    public bool _retract;
    private Vector3 _retractScale;
    private Vector3 _retractPos;
    [SerializeField] private AnimationCurve _retractCurve;

    public Vector3 _startPos;
    public Vector3 _startScale;
    private float _timer;
    private float _spawnDuration;
    private Collider _collider;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private PhotonView _owner;
    [SerializeField] private float _minSpawnDuration;
    [SerializeField] private float _maxSpawnDuration;
    [SerializeField] private AnimationCurve _animCurve;
    private bool _isAvailable = true;
    private bool _respawn;

    public PhotonView Owner
    {
        get { return _owner; }
    }

    public bool IsAvailable
    {
        get { return _isAvailable; }
        set { _isAvailable = value; }
    }

    public bool Respawn
    {
        get
        {
            return _respawn;
        }
        set
        {
            _respawn = value;

            if (_respawn)
                transform.position = _startPos;
        }
    }

    private void Start()
    {
        _startScale = transform.localScale;
        _startPos = transform.position;
        _collider = GetComponent<Collider>();
        GameManager.Instance.Respawn += OnRespawn;

        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
            _spawnDuration = Random.Range(_minSpawnDuration, _maxSpawnDuration);
    }

    void Update()
    {
        if (_isBeingGathered && timer <= 0)
        {
            InAudio.PostEvent(gameObject, suck);
            timer = .47f;
        }

        if (timer > 0)
            timer -= Time.deltaTime;

        if (!PhotonNetwork.isMasterClient)
            return;

        if (Respawn)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, _startScale, _animCurve.Evaluate(_timer));

            _timer += Time.deltaTime / _spawnDuration;

            if (_timer >= 1)
            {
                _timer = 0;
                _spawnDuration = Random.Range(_minSpawnDuration, _maxSpawnDuration);
                IsAvailable = true;
                Respawn = false;
            }
        }

        if (_retract)
        {
            _retractTimer += Time.deltaTime / .25f;
            var evaluated = _retractCurve.Evaluate(_retractTimer);

            transform.position = Vector3.LerpUnclamped(_retractPos, _startPos, evaluated);
            transform.localScale = Vector3.LerpUnclamped(_retractScale, _startScale, evaluated);

            if (_retractTimer >= 1)
            {
                _retract = false;
            }
        }
    }

    public void Toggle(bool toggler)
    {
        IsAvailable = false;

        if (!toggler)
            transform.localScale = Vector3.zero;

        //_collider.enabled = toggler;
        //_meshRenderer.enabled = toggler;
        //_meshRenderer.UpdateGIMaterials();
    }

    [PunRPC]
    public void RPCDisable(bool toggler)
    {
        InAudio.PostEvent(gameObject, breaksound);
        Toggle(toggler);
    }

    private void OnRespawn()
    {
        Toggle(true);
    }

    [PunRPC]
    public void RPCResetDimension()
    {
        _retractTimer = 0;
        _retract = true;
        _retractPos = transform.position;
        _retractScale = transform.localScale;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.Respawn -= OnRespawn;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(_isBeingGathered);
            stream.SendNext(_retract);
            stream.SendNext(IsAvailable);
        }
        else
        {
            _isBeingGathered = (bool)stream.ReceiveNext();
            _retract = (bool)stream.ReceiveNext();
            IsAvailable = (bool)stream.ReceiveNext();
        }
    }
}
