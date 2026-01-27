using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomInput;
    public TextMeshProUGUI statusText;

    public void CreateRoom()
    {
        string roomCode = Random.Range(1000, 9999).ToString();

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,        // future-ready
            IsVisible = false,     // private room
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomCode, options);
        statusText.text = "Creating Room : " + roomCode;
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomInput.text))
        {
            statusText.text = "Please enter room code";
            return;
        }

        PhotonNetwork.JoinRoom(roomInput.text.ToUpper());
        statusText.text = "Joining Room...";
    }

    public override void OnJoinedRoom()
    {
        statusText.text =
            "Joined Room : " +
            PhotonNetwork.CurrentRoom.Name +
            " (" + PhotonNetwork.CurrentRoom.PlayerCount + "/" +
            PhotonNetwork.CurrentRoom.MaxPlayers + ")";

        CheckRoomReady();
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
        if (PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount ==
            PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            statusText.text = "Room Full. Loading SelectCar...";
            PhotonNetwork.LoadLevel("SelectCar");
        }
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "Join Failed : " + message;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "Create Failed : " + message;
    }
}

