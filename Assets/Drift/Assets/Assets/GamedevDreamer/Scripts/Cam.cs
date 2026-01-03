using UnityEngine;
using UnityEngine.SceneManagement;

public class Cam : MonoBehaviour
{

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "SelectCar")
        {
            gameObject.SetActive(false);
        }

    }
}
