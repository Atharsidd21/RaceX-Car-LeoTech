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
        if (!PhotonNetwork.InRoom) return;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("ready")) return;
            if (!(bool)p.CustomProperties["ready"]) return;
        }

        Debug.Log("[MP] All players ready -> Open Level Select");

        //  Room flag (menu will read this)
        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable { { "OpenLevelSelect", true } }
        );

        //  SAFE delayed scene load
        StartCoroutine(LoadMainMenuSafe());
    }

    System.Collections.IEnumerator LoadMainMenuSafe()
    {
        yield return null;          // wait 1 frame
        yield return new WaitForSeconds(0.1f);

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LoadLevel("Main Menu");
        }
    }

}
