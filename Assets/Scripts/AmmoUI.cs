using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    public Image image;

    private void Update()
    {
        if (GameManager.Instance.localPlayer)
            image.enabled = !GameManager.Instance.localPlayer.gun.IsOutOfAmmo;
    }
}
