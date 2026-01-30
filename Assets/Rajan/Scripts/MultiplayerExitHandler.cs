using Photon.Pun;
using UnityEngine;

public class MultiplayerExitHandler : MonoBehaviourPun
{
    private static bool exitRequested;
    
    public static void RequestExit()
    {
        if (exitRequested) return;
        exitRequested = true;
       
    }

    void Update()
    {
        if (!exitRequested) return;

        
        StartCoroutine(ExitPhotonFast());
    }

    System.Collections.IEnumerator ExitPhotonFast()
    {
        Debug.Log("[MP] Fast Exit Requested");

        //  KEY FIX — NO LeaveRoom for menu exit
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

        // Wait max 2 seconds only
        float timer = 0f;
        while (PhotonNetwork.IsConnected && timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("[MP] Photon Exit Complete (Fast)");
        
    }
}
