using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AcrylecSkeleton.Utilities;
using Photon;
using TMPro;
using System.Linq;
using System;

public class ScoreBoard : Singleton<ScoreBoard>
{
    public TextMeshProUGUI text;
    public GameObject panel;
    private Dictionary<PhotonPlayer, int> playerScores = new Dictionary<PhotonPlayer, int>();

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            panel.SetActive(true);
            string newText = "";

            foreach (var item in playerScores)
            {
                newText += item.Key.name + ": " + item.Value + Environment.NewLine;
            }

            text.text = newText;
        }
        else
            panel.SetActive(false);
    }

    public void AddScore(PhotonPlayer player)
    {
        if (playerScores.Keys.Contains(player))
            playerScores[player]++;
        else
        {
            playerScores.Add(player, 1);
        }
    }
}
