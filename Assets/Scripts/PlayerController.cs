using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float shootForce;
    [SerializeField]
    private Transform camTrans;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private Transform model;

    private Rigidbody rb;
    private PhotonView pw;

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    private bool onGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pw = GetComponent<PhotonView>();

        Vector3 rot = camTrans.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    // Update is called once per frame
    void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        Vector3 newPos = transform.position + transform.forward * y * speed * Time.deltaTime;
        newPos += transform.right * x * speed * Time.deltaTime;
        rb.MovePosition(newPos);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        camTrans.rotation = localRotation;

        rb.MoveRotation(Quaternion.Euler(0, rotY, 0));

        if (pw.isMine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var go = PhotonNetwork.Instantiate("Bullet", camTrans.position + camTrans.forward, Quaternion.identity, 0);
                go.GetComponent<Rigidbody>().AddForce(camTrans.forward * shootForce);
            }

            if (onGround && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpForce);
            }

            if (Input.GetKey(KeyCode.LeftShift))
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
