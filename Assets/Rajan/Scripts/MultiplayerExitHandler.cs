using Photon.Pun;
using UnityEngine;
using System.Collections;

public class MultiplayerExitHandler : MonoBehaviourPun
{
    private static bool exitRequested;
    private static bool exitInProgress;

    public static void RequestExit()
    {
        if (exitRequested) return;
        exitRequested = true;
    }

    void Update()
    {
        if (!exitRequested || exitInProgress) return;

        exitInProgress = true;
        StartCoroutine(ExitPhotonFast());
    }

    IEnumerator ExitPhotonFast()
    {
        Debug.Log("[MP] Fast Exit Requested");

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        float timer = 0f;
        while (PhotonNetwork.IsConnected && timer < 2f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("[MP] Photon Exit Complete");

        exitRequested = false;
        exitInProgress = false;
    }
}
