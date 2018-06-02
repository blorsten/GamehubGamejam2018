using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuProxy : MonoBehaviour
{
    public InputField inputField;
    public GameObject UiOffline;
    public GameObject UiConnecting;
    public Text statusMessage;
    public Button startNowButton;

    private void Update()
    {
        bool offline = !PhotonNetwork.connected && !PUNManager.Instance.SearchingForMatch;
        UiOffline.SetActive(offline);

        bool connecting = PhotonNetwork.inRoom;
        UiConnecting.SetActive(connecting);
        if (connecting)
        {
            startNowButton.gameObject.SetActive(PhotonNetwork.isMasterClient);

            string text = "Looking for players... (" + PhotonNetwork.room.PlayerCount + "/" + PhotonNetwork.room.MaxPlayers + ")";
            statusMessage.text = text;
        }
    }

    public void SearchForMatch()
    {
        PUNManager.Instance.Connect(inputField.text);
    }

    public void StartMatchNow()
    {
        PUNManager.Instance.StartGame();
    }

    public void CancelSearch()
    {
        PUNManager.Instance.CancelSearch();
    }
}
