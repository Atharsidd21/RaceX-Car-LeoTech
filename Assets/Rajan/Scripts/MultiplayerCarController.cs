using Photon.Pun;
using UnityEngine;

public class MultiplayerCarController : MonoBehaviourPun
{
    private RCC_CarControllerV3 carController;

    void Awake()
    {
        carController = GetComponent<RCC_CarControllerV3>();

        if (!photonView.IsMine)
        {
            carController.enabled = false; //  no control
        }
    }
}
