using UnityEngine;

public class SpeedBoostPad : MonoBehaviour
{
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        // ?? Find Controller in parent hierarchy
        Controller controller = other.GetComponentInParent<Controller>();

        if (controller == null)
            return;

        Debug.Log("BOOST TRIGGERED FROM PAD");

        controller.ActivateSpeedBoost(boostMultiplier, boostDuration);
    }

}
