using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMode { Normal, Spectator }

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public float crouchSpeed = 3;
    public Transform HeadTrans;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Transform playerModel;
    public Transform GunModel;

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

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PhotonNetwork.connected)
            Destroy(gameObject);

        if (pw.isMine)
        {
            Instantiate(playerCam, HeadTrans);
            Vector3 rot = HeadTrans.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
            GameManager.Instance.localPlayer = this;
        }
    }

    public void SetPlayerMode(PlayerMode mode)
    {
        playerMode = mode;

        Debug.Log("Setting player mode to: " + mode);

        switch (playerMode)
        {
            case PlayerMode.Normal:
                playerModel.GetComponent<MeshRenderer>().enabled = true;
                GunModel.GetComponent<MeshRenderer>().enabled = true;
                gameObject.layer = LayerMask.NameToLayer("Player");
                playerModel.gameObject.layer = LayerMask.NameToLayer("Player");
                break;
            case PlayerMode.Spectator:
                playerModel.GetComponent<MeshRenderer>().enabled = false;
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
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            if (Input.GetKey(KeyCode.LeftControl))
            {
                _isCrouching = true;
                playerModel.localScale = new Vector3(1, .5f, 1);
                playerModel.localPosition = new Vector3(0, .5f, 0);
                HeadTrans.localPosition = new Vector3(0, .75f, 0);
            }
            else
            {
                _isCrouching = false;
                playerModel.localScale = new Vector3(1, 1, 1);
                playerModel.localPosition = new Vector3(0, 1, 0);
                HeadTrans.localPosition = new Vector3(0, 1.5f, 0);
            }

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
        }
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
}
