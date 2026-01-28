using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public TMP_InputField roomInput;
    public TextMeshProUGUI statusText;

    [Header("Buttons")]
    public GameObject createButton;
    public GameObject joinButton;

    private bool isReadyForMatchmaking = false;

    void Start()
    {
        statusText.text = "Connecting...";

        createButton.SetActive(false);
        joinButton.SetActive(false);

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // ===================== CONNECTION FLOW =====================

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        isReadyForMatchmaking = true;

        statusText.text = "Ready";

        createButton.SetActive(true);
        joinButton.SetActive(true);
    }

    // ===================== CREATE ROOM =====================

    public void CreateRoom()
    {
        if (!isReadyForMatchmaking)
        {
            statusText.text = "Please wait...";
            return;
        }

        string roomCode = Random.Range(1000, 9999).ToString();

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = false,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomCode, options);
        statusText.text = "Creating Room : " + roomCode;
    }

    // ===================== JOIN ROOM =====================

    public void JoinRoom()
    {
        if (!isReadyForMatchmaking)
        {
            statusText.text = "Please wait...";
            return;
        }

        if (string.IsNullOrEmpty(roomInput.text))
        {
            statusText.text = "Enter room code";
            return;
        }

        PhotonNetwork.JoinRoom(roomInput.text.Trim());
        statusText.text = "Joining Room...";
    }

    // ===================== ROOM CALLBACKS =====================

    public override void OnJoinedRoom()
    {
        statusText.text =
            "Joined Room : " +
            PhotonNetwork.CurrentRoom.Name +
            " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
            PhotonNetwork.CurrentRoom.MaxPlayers + ")";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        statusText.text =
            "Player Joined (" +
            PhotonNetwork.CurrentRoom.PlayerCount + "/" +
            PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        CheckRoomReady();
    }

    void CheckRoomReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (PhotonNetwork.CurrentRoom.PlayerCount ==
            PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            statusText.text = "Room Full. Loading SelectCar...";
            PhotonNetwork.LoadLevel("SelectCar");
        }
    }

    // ===================== FAIL CALLBACKS =====================

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Join Failed : " + message;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Create Failed : " + message;
    }
}
