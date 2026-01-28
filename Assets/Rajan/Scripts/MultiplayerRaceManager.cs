using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MultiplayerRaceManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public GameObject[] carPrefabs; // same order as SelectCar list

    private void Start()
    {
        if (!GameModeManager.IsMultiplayer)
        {
            Debug.Log("Single player race");
            return;
        }

        SpawnPlayerCar();
    }

    void SpawnPlayerCar()
    {
        int carIndex = 0;

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("carIndex"))
        {
            carIndex = (int)PhotonNetwork.LocalPlayer.CustomProperties["carIndex"];
        }

        int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        spawnIndex = Mathf.Clamp(spawnIndex, 0, spawnPoints.Length - 1);

        PhotonNetwork.Instantiate(
            carPrefabs[carIndex].name,
            spawnPoints[spawnIndex].position,
            spawnPoints[spawnIndex].rotation
        );
    }
}
