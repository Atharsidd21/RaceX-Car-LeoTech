using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI statusText;


    void Awake()
    {
        transform.SetParent(null); // ROOT
        DontDestroyOnLoad(gameObject);   
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        statusText.text = "Connecting to Photon...";
       
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
