using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    public static bool IsConnecting = false;

    public void TryConnect()
    {
        // FIXED CONDITION
        if (PhotonNetwork.IsConnected || IsConnecting)
        {
            Debug.Log("[Photon] Already connected / connecting");
            return;
        }

        Debug.Log("[Photon] Connecting...");
        IsConnecting = true;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[Photon] Connected to Master");
        IsConnecting = false;

        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("[Photon] Disconnected: " + cause);
        IsConnecting = false;
    }
}
