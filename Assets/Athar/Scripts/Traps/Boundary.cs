using UnityEngine;

public class Boundary : MonoBehaviour
{
    [SerializeField] public float DestroyDelay = 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, DestroyDelay);
    }

}
