using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("PANELS")]
    public GameObject mainLobbyPanel;
    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;

    [Header("MAIN LOBBY BUTTONS")]
    public Button openCreatePanelBtn;
    public Button openJoinPanelBtn;

    [Header("CREATE ROOM PANEL")]
    public Button createRoomBtn;
    public Button copyCodeBtn;
    public Button createBackBtn;
    public TMP_InputField createRoomCodeField;   // readonly
    public TextMeshProUGUI createRoomStatusText; // Player (0/2)

    [Header("JOIN ROOM PANEL")]
    public Button joinRoomBtn;
    public Button pasteCodeBtn;
    public Button joinBackBtn;
    public TMP_InputField joinRoomInputField;    // editable
    public TextMeshProUGUI joinRoomStatusText;

    private bool isReady = false;
    bool isMatchmakingInProgress = false;

    // ================= START =================

    void Start()
    {
        // Panel state
        mainLobbyPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);

        createRoomStatusText.text = "";
        joinRoomStatusText.text = "";

        createRoomCodeField.interactable = false;

        // Button listeners (NO OnClick usage)
        openCreatePanelBtn.onClick.AddListener(OpenCreateRoomPanel);
        openJoinPanelBtn.onClick.AddListener(OpenJoinRoomPanel);

        createRoomBtn.onClick.AddListener(CreateRoom);
        copyCodeBtn.onClick.AddListener(CopyRoomCode);
        createBackBtn.onClick.AddListener(BackToMainLobby);

        joinRoomBtn.onClick.AddListener(JoinRoom);
        pasteCodeBtn.onClick.AddListener(PasteRoomCode);
        joinBackBtn.onClick.AddListener(BackToMainLobby);

       
    }

    // ================= PHOTON =================

    bool CanMatchmake()
    {
        return PhotonNetwork.IsConnected &&
               PhotonNetwork.NetworkClientState == ClientState.JoinedLobby;
    }

    public override void OnJoinedLobby()
    {
        isReady = true;
        Debug.Log("[MP] Lobby Joined – Ready");
    }

    // ================= PANEL FLOW =================

    void OpenCreateRoomPanel()
    {
        Debug.Log("[MP] Open Create Room Panel");

        mainLobbyPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
        createRoomPanel.SetActive(true);

        createRoomCodeField.text = "";
        createRoomStatusText.text = "Player (0/2)";
    }

    void OpenJoinRoomPanel()
    {
        Debug.Log("[MP] Open Join Room Panel");

        mainLobbyPanel.SetActive(true);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(true);

        joinRoomInputField.text = "";
        joinRoomStatusText.text = "";
    }

    void BackToMainLobby()
    {
        Debug.Log("[MP] Back pressed");

        StartCoroutine(LeaveRoomAndReturnToLobby());
    }

    IEnumerator LeaveRoomAndReturnToLobby()
    {
        //  RULE 1: Agar room me ho → ALWAYS leave
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("[MP] Leaving room...");
            PhotonNetwork.LeaveRoom();

            while (PhotonNetwork.InRoom)
                yield return null;
        }

        //  RULE 2: Lobby ensure karo
        if (!PhotonNetwork.InLobby &&
            PhotonNetwork.NetworkClientState == ClientState.ConnectedToMaster)
        {
            Debug.Log("[MP] Joining lobby...");
            PhotonNetwork.JoinLobby();

            while (!PhotonNetwork.InLobby)
                yield return null;
        }

        //  RULE 3: Reset flags
        isMatchmakingInProgress = false;

        //  RULE 4: UI AFTER Photon ready
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
        mainLobbyPanel.SetActive(true);

        Debug.Log("[MP] Back complete — Lobby ready");
    }





    // ================= CREATE ROOM =================

    void CreateRoom()
    {
        if (!CanMatchmake())
        {
            Debug.Log("[MP] Not ready for matchmaking");
            return;
        }

        isMatchmakingInProgress = true; // 🔥 IMPORTANT

        string roomCode = Random.Range(1000, 9999).ToString();
        createRoomCodeField.text = roomCode;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = false,
            IsOpen = true
        };

        Debug.Log("[MP] Creating Room: " + roomCode);
        PhotonNetwork.CreateRoom(roomCode, options);
    }



    public override void OnJoinedRoom()
    {
        isMatchmakingInProgress = false; // 🔥 RESET

        Debug.Log("[MP] Joined Room");
        createRoomStatusText.text =
            $"Player ({PhotonNetwork.CurrentRoom.PlayerCount}/2)";
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("[MP] Player Joined");

        createRoomStatusText.text =
            $"Player ({PhotonNetwork.CurrentRoom.PlayerCount}/2)";

        if (PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("[MP] Room Full -> Load SelectCar");
            PhotonNetwork.LoadLevel("SelectCar");
        }
    }

    // ================= JOIN ROOM =================

    void JoinRoom()
    {
        if (!CanMatchmake())
        {
            joinRoomStatusText.text = "Connecting...";
            return;
        }

        if (string.IsNullOrEmpty(joinRoomInputField.text))
        {
            joinRoomStatusText.text = "Enter room code";
            return;
        }

        isMatchmakingInProgress = true; // 🔥 IMPORTANT

        Debug.Log("[MP] Joining Room: " + joinRoomInputField.text);
        PhotonNetwork.JoinRoom(joinRoomInputField.text.Trim());
    }



    public override void OnJoinRoomFailed(short code, string message)
    {
        isMatchmakingInProgress = false; // 🔥 RESET
        Debug.LogWarning("[MP] Join Failed: " + message);
        joinRoomStatusText.text = message;
    }

    public override void OnCreateRoomFailed(short code, string message)
    {
        isMatchmakingInProgress = false; // 🔥 RESET
        Debug.LogWarning("[MP] Create Failed: " + message);
        createRoomStatusText.text = message;
    }

    // ================= COPY / PASTE =================

    void CopyRoomCode()
    {
        GUIUtility.systemCopyBuffer = createRoomCodeField.text;
        Debug.Log("[MP] Room Code Copied");
    }

    void PasteRoomCode()
    {
        joinRoomInputField.text = GUIUtility.systemCopyBuffer;
        Debug.Log("[MP] Room Code Pasted");
    }
}
