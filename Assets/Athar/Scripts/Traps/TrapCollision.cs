using UnityEngine;

public class TrapCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           // GameManager.Instance.DamageCar();
            Destroy(gameObject,1.2f);
        }
    }
}
