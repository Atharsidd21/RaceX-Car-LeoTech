using UnityEngine;

public class CameraLock : MonoBehaviour
{
    [Header("Lock Position Axes")]
    public bool lockX;
    public bool lockY;
    public bool lockZ;

    [Header("Lock Rotation Axes")]
    public bool lockRotX;
    public bool lockRotY;
    public bool lockRotZ;

    private Vector3 lockedLocalPosition;
    private Vector3 lockedLocalRotation;

    void Start()
    {
        // Store the values ONCE
        lockedLocalPosition = transform.localPosition;
        lockedLocalRotation = transform.localEulerAngles;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.localPosition;
        Vector3 rot = transform.localEulerAngles;

        if (lockX) pos.x = lockedLocalPosition.x;
        if (lockY) pos.y = lockedLocalPosition.y;
        if (lockZ) pos.z = lockedLocalPosition.z;

        if (lockRotX) rot.x = lockedLocalRotation.x;
        if (lockRotY) rot.y = lockedLocalRotation.y;
        if (lockRotZ) rot.z = lockedLocalRotation.z;

        transform.localPosition = pos;
        transform.localEulerAngles = rot;
    }

    // ?? Call this when you want to SET and LOCK again
    public void SetAndLock()
    {
        lockedLocalPosition = transform.localPosition;
        lockedLocalRotation = transform.localEulerAngles;
    }
}
