using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class SpectatorEffects : MonoBehaviour
{
    public PostProcessingBehaviour post;

    void Update()
    {
        if (GameManager.Instance.localPlayer)
            post.enabled = GameManager.Instance.localPlayer.playerMode == PlayerMode.Spectator;
    }
}
