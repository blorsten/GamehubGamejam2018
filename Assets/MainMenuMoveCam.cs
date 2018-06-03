using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMoveCam : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Vector3.zero);
        transform.Translate(Vector3.right * Time.deltaTime);
    }
}
