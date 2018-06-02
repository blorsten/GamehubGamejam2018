using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuProxy : MonoBehaviour
{
    public InputField inputField;
    public Button StartButton;
    public Button SearchButton;

    private void Update()
    {
        SearchButton.gameObject.SetActive(!PhotonNetwork.connected && !PUNManager.Instance.SearchingForMatch);
        inputField.gameObject.SetActive(!PhotonNetwork.connected && !PUNManager.Instance.SearchingForMatch);
        StartButton.gameObject.SetActive(PhotonNetwork.inRoom);
    }

    public void SearchForMatch()
    {
        PUNManager.Instance.Connect(inputField.text);
    }

    public void StartMatchNow()
    {
        PUNManager.Instance.StartGame();
    }
}
