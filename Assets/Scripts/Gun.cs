﻿using Photon;
using UnityEngine;

public class Gun : PunBehaviour, IPunObservable
{
    private Vector3? _gatherPosition;
    public bool IsOutOfAmmo;
    private float _gatheringTimer;

    private RaycastHit[] _targetRaycasts = new RaycastHit[1];
    private Vector3 _targetRayCastPoint;
    private Mineral _targetMineral;

    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private PlayerController _owner;
    [SerializeField] private Bullet __bullet;
    [SerializeField] private float _shootForce;
    [SerializeField] private float _reloadDistance;
    [SerializeField] private float _reloadDuration;
    [SerializeField] private float _reloadDistanceUntilBreak;

    void Start()
    {
        GameManager.Instance.Respawn += OnRespawn;
    }

    private void OnRespawn()
    {
        IsOutOfAmmo = false;
    }

    void Update()
    {
        if (_targetRayCastPoint != Vector3.zero)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, _gunBarrel.transform.position);
            _lineRenderer.SetPosition(1, _targetRayCastPoint);
        }
        else
            _lineRenderer.positionCount = 0;

        if (!_owner.pw.isMine || !PhotonNetwork.connected)
            return;

        //Shoot
        if (_owner.playerMode == PlayerMode.Normal && Input.GetMouseButtonDown(0) && !IsOutOfAmmo)
        {
            var HeadTrans = _owner.HeadTrans;
            var go = PhotonNetwork.Instantiate("Bullet", HeadTrans.position + HeadTrans.forward, Quaternion.identity, 0);
            go.GetComponent<Rigidbody>().AddForce(HeadTrans.forward * _shootForce);
            go.GetComponent<Bullet>().controller = _owner;

            IsOutOfAmmo = true;
        }

        //Reload
        if (Input.GetMouseButtonDown(1) && IsOutOfAmmo)
        {
            if (!_targetMineral)
            {
                var hits = Physics.RaycastNonAlloc(
                    _owner.HeadTrans.transform.position,
                    _owner.HeadTrans.forward,
                    _targetRaycasts,
                    _reloadDistance, LayerMask.GetMask("Mineral"));

                if (hits == 1)
                {
                    var raycastHit = _targetRaycasts[0];
                    _targetRayCastPoint = raycastHit.point;
                    _targetMineral = raycastHit.transform.GetComponent<Mineral>();
                    _gatherPosition = _owner.transform.position;
                }
            }
        }

        if (Input.GetMouseButton(1) && _targetMineral)
        {
            //We're gathering!
            _gatheringTimer += Time.deltaTime / _reloadDuration;

            //Did we finish gathering?
            if (_gatheringTimer >= 1)
                FinishGathering(true);

            //Did we break by moving?
            if (_gatherPosition.HasValue)
            {
                float dist = Vector3.Distance(_gatherPosition.Value, _owner.transform.position);

                if (dist > _reloadDistanceUntilBreak)
                    FinishGathering(false);
            }
        }
        else if (_targetMineral)
        {
            //We interrupted the gathering process.
            FinishGathering(false);
        }
    }

    private void FinishGathering(bool success)
    {
        if (success)
            _targetMineral.Owner.RPC("RPCDisable", PhotonTargets.All);

        IsOutOfAmmo = !success;
        _targetMineral = null;
        _gatheringTimer = 0;
        _gatherPosition = null;
        _targetRayCastPoint = Vector3.zero;
        _lineRenderer.positionCount = 0;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(_targetRayCastPoint);
        }
        else
        {
            _targetRayCastPoint = (Vector3)stream.ReceiveNext();
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.Respawn += OnRespawn;
    }
}
