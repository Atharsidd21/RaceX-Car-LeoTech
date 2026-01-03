using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public GameObject PlayerPrefab;


   void Start()
    {
        StartCoroutine(StartingSpawnObject());
    }

    private IEnumerator StartingSpawnObject()
    {
        yield return null;
        Instantiate(PlayerPrefab);
    }

}
