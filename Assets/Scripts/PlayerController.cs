using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMode { Normal, Spectator }

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    public Transform camTrans;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Transform model;

    public GameObject playerCam;

    private Rigidbody rb;
    public PhotonView pw;
    public PlayerMode playerMode = PlayerMode.Normal;
    private bool onGround;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    private void Start()
    {
        if (!PhotonNetwork.connected)
            Destroy(gameObject);

        rb = GetComponent<Rigidbody>();
        pw = GetComponent<PhotonView>();

        if (pw.isMine)
        {
            camTrans = Instantiate(playerCam, transform).transform;
            Vector3 rot = camTrans.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;
        }

        Respawn();
        SetPlayerMode(playerMode);
    }

    public void SetPlayerMode(PlayerMode mode)
    {
        playerMode = mode;

        Debug.Log("Setting player mode to: " + mode);

        switch (playerMode)
        {
            case PlayerMode.Normal:
                model.GetComponent<MeshRenderer>().enabled = true;
                gameObject.layer = LayerMask.NameToLayer("Player");
                break;
            case PlayerMode.Spectator:
                model.GetComponent<MeshRenderer>().enabled = false;
                gameObject.layer = LayerMask.NameToLayer("Spectator");
                break;
        }
    }

    public void Respawn()
    {
        if (!pw.isMine)
            return;

        transform.position = GameManager.Instance.spawnPoints[pw.ownerId - 1].transform.position;
        transform.LookAt(new Vector3(0, camTrans.position.y, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (pw.isMine)
        {
            var x = Input.GetAxis("Horizontal");
            var y = Input.GetAxis("Vertical");

            Vector3 newPos = transform.position + transform.forward * y * speed * Time.deltaTime;
            newPos += transform.right * x * speed * Time.deltaTime;

            if (onGround && Input.GetKeyDown(KeyCode.Space))
                rb.AddForce(transform.up * jumpForce);

            if (Input.GetKey(KeyCode.LeftControl))
            {
                model.localScale = new Vector3(1, .5f, 1);
                model.localPosition = new Vector3(0, .5f, 0);
                camTrans.localPosition = new Vector3(0, .75f, 0);
            }
            else
            {
                model.localScale = new Vector3(1, 1, 1);
                model.localPosition = new Vector3(0, 1, 0);
                camTrans.localPosition = new Vector3(0, 1.5f, 0);
            }

            rb.MovePosition(newPos);

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            rotY += mouseX * mouseSensitivity * Time.deltaTime;
            rotX += mouseY * mouseSensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            camTrans.rotation = localRotation;

            rb.MoveRotation(Quaternion.Euler(0, rotY, 0));

        }
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
