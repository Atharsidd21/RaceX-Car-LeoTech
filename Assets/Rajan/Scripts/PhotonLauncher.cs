using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;

    void Awake()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnected)
        {
            statusText.text = "Connecting to Photon...";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            // Already connected case (BACK - MULTIPLAYER again)
            statusText.text = "Already connected. Joining Lobby...";
            PhotonNetwork.JoinLobby();
        }
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
