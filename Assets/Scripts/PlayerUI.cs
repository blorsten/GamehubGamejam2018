using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerController controller;
    public TextMeshPro text;

    // Use this for initialization
    void Start()
    {
        if (controller.pw.isMine)
            Destroy(gameObject);
        text.text = controller.pw.owner.NickName;
    }

    private void Update()
    {
        var camPos = GameManager.Instance.localPlayer.transform.position;
        Vector3 target = new Vector3(camPos.x, transform.position.y, camPos.z);
        transform.LookAt(target);
    }
}
