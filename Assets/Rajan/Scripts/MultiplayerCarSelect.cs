using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using ExitGames.Client.Photon;

public class MultiplayerCarSelect : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if (!GameModeManager.IsMultiplayer)
            gameObject.SetActive(false);
    }

    public void Ready()
    {
        int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);

        Hashtable props = new Hashtable
        {
            { "carIndex", selectedIndex },
            { "ready", true }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        CheckAllReady();
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps)
    {
        CheckAllReady();
    }

    void CheckAllReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("ready")) return;
            if (!(bool)p.CustomProperties["ready"]) return;
        }

        //  ALL READY
        Debug.Log("All players ready, returning to Main Menu");

        // flag set
        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable { { "GoToLevelSelect", true } }
        );

        PhotonNetwork.LoadLevel("Main Menu"); //  MAIN MENU ONLY
    }
}
