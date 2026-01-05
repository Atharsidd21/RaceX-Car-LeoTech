using UnityEngine;

public class Health : MonoBehaviour
{ private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
          // GameManager.Instance.HealCar();
            Destroy(gameObject,0.2f); // Self destroy after pickup
        }
    }
}