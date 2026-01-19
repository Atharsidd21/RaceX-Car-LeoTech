using UnityEngine;

public class WrongWayBlocker : MonoBehaviour
{
    [SerializeField] private GameObject blockerObject;

    private void OnEnable()
    {
        WrongWayDetector.OnWrongWayChanged += ToggleBlocker;
    }

    private void OnDisable()
    {
        WrongWayDetector.OnWrongWayChanged -= ToggleBlocker;
    }

    private void ToggleBlocker(bool isWrong)
    {
        blockerObject.SetActive(isWrong);
    }
}
