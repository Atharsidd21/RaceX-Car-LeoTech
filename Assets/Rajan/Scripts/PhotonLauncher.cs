using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        statusText.text = "Connecting to Photon...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected. Joining Lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Lobby Joined";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = "Disconnected: " + cause.ToString();
    }
}
