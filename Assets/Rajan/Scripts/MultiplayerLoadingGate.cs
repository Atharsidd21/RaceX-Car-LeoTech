using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerLoadingGate : MonoBehaviourPunCallbacks
{
    [Header("PANELS")]
    public GameObject loadingPanel;
    public GameObject mainLobbyPanel;
    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;

    [Header("LOADING UI")]
    public Slider progressBar;
    public Text progressText;

    private bool lobbyReadyHandled = false;

    void Start()
    {
        Debug.Log("[LoadingGate] Started");

        loadingPanel.SetActive(true);
        mainLobbyPanel.SetActive(false);

        progressBar.value = 0f;
        progressText.text = "Connecting...";

        // SINGLE SOURCE OF CONNECT
        PhotonLauncher launcher = FindObjectOfType<PhotonLauncher>();
        launcher.TryConnect();
    }


    void Update()
    {
        if (!lobbyReadyHandled)
        {
            // Smooth fake loading till 90%
            if (progressBar.value < 0.9f)
            {
                progressBar.value += Time.deltaTime * 0.3f;
                progressText.text =
                    Mathf.RoundToInt(progressBar.value * 100f) + "%";
            }

            // SAFETY NET
            CheckPhotonState();
        }
    }

    void CheckPhotonState()
    {
        if (!lobbyReadyHandled &&
            PhotonNetwork.IsConnected &&
            PhotonNetwork.InLobby)
        {
            Debug.Log("[LoadingGate] Lobby detected via state check");
            HandleLobbyReady();
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[LoadingGate] OnJoinedLobby callback");
        HandleLobbyReady();
    }

    void HandleLobbyReady()
    {
        lobbyReadyHandled = true;

        progressBar.value = 1f;
        progressText.text = "100%";

        Invoke(nameof(ShowLobby), 0.2f);
    }

    void ShowLobby()
    {
        loadingPanel.SetActive(false);
        mainLobbyPanel.SetActive(true);

        Debug.Log("[LoadingGate] Multiplayer Lobby Unlocked");
    }
}
