using UnityEngine;

public class WrongWayUI : MonoBehaviour
{
    [SerializeField] private GameObject wrongWayPanel;

    private void OnEnable()
    {
        WrongWayDetector.OnWrongWayChanged += HandleWrongWay;
    }

    private void OnDisable()
    {
        WrongWayDetector.OnWrongWayChanged -= HandleWrongWay;
    }

    private void HandleWrongWay(bool isWrong)
    {
        wrongWayPanel.SetActive(isWrong);
    }
}
