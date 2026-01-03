using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] Buttons;
    public VehicleLiist ListOfVehicles; // Add reference to the car list

    /* public void OpenLevel(int Leveid)
     {
         int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);
         GameObject selectedCar = ListOfVehicles.Vehicals[selectedIndex];
         string carName = selectedCar.GetComponent<CarController>().CarName;

         int sceneIndex = 0;
         PlayerPrefs.SetInt(Menu.SelectedLevel, Leveid);

         // ✅ Only allow playing if the car is owned
         if (PlayerPrefs.GetInt(carName, 0) == 1)
         {
             string LevelName = "Level " + Leveid;
             SceneManager.LoadScene(Leveid);
             //SceneManager.LoadScene(LevelName);
         }
         else
         {
             Debug.Log("Car is not unlocked! Cannot start game.");
         }
     }*/
    public void OpenLevel(int Leveid)
    {
        int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);
        GameObject selectedCar = ListOfVehicles.Vehicals[selectedIndex];
        string carName = selectedCar.GetComponent<CarController>().CarName;

        PlayerPrefs.SetInt(Menu.SelectedLevel, Leveid);

        // ✅ Only allow playing if the car is owned
        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            // 🔊 STOP MENU MUSIC BEFORE GAMEPLAY
            if (MusicManager.Instance != null)
                MusicManager.Instance.FadeOutAndStop();

            SceneManager.LoadScene(Leveid);
        }
        else
        {
            Debug.Log("Car is not unlocked! Cannot start game.");
        }
    }

}
