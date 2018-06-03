using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMode { Normal, Spectator }

public class PlayerController : MonoBehaviour, IPunObservable
{
    public InAudioEvent crouchSound;
    public InAudioEvent walkSound;

    [SerializeField]
    private float speed;
    public float crouchSpeed = 3;
    public Transform HeadTrans;
    public GameObject lips;
    public Gun gun;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Transform playerModel;
    private Vector3 headTarget;
    public Transform collider;
    public Transform GunModel;
    public Animator animator;

    public GameObject playerCam;

    private Rigidbody rb;
    public PhotonView pw;
    public PlayerMode playerMode = PlayerMode.Normal;
    private bool onGround;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    private bool _isCrouching;
    private bool _isWalking;
    private bool _lastIsCrouching;
    private bool _lastIsWalking;
    private float walkrimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {
        headTarget = HeadTrans.localPosition;

        if (!PhotonNetwork.connected)
            Destroy(gameObject);

        if (pw.isMine)
        {
            Instantiate(playerCam, HeadTrans);
            Vector3 rot = HeadTrans.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
            GameManager.Instance.localPlayer = this;

            foreach (Transform t in playerModel.transform)
            {
                Destroy(t.gameObject);
            }

        }
    }

    public void SetPlayerMode(PlayerMode mode)
    {
        playerMode = mode;

        Debug.Log("Setting player mode to: " + mode);

        switch (playerMode)
        {
            case PlayerMode.Normal:
                foreach (Transform t in playerModel.transform)
                {
                    t.GetComponent<MeshRenderer>().enabled = true;
                }
                GunModel.GetComponent<MeshRenderer>().enabled = true;
                gameObject.layer = LayerMask.NameToLayer("Player");
                playerModel.gameObject.layer = LayerMask.NameToLayer("Player");
                break;
            case PlayerMode.Spectator:

                foreach (Transform t in playerModel.transform)
                {
                    t.GetComponent<MeshRenderer>().enabled = false;
                }
                GunModel.GetComponent<MeshRenderer>().enabled = false;
                playerModel.gameObject.layer = LayerMask.NameToLayer("Spectator");
                gameObject.layer = LayerMask.NameToLayer("Spectator");
                break;
        }
    }

    [PunRPC]
    public void RPCRespawn(Vector3 spawnPos, float rotationY)
    {
        SetPlayerMode(PlayerMode.Normal);
        transform.position = spawnPos;
        rotY = rotationY;
        rb.velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (pw.isMine)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                headTarget = new Vector3(0, 1.75f / 2, 0);
                collider.localScale = new Vector3(1, .5f, 1);
                _isCrouching = true;
            }
            else
            {
                headTarget = new Vector3(0, 1.75f, 0);
                collider.localScale = new Vector3(1, 1f, 1);
                _isCrouching = false;
            }

            HeadTrans.localPosition = Vector3.Lerp(HeadTrans.localPosition, headTarget, .1f);

            if (onGround && Input.GetKeyDown(KeyCode.Space))
                rb.AddForce(transform.up * jumpForce);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            HeadTrans.rotation = localRotation;

            rb.MoveRotation(Quaternion.Euler(0, rotY, 0));

            animator.SetBool("Walking", _isWalking);
            animator.SetBool("Crouching", _isCrouching);
        }

        if (_isWalking && walkrimer <= 0)
        {
            InAudio.PostEvent(gameObject, walkSound);
            walkrimer = .9f;
        }

        if (walkrimer > 0)
            walkrimer -= Time.deltaTime;

        _lastIsCrouching = _isCrouching;
        _lastIsWalking = _isWalking;
    }

    void FixedUpdate()
    {
        if (pw.isMine)
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            Vector3 newPos = transform.forward * y;
            newPos += transform.right * x;

            if (onGround && Input.GetKeyDown(KeyCode.Space))
                rb.AddForce(transform.up * jumpForce);

            if (x != 0 || y != 0)
                _isWalking = true;
            else
                _isWalking = false;

            rb.MovePosition(transform.position + newPos * (_isCrouching ? crouchSpeed : speed) * Time.deltaTime);
        }
    }

    [PunRPC]
    public void RPCKill()
    {
        GameManager.Instance.KillPlayer(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            onGround = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            onGround = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(_isWalking);
            stream.SendNext(_isCrouching);
        }
        else
        {
            _isWalking = (bool)stream.ReceiveNext();
            _lastIsCrouching = (bool)stream.ReceiveNext();

            animator.SetBool("Walking", _isWalking);
            animator.SetBool("Crouching", _lastIsCrouching);
        }
    }
}
