using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] ParticleSystem hiteffectPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Call coin and target logic from GameManager
            if (GameManager.Instance != null)
            {
              //  GameManager.Instance.CollectCoin();
               // GameManager.Instance.TargetDestroyed();
            }
            // Play hit effect
            ParticleSystem ps = Instantiate(hiteffectPrefab, collision.transform.position, Quaternion.identity);
            ps.Play();
            Destroy(gameObject); // Destroy this target
        }
    }
}