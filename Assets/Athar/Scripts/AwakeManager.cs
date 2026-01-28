using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using DG.Tweening;


public class AwakeManager : MonoBehaviour
{
    public GameObject Torotate;
    public Transform turntable;
    public float RotateSpeed;
    public VehicleLiist ListOfVehicles;
    public int VehiclePointer = 0;
    // For transitioning when selecting cars
    //[SerializeField] float slideDistance = 4f;
    // [SerializeField] float slideDuration = 0.5f;

    //  bool isTransitioning = false;

    public TMP_Text Currencytext;
    public TMP_Text CarNameText;
    public TMPro.TMP_Text CarPriceText;
    public Image CurrencyIcon;
    public GameObject BuyBtn;
    public GameObject StartBtn;
    public GameObject Player;
    public GameObject CurrentCar;
    //  [SerializeField] private PlayerProfileView selectCarProfileView;



    private const string CurrencyKey = "Currency";

    private void Awake()
    {
        // PlayerProfileController.Instance?.RefreshProfile(selectCarProfileView);

        // Play background music
        MusicManager.Instance.PlayMusic();

        // Ensure first car is unlocked by default
        string firstCarName = ListOfVehicles.Vehicals[0].GetComponent<CarController>().CarName;
        if (PlayerPrefs.GetInt(firstCarName, 0) != 1)
        {
            PlayerPrefs.SetInt(firstCarName, 1); // Unlock first car
            PlayerPrefs.Save();
        }

        // Load previously selected car (or fallback to first owned)
        VehiclePointer = PlayerPrefs.GetInt("Pointer", 0);
        if (!IsCarOwned(VehiclePointer))
        {
            VehiclePointer = GetFirstOwnedCarIndex();
            PlayerPrefs.SetInt("Pointer", VehiclePointer);
        }

        InstantiateSelectedCar();
        GetCarInfo();
    }

    private void FixedUpdate()
    {
        if (Player != null)
        {
            Player.transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime);
        }
    }

    void InstantiateSelectedCar()
    {
        GameObject carPrefab = ListOfVehicles.Vehicals[VehiclePointer];
        Player = Instantiate(carPrefab, turntable.position, Quaternion.identity);
        CurrentCar = Player;
        Player.tag = "Player";
        RCC_SceneManager.Instance.activePlayerVehicle =
    Player.GetComponent<RCC_CarControllerV3>();

    }

    void DestroyCurrentCar()
    {
        GameObject oldCar = GameObject.FindGameObjectWithTag("Player");
        if (oldCar != null)
            Destroy(oldCar);
        Destroy(CurrentCar);
    }

    public void NextBTN()
    {
        if (VehiclePointer >= ListOfVehicles.Vehicals.Length - 1) return;

        DestroyCurrentCar();

        VehiclePointer++;
        PlayerPrefs.SetInt("Pointer", VehiclePointer);

        InstantiateSelectedCar();
        GetCarInfo();
    }

    public void PreviousBTN()
    {
        if (VehiclePointer <= 0) return;

        DestroyCurrentCar();

        VehiclePointer--;
        PlayerPrefs.SetInt("Pointer", VehiclePointer);

        InstantiateSelectedCar();
        GetCarInfo();
    }


    public void GetCarInfo()
    {
        int currency = PlayerPrefs.GetInt(CurrencyKey, 0);
        Currencytext.text = currency.ToString();

        CarController car = GetCurrentCar();

        // ✅ CAR NAME — ALWAYS SET
        CarNameText.text = car.CarName;

        bool isOwned = PlayerPrefs.GetInt(car.CarName, 0) == 1;

        if (isOwned)
        {
            // ✅ OWNED STATE
            CarPriceText.text = "Owned";
            CurrencyIcon.gameObject.SetActive(false);
            BuyBtn.SetActive(false);
            StartBtn.GetComponent<Button>().interactable = true;
        }
        else
        {
            // 🔒 LOCKED STATE
            CarPriceText.text = car.CarPrice.ToString();
            CurrencyIcon.gameObject.SetActive(true);
            BuyBtn.SetActive(true);

            BuyBtn.GetComponent<Button>().interactable =
                currency >= car.CarPrice;
            StartBtn.GetComponent<Button>().interactable = false;
        }
    }

    public void PlayButton()
    {

        //  MULTIPLAYER PATH (NEW)
        if (GameModeManager.IsMultiplayer)
        {
            // MultiplayerCarSelect handle karega aage ka flow
            FindObjectOfType<MultiplayerCarSelect>()?.Ready();
            return; //  STOP single-player logic
        }

        //  SINGLE PLAYER (OLD LOGIC - UNCHANGED)
        int selectedIndex = PlayerPrefs.GetInt("Pointer", 0);
        GameObject selectedCar = ListOfVehicles.Vehicals[selectedIndex];
        string carName = selectedCar.GetComponent<CarController>().CarName;

        //  Only allow playing if the car is owned
        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            //int sceneInd = PlayerPrefs.GetInt(Menu.SelectedLevel, 2);
            //OnClickLetsPlayBtn();
            PlayerPrefs.SetInt(Menu.GotoLevelSelection, 1);
            //SceneManager.LoadScene(0);// sceneInd);
            //  SceneTransition.Instance.LoadScene(0); Transition 1
            SceneTransition.Instance.LoadSceneSlide(0, true); //Transition 2


        }
    }

    public void BuyBUtton()
    {
        // 🔊 BUY BUTTON SOUND (ONLY HERE)
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PlayBuySound();
        }

        CarController car = GetCurrentCar();
        int price = car.CarPrice;
        int currency = PlayerPrefs.GetInt(CurrencyKey, 0);

        if (currency >= price)
        {
            PlayerPrefs.SetInt(car.CarName, 1); // Set to owned
            PlayerPrefs.SetInt(CurrencyKey, currency - price);
            PlayerPrefs.Save();

            Debug.Log($"{car.CarName} purchased!");
            GetCarInfo();
        }
        else
        {
            Debug.Log("Not enough currency.");
        }
    }

    public void AwakeGameBtn()
    {
        string carName = GetCurrentCar().CarName;
        if (PlayerPrefs.GetInt(carName, 0) == 1)
        {
            PlayerPrefs.SetInt("Pointer", VehiclePointer);
            SceneManager.LoadScene("Level1");
        }
        else
        {
            Debug.Log("Car not owned!");
        }
    }

    CarController GetCurrentCar()
    {
        return ListOfVehicles.Vehicals[VehiclePointer].GetComponent<CarController>();
    }

    bool IsCarOwned(int index)
    {
        string carName = ListOfVehicles.Vehicals[index].GetComponent<CarController>().CarName;
        return PlayerPrefs.GetInt(carName, 0) == 1;
    }

    int GetFirstOwnedCarIndex()
    {
        for (int i = 0; i < ListOfVehicles.Vehicals.Length; i++)
        {
            if (IsCarOwned(i)) return i;
        }
        return 0;
    }

    public void OnClickBackBtn()
    {
        PlayerPrefs.SetInt(Menu.GotoHome, 1);
        // SceneManager.LoadScene(0);
        // SceneTransition.Instance.LoadScene(0); Transition 1
        SceneTransition.Instance.LoadSceneSlide(0, true); //Transition 2


    }
}